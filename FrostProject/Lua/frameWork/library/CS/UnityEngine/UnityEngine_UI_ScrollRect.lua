---@class CS.UnityEngine.UI.ScrollRect : CS.UnityEngine.EventSystems.UIBehaviour
---@field public content CS.UnityEngine.RectTransform
---@field public horizontal boolean
---@field public vertical boolean
---@field public movementType number
---@field public elasticity number
---@field public inertia boolean
---@field public decelerationRate number
---@field public scrollSensitivity number
---@field public viewport CS.UnityEngine.RectTransform
---@field public horizontalScrollbar CS.UnityEngine.UI.Scrollbar
---@field public verticalScrollbar CS.UnityEngine.UI.Scrollbar
---@field public horizontalScrollbarVisibility number
---@field public verticalScrollbarVisibility number
---@field public horizontalScrollbarSpacing number
---@field public verticalScrollbarSpacing number
---@field public onValueChanged CS.UnityEngine.UI.ScrollRect.ScrollRectEvent
---@field public velocity CS.UnityEngine.Vector2
---@field public normalizedPosition CS.UnityEngine.Vector2
---@field public horizontalNormalizedPosition number
---@field public verticalNormalizedPosition number
---@field public minWidth number
---@field public preferredWidth number
---@field public flexibleWidth number
---@field public minHeight number
---@field public preferredHeight number
---@field public flexibleHeight number
---@field public layoutPriority number
CS.UnityEngine.UI.ScrollRect = { }
---@param executing number
function CS.UnityEngine.UI.ScrollRect:Rebuild(executing) end
function CS.UnityEngine.UI.ScrollRect:LayoutComplete() end
function CS.UnityEngine.UI.ScrollRect:GraphicUpdateComplete() end
---@return boolean
function CS.UnityEngine.UI.ScrollRect:IsActive() end
function CS.UnityEngine.UI.ScrollRect:StopMovement() end
---@param data CS.UnityEngine.EventSystems.PointerEventData
function CS.UnityEngine.UI.ScrollRect:OnScroll(data) end
---@param eventData CS.UnityEngine.EventSystems.PointerEventData
function CS.UnityEngine.UI.ScrollRect:OnInitializePotentialDrag(eventData) end
---@param eventData CS.UnityEngine.EventSystems.PointerEventData
function CS.UnityEngine.UI.ScrollRect:OnBeginDrag(eventData) end
---@param eventData CS.UnityEngine.EventSystems.PointerEventData
function CS.UnityEngine.UI.ScrollRect:OnEndDrag(eventData) end
---@param eventData CS.UnityEngine.EventSystems.PointerEventData
function CS.UnityEngine.UI.ScrollRect:OnDrag(eventData) end
function CS.UnityEngine.UI.ScrollRect:CalculateLayoutInputHorizontal() end
function CS.UnityEngine.UI.ScrollRect:CalculateLayoutInputVertical() end
function CS.UnityEngine.UI.ScrollRect:SetLayoutHorizontal() end
function CS.UnityEngine.UI.ScrollRect:SetLayoutVertical() end
return CS.UnityEngine.UI.ScrollRect
