---@class CS.UnityEngine.EventSystems.RaycastResult : CS.System.ValueType
---@field public module CS.UnityEngine.EventSystems.BaseRaycaster
---@field public distance number
---@field public index number
---@field public depth number
---@field public sortingLayer number
---@field public sortingOrder number
---@field public worldPosition CS.UnityEngine.Vector3
---@field public worldNormal CS.UnityEngine.Vector3
---@field public screenPosition CS.UnityEngine.Vector2
---@field public gameObject CS.UnityEngine.GameObject
---@field public isValid boolean
CS.UnityEngine.EventSystems.RaycastResult = { }
function CS.UnityEngine.EventSystems.RaycastResult:Clear() end
---@return string
function CS.UnityEngine.EventSystems.RaycastResult:ToString() end
return CS.UnityEngine.EventSystems.RaycastResult
