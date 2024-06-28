---@class FrostComponent:FrostObject
local FrostComponent = class("FrostComponent", FrostObject)
----------------------------------------------------------------
--- 初始化
function FrostComponent:ctor()
    -- 组件
    self._transform = false
    -- 物体
    self._gameObject = false
    
end


return FrostComponent
