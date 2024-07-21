--[[
    author : zhangjingyi 
    作用： 1. 获取配置表
--]]

---@class DataTableService:ServiceBase
local DataTableService = Class("DataTableService",ClassLibraryMap.ServiceBase)

function DataTableService:ctor()
    self._mapDataTableInfo = {}
end

return DataTableService