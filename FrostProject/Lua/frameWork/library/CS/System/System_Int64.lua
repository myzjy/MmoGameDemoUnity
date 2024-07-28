---@class CS.System.Int64 : CS.System.ValueType
---@field public MaxValue int64
---@field public MinValue int64
CS.System.Int64 = { }
---@overload fun(value:CS.System.Object): number
---@return number
---@param value int64
function CS.System.Int64:CompareTo(value) end
---@overload fun(obj:CS.System.Object): boolean
---@return boolean
---@param obj int64
function CS.System.Int64:Equals(obj) end
---@return number
function CS.System.Int64:GetHashCode() end
---@overload fun(): string
---@overload fun(provider:CS.System.IFormatProvider): string
---@overload fun(format:string): string
---@return string
---@param format string
---@param provider CS.System.IFormatProvider
function CS.System.Int64:ToString(format, provider) end
---@overload fun(s:string): int64
---@overload fun(s:string, style:number): int64
---@overload fun(s:string, provider:CS.System.IFormatProvider): int64
---@return int64
---@param s string
---@param style number
---@param provider CS.System.IFormatProvider
function CS.System.Int64.Parse(s, style, provider) end
---@overload fun(s:string, result:CS.System.Int64): boolean
---@return boolean
---@param s string
---@param style number
---@param provider CS.System.IFormatProvider
---@param result CS.System.Int64
function CS.System.Int64.TryParse(s, style, provider, result) end
---@return number
function CS.System.Int64:GetTypeCode() end
return CS.System.Int64
