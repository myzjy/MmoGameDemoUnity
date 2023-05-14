--- 最基础 ui继承

local UIBaseView = require "Common.UI.UIBaseView"
local UIBaseModule = BaseClass()
local CommonController = CS.ZJYFrameWork.UISerializable.Common.CommonController
local CommonManager = CS.ZJYFrameWork.Common.CommonManager

--- 创建
--- @param prefabName string 预制体名字
--- @param sortType number   排序格式
--- @param canvasType number 层级
function UIBaseModule.Create(prefabName, sortType, canvasType, selfView, viewPanel)
    local data = UIBaseModule.New()
    CS.Debug.Log(string.format("创建%s ，%d，%d，", prefabName, sortType, canvasType))
    ---预制体名字
    UIBaseModule.prefabName = prefabName
    --- UI 排序格式
    UIBaseModule.sortType = sortType
    --- UI层级
    UIBaseModule.canvasType = canvasType

    UIBaseModule.selfView = selfView
    if selfView ~= nil then
        CS.Debug.Log("当前UIView不为空")
    end
    UIBaseModule.viewPanel = viewPanel
    return data
end

isResuse = false

function UIBaseModule:Init()
end

local InstanceID = 0
--- 刷新 复用
function UIBaseModule:ReUse()
    --- 调用对应 view 基础ReUse 打开UI
    self.selfView.ReUse()
    ---将变换移动到本地变换列表的开头
    if self.sortType == UIConfigEnum.UISortType.First then
        self.SelfUIView.selfUIView.GetTransform.SetAsFirstSibling()
    else
        --- 将变换移动到本地变换列表的末尾
        if self.sortType == UIConfigEnum.UISortType.Last then
            self.SelfUIView.selfUIView.GetTransform.SetAsLastSibling()
        end
    end
end

function UIBaseModule.InstanceOrReuse()
    print(UIBaseModule.prefabName, "当前UI创建,创建位置", UIBaseModule.canvasType, ",当前还isResuse:", isResuse)
    if isResuse then
        if InstanceID == 0 then
            return
        end
        --- 调用 UI 排序
        UIBaseModule:ReUse()
    else
        print("创建UI")
        UIBaseModule:InstancePrefab()
    end
end

function UIBaseModule:InstancePrefab()
    print("UIBaseModule:InstancePrefab[77]")
    ---打开loading 界面
    CommonController.Instance.loadingRotate:OnShow()
    print("预制体名字", self.prefabName)
    print("CommonManager.Instance:", CommonManager.Instance)
    local actionObj = function(obj)
        print("obj", obj)
        UIBaseModule:InstantiateGameObject(obj)
        CommonController.Instance.loadingRotate:OnClose()
    end
    --- 根据预制体名字 生成 预制体
    CommonManager.Instance:LoadAsset(self.prefabName, actionObj)
    print("UIObject", UIBaseModule.UIObject)
    UIBaseModule.viewPanel:Init(UIBaseModule.UIObject)
    self.selfView:SetUIView(UIBaseModule.UIObject, UIBaseModule.viewPanel)
    self.selfView.OnInit()
end
UIBaseModule.UIObject = {}
_UIObject = {}
function UIBaseModule:InstantiateGameObject(gameObject)
    CS.Debug.Log(" CommonManager.Instance:LoadAsset->UIBaseModule:InstantiateGameObject()")
    print("UIBaseModule:InstantiateGameObject[86]")
    local parent = UIBaseModule:GetPanelUIRoot(self.canvasType)
    local go = CS.UnityEngine.Object.Instantiate(gameObject, parent, true) or CS.UnityEngine.GameObject
    print("当前生成", go)
    local rectTransform = go:GetComponent("RectTransform")
    print("rectTransform 获取组件", rectTransform)
    UIBaseModule.UIObject = go
    rectTransform.offsetMax = Vector2.zero
    rectTransform.offsetMin = Vector2.zero
    rectTransform.localScale = Vector3.one
    rectTransform.localPosition = Vector3.zero
    UIBaseModule:ActionGameObject()
    return go
end

function UIBaseModule:ActionGameObject()
    CS.Debug.Log("UIBaseModule:InstantiateGameObject-->UIBaseModule:ActionGameObject()")
    local rtf = UIBaseModule.UIObject:GetComponent("RectTransform")
    local uiView = UIBaseModule.UIObject:GetComponent("UIView")
    -- CS.Debug.Log(('获取到UIView'..uiView))
    print("获取到UIView", uiView)
    if rtf ~= nil then
        rtf.offsetMin = Vector2.zero
        rtf.offsetMax = Vector2.one
    end
    --- 将变换移动到本地变换列表的开头
    if self.sortType == UIConfigEnum.UISortType.First then
        UIBaseModule.UIObject.transform:SetAsFirstSibling()
    else
        --- 将变换移动到本地变换列表的末尾
        if self.sortType == UIConfigEnum.UISortType.Last then
            UIBaseModule.UIObject.transform:SetAsLastSibling()
        end
    end
    InstanceID = UIBaseModule.UIObject:GetInstanceID()
    --- 重制UI位置 大小
    uiView.GetTransform.localScale = Vector3.one
    uiView.GetTransform.localPosition = Vector3.zero
end

function UIBaseModule:GetPanelUIRoot(canvasType)
    ---UIRoot决定 UI所生成的位置
    local Root = CS.ZJYFrameWork.Config.AppConfig.GetRoot
    local switch = {
        [1] = function()
            return Root.GetBgTransformPanel
        end,
        [2] = function()
            return Root.GetUITransformPanel
        end,
        [3] = function()
            return Root.GetUITopTransformPanel
        end,
        [4] = function()
            return Root.GetNoticeCanvasTransformPanel
        end,
        [5] = function()
            return Root.GetBgTransformPanel
        end,
        [6] = function()
            return Root.GetActivieseCanvasTransformPanel
        end
    }
    local switchAction = switch[canvasType]
    if switchAction then
        return switchAction()
    end
    --- 返回空
    return nil
end

return UIBaseModule
