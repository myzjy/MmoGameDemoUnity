--[[
    author : zhangjingyi

    作用 ： mvc 服务
--]]

---@class MVCService:ServiceBase
local MVCService = Class("MVCService", ClassLibraryMap.ServiceBase)

function MVCService:ctor()
    --- 管理本地全局 Module 类
    self._module = {}

   -- 键位输入回调
   self._inputMapDelegate = {}              -- 生效事件
   self._inputDelegateQueue = {}            -- 生效事件队列，保序执行
   self._inputPendingExecDelegateQueue = {} -- 等待执行
   -- ｛DTPath => refCount｝
   self._inputSystemContextData = {}
   
   self._uiStatePreChangeDelegate = {}
   self._modalCloseCallback = {}
   self._specialAndroidBackCallback = {}
   self._unmanagedPrefabClassCallBack = {}
   self._bindFunc = false
   self.LastPinchValue = 1
   self.IsPinching = false
   self.LastTouchX = false
   self.LastTouchY = false

   self._prefabClass = {}
   self._asyncPrefabInfo = {}
   self._prefabCustomID = 1

   -- 缓存当前显示的Prefab类型列表
   self._showPrefabTypeList = {}
end

------------------------------------------------------------------------
--- 返回类的静态配置数据
---@return table
------------------------------------------------------------------------
function MVCService:vGetConfig()
    return {
        name = "MVCService",
    }
end

--- 执行初始化
function MVCService:vInitialize()
    self:_onLoadModuleConfig()
end

function MVCService:vDeinitialize()
    EventService:UnListenEvent(self)
end

function MVCService:OnModalClose(prefabPath)
    for index = #self._modalCloseCallback, 1, -1 do
        local data = self._modalCloseCallback[index]
        if data and data ~= nil then
            data.func(data.tbl, prefabPath)
        end
    end
    table.removeNullFromArray(self._modalCloseCallback)
end

function MVCService:OnDisableUIPrefabRender(inPrefabPath)
    self:_DoEachModule(function(inModuleInstance)
        inModuleInstance:DisableUIPrefabRender(inPrefabPath)
    end)
end

function MVCService:IsAllowSwitchGameState(inSwitchType, inCurrentState, inTargetState, inUserData)
    local tIsAllowSwitch = self:_DoEachModule(function (inModuleInstance)
        if not inModuleInstance:IsAllowSwitchGameState(inSwitchType, inCurrentState, inTargetState, inUserData) then
            return false
        end
    end)

    if tIsAllowSwitch ~= false then
        return false
    end

    self:_DoEachModule(function (inModuleInstance)
        inModuleInstance:vOnStatePrevSwitch(true, inSwitchType, inCurrentState, inTargetState, inUserData)
    end)

    return true
end

function MVCService:OnShowFullScreen(show)
    self:_DoEachModule(function (inModuleInstance)
        inModuleInstance:ShowFullScreen(show)
    end)
end

function MVCService:SwitchGameState(operator, inSwitchType, inStateName, inUserData)
    self:_DoEachModule(function (inModuleInstance)
        inModuleInstance:SwitchGameState(operator, inSwitchType, inStateName, inUserData)
    end)
end

function MVCService:IsAllowSwitchUIState(inSwitchType, outStateName, inStateName, userData)
    for i = #self._uiStatePreChangeDelegate, 1, -1 do
        local data = self._uiStatePreChangeDelegate[i]
        if data and data ~= null then
            if not data.func(data.tbl, inSwitchType, outStateName, inStateName, userData) then
                return false
            end
        end
    end
    
    table.removeNullFromArray(self._uiStatePreChangeDelegate)
    
    self:_DoEachModule(function(inModuleInstance)
        inModuleInstance:vOnStatePrevSwitch(false, inSwitchType, outStateName, inStateName, userData)
    end)
    
    return true
