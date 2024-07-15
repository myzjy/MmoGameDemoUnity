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



        ::continue::
    end

end

return ServiceManager