---@class UnityEngine.Bounds : System.ValueType
---@field public center UnityEngine.Vector3
---@field public size UnityEngine.Vector3
---@field public extents UnityEngine.Vector3
---@field public min UnityEngine.Vector3
---@field public max UnityEngine.Vector3

---@type UnityEngine.Bounds
UnityEngine.Bounds = { }
---@return UnityEngine.Bounds
---@param center UnityEngine.Vector3
---@param size UnityEngine.Vector3
function UnityEngine.Bounds.New(center, size) end
---@return number
function UnityEngine.Bounds:GetHashCode() end
---@overload fun(other:System.Object): boolean
---@return boolean
---@param other UnityEngine.Bounds
function UnityEngine.Bounds:Equals(other) end
---@return boolean
---@param lhs UnityEngine.Bounds
---@param rhs UnityEngine.Bounds
function UnityEngine.Bounds.op_Equality(lhs, rhs) end
---@return boolean
---@param lhs UnityEngine.Bounds
---@param rhs UnityEngine.Bounds
function UnityEngine.Bounds.op_Inequality(lhs, rhs) end
---@param min UnityEngine.Vector3
---@param max UnityEngine.Vector3
function UnityEngine.Bounds:SetMinMax(min, max) end
---@overload fun(point:UnityEngine.Vector3): void
---@param bounds UnityEngine.Bounds
function UnityEngine.Bounds:Encapsulate(bounds) end
---@overload fun(amount:number): void
---@param amount UnityEngine.Vector3
function UnityEngine.Bounds:Expand(amount) end
---@return boolean
---@param bounds UnityEngine.Bounds
function UnityEngine.Bounds:Intersects(bounds) end
---@overload fun(ray:UnityEngine.Ray): boolean
---@return boolean
---@param ray UnityEngine.Ray
---@param distance System.Single
function UnityEngine.Bounds:IntersectRay(ray, distance) end
---@overload fun(): string
---@return string
---@param format string
function UnityEngine.Bounds:ToString(format) end
---@return boolean
---@param point UnityEngine.Vector3
function UnityEngine.Bounds:Contains(point) end
---@return number
---@param point UnityEngine.Vector3
function UnityEngine.Bounds:SqrDistance(point) end
---@return UnityEngine.Vector3
---@param point UnityEngine.Vector3
function UnityEngine.Bounds:ClosestPoint(point) end
return UnityEngine.Bounds
