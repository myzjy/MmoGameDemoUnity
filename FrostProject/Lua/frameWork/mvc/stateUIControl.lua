--[[
    author : zhangjingyi
    description : UI状态控制
--]]

---@class StateUIControl
local StateUIControl = Class("StateUIControl")
local this = false

---@param service StateService
function StateUIControl:ctor(service)
    this = self
    self._service = service

    self._config = {}
    self._root = false -- root state name - string
    self._data = {}  -- array of {StateName - string, UserData - any, Child - array}
    self._history = {} -- array of string

    self._cacheOperator = false
    self._cacheStateName = false
    self._cacheUserData = null
end

function StateUIControl:Create()
end

function StateUIControl:Destroy()
    if self._root then
        FrostLogD(self.__classname, "StateUIControl:Destroy : residual ui state - %s", self._root)
    end
    
    for _, v in ipairs(self._data) do
        FrostLogD(self.__classname, "StateUIControl:Destroy : residual ui state - %s", v.StateName)
    end
end

function StateUIControl:GetCurrentRootStateName()
    return self._root or ""
end

function StateUIControl:GetCurrentStateName()
    return #self._data > 0 and self._data[#self._data].StateName or ""
end

function StateUIControl:GetPreStateName()
    return #self._data > 1 and self._data[#self._data - 1].StateName or ""
end

function StateUIControl:PrintState()
    local uiStack = "PrintUIState Stack: "
    for i = 1, #self._data do
        uiStack = string.format("%s %s", uiStack, self._data[i].StateName)
    end
    
    FrostLogD(self.__classname,GetCurrentWorld().name, uiStack)
    return uiStack
end

function StateUIControl:GetBottomStateName()
    return #self._data > 0 and self._data[1].StateName or ""
end

