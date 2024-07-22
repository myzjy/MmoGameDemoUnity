--[[*********************************************************************
       author : zhangjingyi
       
       purpose : table cache
*********************************************************************--]]

---@class UPoolClass
local UPoolClass = Class("UPoolClass")

function UPoolClass:ctor()
    self._tablePool = {}
    self._count = {}
end
function UPoolClass:CreateTable()
    if self._count == 0 then
        return {}
    end
    local t = self._tablePool[self._count]
    self._tablePool[self._count] = nil
    self._count = self._count - 1
    self._tablePool[t] = nil

    return t
end

-- delete a table
-- @param t table
function UPoolClass:DestroyTable(t)
    if not t then
        return
    end
    
    table.Clear(t)
    
    if self._tablePool[t] then
        FrostLogE("Normal", "UPoolClass:DestroyTable : duplicate destroy table")
        return
    end
        
    self._count = self._count + 1
    self._tablePool[self._count] = t
    self._tablePool[t] = true
end

_G.UPool = UPoolClass()

return UPoolClass