---@class UnityEngine.EventSystems.BaseRaycaster : UnityEngine.EventSystems.UIBehaviour
---@field public eventCamera UnityEngine.Camera
---@field public sortOrderPriority number
---@field public renderOrderPriority number

---@type UnityEngine.EventSystems.BaseRaycaster
UnityEngine.EventSystems.BaseRaycaster = { }
---@param eventData UnityEngine.EventSystems.PointerEventData
---@param resultAppendList System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult
function UnityEngine.EventSystems.BaseRaycaster:Raycast(eventData, resultAppendList) end
---@return string
function UnityEngine.EventSystems.BaseRaycaster:ToString() end
return UnityEngine.EventSystems.BaseRaycaster
