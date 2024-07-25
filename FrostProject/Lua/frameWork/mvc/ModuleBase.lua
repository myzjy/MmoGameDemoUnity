--[[    Generated by EmmyLua(https://github.com/EmmyLua)
    Created by zhangjingyi.
    DateTime: 2024/7/18 下午2:24
--]]
---@class ModuleBase
local ModuleBase = Class("ModuleBase")

function ModuleBase:ctor()
    self._localFilePaths = false -- 仅在存在非全生命周期
    self._mediator = {}
end

function ModuleBase:Create()

end

function ModuleBase:Destroy()

end

--- 重载 初始化
function ModuleBase:vOnInitializeModule()
end

--- 重载 反始化
function ModuleBase:vOnUninitializeModule()
end

--- 是否允许状态切换
--- @param inSwitchType string|number 切换类型 具体的类型 参考 StateConstant
--- @param inCurrentState string|nil 当前状态
--- @param inTargetState string|nil 目标状态
--- @param inUserData any 自定义数据
--- @return boolean 是否允许状态切换
function ModuleBase:vOnGameStateAllowSwitch(inSwitchType, inCurrentState, inTargetState, inUserData)
    return false
end
--- 状态切换（UI状态和游戏状态）前通知
--- @param inKind boolean 游戏状态（true）UI状态（false）
--- @param inSwitchType string|number switchType 切换类型，参见StateConstant
--- @param inCurrentState string|nil 当前状态，可能为""/nil
--- @param inTargetState string|nil 目标状态，可能为""/nil
--- @param inUserData any 自定义数据
--- @return any 无
function ModuleBase:vOnStatePrevSwitch(inKind, inSwitchType, inCurrentState, inTargetState, inUserData)
end

-- 状态切换（UI状态和游戏状态）后通知
--- @param inKind boolean 游戏状态（true）UI状态（false）注意：当前还没有UI状态的通知，后面可能会加，所以程序一定要判断kind
--- @param inSwitchType string|number switchType 切换类型，参见StateConstant
--- @return any 无
function ModuleBase:vOnStatePostSwitch(inKind, inSwitchType)
end

-- 游戏状态进入
--- @param inSwitchType string|number switchType 切换类型，参见StateConstant
--- @param inStateName string 进入的状态名，参见GameState
--- @param inUserData any 自定义数据
--- @return any 无
function ModuleBase:vOnStateEnter(inSwitchType, inStateName, inUserData)
end

-- 游戏状态离开
--- @param inSwitchType string|number switchType 切换类型，参见StateConstant
--- @param inStateName string 离开的状态名，参见GameState
--- @param inUserData any 自定义数据
--- @return any 无
function ModuleBase:vOnStateLeave(inSwitchType, inStateName, inUserData)
end

-- 重载
-- 操作回调
--- @param id string|number 操作标识，推荐使用字符串和数字
--- @param argument any 操作参数
function ModuleBase:vOnAction(id, argument)
end

-- 发送更新UI（只能在实例内部调用该函数，如：self:SendUpdateUI("zvlc")
--- @param id string|number 刷新ID标识
--- @param argument any 参数
function ModuleBase:SendUpdateUI(id, argument)
    if not id then
        FrostLogE(self.__classname, "UIMediatorClass.SendUpdateUI : invalid parameter")
        return
    end

    for i = 1, #self._mediator do
        local mediator = self._mediator[i]
        mediator:SendUpdateUI(id, argument, true)
    end
end

-- 查找UIPrefab对应的PrefabClass
-- @param uiPrefab UIPrefab对象
-- @return 获取到的UIPrefabClass
function ModuleBase:GetPrefabClassByUIPrefab(uiPrefab)
    for i = 1, #self._mediator do
        local mediator = self._mediator[i]
        local prefabClass, autoLoadNode = mediator:GetPrefabClassByUIPrefab(uiPrefab)
        if prefabClass then
            return { PrefabClass = prefabClass, AutoLoadNode = autoLoadNode }
        end
    end
    return nil
end

-- 查找prefabName对应的PrefabClass
-- @param prefabName Prefab名字
-- @return 获取到的UIPrefabClass
function ModuleBase:GetPrefabClassByName(prefabName)
    for i = 1, #self._mediator do
        local mediator = self._mediator[i]
        local prefabClass = mediator:GetPrefabClassByName(prefabName)
        if prefabClass then
            return prefabClass
        end
    end

    return nil
end

-- 查找mediatorName对应的MediatorClass
-- @param mediatorName Mediator名字
-- @return 获取到的MediatorClass
-- 说明：新手引导在使用该函数，在跨平台而UI不同的Module（比如技能UI）中，需要得到Mediator，去获取相应的UIPrefabClass
function ModuleBase:GetMediatorClassByName(mediatorName)
    if not mediatorName then
        FrostLogE(self.__classname, "GetMediatorClassByName : invalid parameter")
        return nil
    end

    for i = 1, #self._mediator do
        local mediator = self._mediator[i]
        if mediator and mediator.__classname == mediatorName then
            return mediator
        end
    end

    return nil
end

-------------------------------------------------------------------------------------------
-- 获得Module对应的DataCentre
--  1. 当Module挂载在Entity下面时（如背包模块），获得Entity对应的DataCentre
--  2. 当Module不属于任何Entity时（如登录模块），则获得全局的DataCentre
-- @return(DataCentre) 对应的DataCentre
-------------------------------------------------------------------------------------------
---@return DataCentreService
function ModuleBase:GetDataCentre()
    return DataCentreService
end

-------------------------------------------------------------------------------------------
-- 响应玩家进入游戏后的处理
-- 客户端：玩家登录服务器后，先创建Entity，预载接收后触发
-- 服务器：先开始游戏，当玩家与服务器建立连接后，创建Entity之后立即触发
-------------------------------------------------------------------------------------------
function ModuleBase:OnEnterGame()
    self:vOnEnterGame()
end

-------------------------------------------------------------------------------------------
-- 角色已登录游戏，可以开始向服务器请求玩家数据
-------------------------------------------------------------------------------------------
function ModuleBase:vOnEnterGame()
end



return ModuleBase