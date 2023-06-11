---@class MapGuideView
local MapGuideView = BaseClass()
local _UIView = {}
function MapGuideView:Init(view)
	_UIView = view:GetComponent("UIView")
end

return MapGuideView