---@class UnityEngine.Object
---@field public name string
---@field public hideFlags number

---@type UnityEngine.Object
UnityEngine.Object = { }
---@return UnityEngine.Object
function UnityEngine.Object.New() end
---@return number
function UnityEngine.Object:GetInstanceID() end
---@return number
function UnityEngine.Object:GetHashCode() end
---@return boolean
---@param other System.Object
function UnityEngine.Object:Equals(other) end
---@return boolean
---@param exists UnityEngine.Object
function UnityEngine.Object.op_Implicit(exists) end
---@overload fun(original:UnityEngine.Object): UnityEngine.Object
---@overload fun(original:UnityEngine.Object, parent:UnityEngine.Transform): UnityEngine.Object
---@overload fun(original:UnityEngine.Object, position:UnityEngine.Vector3, rotation:UnityEngine.Quaternion): UnityEngine.Object
---@overload fun(original:UnityEngine.Object, parent:UnityEngine.Transform, instantiateInWorldSpace:boolean): UnityEngine.Object
---@return UnityEngine.Object
---@param original UnityEngine.Object
---@param position UnityEngine.Vector3
---@param rotation UnityEngine.Quaternion
---@param parent UnityEngine.Transform
function UnityEngine.Object.Instantiate(original, position, rotation, parent) end
---@overload fun(obj:UnityEngine.Object): void
---@param obj UnityEngine.Object
---@param t number
function UnityEngine.Object.Destroy(obj, t) end
---@overload fun(obj:UnityEngine.Object): void
---@param obj UnityEngine.Object
---@param allowDestroyingAssets boolean
function UnityEngine.Object.DestroyImmediate(obj, allowDestroyingAssets) end
---@return UnityEngine.Object[]
---@param t string
function UnityEngine.Object.FindObjectsOfType(t) end
---@param target UnityEngine.Object
function UnityEngine.Object.DontDestroyOnLoad(target) end
---@return UnityEngine.Object
---@param t string
function UnityEngine.Object.FindObjectOfType(t) end
---@return string
function UnityEngine.Object:ToString() end
---@return boolean
---@param x UnityEngine.Object
---@param y UnityEngine.Object
function UnityEngine.Object.op_Equality(x, y) end
---@return boolean
---@param x UnityEngine.Object
---@param y UnityEngine.Object
function UnityEngine.Object.op_Inequality(x, y) end
return UnityEngine.Object