end
function MVCService:SwitchUIState(operator, switchType, rootStateName, stateName, childStateName, userData, childUserData, onlyEntityModule)
    
    --local StartTimeMS = NGRHelper.GetTimestampNowMs()
    local data = operator == StateConstant.In or nil
    
    self:_DoEachModule(function(inModuleInstance)
        inModuleInstance:SwitchUIState(operator, switchType, rootStateName, stateName, childStateName, userData, data)
    end, onlyEntityModule)
    
    if data then
        if rootStateName and rootStateName ~= "" then
            for i = 1, #data, 2 do
                if data[i] == rootStateName then
                    data[i + 1]:SwitchUIStateIn(switchType, userData)
                end
            end
        end
    
        if stateName and stateName ~= "" then
            for i = 1, #data, 2 do
                if data[i] == stateName then
                    data[i + 1]:SwitchUIStateIn(switchType, userData)
                end
            end
            AssetService:RecordChangeUIState(stateName)
        end
        
        if childStateName and childStateName ~= "" then
            for i = 1, #data, 2 do
                if data[i] == childStateName then
                    data[i + 1]:SwitchUIStateIn(switchType, childUserData)
                end
            end
        end
        
        data = false
    end
end

function MVCService:PostSwitchState(kind, switchType)
    self:_DoEachModule(function(inModuleInstance)
        inModuleInstance:vOnStatePostSwitch(kind, switchType)
    end)
end

function MVCService:GetPrepareAssetData(collection)
    self:_DoEachModule(function(inModuleInstance)
        inModuleInstance:GetPrepareAssetData(collection)
    end)
end

function MVCService.ActivePrefabClassCookie(active, classname, csInstanceID, rootGo, rootTf, uiPrefab, csPrefabClass)
    local self = MVCService
    self:_DoEachModule(function(inModuleInstance)
        inModuleInstance:ActivePrefabClassCookie(active, classname, csInstanceID, rootGo, rootTf, uiPrefab, csPrefabClass)
    end)
end

function MVCService:RegisterPrefabClassCookie(name, bind)
    --local interaction = self._luaInteraction
    --interaction.RegisterPrefabClassCookie(name, bind)
end

