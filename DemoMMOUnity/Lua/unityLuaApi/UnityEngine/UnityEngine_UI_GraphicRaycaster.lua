---@class UnityEngine.UI.GraphicRaycaster : UnityEngine.EventSystems.BaseRaycaster
---@field public sortOrderPriority number
---@field public renderOrderPriority number
---@field public ignoreReversedGraphics boolean
---@field public blockingObjects number
---@field public eventCamera UnityEngine.Camera

---@type UnityEngine.UI.GraphicRaycaster
UnityEngine.UI.GraphicRaycaster = { }
---@param eventData UnityEngine.EventSystems.PointerEventData
---@param resultAppendList System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult
function UnityEngine.UI.GraphicRaycaster:Raycast(eventData, resultAppendList) end
return UnityEngine.UI.GraphicRaycaster
