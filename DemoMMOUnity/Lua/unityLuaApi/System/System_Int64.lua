---@class System.Int64 : System.ValueType
---@field public MaxValue int64
---@field public MinValue int64

---@type System.Int64
System.Int64 = { }
---@overload fun(value:System.Object): number
---@return number
---@param value int64
function System.Int64:CompareTo(value) end
---@overload fun(obj:System.Object): boolean
---@return boolean
---@param obj int64
function System.Int64:Equals(obj) end
---@return number
function System.Int64:GetHashCode() end
---@overload fun(): string
---@overload fun(provider:System.IFormatProvider): string
---@overload fun(format:string): string
---@return string
---@param format string
---@param provider System.IFormatProvider
function System.Int64:ToString(format, provider) end
---@overload fun(s:string): int64
---@overload fun(s:string, style:number): int64
---@overload fun(s:string, provider:System.IFormatProvider): int64
---@return int64
---@param s string
---@param style number
---@param provider System.IFormatProvider
function System.Int64.Parse(s, style, provider) end
---@overload fun(s:string, result:System.Int64): boolean
---@return boolean
---@param s string
---@param style number
---@param provider System.IFormatProvider
---@param result System.Int64
function System.Int64.TryParse(s, style, provider, result) end
---@return number
function System.Int64:GetTypeCode() end
return System.Int64
