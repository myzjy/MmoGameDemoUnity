---@class CS.UnityEngine.EventSystems.BaseRaycaster : CS.UnityEngine.EventSystems.UIBehaviour
---@field public eventCamera CS.UnityEngine.Camera
---@field public sortOrderPriority number
---@field public renderOrderPriority number
CS.UnityEngine.EventSystems.BaseRaycaster = { }
---@param eventData CS.UnityEngine.EventSystems.PointerEventData
---@param resultAppendList CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult
function CS.UnityEngine.EventSystems.BaseRaycaster:Raycast(eventData, resultAppendList) end
---@return string
function CS.UnityEngine.EventSystems.BaseRaycaster:ToString() end
return CS.UnityEngine.EventSystems.BaseRaycaster