function StateUIControl:GetCurrentChildStateName()
    local data = #self._data > 0 and self._data[#self._data] or nil
    if data and data.Child then
        return data.Child[#data.Child].StateName
    end
    
    return ""
end

function StateUIControl:GetHistoryStateName()
    local ret = table.concat(self._history, "/")
    
    if self._root and self._root ~= "" then
        if string.len(ret) > 0 then
            ret = ret .. "/" .. self._root
        else
            ret = self._root
        end
    end
    
    return ret
end

function StateUIControl:ChangeAllState(stateName, userData, dependOnGameState)
    stateName = stateName or ""
    
    if dependOnGameState then
        self:ImplChangeAllUIState(stateName, userData, true)        
        return
    end
    
    if self._service._switching then
        FrostLogE(self.__classname, "StateUIControl:ChangeAllState : state is switching - %s", stateName)
        return
    end

    self._service._switching = true
    
    if stateName ~= "" and AssetService:Load(nil, self:TranslateStateNameToResource(stateName), self.OnLoadCompleted) then
        self._cacheOperator = StateConstant.ChangeAll
        self._cacheStateName = stateName
        self._cacheUserData = userData == nil and null or userData
        return
    end

    self:ImplChangeAllUIState(stateName, userData)
  
    self._service._switching = false
    MVCService:PostSwitchState(false, StateConstant.Change)
end

function StateUIControl:ChangeState(stateName, userData)
    stateName = stateName or ""
    
    if self._service._switching then
        FrostLogE(self.__classname, "StateUIControl:ChangeState : state is switching - %s", stateName)
        return
    end
    
    if not MVCService:IsAllowSwitchUIState(StateConstant.Change, self:GetCurrentStateName(), stateName, userData) then
        return
    end
    
    self._service._switching = true

    if stateName ~= "" and AssetService:Load(nil, self:TranslateStateNameToResource(stateName), AssetLoadUI.LoadUI_Wait, self.OnLoadCompleted) then
        self._cacheOperator = StateConstant.Change
        self._cacheStateName = stateName
        self._cacheUserData = userData == nil and null or userData
        return
    end

    self:ImplChangeState(stateName, userData)

    self._service._switching = false
    MVCService:PostSwitchState(false, StateConstant.Change)
end
function StateUIControl:ChangeTopState(stateName, userData)
    if not stateName or stateName == "" then
        FrostLogE(self.__classname, "StateUIControl:ChangeTopState : invalid parameter")
        return
    end
    
    if self._service._switching then
        FrostLogE(self.__classname, "StateUIControl:ChangeTopState : state is switching - %s", stateName)
        return
    end
    
    if not MVCService:IsAllowSwitchUIState(StateConstant.Change, self:GetCurrentStateName(), stateName, userData) then
        return
    end
    
    self._service._switching = true

    if AssetService:Load(nil, self:TranslateStateNameToResource(stateName), AssetLoadUI.LoadUI_Wait, self.OnLoadCompleted) then
        self._cacheOperator = StateConstant.ChangeTop
        self._cacheStateName = stateName
        self._cacheUserData = userData == nil and null or userData
        return
    end

    self:ImplChangeTopState(stateName, userData)

    self._service._switching = false
    MVCService:PostSwitchState(false, StateConstant.Change)
end

function StateUIControl:PushState(stateName, userData, dependOnGameState)
    if not stateName or stateName == "" then
        FrostLogE(self.__classname, "StateUIControl:PushState : invalid parameter")
        return
    end
    
    if dependOnGameState then
        self:ImplPushState(stateName, userData)
        return
    end
        
    if self._service._switching then
        FrostLogE(self.__classname, "StateUIControl:PushState : state is switching - %s", stateName)
        return
    end
    
    if not MVCService:IsAllowSwitchUIState(StateConstant.Push, self:GetCurrentStateName(), stateName, userData) then
        return
    end
        
    self._service._switching = true


    if AssetService:Load(nil, self:TranslateStateNameToResource(stateName), AssetLoadUI.LoadUI_Wait, self.OnLoadCompleted) then
        self._cacheOperator = StateConstant.Push
        self._cacheStateName = stateName
        self._cacheUserData = userData == nil and null or userData
        return
    end

    self:ImplPushState(stateName, userData)

    self._service._switching = false
    MVCService:PostSwitchState(false, StateConstant.Push)
end

function StateUIControl:PopState(userData, unCheckAllow)
    if self._service._switching then
        FrostLogE(self.__classname, "StateUIControl:PopState : state is switching")
        return
    end
    
    local currentStateName = self:GetCurrentStateName()
    if currentStateName == "" then
        FrostLogE(self.__classname, "StateUIControl:PopState : state is empty")
        return
    end
    
    local stateName = ""
    if #self._data > 1 then
        local d = self._data[#self._data-1]
        stateName = d.StateName
        
        if userData == nil then
            userData = d.UserData
        end
    end
    if not unCheckAllow and not MVCService:IsAllowSwitchUIState(StateConstant.Pop, currentStateName, stateName, userData) then
        return
    end
    
    self._service._switching = true
    

    if stateName ~= "" and AssetService:Load(nil, stateName, self.OnLoadCompleted) then
        self._cacheOperator = StateConstant.Pop
        self._cacheStateName = stateName
        self._cacheUserData = userData == nil and null or userData
        return
    end

    self:ImplPopState(userData)
    self._service._switching = false
    MVCService:PostSwitchState(false, StateConstant.Pop)
end

function StateUIControl:PushChildState(stateName, userData)
    if not stateName or stateName == "" then
        FrostLogE(self.__classname, "StateUIControl:PushChildState : invalid parameter")
        return
    end
    
    if self._service._switching then
        FrostLogE(self.__classname, "StateUIControl:PushChildState : state is switching - %s", stateName)
        return
    end
        
    if #self._data == 0 then
        FrostLogE(self.__classname, "StateUIControl:PushChildState : state is empty - %s", stateName)
        return
    end
    
    if not MVCService:IsAllowSwitchUIState(StateConstant.PushChild, self:GetCurrentChildStateName(), stateName, userData) then
        FrostLogD(self.__classname, "StateUIControl:PushChildState : Not allow switch state")
        return
    end

    if AssetService:Load(nil, self:TranslateStateNameToResource(stateName), AssetLoadUI.LoadUI_Wait, self.OnLoadCompleted) then
        self._cacheOperator = StateConstant.PushChild
        self._cacheStateName = stateName
        self._cacheUserData = userData == nil and null or userData
        return
    end

    self:ImplPushChildState(stateName, userData)

    self._service._switching = false
    MVCService:PostSwitchState(false, StateConstant.PushChild)
end

function StateUIControl:PopChildState(userData)
    if self._service._switching then
        FrostLogE(self.__classname, "StateUIControl:PopChildState : state is switching")
        return
    end
    
    local currentStateName = self:GetCurrentChildStateName()
    if currentStateName == "" then
        FrostLogE(self.__classname, "StateUIControl:PopChildState : state is empty")
        return
    end
        
    local stateName = ""
    local data = self._data[#self._data]
    if #data.Child > 1 then
        local d = data.Child[#data.Child-1]
        stateName = d.StateName
        
        if userData == nil then
            userData = d.UserData
        end
    end
    
    if not MVCService:IsAllowSwitchUIState(StateConstant.PopChild, currentStateName, stateName, userData) then
        return
    end
    self._service._switching = true
    
    if stateName ~= "" and AssetService:Load(nil, stateName, self.OnLoadCompleted) then
        self._cacheOperator = StateConstant.PopChild
        self._cacheUserData = userData == nil and null or userData
        return
    end

    self:ImplPopChildState(userData)

    self._service._switching = false
    MVCService:PostSwitchState(false, StateConstant.PopChild)
end

function StateUIControl.OnLoadCompleted()

    local self = this
    if self ~=nil then
        return
    end
    local constant = StateConstant
    
    local userData = self._cacheUserData
    if userData == null then
        userData = nil
    end
    
    if self._cacheOperator == constant.Change then
        self:ImplChangeState(self._cacheStateName, userData)
        
    elseif self._cacheOperator == constant.Push then
        self:ImplPushState(self._cacheStateName, userData)
    
    elseif self._cacheOperator == constant.Pop then
        self:ImplPopState(userData)
        
    elseif self._cacheOperator == constant.PushChild then
        self:ImplPushChildState(self._cacheStateName, userData)
        
    elseif self._cacheOperator == constant.PopChild then
        self:ImplPopChildState(userData)
        
    elseif self._cacheOperator == constant.ChangeTop then
        self:ImplChangeTopState(self._cacheStateName, userData)
        
    elseif self._cacheOperator == constant.ChangeAll then
        self:ImplChangeAllUIState(self._cacheStateName, userData)
    end
    
    self._service._switching = false
    self._cacheOperator = false
    self._cacheStateName = false
    self._cacheUserData = null

    MVCService:PostSwitchState(false, self._cacheOperator)
end

function StateUIControl:ImplChangeAllUIState(stateName, userData, dontClearCache)
    -- data
    self._root = false
    
    for i = #self._data, 1, -1 do
        self:DestroyData(self._data[i])
        table.remove(self._data, i)
    end
    
    local rootStateName, systemStateName, childStateName = self:SplitStateName(stateName)
    if rootStateName ~= "" then
        self._root = rootStateName
    end
    
    if systemStateName ~= "" then
        local ud = nil
        if childStateName == "" then
            ud = userData
        end
        
        self:ImplPushState(systemStateName, ud, true)
    end
    
    if childStateName ~= "" then
        self:ImplPushChildState(childStateName, userData, true)
    end
    
    -- logic
    self:UpdateStateEnvironment()
    self:SendMessage(StateConstant.Change, userData, nil, dontClearCache)
end

function StateUIControl:ImplChangeState(stateName, userData)
    -- data
    for i = #self._data, 1, -1 do
        self:DestroyData(self._data[i])
        table.remove(self._data, i)
    end
    
    self:ImplPushState(stateName, userData, true)
        
    -- logic
    self:UpdateStateEnvironment()
    self:SendMessage(StateConstant.Change, userData)
end

function StateUIControl:ImplChangeTopState(stateName, userData)
    -- data
    if #self._data > 0 then
        self:DestroyData(table.remove(self._data))
    end
    
    self:ImplPushState(stateName, userData, true)
    
    -- logic
    self:UpdateStateEnvironment()
    self:SendMessage(StateConstant.Change, userData)
end

function StateUIControl:ImplPushState(stateName, userData, onlyData)
    --data
    local systemStateName, childStateName = self:SplitStateName(stateName)
    if childStateName ~= "" then
        self:AddStateName(self._data, systemStateName, nil)
        self:ImplPushChildState(childStateName, userData, true)
    else
        self:AddStateName(self._data, systemStateName, userData)
    end
    
    if onlyData then
        return
    end
    
    -- logic
    self:UpdateStateEnvironment()
    self:SendMessage(StateConstant.Push, userData)
end

function StateUIControl:ImplPopState(userData)
    -- data
    if #self._data > 0 then
        self:DestroyData(table.remove(self._data))
    end
    
    local data = #self._data > 0 and self._data[#self._data] or nil
    local childUserData = nil
    if data then
        if userData ~= nil then
            data.UserData = userData
        else
            userData = data.UserData
        end
        
        self:AddHistory(data.StateName)
        
        if data.Child then
            self:AddHistory(data.Child[#data.Child].StateName)
            childUserData = data.Child[#data.Child].UserData
        end
    end

    -- logic
    self:UpdateStateEnvironment()
    self:SendMessage(StateConstant.Pop, userData, childUserData)
end

function StateUIControl:ImplPushChildState(stateName, userData, onlyData)
    -- data
    if #self._data == 0 then
        return
    end
    
    local data = self._data[#self._data]
    if not data.Child then
        data.Child = UPool:CreateTable()
    end
    
    self:AddStateName(data.Child, stateName, userData)
    if #data.Child == 0 then
        UPool:DestroyTable(data.Child)
        data.Child = nil
    end
    
    if onlyData then
        return
    end
        
    -- logic  
    self:UpdateStateEnvironment()
    self:SendMessage(StateConstant.PushChild, userData)
end

function StateUIControl:ImplPopChildState(userData)
    -- data
    if #self._data == 0 then
        return
    end
    
    local data = self._data[#self._data]
    if not data.Child then
        return
    end
    
    self:DestroyData(table.remove(data.Child))
    
    if #data.Child == 0 then
        UPool:DestroyTable(data.Child)
        data.Child = nil
        
        self:AddHistory(data.StateName)
    else
        local childData = data.Child[#data.Child]
        if userData ~= nil then
            childData.UserData = userData
        else
            userData = childData.UserData
        end
        
        self:AddHistory(childData.StateName)
    end
        
    -- logic
    self:UpdateStateEnvironment()
    self:SendMessage(StateConstant.PopChild, userData)
end

function StateUIControl:AddHistory(stateName)
    for i, v in ipairs(self._history) do
        if v == stateName then
            table.remove(self._history, i)
            break
        end
    end
    
    self._history[#self._history+1] = stateName
    
    if #self._history > 20 then
        table.remove(self._history, 1)
    end
end

function StateUIControl:UpdateStateEnvironment()
    local stateName = self:GetCurrentChildStateName()
    if not stateName or stateName == "" then
        stateName = self:GetCurrentStateName()
    end
    
    if not stateName or stateName == "" then
        stateName = self._root
    end
    
    if not stateName then
        stateName = ""
    end
    
    local layout = nil
    stateName = self:GetCurrentChildStateName()
    if stateName and stateName ~= "" then
        layout = self._config[stateName]
    end
    
    if not layout then
        stateName = self:GetCurrentStateName()
        if stateName and stateName ~= "" then
            layout = self._config[stateName]
        end
    end
    
    if not layout then
        if self._root and self._root ~= "" then
            layout = self._config[self._root]
        end
    end
end
function StateUIControl:SplitStateName(stateName)
    if not stateName or stateName == "" then
        return "", "", ""
    end
    
    return string.match(stateName, "([%w_-]*)/?([%w_-]*)/?([%w_-]*)/?")
end

function StateUIControl:TranslateStateNameToResource(stateName)
    return string.gsub(stateName, "[%w_]*%-", "")
end

function StateUIControl:AddStateName(list, stateName, userData)
    if not stateName or stateName == "" then
        return
    end
    
    for v in string.gmatch(stateName, "([%w_]+)") do
        for i = 1, #list do
            if list[i].StateName == v then
                self:DestroyData(list[i])
                table.remove(list, i)
                break
            end
        end
        
        local data = UPool:CreateTable()
        data.StateName = v
        list[#list + 1] = data
        self:AddHistory(v)
    end
    
    if #list > 0 then
        list[#list].UserData = userData
    end
end

function StateUIControl:DestroyData(data)
    local pool = UPool
    if data.Child then
        for i = 1, #data.Child do
            pool:DestroyTable(data.Child[i])
        end
        
        pool:DestroyTable(data.Child)
        data.Child = nil
    end
    
    pool:DestroyTable(data)
end

function StateUIControl:SendMessage(switchType, userData, childUserData, dontClearCache)
    local mvcService = MVCService
    local constant = StateConstant
    
    local rootStateName = self:GetCurrentRootStateName()
    local stateName = self:GetCurrentStateName()
    local childStateName = self:GetCurrentChildStateName()

    if childUserData == nil then
        childUserData = userData --默认使用UI状态的参数，保持和原来的逻辑一致
    end
    
    mvcService:SwitchUIState(constant.Out, switchType, rootStateName, stateName, childStateName, userData, childUserData)
    
    if not dontClearCache then
        self._service:ClearCache()
    end
    
    mvcService:SwitchUIState(constant.In, switchType, rootStateName, stateName, childStateName, userData, childUserData)
    mvcService:SwitchUIState(constant.Stay, switchType, rootStateName, stateName, childStateName, userData, childUserData)
end

