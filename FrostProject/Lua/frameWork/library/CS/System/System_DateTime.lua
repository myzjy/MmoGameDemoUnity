---@class CS.System.DateTime : CS.System.ValueType
---@field public MinValue CS.System.DateTime
---@field public MaxValue CS.System.DateTime
---@field public Date CS.System.DateTime
---@field public Day number
---@field public DayOfWeek number
---@field public DayOfYear number
---@field public Hour number
---@field public Kind number
---@field public Millisecond number
---@field public Minute number
---@field public Month number
---@field public Now CS.System.DateTime
---@field public UtcNow CS.System.DateTime
---@field public Second number
---@field public Ticks int64
---@field public TimeOfDay CS.System.TimeSpan
---@field public Today CS.System.DateTime
---@field public Year number
CS.System.DateTime = { }
---@overload fun(ticks:int64): CS.System.DateTime
---@overload fun(ticks:int64, kind:number): CS.System.DateTime
---@overload fun(year:number, month:number, day:number): CS.System.DateTime
---@overload fun(year:number, month:number, day:number, calendar:CS.System.Globalization.Calendar): CS.System.DateTime
---@overload fun(year:number, month:number, day:number, hour:number, minute:number, second:number): CS.System.DateTime
---@overload fun(year:number, month:number, day:number, hour:number, minute:number, second:number, kind:number): CS.System.DateTime
---@overload fun(year:number, month:number, day:number, hour:number, minute:number, second:number, calendar:CS.System.Globalization.Calendar): CS.System.DateTime
---@overload fun(year:number, month:number, day:number, hour:number, minute:number, second:number, millisecond:number): CS.System.DateTime
---@overload fun(year:number, month:number, day:number, hour:number, minute:number, second:number, millisecond:number, kind:number): CS.System.DateTime
---@overload fun(year:number, month:number, day:number, hour:number, minute:number, second:number, millisecond:number, calendar:CS.System.Globalization.Calendar): CS.System.DateTime
---@return CS.System.DateTime
---@param year number
---@param month number
---@param day number
---@param hour number
---@param minute number
---@param second number
---@param millisecond number
---@param calendar CS.System.Globalization.Calendar
---@param kind number
function CS.System.DateTime.New(year, month, day, hour, minute, second, millisecond, calendar, kind) end
---@return CS.System.DateTime
---@param value CS.System.TimeSpan
function CS.System.DateTime:Add(value) end
---@return CS.System.DateTime
---@param value number
function CS.System.DateTime:AddDays(value) end
---@return CS.System.DateTime
---@param value number
function CS.System.DateTime:AddHours(value) end
---@return CS.System.DateTime
---@param value number
function CS.System.DateTime:AddMilliseconds(value) end
---@return CS.System.DateTime
---@param value number
function CS.System.DateTime:AddMinutes(value) end
---@return CS.System.DateTime
---@param months number
function CS.System.DateTime:AddMonths(months) end
---@return CS.System.DateTime
---@param value number
function CS.System.DateTime:AddSeconds(value) end
---@return CS.System.DateTime
---@param value int64
function CS.System.DateTime:AddTicks(value) end
---@return CS.System.DateTime
---@param value number
function CS.System.DateTime:AddYears(value) end
---@return number
---@param t1 CS.System.DateTime
---@param t2 CS.System.DateTime
function CS.System.DateTime.Compare(t1, t2) end
---@overload fun(value:CS.System.Object): number
---@return number
---@param value CS.System.DateTime
function CS.System.DateTime:CompareTo(value) end
---@return number
---@param year number
---@param month number
function CS.System.DateTime.DaysInMonth(year, month) end
---@overload fun(value:CS.System.Object): boolean
---@overload fun(value:CS.System.DateTime): boolean
---@return boolean
---@param t1 CS.System.DateTime
---@param t2 CS.System.DateTime
function CS.System.DateTime:Equals(t1, t2) end
---@return CS.System.DateTime
---@param dateData int64
function CS.System.DateTime.FromBinary(dateData) end
---@return CS.System.DateTime
---@param fileTime int64
function CS.System.DateTime.FromFileTime(fileTime) end
---@return CS.System.DateTime
---@param fileTime int64
function CS.System.DateTime.FromFileTimeUtc(fileTime) end
---@return CS.System.DateTime
---@param d number
function CS.System.DateTime.FromOADate(d) end
---@return boolean
function CS.System.DateTime:IsDaylightSavingTime() end
---@return CS.System.DateTime
---@param value CS.System.DateTime
---@param kind number
function CS.System.DateTime.SpecifyKind(value, kind) end
---@return int64
function CS.System.DateTime:ToBinary() end
---@return number
function CS.System.DateTime:GetHashCode() end
---@return boolean
---@param year number
function CS.System.DateTime.IsLeapYear(year) end
---@overload fun(s:string): CS.System.DateTime
---@overload fun(s:string, provider:CS.System.IFormatProvider): CS.System.DateTime
---@return CS.System.DateTime
---@param s string
---@param provider CS.System.IFormatProvider
---@param styles number
function CS.System.DateTime.Parse(s, provider, styles) end
---@overload fun(s:string, format:string, provider:CS.System.IFormatProvider): CS.System.DateTime
---@overload fun(s:string, format:string, provider:CS.System.IFormatProvider, style:number): CS.System.DateTime
---@return CS.System.DateTime
---@param s string
---@param formats CS.System.String[]
---@param provider CS.System.IFormatProvider
---@param style number
function CS.System.DateTime.ParseExact(s, formats, provider, style) end
---@overload fun(value:CS.System.DateTime): CS.System.TimeSpan
---@return CS.System.TimeSpan
---@param value CS.System.TimeSpan
function CS.System.DateTime:Subtract(value) end
---@return number
function CS.System.DateTime:ToOADate() end
---@return int64
function CS.System.DateTime:ToFileTime() end
---@return int64
function CS.System.DateTime:ToFileTimeUtc() end
---@return CS.System.DateTime
function CS.System.DateTime:ToLocalTime() end
---@return string
function CS.System.DateTime:ToLongDateString() end
---@return string
function CS.System.DateTime:ToLongTimeString() end
---@return string
function CS.System.DateTime:ToShortDateString() end
---@return string
function CS.System.DateTime:ToShortTimeString() end
---@overload fun(): string
---@overload fun(format:string): string
---@overload fun(provider:CS.System.IFormatProvider): string
---@return string
---@param format string
---@param provider CS.System.IFormatProvider
function CS.System.DateTime:ToString(format, provider) end
---@return CS.System.DateTime
function CS.System.DateTime:ToUniversalTime() end
---@overload fun(s:string, result:CS.System.DateTime): boolean
---@return boolean
---@param s string
---@param provider CS.System.IFormatProvider
---@param styles number
---@param result CS.System.DateTime
function CS.System.DateTime.TryParse(s, provider, styles, result) end
---@overload fun(s:string, format:string, provider:CS.System.IFormatProvider, style:number, result:CS.System.DateTime): boolean
---@return boolean
---@param s string
---@param formats CS.System.String[]
---@param provider CS.System.IFormatProvider
---@param style number
---@param result CS.System.DateTime
function CS.System.DateTime.TryParseExact(s, formats, provider, style, result) end
---@return CS.System.DateTime
---@param d CS.System.DateTime
---@param t CS.System.TimeSpan
function CS.System.DateTime.op_Addition(d, t) end
---@overload fun(d:CS.System.DateTime, t:CS.System.TimeSpan): CS.System.DateTime
---@return CS.System.DateTime
---@param d1 CS.System.DateTime
---@param d2 CS.System.DateTime
function CS.System.DateTime.op_Subtraction(d1, d2) end
---@return boolean
---@param d1 CS.System.DateTime
---@param d2 CS.System.DateTime
function CS.System.DateTime.op_Equality(d1, d2) end
---@return boolean
---@param d1 CS.System.DateTime
---@param d2 CS.System.DateTime
function CS.System.DateTime.op_Inequality(d1, d2) end
---@return boolean
---@param t1 CS.System.DateTime
---@param t2 CS.System.DateTime
function CS.System.DateTime.op_LessThan(t1, t2) end
---@return boolean
---@param t1 CS.System.DateTime
---@param t2 CS.System.DateTime
function CS.System.DateTime.op_LessThanOrEqual(t1, t2) end
---@return boolean
---@param t1 CS.System.DateTime
---@param t2 CS.System.DateTime
function CS.System.DateTime.op_GreaterThan(t1, t2) end
---@return boolean
---@param t1 CS.System.DateTime
---@param t2 CS.System.DateTime
function CS.System.DateTime.op_GreaterThanOrEqual(t1, t2) end
---@overload fun(): CS.System.String[]
---@overload fun(provider:CS.System.IFormatProvider): CS.System.String[]
---@overload fun(format:number): CS.System.String[]
---@return CS.System.String[]
---@param format number
---@param provider CS.System.IFormatProvider
function CS.System.DateTime:GetDateTimeFormats(format, provider) end
---@return number
function CS.System.DateTime:GetTypeCode() end
return CS.System.DateTime
