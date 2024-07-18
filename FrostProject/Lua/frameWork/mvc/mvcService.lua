--[[
    author : zhangjingyi

    作用 ： mvc 服务
--]]

---@class MVCService:ServiceBase
local MVCService = Class("MVCService", ClassLibraryMap.ServiceBase)

function MVCService:ctor()
    --- 管理本地全局 Module 类
    self._module = {}

    -- 缓存当前 显示的Prefab 类型
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
    local md = self:OnCreateModule(tLoadModuleClass
    )
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




return MVCService