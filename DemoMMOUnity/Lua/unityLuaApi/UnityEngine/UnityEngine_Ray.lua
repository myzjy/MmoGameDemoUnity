---@class UnityEngine.Ray : System.ValueType
---@field public origin UnityEngine.Vector3
---@field public direction UnityEngine.Vector3

---@type UnityEngine.Ray
UnityEngine.Ray = { }
---@return UnityEngine.Ray
---@param origin UnityEngine.Vector3
---@param direction UnityEngine.Vector3
function UnityEngine.Ray.New(origin, direction) end
---@return UnityEngine.Vector3
---@param distance number
function UnityEngine.Ray:GetPoint(distance) end
---@overload fun(): string
---@return string
---@param format string
function UnityEngine.Ray:ToString(format) end
return UnityEngine.Ray
