---@class UnityEngine.EventSystems.RaycastResult : System.ValueType
---@field public module UnityEngine.EventSystems.BaseRaycaster
---@field public distance number
---@field public index number
---@field public depth number
---@field public sortingLayer number
---@field public sortingOrder number
---@field public worldPosition UnityEngine.Vector3
---@field public worldNormal UnityEngine.Vector3
---@field public screenPosition UnityEngine.Vector2
---@field public gameObject UnityEngine.GameObject
---@field public isValid boolean

---@type UnityEngine.EventSystems.RaycastResult
UnityEngine.EventSystems.RaycastResult = { }
function UnityEngine.EventSystems.RaycastResult:Clear() end
---@return string
function UnityEngine.EventSystems.RaycastResult:ToString() end
return UnityEngine.EventSystems.RaycastResult
