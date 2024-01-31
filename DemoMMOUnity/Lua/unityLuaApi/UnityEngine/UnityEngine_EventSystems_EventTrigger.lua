---@class UnityEngine.EventSystems.EventTrigger : UnityEngine.MonoBehaviour
---@field public triggers System.Collections.Generic.List_UnityEngine.EventSystems.EventTrigger.Entry

---@type UnityEngine.EventSystems.EventTrigger
UnityEngine.EventSystems.EventTrigger = { }
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.EventSystems.EventTrigger:OnPointerEnter(eventData) end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.EventSystems.EventTrigger:OnPointerExit(eventData) end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.EventSystems.EventTrigger:OnDrag(eventData) end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.EventSystems.EventTrigger:OnDrop(eventData) end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.EventSystems.EventTrigger:OnPointerDown(eventData) end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.EventSystems.EventTrigger:OnPointerUp(eventData) end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.EventSystems.EventTrigger:OnPointerClick(eventData) end
---@param eventData UnityEngine.EventSystems.BaseEventData
function UnityEngine.EventSystems.EventTrigger:OnSelect(eventData) end
---@param eventData UnityEngine.EventSystems.BaseEventData
function UnityEngine.EventSystems.EventTrigger:OnDeselect(eventData) end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.EventSystems.EventTrigger:OnScroll(eventData) end
---@param eventData UnityEngine.EventSystems.AxisEventData
function UnityEngine.EventSystems.EventTrigger:OnMove(eventData) end
---@param eventData UnityEngine.EventSystems.BaseEventData
function UnityEngine.EventSystems.EventTrigger:OnUpdateSelected(eventData) end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.EventSystems.EventTrigger:OnInitializePotentialDrag(eventData) end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.EventSystems.EventTrigger:OnBeginDrag(eventData) end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.EventSystems.EventTrigger:OnEndDrag(eventData) end
---@param eventData UnityEngine.EventSystems.BaseEventData
function UnityEngine.EventSystems.EventTrigger:OnSubmit(eventData) end
---@param eventData UnityEngine.EventSystems.BaseEventData
function UnityEngine.EventSystems.EventTrigger:OnCancel(eventData) end
return UnityEngine.EventSystems.EventTrigger
