---@class CS.UnityEngine.UI.GraphicRaycaster : CS.UnityEngine.EventSystems.BaseRaycaster
---@field public sortOrderPriority number
---@field public renderOrderPriority number
---@field public ignoreReversedGraphics boolean
---@field public blockingObjects number
---@field public eventCamera CS.UnityEngine.Camera
CS.UnityEngine.UI.GraphicRaycaster = { }
---@param eventData CS.UnityEngine.EventSystems.PointerEventData
---@param resultAppendList CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult
function CS.UnityEngine.UI.GraphicRaycaster:Raycast(eventData, resultAppendList) end

