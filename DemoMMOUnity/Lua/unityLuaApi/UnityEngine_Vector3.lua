---@class UnityEngine.Vector3 : System.ValueType
---@field public kEpsilon number
---@field public kEpsilonNormalSqrt number
---@field public x number
---@field public y number
---@field public z number
---@field public Item number
---@field public normalized UnityEngine.Vector3
---@field public magnitude number
---@field public sqrMagnitude number
---@field public zero UnityEngine.Vector3
---@field public one UnityEngine.Vector3
---@field public forward UnityEngine.Vector3
---@field public back UnityEngine.Vector3
---@field public up UnityEngine.Vector3
---@field public down UnityEngine.Vector3
---@field public left UnityEngine.Vector3
---@field public right UnityEngine.Vector3
---@field public positiveInfinity UnityEngine.Vector3
---@field public negativeInfinity UnityEngine.Vector3

---@type UnityEngine.Vector3
UnityEngine.Vector3 = { }
---@overload fun(x:number, y:number): UnityEngine.Vector3
---@return UnityEngine.Vector3
---@param x number
---@param y number
---@param z number
function UnityEngine.Vector3.New(x, y, z) end
---@return UnityEngine.Vector3
---@param a UnityEngine.Vector3
---@param b UnityEngine.Vector3
---@param t number
function UnityEngine.Vector3.Slerp(a, b, t) end
---@return UnityEngine.Vector3
---@param a UnityEngine.Vector3
---@param b UnityEngine.Vector3
---@param t number
function UnityEngine.Vector3.SlerpUnclamped(a, b, t) end
---@overload fun(normal:UnityEngine.Vector3, tangent:UnityEngine.Vector3): void
---@param normal UnityEngine.Vector3
---@param tangent UnityEngine.Vector3
---@param binormal UnityEngine.Vector3
function UnityEngine.Vector3.OrthoNormalize(normal, tangent, binormal) end
---@return UnityEngine.Vector3
---@param current UnityEngine.Vector3
---@param target UnityEngine.Vector3
---@param maxRadiansDelta number
---@param maxMagnitudeDelta number
function UnityEngine.Vector3.RotateTowards(current, target, maxRadiansDelta, maxMagnitudeDelta) end
---@return UnityEngine.Vector3
---@param a UnityEngine.Vector3
---@param b UnityEngine.Vector3
---@param t number
function UnityEngine.Vector3.Lerp(a, b, t) end
---@return UnityEngine.Vector3
---@param a UnityEngine.Vector3
---@param b UnityEngine.Vector3
---@param t number
function UnityEngine.Vector3.LerpUnclamped(a, b, t) end
---@return UnityEngine.Vector3
---@param current UnityEngine.Vector3
---@param target UnityEngine.Vector3
---@param maxDistanceDelta number
function UnityEngine.Vector3.MoveTowards(current, target, maxDistanceDelta) end
---@overload fun(current:UnityEngine.Vector3, target:UnityEngine.Vector3, currentVelocity:UnityEngine.Vector3, smoothTime:number): UnityEngine.Vector3
---@overload fun(current:UnityEngine.Vector3, target:UnityEngine.Vector3, currentVelocity:UnityEngine.Vector3, smoothTime:number, maxSpeed:number): UnityEngine.Vector3
---@return UnityEngine.Vector3
---@param current UnityEngine.Vector3
---@param target UnityEngine.Vector3
---@param currentVelocity UnityEngine.Vector3
---@param smoothTime number
---@param maxSpeed number
---@param deltaTime number
function UnityEngine.Vector3.SmoothDamp(current, target, currentVelocity, smoothTime, maxSpeed, deltaTime) end
---@param newX number
---@param newY number
---@param newZ number
function UnityEngine.Vector3:Set(newX, newY, newZ) end
---@overload fun(scale:UnityEngine.Vector3): void
---@param a UnityEngine.Vector3
---@param b UnityEngine.Vector3
function UnityEngine.Vector3:Scale(a, b) end
---@return UnityEngine.Vector3
---@param lhs UnityEngine.Vector3
---@param rhs UnityEngine.Vector3
function UnityEngine.Vector3.Cross(lhs, rhs) end
---@return number
function UnityEngine.Vector3:GetHashCode() end
---@overload fun(other:System.Object): boolean
---@return boolean
---@param other UnityEngine.Vector3
function UnityEngine.Vector3:Equals(other) end
---@return UnityEngine.Vector3
---@param inDirection UnityEngine.Vector3
---@param inNormal UnityEngine.Vector3
function UnityEngine.Vector3.Reflect(inDirection, inNormal) end
---@overload fun(): void
---@param value UnityEngine.Vector3
function UnityEngine.Vector3:Normalize(value) end
---@return number
---@param lhs UnityEngine.Vector3
---@param rhs UnityEngine.Vector3
function UnityEngine.Vector3.Dot(lhs, rhs) end
---@return UnityEngine.Vector3
---@param vector UnityEngine.Vector3
---@param onNormal UnityEngine.Vector3
function UnityEngine.Vector3.Project(vector, onNormal) end
---@return UnityEngine.Vector3
---@param vector UnityEngine.Vector3
---@param planeNormal UnityEngine.Vector3
function UnityEngine.Vector3.ProjectOnPlane(vector, planeNormal) end
---@return number
---@param from UnityEngine.Vector3
---@param to UnityEngine.Vector3
function UnityEngine.Vector3.Angle(from, to) end
---@return number
---@param from UnityEngine.Vector3
---@param to UnityEngine.Vector3
---@param axis UnityEngine.Vector3
function UnityEngine.Vector3.SignedAngle(from, to, axis) end
---@return number
---@param a UnityEngine.Vector3
---@param b UnityEngine.Vector3
function UnityEngine.Vector3.Distance(a, b) end
---@return UnityEngine.Vector3
---@param vector UnityEngine.Vector3
---@param maxLength number
function UnityEngine.Vector3.ClampMagnitude(vector, maxLength) end
---@return number
---@param vector UnityEngine.Vector3
function UnityEngine.Vector3.Magnitude(vector) end
---@return number
---@param vector UnityEngine.Vector3
function UnityEngine.Vector3.SqrMagnitude(vector) end
---@return UnityEngine.Vector3
---@param lhs UnityEngine.Vector3
---@param rhs UnityEngine.Vector3
function UnityEngine.Vector3.Min(lhs, rhs) end
---@return UnityEngine.Vector3
---@param lhs UnityEngine.Vector3
---@param rhs UnityEngine.Vector3
function UnityEngine.Vector3.Max(lhs, rhs) end
---@return UnityEngine.Vector3
---@param a UnityEngine.Vector3
---@param b UnityEngine.Vector3
function UnityEngine.Vector3.op_Addition(a, b) end
---@return UnityEngine.Vector3
---@param a UnityEngine.Vector3
---@param b UnityEngine.Vector3
function UnityEngine.Vector3.op_Subtraction(a, b) end
---@return UnityEngine.Vector3
---@param a UnityEngine.Vector3
function UnityEngine.Vector3.op_UnaryNegation(a) end
---@overload fun(a:UnityEngine.Vector3, d:number): UnityEngine.Vector3
---@return UnityEngine.Vector3
---@param d number
---@param a UnityEngine.Vector3
function UnityEngine.Vector3.op_Multiply(d, a) end
---@return UnityEngine.Vector3
---@param a UnityEngine.Vector3
---@param d number
function UnityEngine.Vector3.op_Division(a, d) end
---@return boolean
---@param lhs UnityEngine.Vector3
---@param rhs UnityEngine.Vector3
function UnityEngine.Vector3.op_Equality(lhs, rhs) end
---@return boolean
---@param lhs UnityEngine.Vector3
---@param rhs UnityEngine.Vector3
function UnityEngine.Vector3.op_Inequality(lhs, rhs) end
---@overload fun(): string
---@return string
---@param format string
function UnityEngine.Vector3:ToString(format) end
---@return ROGameLibs.Vector3
function UnityEngine.Vector3:ToGameLibVector() end
---@return ROGameLibs.Vector3
function UnityEngine.Vector3:PtrToVector3() end
---@return ROGameLibs.Vector3
function UnityEngine.Vector3:Round() end
return UnityEngine.Vector3
