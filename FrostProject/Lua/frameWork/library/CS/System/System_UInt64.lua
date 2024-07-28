---@class CS.System.UInt64 : CS.System.ValueType
---@field public MaxValue CS.System.UInt64
---@field public MinValue CS.System.UInt64
CS.System.UInt64 = { }
---@overload fun(value:CS.System.Object): number
---@return number
---@param value CS.System.UInt64
function CS.System.UInt64:CompareTo(value) end
---@overload fun(obj:CS.System.Object): boolean
---@return boolean
---@param obj CS.System.UInt64
function CS.System.UInt64:Equals(obj) end
---@return number
function CS.System.UInt64:GetHashCode() end
---@overload fun(): string
---@overload fun(provider:CS.System.IFormatProvider): string
---@overload fun(format:string): string
---@return string
---@param format string
---@param provider CS.System.IFormatProvider
function CS.System.UInt64:ToString(format, provider) end
---@overload fun(s:string): CS.System.UInt64
---@overload fun(s:string, style:number): CS.System.UInt64
---@overload fun(s:string, provider:CS.System.IFormatProvider): CS.System.UInt64
---@return CS.System.UInt64
---@param s string
---@param style number
---@param provider CS.System.IFormatProvider
function CS.System.UInt64.Parse(s, style, provider) end
---@overload fun(s:string, result:CS.System.UInt64): boolean
---@return boolean
---@param s string
---@param style number
---@param provider CS.System.IFormatProvider
---@param result CS.System.UInt64
function CS.System.UInt64.TryParse(s, style, provider, result) end
---@return number
function CS.System.UInt64:GetTypeCode() end
return CS.System.UInt64
