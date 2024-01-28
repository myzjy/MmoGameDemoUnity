---@class UnityEngine.EventSystems.PointerEventData : UnityEngine.EventSystems.BaseEventData
---@field public hovered System.Collections.Generic.List_UnityEngine.GameObject
---@field public pointerEnter UnityEngine.GameObject
---@field public lastPress UnityEngine.GameObject
---@field public rawPointerPress UnityEngine.GameObject
---@field public pointerDrag UnityEngine.GameObject
---@field public pointerCurrentRaycast UnityEngine.EventSystems.RaycastResult
---@field public pointerPressRaycast UnityEngine.EventSystems.RaycastResult
---@field public eligibleForClick boolean
---@field public pointerId number
---@field public position UnityEngine.Vector2
---@field public delta UnityEngine.Vector2
---@field public pressPosition UnityEngine.Vector2
---@field public clickTime number
---@field public clickCount number
---@field public scrollDelta UnityEngine.Vector2
---@field public useDragThreshold boolean
---@field public dragging boolean
---@field public button number
---@field public enterEventCamera UnityEngine.Camera
---@field public pressEventCamera UnityEngine.Camera
---@field public pointerPress UnityEngine.GameObject

---@type UnityEngine.EventSystems.PointerEventData
UnityEngine.EventSystems.PointerEventData = { }
---@return UnityEngine.EventSystems.PointerEventData
---@param eventSystem UnityEngine.EventSystems.EventSystem
function UnityEngine.EventSystems.PointerEventData.New(eventSystem) end
---@return boolean
function UnityEngine.EventSystems.PointerEventData:IsPointerMoving() end
---@return boolean
function UnityEngine.EventSystems.PointerEventData:IsScrolling() end
---@return string
function UnityEngine.EventSystems.PointerEventData:ToString() end
return UnityEngine.EventSystems.PointerEventData
