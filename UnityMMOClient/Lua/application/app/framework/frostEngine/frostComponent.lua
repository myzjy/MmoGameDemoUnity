---@class FrostComponent
local FrostComponent = class("FrostComponent")

----------------------------------------------------------------
--- 初始化
function FrostComponent:ctor()
    -- 组件
    self._transform = false
    -- 物体
    self._gameObject = false
    
end


return FrostComponent
