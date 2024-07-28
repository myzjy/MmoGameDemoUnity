---@class CS.System.Object
CS.System.Object = { }
---@return CS.System.Object
function CS.System.Object.New() end
---@overload fun(obj:CS.System.Object): boolean
---@return boolean
---@param objA CS.System.Object
---@param objB CS.System.Object
function CS.System.Object:Equals(objA, objB) end
---@return number
function CS.System.Object:GetHashCode() end
---@return string
function CS.System.Object:GetType() end
---@return string
function CS.System.Object:ToString() end
---@return boolean
---@param objA CS.System.Object
---@param objB CS.System.Object
function CS.System.Object.ReferenceEquals(objA, objB) end
---@return string
---@param format string
function CS.System.Object:ConverToString(format) end
---@return string
---@param t string
function CS.System.Object:ToStringableObjectConvertToString(t) end
return CS.System.Object
