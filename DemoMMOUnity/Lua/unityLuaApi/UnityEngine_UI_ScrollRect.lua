---@class UnityEngine.UI.ScrollRect : UnityEngine.EventSystems.UIBehaviour
---@field public content UnityEngine.RectTransform
---@field public horizontal boolean
---@field public vertical boolean
---@field public movementType number
---@field public elasticity number
---@field public inertia boolean
---@field public decelerationRate number
---@field public scrollSensitivity number
---@field public viewport UnityEngine.RectTransform
---@field public horizontalScrollbar UnityEngine.UI.Scrollbar
---@field public verticalScrollbar UnityEngine.UI.Scrollbar
---@field public horizontalScrollbarVisibility number
---@field public verticalScrollbarVisibility number
---@field public horizontalScrollbarSpacing number
---@field public verticalScrollbarSpacing number
---@field public onValueChanged UnityEngine.UI.ScrollRect.ScrollRectEvent
---@field public velocity UnityEngine.Vector2
---@field public normalizedPosition UnityEngine.Vector2
---@field public horizontalNormalizedPosition number
---@field public verticalNormalizedPosition number
---@field public minWidth number
---@field public preferredWidth number
---@field public flexibleWidth number
---@field public minHeight number
---@field public preferredHeight number
---@field public flexibleHeight number
---@field public layoutPriority number

---@type UnityEngine.UI.ScrollRect
UnityEngine.UI.ScrollRect = { }
---@param executing number
function UnityEngine.UI.ScrollRect:Rebuild(executing) end
function UnityEngine.UI.ScrollRect:LayoutComplete() end
function UnityEngine.UI.ScrollRect:GraphicUpdateComplete() end
---@return boolean
function UnityEngine.UI.ScrollRect:IsActive() end
function UnityEngine.UI.ScrollRect:StopMovement() end
---@param data UnityEngine.EventSystems.PointerEventData
function UnityEngine.UI.ScrollRect:OnScroll(data) end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.UI.ScrollRect:OnInitializePotentialDrag(eventData) end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.UI.ScrollRect:OnBeginDrag(eventData) end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.UI.ScrollRect:OnEndDrag(eventData) end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.UI.ScrollRect:OnDrag(eventData) end
function UnityEngine.UI.ScrollRect:CalculateLayoutInputHorizontal() end
function UnityEngine.UI.ScrollRect:CalculateLayoutInputVertical() end
function UnityEngine.UI.ScrollRect:SetLayoutHorizontal() end
function UnityEngine.UI.ScrollRect:SetLayoutVertical() end
return UnityEngine.UI.ScrollRect
