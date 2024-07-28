---@class CS.System.Enum : CS.System.ValueType
CS.System.Enum = { }
---@overload fun(enumType:string, value:string): CS.System.Object
---@return CS.System.Object
---@param enumType string
---@param value string
---@param ignoreCase boolean
function CS.System.Enum.Parse(enumType, value, ignoreCase) end
---@return string
---@param enumType string
function CS.System.Enum.GetUnderlyingType(enumType) end
---@return CS.System.Array
---@param enumType string
function CS.System.Enum.GetValues(enumType) end
---@return string
---@param enumType string
---@param value CS.System.Object
function CS.System.Enum.GetName(enumType, value) end
---@return CS.System.String[]
---@param enumType string
function CS.System.Enum.GetNames(enumType) end
---@overload fun(enumType:string, value:CS.System.Object): CS.System.Object
---@overload fun(enumType:string, value:number): CS.System.Object
---@overload fun(enumType:string, value:number): CS.System.Object
---@overload fun(enumType:string, value:number): CS.System.Object
---@overload fun(enumType:string, value:number): CS.System.Object
---@overload fun(enumType:string, value:number): CS.System.Object
---@overload fun(enumType:string, value:number): CS.System.Object
---@overload fun(enumType:string, value:int64): CS.System.Object
---@return CS.System.Object
---@param enumType string
---@param value uint64
function CS.System.Enum.ToObject(enumType, value) end
---@return boolean
---@param enumType string
---@param value CS.System.Object
function CS.System.Enum.IsDefined(enumType, value) end
---@return string
---@param enumType string
---@param value CS.System.Object
---@param format string
function CS.System.Enum.Format(enumType, value, format) end
---@return boolean
---@param obj CS.System.Object
function CS.System.Enum:Equals(obj) end
---@return number
function CS.System.Enum:GetHashCode() end
---@overload fun(): string
---@return string
---@param format string
function CS.System.Enum:ToString(format) end
---@return number
---@param target CS.System.Object
function CS.System.Enum:CompareTo(target) end
---@return boolean
---@param flag CS.System.Enum
function CS.System.Enum:HasFlag(flag) end
---@return number
function CS.System.Enum:GetTypeCode() end
return CS.System.Enum
