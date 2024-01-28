---@class UnityEngine.Transform : UnityEngine.Component
---@field public position UnityEngine.Vector3
---@field public localPosition UnityEngine.Vector3
---@field public eulerAngles UnityEngine.Vector3
---@field public localEulerAngles UnityEngine.Vector3
---@field public right UnityEngine.Vector3
---@field public up UnityEngine.Vector3
---@field public forward UnityEngine.Vector3
---@field public rotation UnityEngine.Quaternion
---@field public localRotation UnityEngine.Quaternion
---@field public localScale UnityEngine.Vector3
---@field public parent UnityEngine.Transform
---@field public worldToLocalMatrix UnityEngine.Matrix4x4
---@field public localToWorldMatrix UnityEngine.Matrix4x4
---@field public root UnityEngine.Transform
---@field public childCount number
---@field public lossyScale UnityEngine.Vector3
---@field public hasChanged boolean
---@field public hierarchyCapacity number
---@field public hierarchyCount number

---@type UnityEngine.Transform
UnityEngine.Transform = { }
---@overload fun(p:UnityEngine.Transform): void
---@param parent UnityEngine.Transform
---@param worldPositionStays boolean
function UnityEngine.Transform:SetParent(parent, worldPositionStays) end
---@param position UnityEngine.Vector3
---@param rotation UnityEngine.Quaternion
function UnityEngine.Transform:SetPositionAndRotation(position, rotation) end
---@overload fun(translation:UnityEngine.Vector3): void
---@overload fun(translation:UnityEngine.Vector3, relativeTo:number): void
---@overload fun(translation:UnityEngine.Vector3, relativeTo:UnityEngine.Transform): void
---@overload fun(x:number, y:number, z:number): void
---@overload fun(x:number, y:number, z:number, relativeTo:number): void
---@param x number
---@param y number
---@param z number
---@param relativeTo UnityEngine.Transform
function UnityEngine.Transform:Translate(x, y, z, relativeTo) end
---@overload fun(eulers:UnityEngine.Vector3): void
---@overload fun(eulers:UnityEngine.Vector3, relativeTo:number): void
---@overload fun(axis:UnityEngine.Vector3, angle:number): void
---@overload fun(xAngle:number, yAngle:number, zAngle:number): void
---@overload fun(axis:UnityEngine.Vector3, angle:number, relativeTo:number): void
---@param xAngle number
---@param yAngle number
---@param zAngle number
---@param relativeTo number
function UnityEngine.Transform:Rotate(xAngle, yAngle, zAngle, relativeTo) end
---@param point UnityEngine.Vector3
---@param axis UnityEngine.Vector3
---@param angle number
function UnityEngine.Transform:RotateAround(point, axis, angle) end
---@overload fun(target:UnityEngine.Transform): void
---@overload fun(worldPosition:UnityEngine.Vector3): void
---@overload fun(target:UnityEngine.Transform, worldUp:UnityEngine.Vector3): void
---@param worldPosition UnityEngine.Vector3
---@param worldUp UnityEngine.Vector3
function UnityEngine.Transform:LookAt(worldPosition, worldUp) end
---@overload fun(direction:UnityEngine.Vector3): UnityEngine.Vector3
---@return UnityEngine.Vector3
---@param x number
---@param y number
---@param z number
function UnityEngine.Transform:TransformDirection(x, y, z) end
---@overload fun(direction:UnityEngine.Vector3): UnityEngine.Vector3
---@return UnityEngine.Vector3
---@param x number
---@param y number
---@param z number
function UnityEngine.Transform:InverseTransformDirection(x, y, z) end
---@overload fun(vector:UnityEngine.Vector3): UnityEngine.Vector3
---@return UnityEngine.Vector3
---@param x number
---@param y number
---@param z number
function UnityEngine.Transform:TransformVector(x, y, z) end
---@overload fun(vector:UnityEngine.Vector3): UnityEngine.Vector3
---@return UnityEngine.Vector3
---@param x number
---@param y number
---@param z number
function UnityEngine.Transform:InverseTransformVector(x, y, z) end
---@overload fun(position:UnityEngine.Vector3): UnityEngine.Vector3
---@return UnityEngine.Vector3
---@param x number
---@param y number
---@param z number
function UnityEngine.Transform:TransformPoint(x, y, z) end
---@overload fun(position:UnityEngine.Vector3): UnityEngine.Vector3
---@return UnityEngine.Vector3
---@param x number
---@param y number
---@param z number
function UnityEngine.Transform:InverseTransformPoint(x, y, z) end
function UnityEngine.Transform:DetachChildren() end
function UnityEngine.Transform:SetAsFirstSibling() end
function UnityEngine.Transform:SetAsLastSibling() end
---@param index number
function UnityEngine.Transform:SetSiblingIndex(index) end
---@return number
function UnityEngine.Transform:GetSiblingIndex() end
---@return UnityEngine.Transform
---@param n string
function UnityEngine.Transform:Find(n) end
---@return boolean
---@param parent UnityEngine.Transform
function UnityEngine.Transform:IsChildOf(parent) end
---@return System.Collections.IEnumerator
function UnityEngine.Transform:GetEnumerator() end
---@return UnityEngine.Transform
---@param index number
function UnityEngine.Transform:GetChild(index) end
function UnityEngine.Transform:ResetTransform() end
---@return UnityEngine.Vector3
---@param pos UnityEngine.Vector3
---@param euler UnityEngine.Vector3
function UnityEngine.Transform:NewRotateAround(pos, euler) end
---@overload fun(pos:UnityEngine.Vector3): void
---@param x number
---@param y number
---@param z number
function UnityEngine.Transform:SetPos(x, y, z) end
---@param x number
function UnityEngine.Transform:SetPosX(x) end
---@param y number
function UnityEngine.Transform:SetPosY(y) end
---@param z number
function UnityEngine.Transform:SetPosZ(z) end
function UnityEngine.Transform:SetPosZero() end
---@overload fun(other:UnityEngine.GameObject): void
---@param other UnityEngine.Transform
function UnityEngine.Transform:SetPosToOther(other) end
---@overload fun(pos:UnityEngine.Vector3): void
---@param x number
---@param y number
---@param z number
function UnityEngine.Transform:SetLocalPos(x, y, z) end
---@param x number
function UnityEngine.Transform:SetLocalPosX(x) end
---@param y number
function UnityEngine.Transform:SetLocalPosY(y) end
---@param z number
function UnityEngine.Transform:SetLocalPosZ(z) end
function UnityEngine.Transform:SetLocalPosZero() end
---@overload fun(other:UnityEngine.GameObject): void
---@param other UnityEngine.Transform
function UnityEngine.Transform:SetLocalPosToOther(other) end
---@param x number
---@param y number
---@param z number
function UnityEngine.Transform:SetLocalScale(x, y, z) end
---@param x number
function UnityEngine.Transform:SetLocalScaleX(x) end
---@param y number
function UnityEngine.Transform:SetLocalScaleY(y) end
---@param z number
function UnityEngine.Transform:SetLocalScaleZ(z) end
function UnityEngine.Transform:SetLocalScaleOne() end
---@param times number
function UnityEngine.Transform:SetLocalScaleDouble(times) end
---@overload fun(other:UnityEngine.GameObject): void
---@param other UnityEngine.Transform
function UnityEngine.Transform:SetLocalScaleToOther(other) end
---@param x number
---@param y number
---@param z number
---@param w number
function UnityEngine.Transform:SetRot(x, y, z, w) end
---@param x number
---@param y number
---@param z number
---@param w number
function UnityEngine.Transform:SetLocalRot(x, y, z, w) end
---@param x number
---@param y number
---@param z number
function UnityEngine.Transform:SetRotEuler(x, y, z) end
---@param x number
function UnityEngine.Transform:SetRotEulerX(x) end
---@param y number
function UnityEngine.Transform:SetRotEulerY(y) end
---@param z number
function UnityEngine.Transform:SetRotEulerZ(z) end
function UnityEngine.Transform:SetRotEulerZero() end
---@overload fun(other:UnityEngine.GameObject): void
---@param other UnityEngine.Transform
function UnityEngine.Transform:SetRotEulerToOther(other) end
---@param x number
---@param y number
---@param z number
function UnityEngine.Transform:SetLocalRotEuler(x, y, z) end
---@param x number
function UnityEngine.Transform:SetLocalRotEulerX(x) end
---@param y number
function UnityEngine.Transform:SetLocalRotEulerY(y) end
---@param z number
function UnityEngine.Transform:SetLocalRotEulerZ(z) end
function UnityEngine.Transform:SetLocalRotEulerZero() end
---@overload fun(other:UnityEngine.GameObject): void
---@param other UnityEngine.Transform
function UnityEngine.Transform:SetLocalRotEulerToOther(other) end
---@return UnityEngine.Vector2
---@param canvas UnityEngine.Canvas
function UnityEngine.Transform:TransformToCanvasLocalPosition(canvas) end
return UnityEngine.Transform
