---@class UnityEngine.TrackedReference

---@type UnityEngine.TrackedReference
UnityEngine.TrackedReference = { }
---@return boolean
---@param x UnityEngine.TrackedReference
---@param y UnityEngine.TrackedReference
function UnityEngine.TrackedReference.op_Equality(x, y) end
---@return boolean
---@param x UnityEngine.TrackedReference
---@param y UnityEngine.TrackedReference
function UnityEngine.TrackedReference.op_Inequality(x, y) end
---@return boolean
---@param o System.Object
function UnityEngine.TrackedReference:Equals(o) end
---@return number
function UnityEngine.TrackedReference:GetHashCode() end
---@return boolean
---@param exists UnityEngine.TrackedReference
function UnityEngine.TrackedReference.op_Implicit(exists) end
return UnityEngine.TrackedReference
