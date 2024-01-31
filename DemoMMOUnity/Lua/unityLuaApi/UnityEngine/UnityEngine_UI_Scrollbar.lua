---@class UnityEngine.UI.Scrollbar : UnityEngine.UI.Selectable
---@field public handleRect UnityEngine.RectTransform
---@field public direction number
---@field public value number
---@field public size number
---@field public numberOfSteps number
---@field public onValueChanged UnityEngine.UI.Scrollbar.ScrollEvent

---@type UnityEngine.UI.Scrollbar
UnityEngine.UI.Scrollbar = { }
---@param executing number
function UnityEngine.UI.Scrollbar:Rebuild(executing) end
function UnityEngine.UI.Scrollbar:LayoutComplete() end
function UnityEngine.UI.Scrollbar:GraphicUpdateComplete() end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.UI.Scrollbar:OnBeginDrag(eventData) end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.UI.Scrollbar:OnDrag(eventData) end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.UI.Scrollbar:OnPointerDown(eventData) end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.UI.Scrollbar:OnPointerUp(eventData) end
---@param eventData UnityEngine.EventSystems.AxisEventData
function UnityEngine.UI.Scrollbar:OnMove(eventData) end
---@return UnityEngine.UI.Selectable
function UnityEngine.UI.Scrollbar:FindSelectableOnLeft() end
---@return UnityEngine.UI.Selectable
function UnityEngine.UI.Scrollbar:FindSelectableOnRight() end
---@return UnityEngine.UI.Selectable
function UnityEngine.UI.Scrollbar:FindSelectableOnUp() end
---@return UnityEngine.UI.Selectable
function UnityEngine.UI.Scrollbar:FindSelectableOnDown() end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.UI.Scrollbar:OnInitializePotentialDrag(eventData) end
---@param direction number
---@param includeRectLayouts boolean
function UnityEngine.UI.Scrollbar:SetDirection(direction, includeRectLayouts) end
return UnityEngine.UI.Scrollbar
