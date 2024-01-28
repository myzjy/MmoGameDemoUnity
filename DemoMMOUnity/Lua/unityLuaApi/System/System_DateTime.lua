---@class System.DateTime : System.ValueType
---@field public MinValue System.DateTime
---@field public MaxValue System.DateTime
---@field public Date System.DateTime
---@field public Day number
---@field public DayOfWeek number
---@field public DayOfYear number
---@field public Hour number
---@field public Kind number
---@field public Millisecond number
---@field public Minute number
---@field public Month number
---@field public Now System.DateTime
---@field public UtcNow System.DateTime
---@field public Second number
---@field public Ticks int64
---@field public TimeOfDay System.TimeSpan
---@field public Today System.DateTime
---@field public Year number

---@type System.DateTime
System.DateTime = { }
---@overload fun(ticks:int64): System.DateTime
---@overload fun(ticks:int64, kind:number): System.DateTime
---@overload fun(year:number, month:number, day:number): System.DateTime
---@overload fun(year:number, month:number, day:number, calendar:System.Globalization.Calendar): System.DateTime
---@overload fun(year:number, month:number, day:number, hour:number, minute:number, second:number): System.DateTime
---@overload fun(year:number, month:number, day:number, hour:number, minute:number, second:number, kind:number): System.DateTime
---@overload fun(year:number, month:number, day:number, hour:number, minute:number, second:number, calendar:System.Globalization.Calendar): System.DateTime
---@overload fun(year:number, month:number, day:number, hour:number, minute:number, second:number, millisecond:number): System.DateTime
---@overload fun(year:number, month:number, day:number, hour:number, minute:number, second:number, millisecond:number, kind:number): System.DateTime
---@overload fun(year:number, month:number, day:number, hour:number, minute:number, second:number, millisecond:number, calendar:System.Globalization.Calendar): System.DateTime
---@return System.DateTime
---@param year number
---@param month number
---@param day number
---@param hour number
---@param minute number
---@param second number
---@param millisecond number
---@param calendar System.Globalization.Calendar
---@param kind number
function System.DateTime.New(year, month, day, hour, minute, second, millisecond, calendar, kind) end
---@return System.DateTime
---@param value System.TimeSpan
function System.DateTime:Add(value) end
---@return System.DateTime
---@param value number
function System.DateTime:AddDays(value) end
---@return System.DateTime
---@param value number
function System.DateTime:AddHours(value) end
---@return System.DateTime
---@param value number
function System.DateTime:AddMilliseconds(value) end
---@return System.DateTime
---@param value number
function System.DateTime:AddMinutes(value) end
---@return System.DateTime
---@param months number
function System.DateTime:AddMonths(months) end
---@return System.DateTime
---@param value number
function System.DateTime:AddSeconds(value) end
---@return System.DateTime
---@param value int64
function System.DateTime:AddTicks(value) end
---@return System.DateTime
---@param value number
function System.DateTime:AddYears(value) end
---@return number
---@param t1 System.DateTime
---@param t2 System.DateTime
function System.DateTime.Compare(t1, t2) end
---@overload fun(value:System.Object): number
---@return number
---@param value System.DateTime
function System.DateTime:CompareTo(value) end
---@return number
---@param year number
---@param month number
function System.DateTime.DaysInMonth(year, month) end
---@overload fun(value:System.Object): boolean
---@overload fun(value:System.DateTime): boolean
---@return boolean
---@param t1 System.DateTime
---@param t2 System.DateTime
function System.DateTime:Equals(t1, t2) end
---@return System.DateTime
---@param dateData int64
function System.DateTime.FromBinary(dateData) end
---@return System.DateTime
---@param fileTime int64
function System.DateTime.FromFileTime(fileTime) end
---@return System.DateTime
---@param fileTime int64
function System.DateTime.FromFileTimeUtc(fileTime) end
---@return System.DateTime
---@param d number
function System.DateTime.FromOADate(d) end
---@return boolean
function System.DateTime:IsDaylightSavingTime() end
---@return System.DateTime
---@param value System.DateTime
---@param kind number
function System.DateTime.SpecifyKind(value, kind) end
---@return int64
function System.DateTime:ToBinary() end
---@return number
function System.DateTime:GetHashCode() end
---@return boolean
---@param year number
function System.DateTime.IsLeapYear(year) end
---@overload fun(s:string): System.DateTime
---@overload fun(s:string, provider:System.IFormatProvider): System.DateTime
---@return System.DateTime
---@param s string
---@param provider System.IFormatProvider
---@param styles number
function System.DateTime.Parse(s, provider, styles) end
---@overload fun(s:string, format:string, provider:System.IFormatProvider): System.DateTime
---@overload fun(s:string, format:string, provider:System.IFormatProvider, style:number): System.DateTime
---@return System.DateTime
---@param s string
---@param formats System.String[]
---@param provider System.IFormatProvider
---@param style number
function System.DateTime.ParseExact(s, formats, provider, style) end
---@overload fun(value:System.DateTime): System.TimeSpan
---@return System.TimeSpan
---@param value System.TimeSpan
function System.DateTime:Subtract(value) end
---@return number
function System.DateTime:ToOADate() end
---@return int64
function System.DateTime:ToFileTime() end
---@return int64
function System.DateTime:ToFileTimeUtc() end
---@return System.DateTime
function System.DateTime:ToLocalTime() end
---@return string
function System.DateTime:ToLongDateString() end
---@return string
function System.DateTime:ToLongTimeString() end
---@return string
function System.DateTime:ToShortDateString() end
---@return string
function System.DateTime:ToShortTimeString() end
---@overload fun(): string
---@overload fun(format:string): string
---@overload fun(provider:System.IFormatProvider): string
---@return string
---@param format string
---@param provider System.IFormatProvider
function System.DateTime:ToString(format, provider) end
---@return System.DateTime
function System.DateTime:ToUniversalTime() end
---@overload fun(s:string, result:System.DateTime): boolean
---@return boolean
---@param s string
---@param provider System.IFormatProvider
---@param styles number
---@param result System.DateTime
function System.DateTime.TryParse(s, provider, styles, result) end
---@overload fun(s:string, format:string, provider:System.IFormatProvider, style:number, result:System.DateTime): boolean
---@return boolean
---@param s string
---@param formats System.String[]
---@param provider System.IFormatProvider
---@param style number
---@param result System.DateTime
function System.DateTime.TryParseExact(s, formats, provider, style, result) end
---@return System.DateTime
---@param d System.DateTime
---@param t System.TimeSpan
function System.DateTime.op_Addition(d, t) end
---@overload fun(d:System.DateTime, t:System.TimeSpan): System.DateTime
---@return System.DateTime
---@param d1 System.DateTime
---@param d2 System.DateTime
function System.DateTime.op_Subtraction(d1, d2) end
---@return boolean
---@param d1 System.DateTime
---@param d2 System.DateTime
function System.DateTime.op_Equality(d1, d2) end
---@return boolean
---@param d1 System.DateTime
---@param d2 System.DateTime
function System.DateTime.op_Inequality(d1, d2) end
---@return boolean
---@param t1 System.DateTime
---@param t2 System.DateTime
function System.DateTime.op_LessThan(t1, t2) end
---@return boolean
---@param t1 System.DateTime
---@param t2 System.DateTime
function System.DateTime.op_LessThanOrEqual(t1, t2) end
---@return boolean
---@param t1 System.DateTime
---@param t2 System.DateTime
function System.DateTime.op_GreaterThan(t1, t2) end
---@return boolean
---@param t1 System.DateTime
---@param t2 System.DateTime
function System.DateTime.op_GreaterThanOrEqual(t1, t2) end
---@overload fun(): System.String[]
---@overload fun(provider:System.IFormatProvider): System.String[]
---@overload fun(format:number): System.String[]
---@return System.String[]
---@param format number
---@param provider System.IFormatProvider
function System.DateTime:GetDateTimeFormats(format, provider) end
---@return number
function System.DateTime:GetTypeCode() end
return System.DateTime
