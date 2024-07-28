---@class CS.System.String
---@field public Empty string
---@field public Chars number
---@field public Length number
CS.System.String = { }
---@overload fun(value:CS.System.Char*): string
---@overload fun(value:CS.System.SByte*): string
---@overload fun(value:CS.System.Char[]): string
---@overload fun(c:number, count:number): string
---@overload fun(value:CS.System.Char*, startIndex:number, length:number): string
---@overload fun(value:CS.System.SByte*, startIndex:number, length:number): string
---@overload fun(value:CS.System.Char[], startIndex:number, length:number): string
---@return string
---@param value CS.System.SByte*
---@param startIndex number
---@param length number
---@param enc CS.System.Text.Encoding
function CS.System.String.New(value, startIndex, length, enc) end
---@overload fun(separator:string, value:CS.System.String[]): string
---@overload fun(separator:string, values:CS.System.Object[]): string
---@overload fun(separator:string, values:CS.System.Collections.Generic.IEnumerable_System.String): string
---@return string
---@param separator string
---@param value CS.System.String[]
---@param startIndex number
---@param count number
function CS.System.String.Join(separator, value, startIndex, count) end
---@overload fun(obj:CS.System.Object): boolean
---@overload fun(value:string): boolean
---@overload fun(value:string, comparisonType:number): boolean
---@overload fun(a:string, b:string): boolean
---@return boolean
---@param a string
---@param b string
---@param comparisonType number
function CS.System.String:Equals(a, b, comparisonType) end
---@return boolean
---@param a string
---@param b string
function CS.System.String.op_Equality(a, b) end
---@return boolean
---@param a string
---@param b string
function CS.System.String.op_Inequality(a, b) end
---@param sourceIndex number
---@param destination CS.System.Char[]
---@param destinationIndex number
---@param count number
function CS.System.String:CopyTo(sourceIndex, destination, destinationIndex, count) end
---@overload fun(): CS.System.Char[]
---@return CS.System.Char[]
---@param startIndex number
---@param length number
function CS.System.String:ToCharArray(startIndex, length) end
---@return boolean
---@param value string
function CS.System.String.IsNullOrEmpty(value) end
---@return boolean
---@param value string
function CS.System.String.IsNullOrWhiteSpace(value) end
---@return number
function CS.System.String:GetHashCode() end
---@overload fun(separator:CS.System.Char[]): CS.System.String[]
---@overload fun(separator:CS.System.Char[], count:number): CS.System.String[]
---@overload fun(separator:CS.System.Char[], options:number): CS.System.String[]
---@overload fun(separator:CS.System.String[], options:number): CS.System.String[]
---@overload fun(separator:CS.System.Char[], count:number, options:number): CS.System.String[]
---@return CS.System.String[]
---@param separator CS.System.String[]
---@param count number
---@param options number
function CS.System.String:Split(separator, count, options) end
---@overload fun(startIndex:number): string
---@return string
---@param startIndex number
---@param length number
function CS.System.String:Substring(startIndex, length) end
---@overload fun(): string
---@return string
---@param trimChars CS.System.Char[]
function CS.System.String:Trim(trimChars) end
---@return string
---@param trimChars CS.System.Char[]
function CS.System.String:TrimStart(trimChars) end
---@return string
---@param trimChars CS.System.Char[]
function CS.System.String:TrimEnd(trimChars) end
---@overload fun(): boolean
---@return boolean
---@param normalizationForm number
function CS.System.String:IsNormalized(normalizationForm) end
---@overload fun(): string
---@return string
---@param normalizationForm number
function CS.System.String:Normalize(normalizationForm) end
---@overload fun(strA:string, strB:string): number
---@overload fun(strA:string, strB:string, ignoreCase:boolean): number
---@overload fun(strA:string, strB:string, comparisonType:number): number
---@overload fun(strA:string, strB:string, culture:CS.System.Globalization.CultureInfo, options:number): number
---@overload fun(strA:string, strB:string, ignoreCase:boolean, culture:CS.System.Globalization.CultureInfo): number
---@overload fun(strA:string, indexA:number, strB:string, indexB:number, length:number): number
---@overload fun(strA:string, indexA:number, strB:string, indexB:number, length:number, ignoreCase:boolean): number
---@overload fun(strA:string, indexA:number, strB:string, indexB:number, length:number, comparisonType:number): number
---@overload fun(strA:string, indexA:number, strB:string, indexB:number, length:number, ignoreCase:boolean, culture:CS.System.Globalization.CultureInfo): number
---@return number
---@param strA string
---@param indexA number
---@param strB string
---@param indexB number
---@param length number
---@param culture CS.System.Globalization.CultureInfo
---@param options number
function CS.System.String.Compare(strA, indexA, strB, indexB, length, culture, options) end
---@overload fun(value:CS.System.Object): number
---@return number
---@param strB string
function CS.System.String:CompareTo(strB) end
---@overload fun(strA:string, strB:string): number
---@return number
---@param strA string
---@param indexA number
---@param strB string
---@param indexB number
---@param length number
function CS.System.String.CompareOrdinal(strA, indexA, strB, indexB, length) end
---@return boolean
---@param value string
function CS.System.String:Contains(value) end
---@overload fun(value:string): boolean
---@overload fun(value:string, comparisonType:number): boolean
---@return boolean
---@param value string
---@param ignoreCase boolean
---@param culture CS.System.Globalization.CultureInfo
function CS.System.String:EndsWith(value, ignoreCase, culture) end
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
function CS.System.String:IndexOf(value, startIndex, count, comparisonType) end
---@overload fun(anyOf:CS.System.Char[]): number
---@overload fun(anyOf:CS.System.Char[], startIndex:number): number
---@return number
---@param anyOf CS.System.Char[]
---@param startIndex number
---@param count number
function CS.System.String:IndexOfAny(anyOf, startIndex, count) end
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
function CS.System.String:LastIndexOf(value, startIndex, count, comparisonType) end
---@overload fun(anyOf:CS.System.Char[]): number
---@overload fun(anyOf:CS.System.Char[], startIndex:number): number
---@return number
---@param anyOf CS.System.Char[]
---@param startIndex number
---@param count number
function CS.System.String:LastIndexOfAny(anyOf, startIndex, count) end
---@overload fun(totalWidth:number): string
---@return string
---@param totalWidth number
---@param paddingChar number
function CS.System.String:PadLeft(totalWidth, paddingChar) end
---@overload fun(totalWidth:number): string
---@return string
---@param totalWidth number
---@param paddingChar number
function CS.System.String:PadRight(totalWidth, paddingChar) end
---@overload fun(value:string): boolean
---@overload fun(value:string, comparisonType:number): boolean
---@return boolean
---@param value string
---@param ignoreCase boolean
---@param culture CS.System.Globalization.CultureInfo
function CS.System.String:StartsWith(value, ignoreCase, culture) end
---@overload fun(): string
---@return string
---@param culture CS.System.Globalization.CultureInfo
function CS.System.String:ToLower(culture) end
---@return string
function CS.System.String:ToLowerInvariant() end
---@overload fun(): string
---@return string
---@param culture CS.System.Globalization.CultureInfo
function CS.System.String:ToUpper(culture) end
---@return string
function CS.System.String:ToUpperInvariant() end
---@overload fun(): string
---@return string
---@param provider CS.System.IFormatProvider
function CS.System.String:ToString(provider) end
---@return CS.System.Object
function CS.System.String:Clone() end
---@return string
---@param startIndex number
---@param value string
function CS.System.String:Insert(startIndex, value) end
---@overload fun(oldChar:number, newChar:number): string
---@return string
---@param oldValue string
---@param newValue string
function CS.System.String:Replace(oldValue, newValue) end
---@overload fun(startIndex:number): string
---@return string
---@param startIndex number
---@param count number
function CS.System.String:Remove(startIndex, count) end
---@overload fun(format:string, arg0:CS.System.Object): string
---@overload fun(format:string, args:CS.System.Object[]): string
---@overload fun(format:string, arg0:CS.System.Object, arg1:CS.System.Object): string
---@overload fun(provider:CS.System.IFormatProvider, format:string, arg0:CS.System.Object): string
---@overload fun(provider:CS.System.IFormatProvider, format:string, args:CS.System.Object[]): string
---@overload fun(format:string, arg0:CS.System.Object, arg1:CS.System.Object, arg2:CS.System.Object): string
---@overload fun(provider:CS.System.IFormatProvider, format:string, arg0:CS.System.Object, arg1:CS.System.Object): string
---@return string
---@param provider CS.System.IFormatProvider
---@param format string
---@param arg0 CS.System.Object
---@param arg1 CS.System.Object
---@param arg2 CS.System.Object
function CS.System.String.Format(provider, format, arg0, arg1, arg2) end
---@return string
---@param str string
function CS.System.String.Copy(str) end
---@overload fun(arg0:CS.System.Object): string
---@overload fun(args:CS.System.Object[]): string
---@overload fun(values:CS.System.Collections.Generic.IEnumerable_System.String): string
---@overload fun(values:CS.System.String[]): string
---@overload fun(arg0:CS.System.Object, arg1:CS.System.Object): string
---@overload fun(str0:string, str1:string): string
---@overload fun(arg0:CS.System.Object, arg1:CS.System.Object, arg2:CS.System.Object): string
---@overload fun(str0:string, str1:string, str2:string): string
---@overload fun(arg0:CS.System.Object, arg1:CS.System.Object, arg2:CS.System.Object, arg3:CS.System.Object): string
---@return string
---@param str0 string
---@param str1 string
---@param str2 string
---@param str3 string
function CS.System.String.Concat(str0, str1, str2, str3) end
---@return string
---@param str string
function CS.System.String.Intern(str) end
---@return string
---@param str string
function CS.System.String.IsInterned(str) end
---@return number
function CS.System.String:GetTypeCode() end
---@return CS.System.CharEnumerator
function CS.System.String:GetEnumerator() end
---@return CS.System.Object
---@param t string
function CS.System.String:GetValue(t) end
---@overload fun(t:string, defultValue:CS.System.Object): CS.System.Object
---@return CS.System.Object
---@param t string
---@param result CS.System.Object
---@param defaultValue CS.System.Object
function CS.System.String:TryGetValue(t, result, defaultValue) end
---@return CS.System.Collections.Generic.List_System.String
---@param listSpriter number
function CS.System.String:ParseList(listSpriter) end
---@return CS.System.Collections.Generic.Dictionary_System.String_System.String
---@param keyValueSpriter number
---@param mapSpriter number
function CS.System.String:ParseMap(keyValueSpriter, mapSpriter) end
---@return number
function CS.System.String:GetRandom() end
---@return string
function CS.System.String:GetTypeByString() end
---@return string
---@param oldValue string
---@param newValue string
---@param startAt number
function CS.System.String:ReplaceFirst(oldValue, newValue, startAt) end
---@return boolean
function CS.System.String:HasChinese() end
---@return boolean
function CS.System.String:HasSpace() end
---@return boolean
function CS.System.String:IsNullOrEmptyR() end
---@return string
---@param targets CS.System.String[]
function CS.System.String:RemoveString(targets) end
---@return CS.System.String[]
---@param separator CS.System.Char[]
function CS.System.String:SplitAndTrim(separator) end
---@return string
function CS.System.String:DeleteEmoji() end
---@return string
function CS.System.String:DeleteRichText() end
---@return string
---@param front string
---@param behined string
function CS.System.String:FindBetween(front, behined) end
---@return string
---@param front string
function CS.System.String:FindAfter(front) end
---@return string
---@param scheme number
function CS.System.String:ReplaceColorName2ColorHex(scheme) end
return CS.System.String
