---@class UnityEngine.Matrix4x4 : System.ValueType
---@field public m00 number
---@field public m10 number
---@field public m20 number
---@field public m30 number
---@field public m01 number
---@field public m11 number
---@field public m21 number
---@field public m31 number
---@field public m02 number
---@field public m12 number
---@field public m22 number
---@field public m32 number
---@field public m03 number
---@field public m13 number
---@field public m23 number
---@field public m33 number
---@field public rotation UnityEngine.Quaternion
---@field public lossyScale UnityEngine.Vector3
---@field public isIdentity boolean
---@field public determinant number
---@field public decomposeProjection UnityEngine.FrustumPlanes
---@field public inverse UnityEngine.Matrix4x4
---@field public transpose UnityEngine.Matrix4x4
---@field public Item number
---@field public Item number
---@field public zero UnityEngine.Matrix4x4
---@field public identity UnityEngine.Matrix4x4

---@type UnityEngine.Matrix4x4
UnityEngine.Matrix4x4 = { }
---@return UnityEngine.Matrix4x4
---@param column0 UnityEngine.Vector4
---@param column1 UnityEngine.Vector4
---@param column2 UnityEngine.Vector4
---@param column3 UnityEngine.Vector4
function UnityEngine.Matrix4x4.New(column0, column1, column2, column3) end
---@return boolean
function UnityEngine.Matrix4x4:ValidTRS() end
---@return number
---@param m UnityEngine.Matrix4x4
function UnityEngine.Matrix4x4.Determinant(m) end
---@return UnityEngine.Matrix4x4
---@param pos UnityEngine.Vector3
---@param q UnityEngine.Quaternion
---@param s UnityEngine.Vector3
function UnityEngine.Matrix4x4.TRS(pos, q, s) end
---@param pos UnityEngine.Vector3
---@param q UnityEngine.Quaternion
---@param s UnityEngine.Vector3
function UnityEngine.Matrix4x4:SetTRS(pos, q, s) end
---@return UnityEngine.Matrix4x4
---@param m UnityEngine.Matrix4x4
function UnityEngine.Matrix4x4.Inverse(m) end
---@return UnityEngine.Matrix4x4
---@param m UnityEngine.Matrix4x4
function UnityEngine.Matrix4x4.Transpose(m) end
---@return UnityEngine.Matrix4x4
---@param left number
---@param right number
---@param bottom number
---@param top number
---@param zNear number
---@param zFar number
function UnityEngine.Matrix4x4.Ortho(left, right, bottom, top, zNear, zFar) end
---@return UnityEngine.Matrix4x4
---@param fov number
---@param aspect number
---@param zNear number
---@param zFar number
function UnityEngine.Matrix4x4.Perspective(fov, aspect, zNear, zFar) end
---@return UnityEngine.Matrix4x4
---@param from UnityEngine.Vector3
---@param to UnityEngine.Vector3
---@param up UnityEngine.Vector3
function UnityEngine.Matrix4x4.LookAt(from, to, up) end
---@overload fun(fp:UnityEngine.FrustumPlanes): UnityEngine.Matrix4x4
---@return UnityEngine.Matrix4x4
---@param left number
---@param right number
---@param bottom number
---@param top number
---@param zNear number
---@param zFar number
function UnityEngine.Matrix4x4.Frustum(left, right, bottom, top, zNear, zFar) end
---@return number
function UnityEngine.Matrix4x4:GetHashCode() end
---@overload fun(other:System.Object): boolean
---@return boolean
---@param other UnityEngine.Matrix4x4
function UnityEngine.Matrix4x4:Equals(other) end
---@overload fun(lhs:UnityEngine.Matrix4x4, rhs:UnityEngine.Matrix4x4): UnityEngine.Matrix4x4
---@return UnityEngine.Matrix4x4
---@param lhs UnityEngine.Matrix4x4
---@param vector UnityEngine.Vector4
function UnityEngine.Matrix4x4.op_Multiply(lhs, vector) end
---@return boolean
---@param lhs UnityEngine.Matrix4x4
---@param rhs UnityEngine.Matrix4x4
function UnityEngine.Matrix4x4.op_Equality(lhs, rhs) end
---@return boolean
---@param lhs UnityEngine.Matrix4x4
---@param rhs UnityEngine.Matrix4x4
function UnityEngine.Matrix4x4.op_Inequality(lhs, rhs) end
---@return UnityEngine.Vector4
---@param index number
function UnityEngine.Matrix4x4:GetColumn(index) end
---@return UnityEngine.Vector4
---@param index number
function UnityEngine.Matrix4x4:GetRow(index) end
---@param index number
---@param column UnityEngine.Vector4
function UnityEngine.Matrix4x4:SetColumn(index, column) end
---@param index number
---@param row UnityEngine.Vector4
function UnityEngine.Matrix4x4:SetRow(index, row) end
---@return UnityEngine.Vector3
---@param point UnityEngine.Vector3
function UnityEngine.Matrix4x4:MultiplyPoint(point) end
---@return UnityEngine.Vector3
---@param point UnityEngine.Vector3
function UnityEngine.Matrix4x4:MultiplyPoint3x4(point) end
---@return UnityEngine.Vector3
---@param vector UnityEngine.Vector3
function UnityEngine.Matrix4x4:MultiplyVector(vector) end
---@return UnityEngine.Plane
---@param plane UnityEngine.Plane
function UnityEngine.Matrix4x4:TransformPlane(plane) end
---@return UnityEngine.Matrix4x4
---@param vector UnityEngine.Vector3
function UnityEngine.Matrix4x4.Scale(vector) end
---@return UnityEngine.Matrix4x4
---@param vector UnityEngine.Vector3
function UnityEngine.Matrix4x4.Translate(vector) end
---@return UnityEngine.Matrix4x4
---@param q UnityEngine.Quaternion
function UnityEngine.Matrix4x4.Rotate(q) end
---@overload fun(): string
---@return string
---@param format string
function UnityEngine.Matrix4x4:ToString(format) end
return UnityEngine.Matrix4x4
