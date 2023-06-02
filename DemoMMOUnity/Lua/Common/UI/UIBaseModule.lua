--- 最基础 ui继承

local UIBaseView = require "Common.UI.UIBaseView"
local CommonController = CS.ZJYFrameWork.UISerializable.Common.CommonController
local CommonManager = CS.ZJYFrameWork.Common.CommonManager

local UIBaseModule = BaseClass()
local Config = nil

----- 创建
--- UIBaseModule 初始化
function UIBaseModule.Init(config, selfView, viewPanel)
    Config = config
    UIBaseModule.selfView = selfView
    if selfView ~= nil then
        CS.Debug.Log("当前UIView不为空")
    end
    UIBaseModule.viewPanel = viewPanel
end

UIBaseModule.isResuse = false

local InstanceID = 0
--- 刷新 复用
function UIBaseModule:ReUse()
    --- 调用对应 view 基础ReUse 打开UI
    self.selfView.ReUse()
    ---将变换移动到本地变换列表的开头
    if Config.sortType == UIConfigEnum.UISortType.First then
        self.SelfUIView.selfUIView.GetTransform.SetAsFirstSibling()
    else
        --- 将变换移动到本地变换列表的末尾
        if Config.sortType == UIConfigEnum.UISortType.Last then
            self.SelfUIView.selfUIView.GetTransform.SetAsLastSibling()
        end
    end
    self.selfView.OnShowUI()
end

function UIBaseModule.InstanceOrReuse()
    print(Config.prefabName, "当前UI创建,创建位置", Config.canvasType, ",当前还isResuse:", UIBaseModule.isResuse)
    if UIBaseModule.isResuse then
        if InstanceID == 0 then
            return
        end
        --- 调用 UI 排序
        UIBaseModule:ReUse()
    else
        UIBaseModule.isResuse = true
        print("创建UI")
        UIBaseModule:InstancePrefab()
    end
end

function UIBaseModule:InstancePrefab()
    printDebug("UIBaseModule:InstancePrefab[77]")
    ---打开loading 界面
    CommonController.Instance.loadingRotate:OnShow()
    printDebug("预制体名字", self.prefabName)
    printDebug("CommonManager.Instance:", CommonController.Instance)
    local actionObj = function(obj)
        printDebug("obj:" .. type(obj))
        UIBaseModule:InstantiateGameObject(obj)
        CommonController.Instance.loadingRotate:OnClose()
    end
    --- 根据预制体名字 生成 预制体
    CommonManager.Instance:LoadAsset(Config.prefabName, actionObj)
    UIBaseModule.viewPanel:Init(UIBaseModule.UIObject)
    self.selfView:SetUIView(UIBaseModule.UIObject, UIBaseModule.viewPanel)
    self.selfView.OnInit()
    self.selfView.OnShow()
end
UIBaseModule.UIObject = {}
_UIObject = {}
function UIBaseModule:InstantiateGameObject(gameObject)
    local parent = UIBaseModule:GetPanelUIRoot(Config.canvasType)
    local go = CS.UnityEngine.Object.Instantiate(gameObject, parent, true) or CS.UnityEngine.GameObject
    printDebug("当前生成:" .. type(go))
    local rectTransform = go:GetComponent("RectTransform")
    printDebug("rectTransform 获取组件:" .. type(rectTransform))
    UIBaseModule.UIObject = go
    rectTransform.offsetMax = Vector2.zero
    rectTransform.offsetMin = Vector2.zero
    rectTransform.localScale = Vector3.one
    rectTransform.localPosition = Vector3.zero
    UIBaseModule:ActionGameObject()
    return go
end

function UIBaseModule:ActionGameObject()
    local rtf = UIBaseModule.UIObject:GetComponent("RectTransform")
    local uiView = UIBaseModule.UIObject:GetComponent("UIView")
    -- CS.Debug.Log(('获取到UIView'..uiView))
    printDebug("获取到UIView:" .. type(uiView))
    if rtf ~= nil then
        rtf.offsetMin = Vector2.zero
        rtf.offsetMax = Vector2.one
    end
    --- 将变换移动到本地变换列表的开头
    if Config.sortType == UIConfigEnum.UISortType.First then
        UIBaseModule.UIObject.transform:SetAsFirstSibling()
    else
        --- 将变换移动到本地变换列表的末尾
        if Config.sortType == UIConfigEnum.UISortType.Last then
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