---@class CS.UnityEngine.TrackedReference
CS.UnityEngine.TrackedReference = { }
---@return boolean
---@param x CS.UnityEngine.TrackedReference
---@param y CS.UnityEngine.TrackedReference
function CS.UnityEngine.TrackedReference.op_Equality(x, y) end
---@return boolean
---@param x CS.UnityEngine.TrackedReference
---@param y CS.UnityEngine.TrackedReference
function CS.UnityEngine.TrackedReference.op_Inequality(x, y) end
---@return boolean
---@param o CS.System.Object
function CS.UnityEngine.TrackedReference:Equals(o) end
---@return number
function CS.UnityEngine.TrackedReference:GetHashCode() end
---@return boolean
---@param exists CS.UnityEngine.TrackedReference
function CS.UnityEngine.TrackedReference.op_Implicit(exists) end
return CS.UnityEngine.TrackedReference
