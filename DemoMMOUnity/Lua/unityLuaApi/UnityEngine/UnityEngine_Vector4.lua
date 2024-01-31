---@class UnityEngine.Vector4 : System.ValueType
---@field public kEpsilon number
---@field public x number
---@field public y number
---@field public z number
---@field public w number
---@field public Item number
---@field public normalized UnityEngine.Vector4
---@field public magnitude number
---@field public sqrMagnitude number
---@field public zero UnityEngine.Vector4
---@field public one UnityEngine.Vector4
---@field public positiveInfinity UnityEngine.Vector4
---@field public negativeInfinity UnityEngine.Vector4

---@type UnityEngine.Vector4
UnityEngine.Vector4 = { }
---@overload fun(x:number, y:number): UnityEngine.Vector4
---@overload fun(x:number, y:number, z:number): UnityEngine.Vector4
---@return UnityEngine.Vector4
---@param x number
---@param y number
---@param z number
---@param w number
function UnityEngine.Vector4.New(x, y, z, w) end
---@param newX number
---@param newY number
---@param newZ number
---@param newW number
function UnityEngine.Vector4:Set(newX, newY, newZ, newW) end
---@return UnityEngine.Vector4
---@param a UnityEngine.Vector4
---@param b UnityEngine.Vector4
---@param t number
function UnityEngine.Vector4.Lerp(a, b, t) end
---@return UnityEngine.Vector4
---@param a UnityEngine.Vector4
---@param b UnityEngine.Vector4
---@param t number
function UnityEngine.Vector4.LerpUnclamped(a, b, t) end
---@return UnityEngine.Vector4
---@param current UnityEngine.Vector4
---@param target UnityEngine.Vector4
---@param maxDistanceDelta number
function UnityEngine.Vector4.MoveTowards(current, target, maxDistanceDelta) end
---@overload fun(scale:UnityEngine.Vector4): void
---@param a UnityEngine.Vector4
---@param b UnityEngine.Vector4
function UnityEngine.Vector4:Scale(a, b) end
---@return number
function UnityEngine.Vector4:GetHashCode() end
---@overload fun(other:System.Object): boolean
---@return boolean
---@param other UnityEngine.Vector4
function UnityEngine.Vector4:Equals(other) end
---@overload fun(): void
---@param a UnityEngine.Vector4
function UnityEngine.Vector4:Normalize(a) end
---@return number
---@param a UnityEngine.Vector4
---@param b UnityEngine.Vector4
function UnityEngine.Vector4.Dot(a, b) end
---@return UnityEngine.Vector4
---@param a UnityEngine.Vector4
---@param b UnityEngine.Vector4
function UnityEngine.Vector4.Project(a, b) end
---@return number
---@param a UnityEngine.Vector4
---@param b UnityEngine.Vector4
function UnityEngine.Vector4.Distance(a, b) end
---@return number
---@param a UnityEngine.Vector4
function UnityEngine.Vector4.Magnitude(a) end
---@return UnityEngine.Vector4
---@param lhs UnityEngine.Vector4
---@param rhs UnityEngine.Vector4
function UnityEngine.Vector4.Min(lhs, rhs) end
---@return UnityEngine.Vector4
---@param lhs UnityEngine.Vector4
---@param rhs UnityEngine.Vector4
function UnityEngine.Vector4.Max(lhs, rhs) end
---@return UnityEngine.Vector4
---@param a UnityEngine.Vector4
---@param b UnityEngine.Vector4
function UnityEngine.Vector4.op_Addition(a, b) end
---@return UnityEngine.Vector4
---@param a UnityEngine.Vector4
---@param b UnityEngine.Vector4
function UnityEngine.Vector4.op_Subtraction(a, b) end
---@return UnityEngine.Vector4
---@param a UnityEngine.Vector4
function UnityEngine.Vector4.op_UnaryNegation(a) end
---@overload fun(a:UnityEngine.Vector4, d:number): UnityEngine.Vector4
---@return UnityEngine.Vector4
---@param d number
---@param a UnityEngine.Vector4
function UnityEngine.Vector4.op_Multiply(d, a) end
---@return UnityEngine.Vector4
---@param a UnityEngine.Vector4
---@param d number
function UnityEngine.Vector4.op_Division(a, d) end
---@return boolean
---@param lhs UnityEngine.Vector4
---@param rhs UnityEngine.Vector4
function UnityEngine.Vector4.op_Equality(lhs, rhs) end
---@return boolean
---@param lhs UnityEngine.Vector4
---@param rhs UnityEngine.Vector4
function UnityEngine.Vector4.op_Inequality(lhs, rhs) end
---@overload fun(v:UnityEngine.Vector3): UnityEngine.Vector4
---@overload fun(v:UnityEngine.Vector4): UnityEngine.Vector3
---@return UnityEngine.Vector4
---@param v UnityEngine.Vector2
function UnityEngine.Vector4.op_Implicit(v) end
---@overload fun(): string
---@return string
---@param format string
function UnityEngine.Vector4:ToString(format) end
---@overload fun(): number
---@return number
---@param a UnityEngine.Vector4
function UnityEngine.Vector4:SqrMagnitude(a) end
return UnityEngine.Vector4
