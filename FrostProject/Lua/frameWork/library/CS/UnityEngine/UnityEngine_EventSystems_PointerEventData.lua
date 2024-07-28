---@class CS.UnityEngine.EventSystems.PointerEventData : CS.UnityEngine.EventSystems.BaseEventData
---@field public hovered CS.System.Collections.Generic.List_UnityEngine.GameObject
---@field public pointerEnter CS.UnityEngine.GameObject
---@field public lastPress CS.UnityEngine.GameObject
---@field public rawPointerPress CS.UnityEngine.GameObject
---@field public pointerDrag CS.UnityEngine.GameObject
---@field public pointerCurrentRaycast CS.UnityEngine.EventSystems.RaycastResult
---@field public pointerPressRaycast CS.UnityEngine.EventSystems.RaycastResult
---@field public eligibleForClick boolean
---@field public pointerId number
---@field public position CS.UnityEngine.Vector2
---@field public delta CS.UnityEngine.Vector2
---@field public pressPosition CS.UnityEngine.Vector2
---@field public clickTime number
---@field public clickCount number
---@field public scrollDelta CS.UnityEngine.Vector2
---@field public useDragThreshold boolean
---@field public dragging boolean
---@field public button number
---@field public enterEventCamera CS.UnityEngine.Camera
---@field public pressEventCamera CS.UnityEngine.Camera
---@field public pointerPress CS.UnityEngine.GameObject
CS.UnityEngine.EventSystems.PointerEventData = { }
---@return CS.UnityEngine.EventSystems.PointerEventData
---@param eventSystem CS.UnityEngine.EventSystems.EventSystem
function CS.UnityEngine.EventSystems.PointerEventData.New(eventSystem) end
---@return boolean
function CS.UnityEngine.EventSystems.PointerEventData:IsPointerMoving() end
---@return boolean
function CS.UnityEngine.EventSystems.PointerEventData:IsScrolling() end
---@return string
function CS.UnityEngine.EventSystems.PointerEventData:ToString() end
return CS.UnityEngine.EventSystems.PointerEventData
