---@class System.TimeSpan : System.ValueType
---@field public TicksPerMillisecond int64
---@field public TicksPerSecond int64
---@field public TicksPerMinute int64
---@field public TicksPerHour int64
---@field public TicksPerDay int64
---@field public Zero System.TimeSpan
---@field public MaxValue System.TimeSpan
---@field public MinValue System.TimeSpan
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

---@type System.TimeSpan
System.TimeSpan = { }
---@overload fun(ticks:int64): System.TimeSpan
---@overload fun(hours:number, minutes:number, seconds:number): System.TimeSpan
---@overload fun(days:number, hours:number, minutes:number, seconds:number): System.TimeSpan
---@return System.TimeSpan
---@param days number
---@param hours number
---@param minutes number
---@param seconds number
---@param milliseconds number
function System.TimeSpan.New(days, hours, minutes, seconds, milliseconds) end
---@return System.TimeSpan
---@param ts System.TimeSpan
function System.TimeSpan:Add(ts) end
---@return number
---@param t1 System.TimeSpan
---@param t2 System.TimeSpan
function System.TimeSpan.Compare(t1, t2) end
---@overload fun(value:System.Object): number
---@return number
---@param value System.TimeSpan
function System.TimeSpan:CompareTo(value) end
---@return System.TimeSpan
---@param value number
function System.TimeSpan.FromDays(value) end
---@return System.TimeSpan
function System.TimeSpan:Duration() end
---@overload fun(value:System.Object): boolean
---@overload fun(obj:System.TimeSpan): boolean
---@return boolean
---@param t1 System.TimeSpan
---@param t2 System.TimeSpan
function System.TimeSpan:Equals(t1, t2) end
---@return number
function System.TimeSpan:GetHashCode() end
---@return System.TimeSpan
---@param value number
function System.TimeSpan.FromHours(value) end
---@return System.TimeSpan
---@param value number
function System.TimeSpan.FromMilliseconds(value) end
---@return System.TimeSpan
---@param value number
function System.TimeSpan.FromMinutes(value) end
---@return System.TimeSpan
function System.TimeSpan:Negate() end
---@return System.TimeSpan
---@param value number
function System.TimeSpan.FromSeconds(value) end
---@return System.TimeSpan
---@param ts System.TimeSpan
function System.TimeSpan:Subtract(ts) end
---@return System.TimeSpan
---@param value int64
function System.TimeSpan.FromTicks(value) end
---@overload fun(s:string): System.TimeSpan
---@return System.TimeSpan
---@param input string
---@param formatProvider System.IFormatProvider
function System.TimeSpan.Parse(input, formatProvider) end
---@overload fun(input:string, format:string, formatProvider:System.IFormatProvider): System.TimeSpan
---@overload fun(input:string, formats:System.String[], formatProvider:System.IFormatProvider): System.TimeSpan
---@overload fun(input:string, format:string, formatProvider:System.IFormatProvider, styles:number): System.TimeSpan
---@return System.TimeSpan
---@param input string
---@param formats System.String[]
---@param formatProvider System.IFormatProvider
---@param styles number
function System.TimeSpan.ParseExact(input, formats, formatProvider, styles) end
---@overload fun(s:string, result:System.TimeSpan): boolean
---@return boolean
---@param input string
---@param formatProvider System.IFormatProvider
---@param result System.TimeSpan
function System.TimeSpan.TryParse(input, formatProvider, result) end
---@overload fun(input:string, format:string, formatProvider:System.IFormatProvider, result:System.TimeSpan): boolean
---@overload fun(input:string, formats:System.String[], formatProvider:System.IFormatProvider, result:System.TimeSpan): boolean
---@overload fun(input:string, format:string, formatProvider:System.IFormatProvider, styles:number, result:System.TimeSpan): boolean
---@return boolean
---@param input string
---@param formats System.String[]
---@param formatProvider System.IFormatProvider
---@param styles number
---@param result System.TimeSpan
function System.TimeSpan.TryParseExact(input, formats, formatProvider, styles, result) end
---@overload fun(): string
---@overload fun(format:string): string
---@return string
---@param format string
---@param formatProvider System.IFormatProvider
function System.TimeSpan:ToString(format, formatProvider) end
---@return System.TimeSpan
---@param t System.TimeSpan
function System.TimeSpan.op_UnaryNegation(t) end
---@return System.TimeSpan
---@param t1 System.TimeSpan
---@param t2 System.TimeSpan
function System.TimeSpan.op_Subtraction(t1, t2) end
---@return System.TimeSpan
---@param t System.TimeSpan
function System.TimeSpan.op_UnaryPlus(t) end
---@return System.TimeSpan
---@param t1 System.TimeSpan
---@param t2 System.TimeSpan
function System.TimeSpan.op_Addition(t1, t2) end
---@return boolean
---@param t1 System.TimeSpan
---@param t2 System.TimeSpan
function System.TimeSpan.op_Equality(t1, t2) end
---@return boolean
---@param t1 System.TimeSpan
---@param t2 System.TimeSpan
function System.TimeSpan.op_Inequality(t1, t2) end
---@return boolean
---@param t1 System.TimeSpan
---@param t2 System.TimeSpan
function System.TimeSpan.op_LessThan(t1, t2) end
---@return boolean
---@param t1 System.TimeSpan
---@param t2 System.TimeSpan
function System.TimeSpan.op_LessThanOrEqual(t1, t2) end
---@return boolean
---@param t1 System.TimeSpan
---@param t2 System.TimeSpan
function System.TimeSpan.op_GreaterThan(t1, t2) end
---@return boolean
---@param t1 System.TimeSpan
---@param t2 System.TimeSpan
function System.TimeSpan.op_GreaterThanOrEqual(t1, t2) end
return System.TimeSpan
