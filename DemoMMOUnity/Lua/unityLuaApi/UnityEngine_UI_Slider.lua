---@class UnityEngine.UI.Slider : UnityEngine.UI.Selectable
---@field public fillRect UnityEngine.RectTransform
---@field public handleRect UnityEngine.RectTransform
---@field public direction number
---@field public minValue number
---@field public maxValue number
---@field public wholeNumbers boolean
---@field public value number
---@field public normalizedValue number
---@field public onValueChanged UnityEngine.UI.Slider.SliderEvent

---@type UnityEngine.UI.Slider
UnityEngine.UI.Slider = { }
---@param executing number
function UnityEngine.UI.Slider:Rebuild(executing) end
function UnityEngine.UI.Slider:LayoutComplete() end
function UnityEngine.UI.Slider:GraphicUpdateComplete() end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.UI.Slider:OnPointerDown(eventData) end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.UI.Slider:OnDrag(eventData) end
---@param eventData UnityEngine.EventSystems.AxisEventData
function UnityEngine.UI.Slider:OnMove(eventData) end
---@return UnityEngine.UI.Selectable
function UnityEngine.UI.Slider:FindSelectableOnLeft() end
---@return UnityEngine.UI.Selectable
function UnityEngine.UI.Slider:FindSelectableOnRight() end
---@return UnityEngine.UI.Selectable
function UnityEngine.UI.Slider:FindSelectableOnUp() end
---@return UnityEngine.UI.Selectable
function UnityEngine.UI.Slider:FindSelectableOnDown() end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.UI.Slider:OnInitializePotentialDrag(eventData) end
---@param direction number
---@param includeRectLayouts boolean
function UnityEngine.UI.Slider:SetDirection(direction, includeRectLayouts) end
return UnityEngine.UI.Slider
