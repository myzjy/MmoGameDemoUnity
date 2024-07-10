--[[    
    Copyright 2024 - 2026 Tencent. All Rights Reserved
    Author : zhangjingyi
    brief : 全局服务的基类，配合ServiceManager的管理，提供子类覆写的接口
--]]

---@class ServiceBase
local ServiceBase = Class("ServiceBase")

-------------------------------------------------------------------------------
--- 声明变量，初始化配置
-------------------------------------------------------------------------------
function ServiceBase:ctor()
end
-------------------------------------------------------------------------------------------
--- 静态函数，返回服务的配置信息，子类覆盖vGetConfig返回自定义的数据
--- return(LuaTable)
--- {
---      name(string) 外部使用服务时的全局名称
---      isClientOnly(boolean) 可选，是否仅在Client上创建
--- }
-------------------------------------------------------------------------------------------
function ServiceBase:GetConfig()
    FrostLogD(self.__classname, "GetConfig")
    return self:vGetConfig()
end

-------------------------------------------------------------------------------------------
--- 初始化，由 ServiceManager 进行调用
-------------------------------------------------------------------------------------------
function ServiceBase:Initialize()
    FrostLogD(self.__classname, "Initialize")
    self:vInitialize()
end

-------------------------------------------------------------------------------------------
-- 反初始化，由 ServiceManager 进行调用
-------------------------------------------------------------------------------------------
function ServiceBase:Deinitialize()
    FrostLogD(self.__classname, "Deinitialize")
    self:vDeinitialize()
end
-------------------------------------------------------------------------------------------
-- 子类覆盖，进行返回类的静态配置数据
-- return(LuaTable)
-- {
--      name(string) 外部使用服务时的全局名称
--      isDSOnly(boolean) 可选，是否仅在DS上创建
--      isClientOnly(boolean) 可选，是否仅在Client上创建
-- }
-------------------------------------------------------------------------------------------
function ServiceBase:vGetConfig()
end

-------------------------------------------------------------------------------------------
-- 子类覆盖，进行初始化逻辑
-- @param inIsRunOnServer(boolean) 当前是否允许在服务器，由 UE4Helper.IsRunOnServer() 决定
-------------------------------------------------------------------------------------------
function ServiceBase:vInitialize(inIsRunOnServer)
end

-------------------------------------------------------------------------------------------
-- 子类覆盖，进行反初始化逻辑
-- @param inIsRunOnServer(boolean) 当前是否允许在服务器，由 UE4Helper.IsRunOnServer() 决定
-------------------------------------------------------------------------------------------
function ServiceBase:vDeinitialize(inIsRunOnServer)
end


return ServiceBase