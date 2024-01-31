---@class UnityEngine.Plane : System.ValueType
---@field public normal UnityEngine.Vector3
---@field public distance number
---@field public flipped UnityEngine.Plane

---@type UnityEngine.Plane
UnityEngine.Plane = { }
---@overload fun(inNormal:UnityEngine.Vector3, inPoint:UnityEngine.Vector3): UnityEngine.Plane
---@overload fun(inNormal:UnityEngine.Vector3, d:number): UnityEngine.Plane
---@return UnityEngine.Plane
---@param a UnityEngine.Vector3
---@param b UnityEngine.Vector3
---@param c UnityEngine.Vector3
function UnityEngine.Plane.New(a, b, c) end
---@param inNormal UnityEngine.Vector3
---@param inPoint UnityEngine.Vector3
function UnityEngine.Plane:SetNormalAndPosition(inNormal, inPoint) end
---@param a UnityEngine.Vector3
---@param b UnityEngine.Vector3
---@param c UnityEngine.Vector3
function UnityEngine.Plane:Set3Points(a, b, c) end
function UnityEngine.Plane:Flip() end
---@overload fun(translation:UnityEngine.Vector3): void
---@param plane UnityEngine.Plane
---@param translation UnityEngine.Vector3
function UnityEngine.Plane:Translate(plane, translation) end
---@return UnityEngine.Vector3
---@param point UnityEngine.Vector3
function UnityEngine.Plane:ClosestPointOnPlane(point) end
---@return number
---@param point UnityEngine.Vector3
function UnityEngine.Plane:GetDistanceToPoint(point) end
---@return boolean
---@param point UnityEngine.Vector3
function UnityEngine.Plane:GetSide(point) end
---@return boolean
---@param inPt0 UnityEngine.Vector3
---@param inPt1 UnityEngine.Vector3
function UnityEngine.Plane:SameSide(inPt0, inPt1) end
---@return boolean
---@param ray UnityEngine.Ray
---@param enter System.Single
function UnityEngine.Plane:Raycast(ray, enter) end
---@overload fun(): string
---@return string
---@param format string
function UnityEngine.Plane:ToString(format) end
return UnityEngine.Plane
