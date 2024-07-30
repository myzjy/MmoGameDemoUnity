--[[
    author : zhangjingyi
    description : 状态服务 
--]]

---@class StateService:ServiceBase
local StateService = Class("StateService", ClassLibraryMap.ServiceBase)

function StateService:ctor()
    self._switching = false
    ---@type StateUIControl
    self._uiStateControl = ClassLibraryMap.StateUIControl(self)
    ---@type StateGameControl
    self._gameStateControl = ClassLibraryMap.StateGameControl(self)
    
    self._switchQueue = {}
end

-------------------------------------------------------------------------------------------
-- 子类覆盖，进行返回类的静态配置数据
-------------------------------------------------------------------------------------------
function StateService:vGetConfig()
    return
    {
        name = "StateService",
    }
end

function StateService:vInitialize()
    self._uiStateControl:Create()
    EventService:ListenEvent("LuaEventID.OnStagePreLoadMap", self, self._OnStagePreLoadMap)
end

function StateService:vDeinitialize()
    EventService:UnListenEvent(self)
    ScheduleService:RemoveUpdater(self)
    self._uiStateControl:Destroy()
end

function StateService:AddSwitchQueue(isGameState, switchType, stateName, userData, param1, param2, param3, param4, param5, param6)
    if not self._switching then
        FrostLogD(self.__classname, "--> StateService:AddSwitchQueue1")
        return false
    end
    
    local data = UPool:CreateTable()
    data.isGameState = isGameState
    data.switchType = switchType
    data.stateName = stateName
    data.userData = userData
    data.param1 = param1
    data.param2 = param2
    data.param3 = param3
    data.param4 = param4
    data.param5 = param5
    data.param6 = param6
    self._switchQueue[#self._switchQueue + 1] = data

    ScheduleService:RemoveUpdater(self, self.OnUpdate)
    ScheduleService:AddUpdater(self, self.OnUpdate)

    local info = string.format("state switch error : isGameState:%s switchType:%d stateName:%s", tostring(isGameState), switchType, stateName or "")
    FrostLogE(self.__classname, info, "callstack:", debug.traceback())
    
    return true
end

function StateService:OnUpdate()
    if self._switching then
        return
    end
    
    if #self._switchQueue == 0 then
        ScheduleService:RemoveUpdater(self, self.OnUpdate)
        return
    end
    
    local data = table.remove(self._switchQueue, 1)
    local constant = StateConstant
    if data.isGameState then
        if data.switchType == constant.Change then
            self:ChangeGameState(data.stateName, data.userData, data.param1, data.param2, data.param3, data.param4, data.param5, data.param6)
        
        elseif data.switchType == constant.Push then
            self:PushGameState(data.stateName, data.userData, data.param1, data.param2, data.param3, data.param4)
            
        elseif data.switchType == constant.Pop then
            self:PopGameState(data.userData, data.param1)
        end
    else
        if data.switchType == constant.ChangeAll then
            self:ChangeAllUIState(data.stateName, data.userData)
            
        elseif data.switchType == constant.Change then
            self:ChangeUIState(data.stateName, data.userData)
            
        elseif data.switchType == constant.ChangeTop then
            self:ChangeUITopState(data.stateName, data.userData)
            
        elseif data.switchType == constant.Push then
            self:PushUIState(data.stateName, data.userData)
            
        elseif data.switchType == constant.Pop then
            self:PopUIState(data.userData, data.param1)
            
        elseif data.switchType == constant.PushChild then
            self:PushUIChildState(data.stateName, data.userData)
            
        elseif data.switchType == constant.PopChild then
            self:PopUIChildState(data.userData)
        end
    end
    
    UPool:DestroyTable(data)
end

-- 是否正在切换状态
-- @return 是否正在切换状态（bool）
function StateService:IsSwitching()
    return self._switching
end

-- 获取当前UI根状态名
-- @return UI状态名（string），如果没有返回""
function StateService:GetCurrentUIRootStateName()
    return self._uiStateControl:GetCurrentRootStateName()
end

-- 获取当前UI状态名
-- @return UI状态名（string），如果没有返回""
function StateService:GetCurrentUIStateName()
    return self._uiStateControl:GetCurrentStateName()
end

-- 获取前一个状态UI状态名
-- @return UI状态名（string），如果没有返回""
function StateService:GetPreUIStateName()
    return self._uiStateControl:GetPreStateName()
end

-- 打印当前UI状态数据
-- @return UI状态名信息
function StateService:PrintUIState()
    return self._uiStateControl:PrintState()
end

-- 获取底层UI状态名
-- @return UI状态名（string），如果没有返回""
function StateService:GetBottomUIStateName()
    return self._uiStateControl:GetBottomStateName()
end

-- 获取当前UI子状态名
-- @return UI子状态名（string），如果没有返回""
function StateService:GetCurrentUIChildStateName()
    return self._uiStateControl:GetCurrentChildStateName()
end

-- 获取当前UI顶层状态名
-- @return UI状态名（string），如果没有返回""
function StateService:GetCurrentUITopStateName()
    local stateName = self:GetCurrentUIChildStateName()
    if stateName and stateName ~= "" then
        return stateName
    end
    
    stateName = self:GetCurrentUIStateName()
    if stateName and stateName ~= "" then
        return stateName
    end
    
    return self:GetCurrentUIRootStateName()
end

-- 切换整个UI状态（该函数千万小心的使用，特别是不得在状态切换的过程中调用）
-- 行为：清空当前的所有UI，然后按照规则设置stateName
-- @param stateName UI状态名（string）
--                  可以是以/分割的多个状态（string），最多三层，如：Hall/Hero/Up
--                  可以是以-分割的多个状态，标识在状态中的连续跳转，如：Hall/Hero-Star/Up
--                  可以是空，标识清空状态后不设置
-- @param userData 用户自定义数据
-- @param dependOnGameState 是否依赖于游戏状态切换（内部使用，请不要管它）
-- @return 无
function StateService:ChangeAllUIState(stateName, userData, dependOnGameState)
    if not dependOnGameState and self:AddSwitchQueue(false, StateConstant.ChangeAll, stateName, userData) then
        return
    end
    
    self._uiStateControl:ChangeAllState(stateName, userData, dependOnGameState)
end

-- 切换UI系统状态
-- 行为：清空所有系统和子状态，然后按照规则设置stateName
-- @param state 目标UI状态，类型：string
--              可以是以/分割的多个状态，最多两层，如Hero/Up
--              可以是以-分割的多个状态，标识在状态中的连续跳转，如Hero-Star/Up
--              可以是空，标识只保留root状态（如果有root）
-- @param userData 目标UI状态的用户自定义数据，类型：用户自定义
-- @return 无
function StateService:ChangeUIState(state, userData)
    if self:AddSwitchQueue(false, StateConstant.Change, state, userData) then
        return
    end
    
    self._uiStateControl:ChangeState(state, userData)
end

-- 切换顶层的UI系统状态
-- 行为：清空当前系统和子状态，然后按照规则设置stateName
-- @param state 目标UI状态，类型：string
--              可以是以/分割的多个状态，最多两层，如Hero/Up
--              可以是以-分割的多个状态，标识在状态中的连续跳转，如Hero-Star/Up
--              不可以是空
-- @param userData 目标UI状态的用户自定义数据，类型：用户自定义
-- @return 无
function StateService:ChangeUITopState(state, userData)
    if self:AddSwitchQueue(false, StateConstant.ChangeTop, state, userData) then
        return
    end
    
    self._uiStateControl:ChangeTopState(state, userData)
end

-- 压入UI系统状态
-- @param stateName 目标UI状态，类型：string
--                  可以是以/分割的多个状态，最多两层，如Hero/Up
--                  可以是以-分割的多个状态，标识在状态中的连续跳转，如Hero-Star/Up
-- @param userData 目标UI状态的用户自定义数据，类型：用户自定义
-- @param dependOnGameState 是否依赖于游戏状态切换（内部使用，请不要管它）
-- @return 无
function StateService:PushUIState(stateName, userData, dependOnGameState)
    if not dependOnGameState and self:AddSwitchQueue(false, StateConstant.Push, stateName, userData) then
        return
    end

    self._uiStateControl:PushState(stateName, userData, dependOnGameState)
end

-- 弹出UI系统状态
-- @param userData 弹出UI状态后，新激活状态的用户自定义数据，类型：用户自定义
-- @param unCheckAllow 不检测是否允许的回调，默认nil为始终检测（bool）
-- @return 无
function StateService:PopUIState(userData, unCheckAllow)
    if self:AddSwitchQueue(false, StateConstant.Pop, nil, userData, unCheckAllow) then
        return
    end
    
    self._uiStateControl:PopState(userData, unCheckAllow)
end

-- 压入UI子状态
-- @param stateName 目标UI子状态，类型：string
--                  可以是以-分割的多个状态，标识在状态中的连续跳转，如Hero-Star
-- @param userData 目标UI子状态的用户自定义数据，类型：用户自定义
-- @return 无
function StateService:PushUIChildState(stateName, userData)
    if self:AddSwitchQueue(false, StateConstant.PushChild, stateName, userData) then
        FrostLogD(self.__classname, "PushUIChildState AddSwitchQueue Success, return!")
        return
    end

    self._uiStateControl:PushChildState(stateName, userData)
end

-- 弹出UI子状态
-- @param userData 弹出UI子状态后，新激活子状态的用户自定义数据，类型：用户自定义
-- @return 无
function StateService:PopUIChildState(userData)
    if self:AddSwitchQueue(false, StateConstant.PopChild, nil, userData) then
        return
    end
    
    self._uiStateControl:PopChildState(userData)
end

-- 获取当前游戏状态
-- @return 游戏状态（string），当前没有游戏状态返回""
function StateService:GetCurrentGameStateName()
    return self._gameStateControl:GetStateName()
end

-- 获取父游戏状态
-- @return 游戏状态（string），当前没有父游戏状态返回""
function StateService:GetParentGameStateName()
    return self._gameStateControl:GetStateName(1)
end

-- 切换游戏状态
-- @param stateName 游戏状态名，可以是以/分割的多个状态（stirng）
-- @param userData 用户自定义数据，类型：用户自定义
-- @param forecastStateName 预测游戏状态，可以是以,分割的多个状态（string）
-- @param uiStateName UI状态名（string），标识切换游戏状态后进入的UI状态，默认""，规则参考ChangeAllUIState函数
-- @param dontLoadResource 不装载资源，默认装载，类型：bool
-- @param dontOperateUI 不操作UI，默认操作，类型：bool
-- @param sceneID 目标场景ID
-- @param loadUI 资源装载界面，默认为LoadUI_Scene，类型：AssetLoadUI
function StateService:ChangeGameState(stateName, userData, forecastStateName, uiStateName, dontLoadResource, dontOperateUI, sceneID, loadUI)
    if self:AddSwitchQueue(true, StateConstant.Change, stateName, userData, forecastStateName, uiStateName, dontLoadResource, dontOperateUI, sceneID, loadUI) then
        return
    end
    
    if dontOperateUI then
        uiStateName = nil
    else
        uiStateName = uiStateName or ""
    end
    
    FrostLogD(self.__classname, "--> StateService:ChangeGameState", stateName,"uiStateName", uiStateName)
    self._gameStateControl:ChangeState(stateName, userData, forecastStateName, uiStateName, dontLoadResource, sceneID, loadUI)
end

-- 压入游戏状态
-- @param stateName 游戏状态名（string）
-- @param userData 用户自定义数据，类型：用户自定义
-- @param uiStateName UI状态名（string），标识压入游戏状态后立即压入的UI状态，规则参考PushUIState
-- @param dontLoadResource 不装载资源，默认装载，类型：bool
-- @param sceneID 目标场景ID
-- @param loadUI 资源装载界面，默认为LoadUI_Scene，类型：AssetLoadUI
function StateService:PushGameState(stateName, userData, uiStateName, dontLoadResource, sceneID, loadUI)
    if self:AddSwitchQueue(true, StateConstant.Push, stateName, userData, uiStateName, dontLoadResource, sceneID, loadUI) then
        return
    end
    
    self._gameStateControl:PushState(stateName, userData, uiStateName, dontLoadResource, sceneID, loadUI)
end

-- 弹出游戏状态
-- @param userData 用户自定义数据，类型：用户自定义
-- @param forecastStateName 预测游戏状态，可以是以,分割的多个状态（string），"*popstate"标识被弹出的状态名
-- @return 无
function StateService:PopGameState(userData, forecastStateName)
    if self:AddSwitchQueue(true, StateConstant.Pop, nil, userData, forecastStateName) then
        return
    end
    
    self._gameStateControl:PopState(userData, forecastStateName)
end

-- 清理缓存
-- clearAllUI 是否完全清理UI（bool），默认值：false
-- @return 无
function StateService:ClearCache(clearAllUI)
    AssetService:ClearCache(self._gameStateControl:GetAllStateName(), (not clearAllUI) and self._uiStateControl:GetHistoryStateName() or nil)
end

-------------------------------------------------------------------------------------------
-- 由于World切换之后会销毁Wrold内界面，所以响应World切换前的事件，主动清理所有的Prefab
-------------------------------------------------------------------------------------------
function StateService:_OnStagePreLoadMap()
    self:ChangeGameState(GameState.EmptyStateID)
    AssetService:ClearAllCache(true)
end

return StateService