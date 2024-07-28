---@class CS.System.Array
---@field public LongLength int64
---@field public IsFixedSize boolean
---@field public IsReadOnly boolean
---@field public IsSynchronized boolean
---@field public SyncRoot CS.System.Object
---@field public Length number
---@field public Rank number
CS.System.Array = { }
---@overload fun(elementType:string, lengths:CS.System.Int64[]): CS.System.Array
---@overload fun(elementType:string, length:number): CS.System.Array
---@overload fun(elementType:string, lengths:CS.System.Int32[]): CS.System.Array
---@overload fun(elementType:string, length1:number, length2:number): CS.System.Array
---@overload fun(elementType:string, lengths:CS.System.Int32[], lowerBounds:CS.System.Int32[]): CS.System.Array
---@return CS.System.Array
---@param elementType string
---@param length1 number
---@param length2 number
---@param length3 number
function CS.System.Array.CreateInstance(elementType, length1, length2, length3) end
---@overload fun(array:CS.System.Array, index:number): void
---@param array CS.System.Array
---@param index int64
function CS.System.Array:CopyTo(array, index) end
---@return CS.System.Object
function CS.System.Array:Clone() end
---@overload fun(array:CS.System.Array, value:CS.System.Object): number
---@overload fun(array:CS.System.Array, value:CS.System.Object, comparer:CS.System.Collections.IComparer): number
---@overload fun(array:CS.System.Array, index:number, length:number, value:CS.System.Object): number
---@return number
---@param array CS.System.Array
---@param index number
---@param length number
---@param value CS.System.Object
---@param comparer CS.System.Collections.IComparer
function CS.System.Array.BinarySearch(array, index, length, value, comparer) end
---@overload fun(sourceArray:CS.System.Array, destinationArray:CS.System.Array, length:int64): void
---@overload fun(sourceArray:CS.System.Array, destinationArray:CS.System.Array, length:number): void
---@overload fun(sourceArray:CS.System.Array, sourceIndex:int64, destinationArray:CS.System.Array, destinationIndex:int64, length:int64): void
---@param sourceArray CS.System.Array
---@param sourceIndex number
---@param destinationArray CS.System.Array
---@param destinationIndex number
---@param length number
function CS.System.Array.Copy(sourceArray, sourceIndex, destinationArray, destinationIndex, length) end
---@return int64
---@param dimension number
function CS.System.Array:GetLongLength(dimension) end
---@overload fun(index:int64): CS.System.Object
---@overload fun(indices:CS.System.Int64[]): CS.System.Object
---@overload fun(indices:CS.System.Int32[]): CS.System.Object
---@overload fun(index:number): CS.System.Object
---@overload fun(index1:int64, index2:int64): CS.System.Object
---@overload fun(index1:number, index2:number): CS.System.Object
---@overload fun(index1:int64, index2:int64, index3:int64): CS.System.Object
---@return CS.System.Object
---@param index1 number
---@param index2 number
---@param index3 number
function CS.System.Array:GetValue(index1, index2, index3) end
---@overload fun(array:CS.System.Array, value:CS.System.Object): number
---@overload fun(array:CS.System.Array, value:CS.System.Object, startIndex:number): number
---@return number
---@param array CS.System.Array
---@param value CS.System.Object
---@param startIndex number
---@param count number
function CS.System.Array.IndexOf(array, value, startIndex, count) end
---@overload fun(array:CS.System.Array, value:CS.System.Object): number
---@overload fun(array:CS.System.Array, value:CS.System.Object, startIndex:number): number
---@return number
---@param array CS.System.Array
---@param value CS.System.Object
---@param startIndex number
---@param count number
function CS.System.Array.LastIndexOf(array, value, startIndex, count) end
---@overload fun(array:CS.System.Array): void
---@param array CS.System.Array
---@param index number
---@param length number
function CS.System.Array.Reverse(array, index, length) end
---@overload fun(value:CS.System.Object, index:int64): void
---@overload fun(value:CS.System.Object, indices:CS.System.Int64[]): void
---@overload fun(value:CS.System.Object, indices:CS.System.Int32[]): void
---@overload fun(value:CS.System.Object, index:number): void
---@overload fun(value:CS.System.Object, index1:int64, index2:int64): void
---@overload fun(value:CS.System.Object, index1:number, index2:number): void
---@overload fun(value:CS.System.Object, index1:int64, index2:int64, index3:int64): void
---@param value CS.System.Object
---@param index1 number
---@param index2 number
---@param index3 number
function CS.System.Array:SetValue(value, index1, index2, index3) end
---@overload fun(array:CS.System.Array): void
---@overload fun(array:CS.System.Array, comparer:CS.System.Collections.IComparer): void
---@overload fun(keys:CS.System.Array, items:CS.System.Array): void
---@overload fun(array:CS.System.Array, index:number, length:number): void
---@overload fun(keys:CS.System.Array, items:CS.System.Array, comparer:CS.System.Collections.IComparer): void
---@overload fun(array:CS.System.Array, index:number, length:number, comparer:CS.System.Collections.IComparer): void
---@overload fun(keys:CS.System.Array, items:CS.System.Array, index:number, length:number): void
---@param keys CS.System.Array
---@param items CS.System.Array
---@param index number
---@param length number
---@param comparer CS.System.Collections.IComparer
function CS.System.Array.Sort(keys, items, index, length, comparer) end
---@return CS.System.Collections.IEnumerator
function CS.System.Array:GetEnumerator() end
---@return number
---@param dimension number
function CS.System.Array:GetLength(dimension) end
---@return number
---@param dimension number
function CS.System.Array:GetLowerBound(dimension) end
---@return number
---@param dimension number
function CS.System.Array:GetUpperBound(dimension) end
---@param array CS.System.Array
---@param index number
---@param length number
function CS.System.Array.Clear(array, index, length) end
---@param sourceArray CS.System.Array
---@param sourceIndex number
---@param destinationArray CS.System.Array
---@param destinationIndex number
---@param length number
function CS.System.Array.ConstrainedCopy(sourceArray, sourceIndex, destinationArray, destinationIndex, length) end
function CS.System.Array:Initialize() end
---@return string
---@param split1 number
function CS.System.Array:ArrConvertToString(split1) end
return CS.System.Array
