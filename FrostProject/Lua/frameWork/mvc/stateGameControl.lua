
---@class StateGameControl
local StateGameControl = Class("StateGameControl")
local this = false


---@param service StateService
function StateGameControl:ctor(service)
    this = self
    self._service = service
    
    self._data = {}  -- array of string
    self._forecast = {} -- array of string
    
    self._cacheOperator = false
    self._cacheUserData = null
    self._cacheUIStateName = false
    self._cacheLoadUI = false
end

function StateGameControl:GetStateName(index)
    index = index or 0
    return #self._data > index and self._data[#self._data - index] or ""
end

function StateGameControl:GetAllStateName()
    return table.concat(self._data, "/")
end

function StateGameControl:ChangeState(stateName, userData, forecastStateName, uiStateName, dontLoadResource, sceneID, loadUI)
    FrostLogD(self.__classname, string.format("--> StateGameControl.ChangeState -1 : %s", stateName))
    if not self:IsValidateState(stateName) then
        return
    end
    
    FrostLogD(self.__classname, string.format("--> StateGameControl.ChangeState 0 : %s", stateName))
    if self._service._switching then
        FrostLogE(self.__classname, string.format("StateGameControl:ChangeState : state is switching - %s", stateName))
        return
    end
    
    local constant = StateConstant
    local mvcService = MVCService
    FrostLogD(self.__classname, string.format("--> StateGameControl.ChangeState 1 : %s", stateName))
    if not mvcService:IsAllowSwitchGameState(constant.Change, self:GetStateName(), stateName, userData) then
        return
    end
    
    FrostLogD(self.__classname, string.format("--> StateGameControl.ChangeState 2 : %s", stateName), self._data)
    self._service._switching = true         
     
     if uiStateName then
         self._service:ChangeAllUIState(nil, nil, true)
     end
     
     for i = #self._data, 1, -1 do
         mvcService:SwitchGameState(constant.Out, constant.Change, self._data[i], userData)
     end

      -- data
    table.Clear(self._data)
    table.Clear(self._forecast)
    
    self._data = string.split(stateName, "/", 1, true)
    if forecastStateName then
        self._forecast = string.split(forecastStateName, ",", 1, true)
    end

    self._service:ClearCache(true)
    
    -- resource
    local uiResource = uiStateName
    if uiResource and uiResource ~= "" then
        uiResource = string.gsub(uiResource, "[%w_]*%-", "")
    end

    -- if not dontLoadResource then
    --     self._cacheOperator = StateConstant.Change
    --     self._cacheUserData = userData == nil and null or userData
    --     self._cacheUIStateName = uiStateName or false
    --     self._cacheLoadUI = loadUI
    --     return
    -- end

    self:ImplChangeState(userData, uiStateName, loadUI)

    self._service._switching = false
    mvcService:PostSwitchState(true)
end

