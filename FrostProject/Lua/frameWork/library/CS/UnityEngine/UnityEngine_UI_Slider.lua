---@class CS.UnityEngine.UI.Slider : CS.UnityEngine.UI.Selectable
---@field public fillRect CS.UnityEngine.RectTransform
---@field public handleRect CS.UnityEngine.RectTransform
---@field public direction number
---@field public minValue number
---@field public maxValue number
---@field public wholeNumbers boolean
---@field public value number
---@field public normalizedValue number
---@field public onValueChanged CS.UnityEngine.UI.Slider.SliderEvent
CS.UnityEngine.UI.Slider = { }
---@param executing number
function CS.UnityEngine.UI.Slider:Rebuild(executing) end
function CS.UnityEngine.UI.Slider:LayoutComplete() end
function CS.UnityEngine.UI.Slider:GraphicUpdateComplete() end
---@param eventData CS.UnityEngine.EventSystems.PointerEventData
function CS.UnityEngine.UI.Slider:OnPointerDown(eventData) end
---@param eventData CS.UnityEngine.EventSystems.PointerEventData
function CS.UnityEngine.UI.Slider:OnDrag(eventData) end
---@param eventData CS.UnityEngine.EventSystems.AxisEventData
function CS.UnityEngine.UI.Slider:OnMove(eventData) end
---@return CS.UnityEngine.UI.Selectable
function CS.UnityEngine.UI.Slider:FindSelectableOnLeft() end
---@return CS.UnityEngine.UI.Selectable
function CS.UnityEngine.UI.Slider:FindSelectableOnRight() end
---@return CS.UnityEngine.UI.Selectable
function CS.UnityEngine.UI.Slider:FindSelectableOnUp() end
---@return CS.UnityEngine.UI.Selectable
function CS.UnityEngine.UI.Slider:FindSelectableOnDown() end
---@param eventData CS.UnityEngine.EventSystems.PointerEventData
function CS.UnityEngine.UI.Slider:OnInitializePotentialDrag(eventData) end
---@param direction number
---@param includeRectLayouts boolean
function CS.UnityEngine.UI.Slider:SetDirection(direction, includeRectLayouts) end
return CS.UnityEngine.UI.Slider
