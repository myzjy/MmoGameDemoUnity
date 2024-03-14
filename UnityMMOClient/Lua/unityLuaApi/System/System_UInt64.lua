---@class System.UInt64 : System.ValueType
---@field public MaxValue System.UInt64
---@field public MinValue System.UInt64
System.UInt64 = { }
---@overload fun(value:System.Object): number
---@return number
---@param value System.UInt64
function System.UInt64:CompareTo(value) end
---@overload fun(obj:System.Object): boolean
---@return boolean
---@param obj System.UInt64
function System.UInt64:Equals(obj) end
---@return number
function System.UInt64:GetHashCode() end
---@overload fun(): string
---@overload fun(provider:System.IFormatProvider): string
---@overload fun(format:string): string
---@return string
---@param format string
---@param provider System.IFormatProvider
function System.UInt64:ToString(format, provider) end
---@overload fun(s:string): System.UInt64
---@overload fun(s:string, style:number): System.UInt64
---@overload fun(s:string, provider:System.IFormatProvider): System.UInt64
---@return System.UInt64
---@param s string
---@param style number
---@param provider System.IFormatProvider
function System.UInt64.Parse(s, style, provider) end
---@overload fun(s:string, result:System.UInt64): boolean
---@return boolean
---@param s string
---@param style number
---@param provider System.IFormatProvider
---@param result System.UInt64
function System.UInt64.TryParse(s, style, provider, result) end
---@return number
function System.UInt64:GetTypeCode() end
return System.UInt64
