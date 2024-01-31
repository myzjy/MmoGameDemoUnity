---@class UnityEngine.Collider : UnityEngine.Component
---@field public enabled boolean
---@field public attachedRigidbody UnityEngine.Rigidbody
---@field public isTrigger boolean
---@field public contactOffset number
---@field public bounds UnityEngine.Bounds
---@field public sharedMaterial UnityEngine.PhysicMaterial
---@field public material UnityEngine.PhysicMaterial

---@type UnityEngine.Collider
UnityEngine.Collider = { }
---@return UnityEngine.Collider
function UnityEngine.Collider.New() end
---@return UnityEngine.Vector3
---@param position UnityEngine.Vector3
function UnityEngine.Collider:ClosestPoint(position) end
---@return boolean
---@param ray UnityEngine.Ray
---@param hitInfo UnityEngine.RaycastHit
---@param maxDistance number
function UnityEngine.Collider:Raycast(ray, hitInfo, maxDistance) end
---@return UnityEngine.Vector3
---@param position UnityEngine.Vector3
function UnityEngine.Collider:ClosestPointOnBounds(position) end
return UnityEngine.Collider
