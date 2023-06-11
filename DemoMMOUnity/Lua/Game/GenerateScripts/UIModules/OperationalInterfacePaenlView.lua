---@class OperationalInterfacePaenlView
local OperationalInterfacePaenlView = BaseClass()
local _UIView = {}
function OperationalInterfacePaenlView:Init(view)
	_UIView = view:GetComponent("UIView")
end

return OperationalInterfacePaenlView