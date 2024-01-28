---@class System.String
---@field public Empty string
---@field public Chars number
---@field public Length number

---@type System.String
System.String = { }
---@overload fun(value:System.Char*): string
---@overload fun(value:System.SByte*): string
---@overload fun(value:System.Char[]): string
---@overload fun(c:number, count:number): string
---@overload fun(value:System.Char*, startIndex:number, length:number): string
---@overload fun(value:System.SByte*, startIndex:number, length:number): string
---@overload fun(value:System.Char[], startIndex:number, length:number): string
---@return string
---@param value System.SByte*
---@param startIndex number
---@param length number
---@param enc System.Text.Encoding
function System.String.New(value, startIndex, length, enc) end
---@overload fun(separator:string, value:System.String[]): string
---@overload fun(separator:string, values:System.Object[]): string
---@overload fun(separator:string, values:System.Collections.Generic.IEnumerable_System.String): string
---@return string
---@param separator string
---@param value System.String[]
---@param startIndex number
---@param count number
function System.String.Join(separator, value, startIndex, count) end
---@overload fun(obj:System.Object): boolean
---@overload fun(value:string): boolean
---@overload fun(value:string, comparisonType:number): boolean
---@overload fun(a:string, b:string): boolean
---@return boolean
---@param a string
---@param b string
---@param comparisonType number
function System.String:Equals(a, b, comparisonType) end
---@return boolean
---@param a string
---@param b string
function System.String.op_Equality(a, b) end
---@return boolean
---@param a string
---@param b string
function System.String.op_Inequality(a, b) end
---@param sourceIndex number
---@param destination System.Char[]
---@param destinationIndex number
---@param count number
function System.String:CopyTo(sourceIndex, destination, destinationIndex, count) end
---@overload fun(): System.Char[]
---@return System.Char[]
---@param startIndex number
---@param length number
function System.String:ToCharArray(startIndex, length) end
---@return boolean
---@param value string
function System.String.IsNullOrEmpty(value) end
---@return boolean
---@param value string
function System.String.IsNullOrWhiteSpace(value) end
---@return number
function System.String:GetHashCode() end
---@overload fun(separator:System.Char[]): System.String[]
---@overload fun(separator:System.Char[], count:number): System.String[]
---@overload fun(separator:System.Char[], options:number): System.String[]
---@overload fun(separator:System.String[], options:number): System.String[]
---@overload fun(separator:System.Char[], count:number, options:number): System.String[]
---@return System.String[]
---@param separator System.String[]
---@param count number
---@param options number
function System.String:Split(separator, count, options) end
---@overload fun(startIndex:number): string
---@return string
---@param startIndex number
---@param length number
function System.String:Substring(startIndex, length) end
---@overload fun(): string
---@return string
---@param trimChars System.Char[]
function System.String:Trim(trimChars) end
---@return string
---@param trimChars System.Char[]
function System.String:TrimStart(trimChars) end
---@return string
---@param trimChars System.Char[]
function System.String:TrimEnd(trimChars) end
---@overload fun(): boolean
---@return boolean
---@param normalizationForm number
function System.String:IsNormalized(normalizationForm) end
---@overload fun(): string
---@return string
---@param normalizationForm number
function System.String:Normalize(normalizationForm) end
---@overload fun(strA:string, strB:string): number
---@overload fun(strA:string, strB:string, ignoreCase:boolean): number
---@overload fun(strA:string, strB:string, comparisonType:number): number
---@overload fun(strA:string, strB:string, culture:System.Globalization.CultureInfo, options:number): number
---@overload fun(strA:string, strB:string, ignoreCase:boolean, culture:System.Globalization.CultureInfo): number
---@overload fun(strA:string, indexA:number, strB:string, indexB:number, length:number): number
---@overload fun(strA:string, indexA:number, strB:string, indexB:number, length:number, ignoreCase:boolean): number
---@overload fun(strA:string, indexA:number, strB:string, indexB:number, length:number, comparisonType:number): number
---@overload fun(strA:string, indexA:number, strB:string, indexB:number, length:number, ignoreCase:boolean, culture:System.Globalization.CultureInfo): number
---@return number
---@param strA string
---@param indexA number
---@param strB string
---@param indexB number
---@param length number
---@param culture System.Globalization.CultureInfo
---@param options number
function System.String.Compare(strA, indexA, strB, indexB, length, culture, options) end
---@overload fun(value:System.Object): number
---@return number
---@param strB string
function System.String:CompareTo(strB) end
---@overload fun(strA:string, strB:string): number
---@return number
---@param strA string
---@param indexA number
---@param strB string
---@param indexB number
---@param length number
function System.String.CompareOrdinal(strA, indexA, strB, indexB, length) end
---@return boolean
---@param value string
function System.String:Contains(value) end
---@overload fun(value:string): boolean
---@overload fun(value:string, comparisonType:number): boolean
---@return boolean
---@param value string
---@param ignoreCase boolean
---@param culture System.Globalization.CultureInfo
function System.String:EndsWith(value, ignoreCase, culture) end
---@overload fun(value:number): number
---@overload fun(value:string): number
---@overload fun(value:number, startIndex:number): number
---@overload fun(value:string, startIndex:number): number
---@overload fun(value:string, comparisonType:number): number
---@overload fun(value:string, startIndex:number, count:number): number
---@overload fun(value:string, startIndex:number, comparisonType:number): number
---@overload fun(value:number, startIndex:number, count:number): number
---@return number
---@param value string
---@param startIndex number
---@param count number
---@param comparisonType number
function System.String:IndexOf(value, startIndex, count, comparisonType) end
---@overload fun(anyOf:System.Char[]): number
---@overload fun(anyOf:System.Char[], startIndex:number): number
---@return number
---@param anyOf System.Char[]
---@param startIndex number
---@param count number
function System.String:IndexOfAny(anyOf, startIndex, count) end
---@overload fun(value:number): number
---@overload fun(value:string): number
---@overload fun(value:number, startIndex:number): number
---@overload fun(value:string, startIndex:number): number
---@overload fun(value:string, comparisonType:number): number
---@overload fun(value:string, startIndex:number, count:number): number
---@overload fun(value:string, startIndex:number, comparisonType:number): number
---@overload fun(value:number, startIndex:number, count:number): number
---@return number
---@param value string
---@param startIndex number
---@param count number
---@param comparisonType number
function System.String:LastIndexOf(value, startIndex, count, comparisonType) end
---@overload fun(anyOf:System.Char[]): number
---@overload fun(anyOf:System.Char[], startIndex:number): number
---@return number
---@param anyOf System.Char[]
---@param startIndex number
---@param count number
function System.String:LastIndexOfAny(anyOf, startIndex, count) end
---@overload fun(totalWidth:number): string
---@return string
---@param totalWidth number
---@param paddingChar number
function System.String:PadLeft(totalWidth, paddingChar) end
---@overload fun(totalWidth:number): string
---@return string
---@param totalWidth number
---@param paddingChar number
function System.String:PadRight(totalWidth, paddingChar) end
---@overload fun(value:string): boolean
---@overload fun(value:string, comparisonType:number): boolean
---@return boolean
---@param value string
---@param ignoreCase boolean
---@param culture System.Globalization.CultureInfo
function System.String:StartsWith(value, ignoreCase, culture) end
---@overload fun(): string
---@return string
---@param culture System.Globalization.CultureInfo
function System.String:ToLower(culture) end
---@return string
function System.String:ToLowerInvariant() end
---@overload fun(): string
---@return string
---@param culture System.Globalization.CultureInfo
function System.String:ToUpper(culture) end
---@return string
function System.String:ToUpperInvariant() end
---@overload fun(): string
---@return string
---@param provider System.IFormatProvider
function System.String:ToString(provider) end
---@return System.Object
function System.String:Clone() end
---@return string
---@param startIndex number
---@param value string
function System.String:Insert(startIndex, value) end
---@overload fun(oldChar:number, newChar:number): string
---@return string
---@param oldValue string
---@param newValue string
function System.String:Replace(oldValue, newValue) end
---@overload fun(startIndex:number): string
---@return string
---@param startIndex number
---@param count number
function System.String:Remove(startIndex, count) end
---@overload fun(format:string, arg0:System.Object): string
---@overload fun(format:string, args:System.Object[]): string
---@overload fun(format:string, arg0:System.Object, arg1:System.Object): string
---@overload fun(provider:System.IFormatProvider, format:string, arg0:System.Object): string
---@overload fun(provider:System.IFormatProvider, format:string, args:System.Object[]): string
---@overload fun(format:string, arg0:System.Object, arg1:System.Object, arg2:System.Object): string
---@overload fun(provider:System.IFormatProvider, format:string, arg0:System.Object, arg1:System.Object): string
---@return string
---@param provider System.IFormatProvider
---@param format string
---@param arg0 System.Object
---@param arg1 System.Object
---@param arg2 System.Object
function System.String.Format(provider, format, arg0, arg1, arg2) end
---@return string
---@param str string
function System.String.Copy(str) end
---@overload fun(arg0:System.Object): string
---@overload fun(args:System.Object[]): string
---@overload fun(values:System.Collections.Generic.IEnumerable_System.String): string
---@overload fun(values:System.String[]): string
---@overload fun(arg0:System.Object, arg1:System.Object): string
---@overload fun(str0:string, str1:string): string
---@overload fun(arg0:System.Object, arg1:System.Object, arg2:System.Object): string
---@overload fun(str0:string, str1:string, str2:string): string
---@overload fun(arg0:System.Object, arg1:System.Object, arg2:System.Object, arg3:System.Object): string
---@return string
---@param str0 string
---@param str1 string
---@param str2 string
---@param str3 string
function System.String.Concat(str0, str1, str2, str3) end
---@return string
---@param str string
function System.String.Intern(str) end
---@return string
---@param str string
function System.String.IsInterned(str) end
---@return number
function System.String:GetTypeCode() end
---@return System.CharEnumerator
function System.String:GetEnumerator() end
---@return System.Object
---@param t string
function System.String:GetValue(t) end
---@overload fun(t:string, defultValue:System.Object): System.Object
---@return System.Object
---@param t string
---@param result System.Object
---@param defaultValue System.Object
function System.String:TryGetValue(t, result, defaultValue) end
---@return System.Collections.Generic.List_System.String
---@param listSpriter number
function System.String:ParseList(listSpriter) end
---@return System.Collections.Generic.Dictionary_System.String_System.String
---@param keyValueSpriter number
---@param mapSpriter number
function System.String:ParseMap(keyValueSpriter, mapSpriter) end
---@return number
function System.String:GetRandom() end
---@return string
function System.String:GetTypeByString() end
---@return string
---@param oldValue string
---@param newValue string
---@param startAt number
function System.String:ReplaceFirst(oldValue, newValue, startAt) end
---@return boolean
function System.String:HasChinese() end
---@return boolean
function System.String:HasSpace() end
---@return boolean
function System.String:IsNullOrEmptyR() end
---@return string
---@param targets System.String[]
function System.String:RemoveString(targets) end
---@return System.String[]
---@param separator System.Char[]
function System.String:SplitAndTrim(separator) end
---@return string
function System.String:DeleteEmoji() end
---@return string
function System.String:DeleteRichText() end
---@return string
---@param front string
---@param behined string
function System.String:FindBetween(front, behined) end
---@return string
---@param front string
function System.String:FindAfter(front) end
---@return string
---@param scheme number
function System.String:ReplaceColorName2ColorHex(scheme) end
return System.String
