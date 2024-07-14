--[[-------------------------------------------------------------
    Copyright 2024 - 2026 Tencent. All Rights Reserved
    Author : zhangjingyi
    DateTime: 2024/7/13 下午11:51
    brief : 管理数据中心的服务类，目前有两种实例化的场景
                1. 本地实例化一个用于管理与角色无关的数据中心
--]]-------------------------------------------------------------

---@class DataCentreService:ServiceBase
local DataCentreService = Class("DataCentreService", ServiceBase)
-- 缓存静态的数据中心配置
local gArrayDataCentreConfig = require("game.websocket.dataCenterConfig")

-------------------------------------------------------------------------------
--- 根据数据中心的状态判断指定数据配置是否有用
--- @param inConfig table
--- @param inInstance DataCentreService
-------------------------------------------------------------------------------
function DataCentreService:IsValidConfig(inInstance, inConfig)
    
end
function addDataCenter(inInstance, inName, inConfig)
    for _, v in pairs(inConfig.Paths) do 
        require(v)
    end
    local tClass = _G[inName]
    if not tClass then
        FrostLogE(inInstance.__classname, "Get data centre class failed with", inName, inConfig.ClassName)
        return
    end
    local tDataCentre = tClass()
    inInstance[inName] = tDataCentre
    table.insert(inInstance._listDataCentreNames, inName)
    FrostLogD(inInstance.__classname, "Add data centre", inName)
end

function DataCentreService:ctor()
    -- 当前数据中心管理的子类实例名称
    self._listDataCentre = {}
    for tName, tConfig in pairs(gArrayDataCentreConfig) do
        xpcall(addDataCenter, __G__TRACKBACK__, self, tName, tConfig)
    end
end

-------------------------------------------------------------------------------------------
-- 子类覆盖，进行返回类的静态配置数据
-------------------------------------------------------------------------------------------
function DataCentreService:vGetConfig()
    return{
        name = "DataCentreService"
    }
end


-------------------------------------------------------------------------------------------
-- 加载配置，创建所有的数据中心
-------------------------------------------------------------------------------------------
function DataCentreService:vInitialize()
    NGRLogD(self.__classname, "vInitialize")
    for tIndex, tName in pairs(self._listDataCentreNames) do
        xpcall(self._initDataCentre, __G__TRACKBACK__, self, tName)
    end
end
-------------------------------------------------------------------------------------------
-- 释放所有的数据中心
-------------------------------------------------------------------------------------------
function DataCentreService:vDeinitialize()
    FrostLogD(self.__classname, "vDeinitialize")
    for tIndex, tName in pairs(self._listDataCentreNames) do
        FrostLogD(self.__classname, "Start uninit data centre", tName)
        self[tName]:Deinitialize()
        FrostLogD(self.__classname, "End uninit data centre", tName)
    end
    table.clear(self._listDataCentreNames)
end

-------------------------------------------------------------------------------------------
--- 初始化指定的数据中心
--- @param inName(string) 数据中心的名称，需先执行 _addDataCentre
-------------------------------------------------------------------------------------------
function DataCentreService:_initDataCentre(inName)
    local tInstance = self[inName]
    if not tInstance then
        FrostLogE(self.__classname, "Get data centre instance failed with", inName)
        return
    end
    FrostLogD(self.__classname, "Start init data centre", inName)
    tInstance:Initialize(self)
    FrostLogD(self.__classname, "End init data centre", inName)
end


return DataCentreService