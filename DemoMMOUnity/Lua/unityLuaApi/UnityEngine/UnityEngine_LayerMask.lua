---@class UnityEngine.LayerMask : System.ValueType
---@field public value number

---@type UnityEngine.LayerMask
UnityEngine.LayerMask = { }
---@overload fun(mask:UnityEngine.LayerMask): number
---@return number
---@param intVal number
function UnityEngine.LayerMask.op_Implicit(intVal) end
---@return string
---@param layer number
function UnityEngine.LayerMask.LayerToName(layer) end
---@return number
---@param layerName string
function UnityEngine.LayerMask.NameToLayer(layerName) end
---@return number
---@param layerNames System.String[]
function UnityEngine.LayerMask.GetMask(layerNames) end
return UnityEngine.LayerMask
