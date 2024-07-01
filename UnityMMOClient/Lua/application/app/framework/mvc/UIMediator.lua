---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by Administrator.
--- DateTime: 2024/6/23 下午11:09
---
---@class UIMediator
local UIMediator = class("UIMediator")
--- 初始化
function UIMediator:ctor()
    self.module = false

    self._prefabClass = {}
    self._prefabClassAsync = {}

    self._inStatName = false

    self._belongUIStateName = false
    self._inputBelongUIStateName = false
    self._inStateName = false
    self._UIMaskMark = false
    self._inputEnable = false
    self._userData = false      -- StateIn前缓存 StateOut后清除
    self._scheduleTime = false
end

function UIMediator:Create(inModule)
    self.module = inModule
    local stateName, inputStateName = self:vGetBelongUIStateName()
    self._belongUIStateName = stateName or false
    self._inputBelongUIStateName = inputStateName or false

    self:vOnInitialize()
end

function UIMediator:Destroy()
    if self._inStateName then
        self:vOnUIStateOut()
    end
    self:vOnUnInitialize()
    if  self._scheduleTime then
        self._scheduleTime:Stop()
    end
end

--- 重载 初始化
function UIMediator:vOnInitialize()
end

--- 重载
---  反初始化
function UIMediator:vOnUnInitialize()
end
-- 重载
--- 切出状态
--- @param switchType any 切换类型（取值：StateConstant）
--- @param outStateName string 切出的状态名
function UIMediator:vOnUIStateOut(switchType, outStateName)
end

-- 重载
--- 获取Mediator所属的UI状态
--- @return table,string 返回值1：所属的UI状态的数组，返回空标识不属于任何状态  返回值2：可单独指定当前Mediator在什么UI状态下才响应外设输入
--- (默认与前者保持一致，只有Mediator属于多状态或者根状态的时候，才可能需要单独指定)
function UIMediator:vGetBelongUIStateName()
end


function UIMediator:GetUIPath(index)
end

function UIMediator:vGetUIPath()
end

function UIMediator:CreateAsyncPrefabCompleted(async, cls, argument, style, obj)
    if not cls then
        PrintError(self.__classname, "UIMediator:CreateAsyncPrefab 类错误")
    end
    local child = cls()

    self._childList[#self._childList + 1] = child
end

function UIMediator:CreatePrefab(cls, argument, style, obj)
    if not cls then
        PrintError("UIMediator:CreatePrefab invalid parameter")
    end
    local objCls = cls()

    self._prefab[#self._prefab + 1] = objCls
    return objCls
end

--- 向下发送更新UI（只能在实例内部调用该函数，如：self:SendUpdateUI("zvlc")，与vOnUpdateUI配合使用。自己不会调vOnUpdateUI。
--- @param id string 刷新ID标识
--- @param argument table 参数
--- @param dummy boolean 不要管这个参数 在 UIMediatorClass 内管理
function UIMediator:SendUpdateUI(id, argument, dummy)
    if string.IsNullOrEmpty(id) then
        PrintError("id:" .. id .. ",错误")
        return
    end
    if not self._childList then
        PrintError("当前" .. self.__classname .. ",没有创建子物体或者子面板")
        return
    end
    for _, v in pairs(self._childList) do
        if dummy then
            self:vOnUpdateUI(id, argument)
            return
        end
        -- 递归 向子物体发送
        v:SendUpdateUI(id, argument, dummy)
    end
end
--- 向上发送操作（只能在实例内部调用该函数，如：self:SendAction("zvlc")，与vOnAction配合使用。自己不会调vOnAction。
--- @param id string 操作ID标识
--- @param argument table 参数
--- @param dummy boolean 不要管这个参数
function UIMediator:SendAction(id, argument, dummy)
    if not self._parent then
        PrintError(self.__classname .. ",当前没有通知没有父组件")
        return
    end
    if self._parent then
        if dummy then
            self._parent:vOnAction(id, argument)
            return
        end
        self._parent:SendAction(id, argument, dummy)
    end
end

--- 重载
--- 更新UI回调，以 module -> mediator -> parent UI prefab -> child UI prefab 顺序向下传递（其中任何一个都可以自主发起下发传递，不必然从module开始），与SendUpdateUI配合使用
--- @param id string|number 更新标识，推荐使用字符串和数字
--- @param argument table 自定义更新参数
--- @return boolean 是否阻断向下传递刷新，true为阻断
function UIMediator:vOnUpdateUI(id, argument)
end
--- 重载
--- 操作回调，以 child UI prefab -> parent UI prefab -> mediator -> module顺序向上传递（其中任何一个都可以自主发起下发传递，不必然从child UI prefab开始）,与SendAction配合使用
--- @param id string|number 操作标识，推荐使用字符串和数字
--- @param argument table 自定义操作参数
--- @return boolean 是否阻断向上传递操作，true为阻断
function UIMediator:vOnAction(id, argument)
end



return UIMediator