-- 添加/移除Android返回键特殊处理回调
-- @param add 添加/移除
-- @param tbl 接收事件的表对象
-- @param func 接收事件的成员函数对象
-- @return 无
function MVCService:AddRemoveSpecialAndroidBackCallback(add,tbl,func)
    if not tbl then
        FrostLogE(self.__classname, "MVCService.AddRemoveSpecialAndroidBackCallback : invalid parameter")
        return
    end
    
    if add then
        if not func then
            FrostLogE(self.__classname, "MVCService.AddRemoveSpecialAndroidBackCallback : invalid parameter")
            return
        end
        
        for _, v in ipairs(self._specialAndroidBackCallback) do
            if v ~= null and v.tbl == tbl then
                FrostLogE(self.__classname, "MVCService.AddRemoveSpecialAndroidBackCallback : duplicate add - %s", tbl.__classname)
                return
            end
        end
        
        local data = Pool:CreateTable()
        data.tbl = tbl
        data.func = func
        self._specialAndroidBackCallback[#self._specialAndroidBackCallback + 1] = data
    else
        for i = #self._specialAndroidBackCallback, 1, -1 do
            local data = self._specialAndroidBackCallback[i]
            if data.tbl == tbl then
                table.remove(self._specialAndroidBackCallback,i)
                data = {}
                return
            end
        end
    end
end

-- 添加/移除获得未管理PrefabClass回调
-- @param add 添加/移除
-- @param tbl 接收事件的表对象
-- @param func 接收事件的成员函数对象
-- @return 无
function MVCService:AddRemoveUnmanagedPrefabClassCallBack(add,tbl,func)
    if not tbl then
        FrostLogE(self.__classname, "MVCService.AddRemoveUnmanagedPrefabClassCallBack : invalid parameter")
        return
    end
    
    if add then
        if not func then
            FrostLogE(self.__classname, "MVCService.AddRemoveUnmanagedPrefabClassCallBack : invalid parameter")
            return
        end
        
        for _, v in ipairs(self._unmanagedPrefabClassCallBack) do
            if v ~= null and v.tbl == tbl then
                FrostLogE(self.__classname, "MVCService.AddRemoveUnmanagedPrefabClassCallBack : duplicate add - %s", tbl.__classname)
                return
            end
        end
        
        local data = {}
        data.tbl = tbl
        data.func = func
        self._unmanagedPrefabClassCallBack[#self._unmanagedPrefabClassCallBack + 1] = data
    else
        for i = #self._unmanagedPrefabClassCallBack, 1, -1 do
            local data = self._unmanagedPrefabClassCallBack[i]
            if data.tbl == tbl then
                table.remove(self._unmanagedPrefabClassCallBack,i)
                data = {}
                return
            end
        end
    end
end
-- 查找UIPrefab对应的PrefabClass
-- @param uiPrefab UIPrefab对象
-- @return 获取到的UIPrefabClass
function MVCService:GetPrefabClassByUIPrefab(uiPrefab)
    return self:_DoEachModule(function(inModuleInstance)
        return inModuleInstance:GetPrefabClassByUIPrefab(uiPrefab)
    end)
end

-- 查找PrefabName对应的PrefabClass
-- @param prefabName Prefab名字
-- @return 获取到的UIPrefabClass
function MVCService:GetPrefabClassByName(prefabName)
    return self:_DoEachModule(function(inModuleInstance)
        return inModuleInstance:GetPrefabClassByName(prefabName)
    end)
end

-- 查找mediatorName对应的MediatorClass
-- @param mediatorName Mediator名字
-- @return 获取到的MediatorClass
-- 说明：新手引导在使用该函数，在跨平台而UI不同的Module（比如技能UI）中，需要得到Mediator，去获取相应的UIPrefabClass
function MVCService:GetMediatorClassByName(mediatorName)
    return self:_DoEachModule(function(inModuleInstance)
        return inModuleInstance:GetMediatorClassByName(mediatorName)
    end)
end

--- 创建模块时间
---@param inModuleClass any 模块类定义
---@param ... any
function MVCService:OnCreateModule(inModuleClass,...)
    local md = inModuleClass()
    md:Create()

    for i = 1, select('#' ,...) do
        local arg = select(i,  ...)
        md:CreateMediatar(arg)
    end
    self._module[#self._module+1] = md
    return md
end


function MVCService:_onLoadModuleConfig()
    local tAllModulesConfigMap = require("game.module.gameUIModuleConfig")
    local tGlobalModulesConfigMap = {}
    for tModuleName, tModuleConfigData in pairs(tAllModulesConfigMap) do
        tGlobalModulesConfigMap[tModuleName] = tModuleConfigData
    end
    for tModuleName, tModuleConfigData in pairs(tGlobalModulesConfigMap) do
        tGlobalModulesConfigMap[tModuleName] = tModuleConfigData
    end
    for tModuleName, tModuleConfigData in pairs(tGlobalModulesConfigMap) do
        local tOnResult = xpcall(self._onLoadModuleConfig, __G__TRACKBACK__, self, tModuleConfigData)
        if not tOnResult then
            FrostLogE(self.__classname, string.format("UIModule( %s ) load failed!", tModuleName))
        end
    end
end

function MVCService:_onCreateGlobalModule(inModuleConfigData)
    for tIndex, tFilePath in pairs(inModuleConfigData.filesPath) do
        require(tFilePath)
    end
    local tLoadModuleClass = ClassLibraryMap[inModuleConfigData.moduleClassName]
    if not tLoadModuleClass then
        FrostLogE(self.__classname, "Can't find module class with", inModuleConfigData.ModuleClassName)
        return
    end
    local  tMediatorClassList = {}
    if inModuleConfigData.moduleClassName then
        for tIndex, tMediatorClassName in pairs(inModuleConfigData.mediatorClassName) do
            local tMediatorDataClass = ClassLibraryMap[tMediatorClassName]
            if not tMediatorDataClass then
                FrostLogE(self.__classname, "Can't find mediator class with", tMediatorClassName)
            end
            tMediatorClassList[tIndex] = tMediatorDataClass
        end
    end
    local md = self:OnCreateModule(tLoadModuleClass)
    
    return md
end

------------------------------------------------------------------------------------------------
--- 遍历所有的module，指定的方法
--- @param inFunc function
--- @param ... unknown
--- @return boolean
------------------------------------------------------------------------------------------------
function MVCService:_DoEachModule(inFunc, ...)
    local xpCallRes = nil
    local tReturnValue = nil
    for tIndex = 1, #self._module do
        xpCallRes, tReturnValue = xpcall(inFunc, __G__TRACKBACK__, self._module[tIndex], ...)
    if nil ~= tReturnValue then return tReturnValue end
    end
end

-- 添加/移除模态关闭事件
-- @param add 添加/移除
-- @param tbl 接收事件的表对象
-- @param func 接收事件的成员函数对象，原型：
--              void Func(string prefabPath)
--                  prefabPath - 想要关闭的Prefab资源路径，如：UIPrefab/GMCommand/Base/UIPopupGM
-- @return 无
function MVCService:AddRemoveModalCloseCallback(add, tbl, func)
    if not tbl then
        FrostLogE(self.__classname, "MVCService.AddRemoveModalCloseCallback : invalid parameter")
        return
    end
    
    if add then
        if not func then
            FrostLogE(self.__classname, "MVCService.AddRemoveModalCloseCallback : invalid parameter")
            return
        end
        
        for _, v in ipairs(self._modalCloseCallback) do
            if v ~= null and v.tbl == tbl then
                FrostLogE(self.__classname, "MVCService.AddRemoveModalCloseCallback : duplicate add - %s", tbl.__classname)
                return
            end
        end
        
        local data = Pool:CreateTable()
        data.tbl = tbl
        data.func = func
        self._modalCloseCallback[#self._modalCloseCallback + 1] = data
    else
        for i, v in ipairs(self._modalCloseCallback) do
            if v ~= null and v.tbl == tbl then
                self._modalCloseCallback[i] = null
                Pool:DestroyTable(v)
                return
            end
        end
    end
end

-------------------------------------------------------------------------------------------
-- 脱离Mediator在执行的UWidget下创建的独立的Prefab，需要外部管理生命周期对称执行 DestroyWidgetChildPrefab
-- @param inUIPrefabClass(LuaTable) UIPrefabClass 的子类
-- @param inParentWidget(UWidget) Prefab 挂载的父节点
-- @param inArgument(LuaTable) inUIPrefabClass 所需的初始化参数
-- @param inStyle(number) inUIPrefabClass 中可用的样式
-- @param inLayer(number) 层级设定
-- @param customID 自定义ID
-------------------------------------------------------------------------------------------
function MVCService:CreateWidgetChildPrefab(inUIPrefabClass, inParentWidget, inArgument, inStyle, inLayer,customID,asyncLoadID)
    if not inUIPrefabClass or not LuaHelp.IsChildOfClass(inUIPrefabClass, "UIPrefabClass") then
        FrostLogE(self.__classname, "CreateWidgetChildPrefab with invalid UIPrefabClass")
        return
    end
    if not LuaHelp.IsValidObject(inParentWidget) then
        FrostLogE(self.__classname, "CreateWidgetChildPrefab with invalid parent widget")
        return
    end
    local tPrefab = inUIPrefabClass:New()
    self._prefabClass[#self._prefabClass+1] = tPrefab
    tPrefab:Create(nil, nil, inParentWidget, inArgument, customID, inStyle, inLayer,asyncLoadID)
    return tPrefab
end

-------------------------------------------------------------------------------------------
-- 脱离Mediator在执行的UWidget下异步创建的独立的Prefab，需要外部管理生命周期对称执行 DestroyWidgetChildPrefabByAsyncId
-- @param inUIPrefabClass(LuaTable) UIPrefabClass 的子类
-- @param inParentWidget(UWidget) Prefab 挂载的父节点
-- @param inArgument(LuaTable) inUIPrefabClass 所需的初始化参数
-- @param inStyle(number) inUIPrefabClass 中可用的样式
-- @param inLayer(number) 层级设定
-- @param inCallBackTbl(LuaTable) 回调对象
-- @param inCallBackFunc(function) 回调方法
-- @retun customID 异步加载id（当Prefab创建成功后，也作为Prefab的CustomID使用）
-------------------------------------------------------------------------------------------
function MVCService:CreateAsyncWidgetChildPrefab(inUIPrefabClass, inParentWidget, inArgument, inStyle, inLayer,inLoadPriority,inLife,inCallBackTbl,inCallBackFunc)
    if not inUIPrefabClass or not LuaHelp.IsChildOfClass(inUIPrefabClass, "UIPrefabClass") then
        FrostLogE(self.__classname, "CreateAsyncWidgetChildPrefab with invalid UIPrefabClass")
        return
    end
    if not LuaHelp.IsValidObject(inParentWidget) then
        FrostLogE(self.__classname, "CreateAsyncWidgetChildPrefab with invalid parent widget")
        return
    end

    local customID = self._prefabCustomID
    self._prefabCustomID = self._prefabCustomID + 1

    local obj = ClassLibraryMap.UIPrefabClassAsyn:New()
    self._asyncPrefabInfo[#self._asyncPrefabInfo+1] = {
        AsyncPrefab = obj,
        CallBackTbl = inCallBackTbl,
        CallBackFunc = inCallBackFunc,
        Layer = inLayer,
    }
    obj:Create(nil, self, inParentWidget, inUIPrefabClass, inArgument, inLoadPriority or LoadPriority.SpareShow, inLife or LifeType.Immediate, customID, inStyle, inLayer)

    return customID
end

---异步加载ui成功
---@param async UIPrefabClassAsyn 异步ui处理
---@param cls UIPrefab UI界面类定义
---@param parentTf UPanelWidget UE中的父控件（即创建出的控件所要挂载的地方）
---@param argument table 自定义参数，用于vInitialize时读取
---@param customID string|number 自定义索引，给父UI查找用（即父UI可以通过这个ID找到这个UI）
---@param style number ui样式索引，对应vGetPath里的资源路径
---@param asyncLoadID number 异步加载任务ID
function MVCService:CreateChildPrefabClassAsynCompleted(async, cls, parentTf, argument, customID, style, asyncLoadID)
    local prefabClassAsynInfo = false
    for i = 1, #self._asyncPrefabInfo do
        if self._asyncPrefabInfo[i].AsyncPrefab == async then
            prefabClassAsynInfo = self._asyncPrefabInfo[i]
            table.remove(self._asyncPrefabInfo, i)
            break
        end
    end

    if prefabClassAsynInfo then
        local tPrefab = self:CreateWidgetChildPrefab(cls,parentTf,argument,style,prefabClassAsynInfo.Layer,customID,asyncLoadID)
        if prefabClassAsynInfo.CallBackFunc and type(prefabClassAsynInfo.CallBackFunc) == "function" then
            prefabClassAsynInfo.CallBackFunc(prefabClassAsynInfo.CallBackTbl,tPrefab,customID)
        end
    end
end

-------------------------------------------------------------------------------------------
-- 销毁通过 CreateWidgetChildPrefab 创建的实例
-- @param inPrefabInstance(LuaTable) 通过 CreateWidgetChildPrefab 创建的 UIPrefabClass 的子类实例
-------------------------------------------------------------------------------------------
function MVCService:DestroyWidgetChildPrefab(inPrefabInstance)
    if not inPrefabInstance then return end
    if not LuaHelp.IsChildOfClass(inPrefabInstance, "UIPrefabClass") then
        FrostLogE(self.__classname, "DestroyWidgetChildPrefab with invalid UIPrefabClass")
        return
    end

    for i = 1, #self._prefabClass do
        if self._prefabClass[i] == inPrefabInstance then
            table.remove(self._prefabClass, i)
            break
        end
    end

    self:RemovePrefab(inPrefabInstance, inPrefabInstance._parentTf)
    inPrefabInstance:Destroy()
end

--- 销毁通过 CreateAsyncWidgetChildPrefab 创建的实例
--- @param customID number CreateAsyncWidgetChildPrefab的返回值
function MVCService:DestroyWidgetChildPrefabByAsyncId(customID)
    if not customID then
        FrostLogE(self.__classname, "MVCService.DestroyWidgetChildPrefabByAsyncId : invalid parameter")
        return
    end

    for i = #self._prefabClass, 1, -1 do
        local pc = self._prefabClass[i]
        if pc._customID == customID then
            self:DestroyWidgetChildPrefab(pc)
            return -- 已完成销毁提前结束
        end
    end
    
    for i = #self._asyncPrefabInfo, 1, -1 do
        local prefabClassAsynInfo = self._asyncPrefabInfo[i]
        if prefabClassAsynInfo and prefabClassAsynInfo.AsyncPrefab and prefabClassAsynInfo.AsyncPrefab._customID == customID then
            prefabClassAsynInfo.AsyncPrefab:Destroy()
            table.remove(self._asyncPrefabInfo, i)
            break
        end
    end
end

-------------------------------------------------------------------------------------------
-- 销毁所有通过 CreateWidgetChildPrefab 创建的实例和通过 CreateAsyncWidgetChildPrefab 请求的正在执行的异步创建
-------------------------------------------------------------------------------------------
function MVCService:DestroyAllWidgetChildPrefab()
    for tIndex, tPrefab in ipairs(self._prefabClass) do
        self:RemovePrefab(tPrefab, tPrefab._parentTf)
        tPrefab:Destroy()
    end
    table.Clear(self._prefabClass)
    for tIndex, tAsyncPrefabInfo in ipairs(self._asyncPrefabInfo) do
        tAsyncPrefabInfo.AsyncPrefab:Destroy()
    end
    table.Clear(self._asyncPrefabInfo)
end

-------------------------------------------------------------------------------------------
-- 根据自定义ID获取创建的Prefab状态
-- @param inCustomID(*) 自定义的ID，通常为CreateAsyncWidgetChildPrefab的返回值
-- @return(#1) 如果已创建、或者正在创建，返回true
-- @return(#2) 如果已创建，返回创建的实例
-------------------------------------------------------------------------------------------
function MVCService:HasWidgetChildPrefab(inCustomID)
    for tIndex, tPrefab in ipairs(self._prefabClass) do
        if tPrefab:GetCustomID() == inCustomID then
            return true, tPrefab
        end
    end
    for tIndex, tAsyncPrefabInfo in ipairs(self._asyncPrefabInfo) do
        if tAsyncPrefabInfo._customID == inCustomID then
            return true
        end
    end
    return false
end

-------------------------------------------------------------------------------------------
-- 脱离Mediator在执行的UWidget下创建的独立的Prefab，需要外部管理生命周期对称执行 DestroyWidgetComponentPrefab
-- @param inUIPrefabClass(LuaTable) UIPrefabClass 的子类
-- @param inWidgetComponent(UWidgetComponent) UWidgetComponent 挂载的WidgetComponent
-- @param inArgument(LuaTable) inUIPrefabClass 所需的初始化参数
-- @param inStyle(number) inUIPrefabClass 中可用的样式
-- @param inIsAdapterWidgetSize(bool) WidgetCompontent是否自动适配inUIPrefabClass的Size
-------------------------------------------------------------------------------------------
function MVCService:CreateWidgetComponentPrefab(inUIPrefabClass, inWidgetComponent, inArgument, inStyle, inIsAdapterWidgetSize)
    if not inUIPrefabClass or not LuaHelp.IsChildOfClass(inUIPrefabClass, "UIPrefabClass") then
        FrostLogE(self.__classname, "CreateWidgetComponentPrefab with invalid UIPrefabClass")
        return
    end
    if not LuaHelp.IsValidObject(inWidgetComponent) then
        FrostLogE(self.__classname, "CreateWidgetComponentPrefab with invalid widget component")
        return
    end
    local tPrefab = inUIPrefabClass:New()
    tPrefab:Create(nil, nil, nil, inArgument, nil, inStyle)
    inWidgetComponent:SetWidget(tPrefab._prefab)
    inWidgetComponent:SetDrawAtDesiredSize(inIsAdapterWidgetSize or false)
    self:RemovePrefab(tPrefab)
    return tPrefab
end

-------------------------------------------------------------------------------------------
-- 销毁通过 CreateWidgetComponentPrefab 创建的实例
-- @param inPrefabInstance(LuaTable) 通过 CreateWidgetComponentPrefab 创建的 UIPrefabClass 的子类实例
-- @param inWidgetComponent(UWidgetComponent) 通过 CreateWidgetComponentPrefab 挂载的 WidgetComponent
-------------------------------------------------------------------------------------------
function MVCService:DestroyWidgetComponentPrefab(inPrefabInstance, inWidgetComponent)
    if not inPrefabInstance then return end
    if not LuaHelp.IsValidObject(inWidgetComponent) then return end
    if not LuaHelp.IsChildOfClass(inPrefabInstance, "UIPrefabClass") then
        FrostLogE(self.__classname, "DestroyWidgetComponentPrefab with invalid UIPrefabClass")
        return
    end
    inWidgetComponent:SetWidget(nil)
    inPrefabInstance:Destroy()
end

-- 添加UI层级管理
-- @param prefabClass 添加的UIPrefabClass
-- @param parentPrefabClass 父UIPrefabClass
-- @param parentTf 父节点
function MVCService:AddLayerPrefab(prefabClass, parentPrefabClass, parentTf, layer, insertFirst)
    local realLayer = nil
    if not prefabClass then
        FrostLogE(self.__classname, "MVCService.AddLayerPrefab : invalid parameter")
        return
    end

    local widget = prefabClass._prefab
    if not widget then
        FrostLogE(self.__classname, "MVCService.AddLayerPrefab : invalid parameter 2")
        return
    end

    if parentTf then
        self._layerService:AddWidgetToPanel(widget, parentTf, insertFirst)
    else
        realLayer = layer or PanelLayerConfig.LAYER_TYPE_AUTO
        self._layerService:AddWidgetToViewport(widget, realLayer)
    end
    return realLayer
end

-- 移除UI层级管理
-- @param prefab 移除的UIPrefabClass
function MVCService:RemovePrefab(prefabClass, parentTf)
    if not prefabClass then
        FrostLogE(self.__classname, string.format("MVCService.RemovePrefab : invalid parameter\n%s", debug.traceback()))
        return
    end
    
    local widget = prefabClass._prefab
    if not LuaHelp.IsValidObject(widget) then
        FrostLogE(self.__classname, string.format("MVCService.RemovePrefab(%s) : invalid parameter 2\n%s", prefabClass.__classname, debug.traceback()))
        return
    end

    if parentTf then
        self._layerService:RemoveWidgetFromPanel(widget, parentTf)
    else
        self._layerService:RemoveWidgetFromViewport(widget)
    end
end

-- 注册UI状态改变前通知
-- @param tbl 接收通知的表
-- @param func 接收通知的函数，原型：
--             bool xxx(changeType, oldStateName, newStateName, stateParam)
--                  返回 - bool 是否允许进行状态改变
--                  changeType - int 取值StateConstant
--                  oldStateName - string 老的UI状态
--                  newStateName - string 新的UI状态
--                  stateParam - object 参数（可能有，可能没有）
-- @return 无
function MVCService:AddUIStatePreChangeDelegate(tbl, func)
    if not tbl or not func then
        FrostLogE(self.__classname, "MVCService.AddUIStatePreChangeDelegate : invalid parameter")
        return
    end
    
    for i = 1, #self._uiStatePreChangeDelegate do
        local data = self._uiStatePreChangeDelegate[i]
        if data ~= null and data.tbl == tbl and data.func == func then
            FrostLogE(self.__classname, "MVCService.AddUIStatePreChangeDelegate : duplicate add")
            return
        end
    end
    
    local data = Pool:CreateTable()
    data.tbl = tbl
    data.func = func
    self._uiStatePreChangeDelegate[#self._uiStatePreChangeDelegate+1] = data
end

-- 移除UI状态改变前通知
-- @param tbl 接收通知的表
-- @param func 接收通知的函数（可以为nil，标识关于tbl的全部移除）
-- @return 无
function MVCService:RemoveUIStatePreChangeDelegate(tbl, func)
    if not tbl then
        FrostLogE(self.__classname, "MVCService.RemoveUIStatePreChangeDelegate : invalid parameter")
        return
    end
    
    for i = 1, #self._uiStatePreChangeDelegate do
        local data = self._uiStatePreChangeDelegate[i]
        if data ~= null and data.tbl == tbl and (not func or data.func == func) then
            self._uiStatePreChangeDelegate[i] = null
            Pool:DestroyTable(data)
        end
    end
end

-------------------------------------------------------------------------------------------
-- 遍历所有的 Module, 通知ServiceCenter的后初始化完成，可以读取相关表格的静态数据等
-------------------------------------------------------------------------------------------
function MVCService:_onServiceCenterPostInit()
    self:_DoEachModule(function(inModuleInstance)
        inModuleInstance:OnServiceCenterPostInit()
    end)
end

-------------------------------------------------------------------------------------------
-- 遍历所有的 Module, 通知玩家进入游戏，可以开始向服务器请求玩家数据
-------------------------------------------------------------------------------------------
function MVCService:_onEnterGame()
    self:_DoEachModule(function(inModuleInstance)
        inModuleInstance:OnEnterGame()
    end)
end

-- 改变UI层级管理
-- @param prefabClass 添加的UIPrefabClass
-- @param parentPrefabClass 父UIPrefabClass
-- @param oldParentTf 旧父节点
-- @param newParentTf 新父节点
-- @param layer (PanelLayerConfig) 层级配置 默认为LAYER_TYPE_AUTO(自动层)
function MVCService:ChangeLayerPrefab(prefabClass, parentPrefabClass, oldParentTf, newParentTf, layer)
    self:RemovePrefab(prefabClass, oldParentTf)
    self:AddLayerPrefab(prefabClass, parentPrefabClass, newParentTf, layer, false)
end

-- 查询PrefabTypeList中是否包含此类型
-- @param Enum UE4.PrefabType
function MVCService:CheckContainPrefabType(inType)
    return table.contains(self._showPrefabTypeList, inType)
end

-- 添加PrefabType
-- @param Enum UE4.PrefabType
function MVCService:AddPrefabType(inType)
    if table.contains(self._showPrefabTypeList, inType) then
        FrostLogE(self.__classname, "Added duplicates Type = ", inType)
        return
    end
    table.insert(self._showPrefabTypeList, inType)
end

-- 移除PrefabType
-- @param Enum UE4.PrefabType
function MVCService:RemovePrefabType(inType)
    table.removeElement(self._showPrefabTypeList, inType)
end


-- 查找Prefab对应的Lua源码路径
-- 静态方法，不传入self
-- 调试方法，请不要在正式代码中使用
function MVCService.DebugGetPrefabClassSource(uiPrefab)
    local PrefabClzInfo = MVCService:GetPrefabClassByUIPrefab(uiPrefab)
    local PrefabClz = PrefabClzInfo and PrefabClzInfo.PrefabClass
    if not PrefabClz or not PrefabClz.vGetPath then
        return ""
    end

    local SourceInfo = debug.getinfo(PrefabClz.vGetPath, "S")
    return SourceInfo and SourceInfo.source or ""
end


return MVCService