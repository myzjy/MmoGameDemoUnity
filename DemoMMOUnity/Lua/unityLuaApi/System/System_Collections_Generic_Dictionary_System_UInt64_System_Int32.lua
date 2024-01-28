---@class System.Collections.Generic.Dictionary_System.UInt64_System.Int32
---@field public Comparer System.Collections.Generic.IEqualityComparer_System.UInt64
---@field public Count number
---@field public Keys System.Collections.Generic.Dictionary_System.UInt64_System.Int32.KeyCollection
---@field public Values System.Collections.Generic.Dictionary_System.UInt64_System.Int32.ValueCollection
---@field public Item number

---@type System.Collections.Generic.Dictionary_System.UInt64_System.Int32
System.Collections.Generic.Dictionary_System.UInt64_System.Int32 = { }
---@overload fun(): System.Collections.Generic.Dictionary_System.UInt64_System.Int32
---@overload fun(capacity:number): System.Collections.Generic.Dictionary_System.UInt64_System.Int32
---@overload fun(comparer:System.Collections.Generic.IEqualityComparer_System.UInt64): System.Collections.Generic.Dictionary_System.UInt64_System.Int32
---@overload fun(dictionary:System.Collections.Generic.IDictionary_System.UInt64_System.Int32): System.Collections.Generic.Dictionary_System.UInt64_System.Int32
---@overload fun(collection:System.Collections.Generic.IEnumerable_System.Collections.Generic.KeyValuePair_System.UInt64_System.Int32): System.Collections.Generic.Dictionary_System.UInt64_System.Int32
---@overload fun(capacity:number, comparer:System.Collections.Generic.IEqualityComparer_System.UInt64): System.Collections.Generic.Dictionary_System.UInt64_System.Int32
---@overload fun(dictionary:System.Collections.Generic.IDictionary_System.UInt64_System.Int32, comparer:System.Collections.Generic.IEqualityComparer_System.UInt64): System.Collections.Generic.Dictionary_System.UInt64_System.Int32
---@return System.Collections.Generic.Dictionary_System.UInt64_System.Int32
---@param collection System.Collections.Generic.IEnumerable_System.Collections.Generic.KeyValuePair_System.UInt64_System.Int32
---@param comparer System.Collections.Generic.IEqualityComparer_System.UInt64
function System.Collections.Generic.Dictionary_System.UInt64_System.Int32.New(collection, comparer) end
---@param key uint64
---@param value number
function System.Collections.Generic.Dictionary_System.UInt64_System.Int32:Add(key, value) end
function System.Collections.Generic.Dictionary_System.UInt64_System.Int32:Clear() end
---@return boolean
---@param key uint64
function System.Collections.Generic.Dictionary_System.UInt64_System.Int32:ContainsKey(key) end
---@return boolean
---@param value number
function System.Collections.Generic.Dictionary_System.UInt64_System.Int32:ContainsValue(value) end
---@return System.Collections.Generic.Dictionary_System.UInt64_System.Int32.Enumerator
function System.Collections.Generic.Dictionary_System.UInt64_System.Int32:GetEnumerator() end
---@param info System.Runtime.Serialization.SerializationInfo
---@param context System.Runtime.Serialization.StreamingContext
function System.Collections.Generic.Dictionary_System.UInt64_System.Int32:GetObjectData(info, context) end
---@param sender System.Object
function System.Collections.Generic.Dictionary_System.UInt64_System.Int32:OnDeserialization(sender) end
---@overload fun(key:uint64): boolean
---@return boolean
---@param key uint64
---@param value System.Int32
function System.Collections.Generic.Dictionary_System.UInt64_System.Int32:Remove(key, value) end
---@return boolean
---@param key uint64
---@param value System.Int32
function System.Collections.Generic.Dictionary_System.UInt64_System.Int32:TryGetValue(key, value) end
---@return boolean
---@param key uint64
---@param value number
function System.Collections.Generic.Dictionary_System.UInt64_System.Int32:TryAdd(key, value) end
return System.Collections.Generic.Dictionary_System.UInt64_System.Int32
