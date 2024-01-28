---@class System.Enum : System.ValueType

---@type System.Enum
System.Enum = { }
---@overload fun(enumType:string, value:string): System.Object
---@return System.Object
---@param enumType string
---@param value string
---@param ignoreCase boolean
function System.Enum.Parse(enumType, value, ignoreCase) end
---@return string
---@param enumType string
function System.Enum.GetUnderlyingType(enumType) end
---@return System.Array
---@param enumType string
function System.Enum.GetValues(enumType) end
---@return string
---@param enumType string
---@param value System.Object
function System.Enum.GetName(enumType, value) end
---@return System.String[]
---@param enumType string
function System.Enum.GetNames(enumType) end
---@overload fun(enumType:string, value:System.Object): System.Object
---@overload fun(enumType:string, value:number): System.Object
---@overload fun(enumType:string, value:number): System.Object
---@overload fun(enumType:string, value:number): System.Object
---@overload fun(enumType:string, value:number): System.Object
---@overload fun(enumType:string, value:number): System.Object
---@overload fun(enumType:string, value:number): System.Object
---@overload fun(enumType:string, value:int64): System.Object
---@return System.Object
---@param enumType string
---@param value uint64
function System.Enum.ToObject(enumType, value) end
---@return boolean
---@param enumType string
---@param value System.Object
function System.Enum.IsDefined(enumType, value) end
---@return string
---@param enumType string
---@param value System.Object
---@param format string
function System.Enum.Format(enumType, value, format) end
---@return boolean
---@param obj System.Object
function System.Enum:Equals(obj) end
---@return number
function System.Enum:GetHashCode() end
---@overload fun(): string
---@return string
---@param format string
function System.Enum:ToString(format) end
---@return number
---@param target System.Object
function System.Enum:CompareTo(target) end
---@return boolean
---@param flag System.Enum
function System.Enum:HasFlag(flag) end
---@return number
function System.Enum:GetTypeCode() end
return System.Enum
