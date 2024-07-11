--[[*********************************************************************
       author : zhangjingyi
       
       purpose : table cache
*********************************************************************--]]

local UPoolClass = Class("UPoolClass")

function UPoolClass:ctor()
    self._tablePool = {}
    self._count = {}
end

function UPoolClass:UCreateTable()
    if self._count == 0 then
        return {}
    end
    
end

return UPoolClass