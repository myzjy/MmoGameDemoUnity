---@class System.Array
---@field public LongLength int64
---@field public IsFixedSize boolean
---@field public IsReadOnly boolean
---@field public IsSynchronized boolean
---@field public SyncRoot System.Object
---@field public Length number
---@field public Rank number

---@type System.Array
System.Array = { }
---@overload fun(elementType:string, lengths:System.Int64[]): System.Array
---@overload fun(elementType:string, length:number): System.Array
---@overload fun(elementType:string, lengths:System.Int32[]): System.Array
---@overload fun(elementType:string, length1:number, length2:number): System.Array
---@overload fun(elementType:string, lengths:System.Int32[], lowerBounds:System.Int32[]): System.Array
---@return System.Array
---@param elementType string
---@param length1 number
---@param length2 number
---@param length3 number
function System.Array.CreateInstance(elementType, length1, length2, length3) end
---@overload fun(array:System.Array, index:number): void
---@param array System.Array
---@param index int64
function System.Array:CopyTo(array, index) end
---@return System.Object
function System.Array:Clone() end
---@overload fun(array:System.Array, value:System.Object): number
---@overload fun(array:System.Array, value:System.Object, comparer:System.Collections.IComparer): number
---@overload fun(array:System.Array, index:number, length:number, value:System.Object): number
---@return number
---@param array System.Array
---@param index number
---@param length number
---@param value System.Object
---@param comparer System.Collections.IComparer
function System.Array.BinarySearch(array, index, length, value, comparer) end
---@overload fun(sourceArray:System.Array, destinationArray:System.Array, length:int64): void
---@overload fun(sourceArray:System.Array, destinationArray:System.Array, length:number): void
---@overload fun(sourceArray:System.Array, sourceIndex:int64, destinationArray:System.Array, destinationIndex:int64, length:int64): void
---@param sourceArray System.Array
---@param sourceIndex number
---@param destinationArray System.Array
---@param destinationIndex number
---@param length number
function System.Array.Copy(sourceArray, sourceIndex, destinationArray, destinationIndex, length) end
---@return int64
---@param dimension number
function System.Array:GetLongLength(dimension) end
---@overload fun(index:int64): System.Object
---@overload fun(indices:System.Int64[]): System.Object
---@overload fun(indices:System.Int32[]): System.Object
---@overload fun(index:number): System.Object
---@overload fun(index1:int64, index2:int64): System.Object
---@overload fun(index1:number, index2:number): System.Object
---@overload fun(index1:int64, index2:int64, index3:int64): System.Object
---@return System.Object
---@param index1 number
---@param index2 number
---@param index3 number
function System.Array:GetValue(index1, index2, index3) end
---@overload fun(array:System.Array, value:System.Object): number
---@overload fun(array:System.Array, value:System.Object, startIndex:number): number
---@return number
---@param array System.Array
---@param value System.Object
---@param startIndex number
---@param count number
function System.Array.IndexOf(array, value, startIndex, count) end
---@overload fun(array:System.Array, value:System.Object): number
---@overload fun(array:System.Array, value:System.Object, startIndex:number): number
---@return number
---@param array System.Array
---@param value System.Object
---@param startIndex number
---@param count number
function System.Array.LastIndexOf(array, value, startIndex, count) end
---@overload fun(array:System.Array): void
---@param array System.Array
---@param index number
---@param length number
function System.Array.Reverse(array, index, length) end
---@overload fun(value:System.Object, index:int64): void
---@overload fun(value:System.Object, indices:System.Int64[]): void
---@overload fun(value:System.Object, indices:System.Int32[]): void
---@overload fun(value:System.Object, index:number): void
---@overload fun(value:System.Object, index1:int64, index2:int64): void
---@overload fun(value:System.Object, index1:number, index2:number): void
---@overload fun(value:System.Object, index1:int64, index2:int64, index3:int64): void
---@param value System.Object
---@param index1 number
---@param index2 number
---@param index3 number
function System.Array:SetValue(value, index1, index2, index3) end
---@overload fun(array:System.Array): void
---@overload fun(array:System.Array, comparer:System.Collections.IComparer): void
---@overload fun(keys:System.Array, items:System.Array): void
---@overload fun(array:System.Array, index:number, length:number): void
---@overload fun(keys:System.Array, items:System.Array, comparer:System.Collections.IComparer): void
---@overload fun(array:System.Array, index:number, length:number, comparer:System.Collections.IComparer): void
---@overload fun(keys:System.Array, items:System.Array, index:number, length:number): void
---@param keys System.Array
---@param items System.Array
---@param index number
---@param length number
---@param comparer System.Collections.IComparer
function System.Array.Sort(keys, items, index, length, comparer) end
---@return System.Collections.IEnumerator
function System.Array:GetEnumerator() end
---@return number
---@param dimension number
function System.Array:GetLength(dimension) end
---@return number
---@param dimension number
function System.Array:GetLowerBound(dimension) end
---@return number
---@param dimension number
function System.Array:GetUpperBound(dimension) end
---@param array System.Array
---@param index number
---@param length number
function System.Array.Clear(array, index, length) end
---@param sourceArray System.Array
---@param sourceIndex number
---@param destinationArray System.Array
---@param destinationIndex number
---@param length number
function System.Array.ConstrainedCopy(sourceArray, sourceIndex, destinationArray, destinationIndex, length) end
function System.Array:Initialize() end
---@return string
---@param split1 number
function System.Array:ArrConvertToString(split1) end
return System.Array
