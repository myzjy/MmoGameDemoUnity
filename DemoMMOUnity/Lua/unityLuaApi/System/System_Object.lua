---@class System.Object

---@type System.Object
System.Object = { }
---@return System.Object
function System.Object.New() end
---@overload fun(obj:System.Object): boolean
---@return boolean
---@param objA System.Object
---@param objB System.Object
function System.Object:Equals(objA, objB) end
---@return number
function System.Object:GetHashCode() end
---@return string
function System.Object:GetType() end
---@return string
function System.Object:ToString() end
---@return boolean
---@param objA System.Object
---@param objB System.Object
function System.Object.ReferenceEquals(objA, objB) end
---@return string
---@param format string
function System.Object:ConverToString(format) end
---@return string
---@param t string
function System.Object:ToStringableObjectConvertToString(t) end
return System.Object
