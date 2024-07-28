---@class CS.UnityEngine.UI.Scrollbar : CS.UnityEngine.UI.Selectable
---@field public handleRect CS.UnityEngine.RectTransform
---@field public direction number
---@field public value number
---@field public size number
---@field public numberOfSteps number
---@field public onValueChanged CS.UnityEngine.UI.Scrollbar.ScrollEvent
CS.UnityEngine.UI.Scrollbar = { }
---@param executing number
function CS.UnityEngine.UI.Scrollbar:Rebuild(executing) end
function CS.UnityEngine.UI.Scrollbar:LayoutComplete() end
function CS.UnityEngine.UI.Scrollbar:GraphicUpdateComplete() end
---@param eventData CS.UnityEngine.EventSystems.PointerEventData
function CS.UnityEngine.UI.Scrollbar:OnBeginDrag(eventData) end
---@param eventData CS.UnityEngine.EventSystems.PointerEventData
function CS.UnityEngine.UI.Scrollbar:OnDrag(eventData) end
---@param eventData CS.UnityEngine.EventSystems.PointerEventData
function CS.UnityEngine.UI.Scrollbar:OnPointerDown(eventData) end
---@param eventData CS.UnityEngine.EventSystems.PointerEventData
function CS.UnityEngine.UI.Scrollbar:OnPointerUp(eventData) end
---@param eventData CS.UnityEngine.EventSystems.AxisEventData
function CS.UnityEngine.UI.Scrollbar:OnMove(eventData) end
---@return CS.UnityEngine.UI.Selectable
function CS.UnityEngine.UI.Scrollbar:FindSelectableOnLeft() end
---@return CS.UnityEngine.UI.Selectable
function CS.UnityEngine.UI.Scrollbar:FindSelectableOnRight() end
---@return CS.UnityEngine.UI.Selectable
function CS.UnityEngine.UI.Scrollbar:FindSelectableOnUp() end
---@return CS.UnityEngine.UI.Selectable
function CS.UnityEngine.UI.Scrollbar:FindSelectableOnDown() end
---@param eventData CS.UnityEngine.EventSystems.PointerEventData
function CS.UnityEngine.UI.Scrollbar:OnInitializePotentialDrag(eventData) end
---@param direction number
---@param includeRectLayouts boolean
function CS.UnityEngine.UI.Scrollbar:SetDirection(direction, includeRectLayouts) end
return CS.UnityEngine.UI.Scrollbar
