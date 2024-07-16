local ServiceManager = Class("ServiceManager")

function ServiceManager:ctor()
    self._mapServiceInstance = {}
    self._listServiceNames = {}
    -- 缓存当前ServerCenter的后初始化是否完成
    self._isServerCenterPostInit = false
end

------------------------------------------------------------------------------
--- 初始化 
------------------------------------------------------------------------------
function ServiceManager:Initialize()
    local tServiceConfigList = require("frameWork.services.serviceConfig")
    for tIndex, tServiceFilePath in ipairs(tServiceConfigList) do
        FrostLogD(self.__classname, "Loading service class with" , tServiceFilePath)
        local tServiceClass = require(tServiceFilePath)
        if not tServiceClass or not LuaHelp.IsChildOfClass(tServiceClass, "ServiceBase") then
            FrostLogE(self.__classname, "Get service class failed , with", tServiceFilePath)
            goto continue
        end
        ---@type ServiceBase
        local tGameServiceInstance = tServiceClass()
        local tGameServiceConfigStr = tGameServiceInstance:GetConfig()
        if not tGameServiceConfigStr or not tGameServiceConfigStr.name then
            FrostLogE(self.__classname, "Get service class config failed , with", JSON.encode(tGameServiceConfigStr))
            goto continue
        end
        
        local tServiceName = tGameServiceConfigStr.name
        -- 检查全局名称是否已注册
        if LuaHelp.GetGlobalVariable(tServiceName) then
            FrostLogE(self.__classname, "Already has global variable with name", tServiceName)
            goto continue
        end

        -- 检查当前运行环境是否需要创建
        if tServiceClass.isDSOnly then
            FrostLogD(self.__classname, "Skip create service on client", tServiceName, tServiceFilePath, JSON.encode(tGameServiceConfigStr))
            goto continue
        end
        -- 创建服务类的实例
        FrostLogD(self.__classname, "Create service instance with", tServiceName, tServiceFilePath)
        tGameServiceInstance:Initialize()
        self._mapServiceInstance[tServiceName] = tGameServiceInstance
        table.insert(self._listServiceNames, tServiceName)
        LuaHelp.RegisterGlobalVariable(tServiceName, tGameServiceInstance) 
        ::continue::
    end

end

-------------------------------------------------------------------------------------------
-- 反初始化，由 ServiceManager 进行调用
-------------------------------------------------------------------------------------------
function ServiceManager:Deinitialize()
    FrostLogD(self.__classname, "Deinitialize")
    for tIndex = #self._listServiceNames, 1, -1 do
        local tServiceName = self._listServiceNames[tIndex]
        local tServiceInstance = self._mapServiceInstance[tServiceName]
        FrostLogD(self.__classname, "Deinitialize service instance", tServiceName)
        if tServiceInstance then
            -- 清理实例，并移除全局变量
            tServiceInstance:Deinitialize()
            LuaHelp.RegisterGlobalVariable(tServiceName, nil)
        end
    end
    table.Clear(self._listServiceNames)
    table.Clear(self._mapServiceInstance)
end

_G.ServiceManager = ServiceManager()

return ServiceManager