function StateGameControl:PushState(stateName, userData, uiStateName, dontLoadResource, sceneID, loadUI)
    if not self:IsValidateState(stateName) then
        return
    end
    
    if self._service._switching then
        FrostLogE(self.__classname, string.format("StateGameControl:PushState : state is switching - %s", stateName))
        return
    end
    
    local mvcService = MVCService
    if not mvcService:IsAllowSwitchGameState(StateConstant.Push, "", stateName, userData) then
        return
    end
    
    FrostLogD(self.__classname, string.format("StateGameControl.PushState : %s", stateName))
    self._service._switching = true

     -- 加载界面
     loadUI = loadUI or AssetLoadUI.LoadUI_Scene
     local csLoadUI = AssetService:PreprocessLoad(loadUI)
     
     if uiStateName then
         mvcService:SwitchUIState(StateConstant.Out, StateConstant.Push, self._service:GetCurrentUIRootStateName(), "", "", nil)
     end
     
     -- data
     self._data[#self._data+1] = stateName
     self:RemoveForecast(stateName)

     AssetService:SetStateName(self:GetStateName(), nil)
    
     -- resource
     local uiResource = uiStateName
     if uiResource and uiResource ~= "" then
         uiResource = string.gsub(uiResource, "[%w_]*%-", "")
     end

    --  if not dontLoadResource and AssetService:Load(stateName, uiResource, csLoadUI, self.OnLoadCompleted, nil, sceneID) then
    --      self._cacheOperator = StateConstant.Push
    --      self._cacheUserData = userData == nil and null or userData
    --      self._cacheUIStateName = uiStateName or false
    --      self._cacheLoadUI = loadUI
    --      return
    --  end

     self:ImplPushState(userData, uiStateName, loadUI)
     
     self._service._switching = false
     mvcService:PostSwitchState(true)
end

function StateGameControl:PopState(userData, forecastStateName)
    if #self._data == 0 then
        FrostLogE(self.__classname, "StateGameControl:PopState : empty state")
        return
    end
    
    if self._service._switching then
        FrostLogE(self.__classname, "StateGameControl:PopState : state is switching")
        return
    end
    
    local constant = StateConstant
    local mvcService = MVCService
    if not mvcService:IsAllowSwitchGameState(constant.Pop, self:GetStateName(), "", userData) then
        return
    end
    
    FrostLogD(self.__classname, "StateGameControl.PopState")
    self._service._switching = true
    
    -- leave
    mvcService:SwitchGameState(constant.Out, constant.Pop, self:GetStateName(), userData)
    
    -- data
    local stateName = table.remove(self._data)
    if #self._data == 0 then
        table.Clear(self._forecast)
    else
        if forecastStateName then
            if forecastStateName == constant.ForecastPopState then
                self._forecast[#self._forecast+1] = stateName
            else
                string.split(forecastStateName, ",", self._forecast)
            end
        end
    end

    AssetService:SetStateName(self:GetStateName(), nil)

    if CS.UnityEngine.Application.isIOS then
        self._service:ClearCache(true)
    end

    self._service._switching = false
    mvcService:PostSwitchState(true)
end

function StateGameControl.OnLoadCompleted()
    local self = this
    local constant = StateConstant
    
    local userData = self._cacheUserData
    if userData == null then
        userData = nil
    end
    
    if self._cacheOperator == constant.Change then
        self:ImplChangeState(userData, self._cacheUIStateName or nil, self._cacheLoadUI)
        
    elseif self._cacheOperator == constant.Push then
        self:ImplPushState(userData, self._cacheUIStateName or nil, self._cacheLoadUI)
    end
    
    self._service._switching = false
    self._cacheOperator = false
    self._cacheUserData = null
    self._cacheUIStateName = false
    self._cacheLoadUI = false
    
    MVCService:PostSwitchState(true)
end

function StateGameControl:ImplChangeState(userData, uiStateName, loadUI)
    local mvcService = MVCService
    local constant = StateConstant
    
    for _, v in ipairs(self._data) do
        mvcService:SwitchGameState(constant.In, constant.Change, v, userData)
    end
    if uiStateName then
        FrostLogD(self.__classname,"StateGameControl:ImplChangeState uiStateName",uiStateName)
        self._service:ChangeAllUIState(uiStateName, userData, true)
    end
    
    -- AssetService:PostprocessLoad(loadUI)
end

function StateGameControl:ImplPushState(userData, uiStateName, loadUI)
    local constant = StateConstant
    
    local data = self._data[#self._data]
    MVCService:SwitchGameState(constant.In, constant.Push, data, userData)
    
    if uiStateName then
        self._service:PushUIState(uiStateName, nil, true)
    end
    
    AssetService:PostprocessLoad(loadUI)
end

function StateGameControl:RemoveForecast(stateName)
    for i = #self._forecast, 1, -1 do
        if self._forecast[i] == stateName then
            table.remove(self._forecast, i)
        end
    end
end

function StateGameControl:IsValidateState(stateName)
    if not stateName or stateName == "" then
        FrostLogD(self.__classname, "--> StateGameControl:IsValidateState : invalid parameter")
        return false
    end
    
    for _, v in ipairs(self._data) do
        if v == stateName then
            FrostLogD(self.__classname, string.format("--> StateGameControl:IsValidateState : already in state - %s", stateName))
            return false
        end
    end
    
    return true
end