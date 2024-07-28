---@class CS.System.TimeSpan : CS.System.ValueType
---@field public TicksPerMillisecond int64
---@field public TicksPerSecond int64
---@field public TicksPerMinute int64
---@field public TicksPerHour int64
---@field public TicksPerDay int64
---@field public Zero CS.System.TimeSpan
---@field public MaxValue CS.System.TimeSpan
---@field public MinValue CS.System.TimeSpan
---@field public Ticks int64
---@field public Days number
---@field public Hours number
---@field public Milliseconds number
---@field public Minutes number
---@field public Seconds number
---@field public TotalDays number
---@field public TotalHours number
---@field public TotalMilliseconds number
---@field public TotalMinutes number
---@field public TotalSeconds number
CS.System.TimeSpan = { }
---@overload fun(ticks:int64): CS.System.TimeSpan
---@overload fun(hours:number, minutes:number, seconds:number): CS.System.TimeSpan
---@overload fun(days:number, hours:number, minutes:number, seconds:number): CS.System.TimeSpan
---@return CS.System.TimeSpan
---@param days number
---@param hours number
---@param minutes number
---@param seconds number
---@param milliseconds number
function CS.System.TimeSpan.New(days, hours, minutes, seconds, milliseconds) end
---@return CS.System.TimeSpan
---@param ts CS.System.TimeSpan
function CS.System.TimeSpan:Add(ts) end
---@return number
---@param t1 CS.System.TimeSpan
---@param t2 CS.System.TimeSpan
function CS.System.TimeSpan.Compare(t1, t2) end
---@overload fun(value:CS.System.Object): number
---@return number
---@param value CS.System.TimeSpan
function CS.System.TimeSpan:CompareTo(value) end
---@return CS.System.TimeSpan
---@param value number
function CS.System.TimeSpan.FromDays(value) end
---@return CS.System.TimeSpan
function CS.System.TimeSpan:Duration() end
---@overload fun(value:CS.System.Object): boolean
---@overload fun(obj:CS.System.TimeSpan): boolean
---@return boolean
---@param t1 CS.System.TimeSpan
---@param t2 CS.System.TimeSpan
function CS.System.TimeSpan:Equals(t1, t2) end
---@return number
function CS.System.TimeSpan:GetHashCode() end
---@return CS.System.TimeSpan
---@param value number
function CS.System.TimeSpan.FromHours(value) end
---@return CS.System.TimeSpan
---@param value number
function CS.System.TimeSpan.FromMilliseconds(value) end
---@return CS.System.TimeSpan
---@param value number
function CS.System.TimeSpan.FromMinutes(value) end
---@return CS.System.TimeSpan
function CS.System.TimeSpan:Negate() end
---@return CS.System.TimeSpan
---@param value number
function CS.System.TimeSpan.FromSeconds(value) end
---@return CS.System.TimeSpan
---@param ts CS.System.TimeSpan
function CS.System.TimeSpan:Subtract(ts) end
---@return CS.System.TimeSpan
---@param value int64
function CS.System.TimeSpan.FromTicks(value) end
---@overload fun(s:string): CS.System.TimeSpan
---@return CS.System.TimeSpan
---@param input string
---@param formatProvider CS.System.IFormatProvider
function CS.System.TimeSpan.Parse(input, formatProvider) end
---@overload fun(input:string, format:string, formatProvider:CS.System.IFormatProvider): CS.System.TimeSpan
---@overload fun(input:string, formats:CS.System.String[], formatProvider:CS.System.IFormatProvider): CS.System.TimeSpan
---@overload fun(input:string, format:string, formatProvider:CS.System.IFormatProvider, styles:number): CS.System.TimeSpan
---@return CS.System.TimeSpan
---@param input string
---@param formats CS.System.String[]
---@param formatProvider CS.System.IFormatProvider
---@param styles number
function CS.System.TimeSpan.ParseExact(input, formats, formatProvider, styles) end
---@overload fun(s:string, result:CS.System.TimeSpan): boolean
---@return boolean
---@param input string
---@param formatProvider CS.System.IFormatProvider
---@param result CS.System.TimeSpan
function CS.System.TimeSpan.TryParse(input, formatProvider, result) end
---@overload fun(input:string, format:string, formatProvider:CS.System.IFormatProvider, result:CS.System.TimeSpan): boolean
---@overload fun(input:string, formats:CS.System.String[], formatProvider:CS.System.IFormatProvider, result:CS.System.TimeSpan): boolean
---@overload fun(input:string, format:string, formatProvider:CS.System.IFormatProvider, styles:number, result:CS.System.TimeSpan): boolean
---@return boolean
---@param input string
---@param formats CS.System.String[]
---@param formatProvider CS.System.IFormatProvider
---@param styles number
---@param result CS.System.TimeSpan
function CS.System.TimeSpan.TryParseExact(input, formats, formatProvider, styles, result) end
---@overload fun(): string
---@overload fun(format:string): string
---@return string
---@param format string
---@param formatProvider CS.System.IFormatProvider
function CS.System.TimeSpan:ToString(format, formatProvider) end
---@return CS.System.TimeSpan
---@param t CS.System.TimeSpan
function CS.System.TimeSpan.op_UnaryNegation(t) end
---@return CS.System.TimeSpan
---@param t1 CS.System.TimeSpan
---@param t2 CS.System.TimeSpan
function CS.System.TimeSpan.op_Subtraction(t1, t2) end
---@return CS.System.TimeSpan
---@param t CS.System.TimeSpan
function CS.System.TimeSpan.op_UnaryPlus(t) end
---@return CS.System.TimeSpan
---@param t1 CS.System.TimeSpan
---@param t2 CS.System.TimeSpan
function CS.System.TimeSpan.op_Addition(t1, t2) end
---@return boolean
---@param t1 CS.System.TimeSpan
---@param t2 CS.System.TimeSpan
function CS.System.TimeSpan.op_Equality(t1, t2) end
---@return boolean
---@param t1 CS.System.TimeSpan
---@param t2 CS.System.TimeSpan
function CS.System.TimeSpan.op_Inequality(t1, t2) end
---@return boolean
---@param t1 CS.System.TimeSpan
---@param t2 CS.System.TimeSpan
function CS.System.TimeSpan.op_LessThan(t1, t2) end
---@return boolean
---@param t1 CS.System.TimeSpan
---@param t2 CS.System.TimeSpan
function CS.System.TimeSpan.op_LessThanOrEqual(t1, t2) end
---@return boolean
---@param t1 CS.System.TimeSpan
---@param t2 CS.System.TimeSpan
function CS.System.TimeSpan.op_GreaterThan(t1, t2) end
---@return boolean
---@param t1 CS.System.TimeSpan
---@param t2 CS.System.TimeSpan
function CS.System.TimeSpan.op_GreaterThanOrEqual(t1, t2) end
return CS.System.TimeSpan
