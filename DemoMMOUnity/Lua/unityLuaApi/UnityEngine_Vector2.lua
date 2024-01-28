---@class UnityEngine.Vector2 : System.ValueType
---@field public x number
---@field public y number
---@field public kEpsilon number
---@field public kEpsilonNormalSqrt number
---@field public Item number
---@field public normalized UnityEngine.Vector2
---@field public magnitude number
---@field public sqrMagnitude number
---@field public zero UnityEngine.Vector2
---@field public one UnityEngine.Vector2
---@field public up UnityEngine.Vector2
---@field public down UnityEngine.Vector2
---@field public left UnityEngine.Vector2
---@field public right UnityEngine.Vector2
---@field public positiveInfinity UnityEngine.Vector2
---@field public negativeInfinity UnityEngine.Vector2

---@type UnityEngine.Vector2
UnityEngine.Vector2 = { }
---@return UnityEngine.Vector2
---@param x number
---@param y number
function UnityEngine.Vector2.New(x, y) end
---@param newX number
---@param newY number
function UnityEngine.Vector2:Set(newX, newY) end
---@return UnityEngine.Vector2
---@param a UnityEngine.Vector2
---@param b UnityEngine.Vector2
---@param t number
function UnityEngine.Vector2.Lerp(a, b, t) end
---@return UnityEngine.Vector2
---@param a UnityEngine.Vector2
---@param b UnityEngine.Vector2
---@param t number
function UnityEngine.Vector2.LerpUnclamped(a, b, t) end
---@return UnityEngine.Vector2
---@param current UnityEngine.Vector2
---@param target UnityEngine.Vector2
---@param maxDistanceDelta number
function UnityEngine.Vector2.MoveTowards(current, target, maxDistanceDelta) end
---@overload fun(scale:UnityEngine.Vector2): void
---@param a UnityEngine.Vector2
---@param b UnityEngine.Vector2
function UnityEngine.Vector2:Scale(a, b) end
function UnityEngine.Vector2:Normalize() end
---@overload fun(): string
---@return string
---@param format string
function UnityEngine.Vector2:ToString(format) end
---@return number
function UnityEngine.Vector2:GetHashCode() end
---@overload fun(other:System.Object): boolean
---@return boolean
---@param other UnityEngine.Vector2
function UnityEngine.Vector2:Equals(other) end
---@return UnityEngine.Vector2
---@param inDirection UnityEngine.Vector2
---@param inNormal UnityEngine.Vector2
function UnityEngine.Vector2.Reflect(inDirection, inNormal) end
---@return UnityEngine.Vector2
---@param inDirection UnityEngine.Vector2
function UnityEngine.Vector2.Perpendicular(inDirection) end
---@return number
---@param lhs UnityEngine.Vector2
---@param rhs UnityEngine.Vector2
function UnityEngine.Vector2.Dot(lhs, rhs) end
---@return number
---@param from UnityEngine.Vector2
---@param to UnityEngine.Vector2
function UnityEngine.Vector2.Angle(from, to) end
---@return number
---@param from UnityEngine.Vector2
---@param to UnityEngine.Vector2
function UnityEngine.Vector2.SignedAngle(from, to) end
---@return number
---@param a UnityEngine.Vector2
---@param b UnityEngine.Vector2
function UnityEngine.Vector2.Distance(a, b) end
---@return UnityEngine.Vector2
---@param vector UnityEngine.Vector2
---@param maxLength number
function UnityEngine.Vector2.ClampMagnitude(vector, maxLength) end
---@overload fun(): number
---@return number
---@param a UnityEngine.Vector2
function UnityEngine.Vector2:SqrMagnitude(a) end
---@return UnityEngine.Vector2
---@param lhs UnityEngine.Vector2
---@param rhs UnityEngine.Vector2
function UnityEngine.Vector2.Min(lhs, rhs) end
---@return UnityEngine.Vector2
---@param lhs UnityEngine.Vector2
---@param rhs UnityEngine.Vector2
function UnityEngine.Vector2.Max(lhs, rhs) end
---@overload fun(current:UnityEngine.Vector2, target:UnityEngine.Vector2, currentVelocity:UnityEngine.Vector2, smoothTime:number): UnityEngine.Vector2
---@overload fun(current:UnityEngine.Vector2, target:UnityEngine.Vector2, currentVelocity:UnityEngine.Vector2, smoothTime:number, maxSpeed:number): UnityEngine.Vector2
---@return UnityEngine.Vector2
---@param current UnityEngine.Vector2
---@param target UnityEngine.Vector2
---@param currentVelocity UnityEngine.Vector2
---@param smoothTime number
---@param maxSpeed number
---@param deltaTime number
function UnityEngine.Vector2.SmoothDamp(current, target, currentVelocity, smoothTime, maxSpeed, deltaTime) end
---@return UnityEngine.Vector2
---@param a UnityEngine.Vector2
---@param b UnityEngine.Vector2
function UnityEngine.Vector2.op_Addition(a, b) end
---@return UnityEngine.Vector2
---@param a UnityEngine.Vector2
---@param b UnityEngine.Vector2
function UnityEngine.Vector2.op_Subtraction(a, b) end
---@overload fun(a:UnityEngine.Vector2, b:UnityEngine.Vector2): UnityEngine.Vector2
---@overload fun(a:UnityEngine.Vector2, d:number): UnityEngine.Vector2
---@return UnityEngine.Vector2
---@param d number
---@param a UnityEngine.Vector2
function UnityEngine.Vector2.op_Multiply(d, a) end
---@overload fun(a:UnityEngine.Vector2, b:UnityEngine.Vector2): UnityEngine.Vector2
---@return UnityEngine.Vector2
---@param a UnityEngine.Vector2
---@param d number
function UnityEngine.Vector2.op_Division(a, d) end
---@return UnityEngine.Vector2
---@param a UnityEngine.Vector2
function UnityEngine.Vector2.op_UnaryNegation(a) end
---@return boolean
---@param lhs UnityEngine.Vector2
---@param rhs UnityEngine.Vector2
function UnityEngine.Vector2.op_Equality(lhs, rhs) end
---@return boolean
---@param lhs UnityEngine.Vector2
---@param rhs UnityEngine.Vector2
function UnityEngine.Vector2.op_Inequality(lhs, rhs) end
---@overload fun(v:UnityEngine.Vector3): UnityEngine.Vector2
---@return UnityEngine.Vector2
---@param v UnityEngine.Vector2
function UnityEngine.Vector2.op_Implicit(v) end
return UnityEngine.Vector2
