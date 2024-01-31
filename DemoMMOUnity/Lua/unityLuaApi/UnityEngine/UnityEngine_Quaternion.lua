---@class UnityEngine.Quaternion : System.ValueType
---@field public x number
---@field public y number
---@field public z number
---@field public w number
---@field public kEpsilon number
---@field public Item number
---@field public identity UnityEngine.Quaternion
---@field public eulerAngles UnityEngine.Vector3
---@field public normalized UnityEngine.Quaternion

---@type UnityEngine.Quaternion
UnityEngine.Quaternion = { }
---@return UnityEngine.Quaternion
---@param x number
---@param y number
---@param z number
---@param w number
function UnityEngine.Quaternion.New(x, y, z, w) end
---@return UnityEngine.Quaternion
---@param fromDirection UnityEngine.Vector3
---@param toDirection UnityEngine.Vector3
function UnityEngine.Quaternion.FromToRotation(fromDirection, toDirection) end
---@return UnityEngine.Quaternion
---@param rotation UnityEngine.Quaternion
function UnityEngine.Quaternion.Inverse(rotation) end
---@return UnityEngine.Quaternion
---@param a UnityEngine.Quaternion
---@param b UnityEngine.Quaternion
---@param t number
function UnityEngine.Quaternion.Slerp(a, b, t) end
---@return UnityEngine.Quaternion
---@param a UnityEngine.Quaternion
---@param b UnityEngine.Quaternion
---@param t number
function UnityEngine.Quaternion.SlerpUnclamped(a, b, t) end
---@return UnityEngine.Quaternion
---@param a UnityEngine.Quaternion
---@param b UnityEngine.Quaternion
---@param t number
function UnityEngine.Quaternion.Lerp(a, b, t) end
---@return UnityEngine.Quaternion
---@param a UnityEngine.Quaternion
---@param b UnityEngine.Quaternion
---@param t number
function UnityEngine.Quaternion.LerpUnclamped(a, b, t) end
---@return UnityEngine.Quaternion
---@param angle number
---@param axis UnityEngine.Vector3
function UnityEngine.Quaternion.AngleAxis(angle, axis) end
---@overload fun(forward:UnityEngine.Vector3): UnityEngine.Quaternion
---@return UnityEngine.Quaternion
---@param forward UnityEngine.Vector3
---@param upwards UnityEngine.Vector3
function UnityEngine.Quaternion.LookRotation(forward, upwards) end
---@param newX number
---@param newY number
---@param newZ number
---@param newW number
function UnityEngine.Quaternion:Set(newX, newY, newZ, newW) end
---@overload fun(lhs:UnityEngine.Quaternion, rhs:UnityEngine.Quaternion): UnityEngine.Quaternion
---@return UnityEngine.Quaternion
---@param rotation UnityEngine.Quaternion
---@param point UnityEngine.Vector3
function UnityEngine.Quaternion.op_Multiply(rotation, point) end
---@return boolean
---@param lhs UnityEngine.Quaternion
---@param rhs UnityEngine.Quaternion
function UnityEngine.Quaternion.op_Equality(lhs, rhs) end
---@return boolean
---@param lhs UnityEngine.Quaternion
---@param rhs UnityEngine.Quaternion
function UnityEngine.Quaternion.op_Inequality(lhs, rhs) end
---@return number
---@param a UnityEngine.Quaternion
---@param b UnityEngine.Quaternion
function UnityEngine.Quaternion.Dot(a, b) end
---@overload fun(view:UnityEngine.Vector3): void
---@param view UnityEngine.Vector3
---@param up UnityEngine.Vector3
function UnityEngine.Quaternion:SetLookRotation(view, up) end
---@return number
---@param a UnityEngine.Quaternion
---@param b UnityEngine.Quaternion
function UnityEngine.Quaternion.Angle(a, b) end
---@overload fun(euler:UnityEngine.Vector3): UnityEngine.Quaternion
---@return UnityEngine.Quaternion
---@param x number
---@param y number
---@param z number
function UnityEngine.Quaternion.Euler(x, y, z) end
---@param angle System.Single
---@param axis UnityEngine.Vector3
function UnityEngine.Quaternion:ToAngleAxis(angle, axis) end
---@param fromDirection UnityEngine.Vector3
---@param toDirection UnityEngine.Vector3
function UnityEngine.Quaternion:SetFromToRotation(fromDirection, toDirection) end
---@return UnityEngine.Quaternion
---@param from UnityEngine.Quaternion
---@param to UnityEngine.Quaternion
---@param maxDegreesDelta number
function UnityEngine.Quaternion.RotateTowards(from, to, maxDegreesDelta) end
---@overload fun(): void
---@param q UnityEngine.Quaternion
function UnityEngine.Quaternion:Normalize(q) end
---@return number
function UnityEngine.Quaternion:GetHashCode() end
---@overload fun(other:System.Object): boolean
---@return boolean
---@param other UnityEngine.Quaternion
function UnityEngine.Quaternion:Equals(other) end
---@overload fun(): string
---@return string
---@param format string
function UnityEngine.Quaternion:ToString(format) end
return UnityEngine.Quaternion
