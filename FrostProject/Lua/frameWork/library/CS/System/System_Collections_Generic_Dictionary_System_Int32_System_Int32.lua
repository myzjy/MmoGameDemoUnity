---@class CS.System.Collections.Generic.Dictionary_System.Int32_System.Int32
---@field public Comparer CS.System.Collections.Generic.IEqualityComparer_System.Int32
---@field public Count number
---@field public Keys CS.System.Collections.Generic.Dictionary_System.Int32_System.Int32.KeyCollection
---@field public Values CS.System.Collections.Generic.Dictionary_System.Int32_System.Int32.ValueCollection
---@field public Item number
CS.System.Collections.Generic.Dictionary_System.Int32_System.Int32 = { }
---@overload fun(): CS.System.Collections.Generic.Dictionary_System.Int32_System.Int32
---@overload fun(capacity:number): CS.System.Collections.Generic.Dictionary_System.Int32_System.Int32
---@overload fun(comparer:CS.System.Collections.Generic.IEqualityComparer_System.Int32): CS.System.Collections.Generic.Dictionary_System.Int32_System.Int32
---@overload fun(dictionary:CS.System.Collections.Generic.IDictionary_System.Int32_System.Int32): CS.System.Collections.Generic.Dictionary_System.Int32_System.Int32
---@overload fun(collection:CS.System.Collections.Generic.IEnumerable_System.Collections.Generic.KeyValuePair_System.Int32_System.Int32): CS.System.Collections.Generic.Dictionary_System.Int32_System.Int32
---@overload fun(capacity:number, comparer:CS.System.Collections.Generic.IEqualityComparer_System.Int32): CS.System.Collections.Generic.Dictionary_System.Int32_System.Int32
---@overload fun(dictionary:CS.System.Collections.Generic.IDictionary_System.Int32_System.Int32, comparer:CS.System.Collections.Generic.IEqualityComparer_System.Int32): CS.System.Collections.Generic.Dictionary_System.Int32_System.Int32
---@return CS.System.Collections.Generic.Dictionary_System.Int32_System.Int32
---@param collection CS.System.Collections.Generic.IEnumerable_System.Collections.Generic.KeyValuePair_System.Int32_System.Int32
---@param comparer CS.System.Collections.Generic.IEqualityComparer_System.Int32
function CS.System.Collections.Generic.Dictionary_System.Int32_System.Int32.New(collection, comparer) end
---@param key number
---@param value number
function CS.System.Collections.Generic.Dictionary_System.Int32_System.Int32:Add(key, value) end
function CS.System.Collections.Generic.Dictionary_System.Int32_System.Int32:Clear() end
---@return boolean
---@param key number
function CS.System.Collections.Generic.Dictionary_System.Int32_System.Int32:ContainsKey(key) end
---@return boolean
---@param value number
function CS.System.Collections.Generic.Dictionary_System.Int32_System.Int32:ContainsValue(value) end
---@return CS.System.Collections.Generic.Dictionary_System.Int32_System.Int32.Enumerator
function CS.System.Collections.Generic.Dictionary_System.Int32_System.Int32:GetEnumerator() end
---@param info CS.System.Runtime.Serialization.SerializationInfo
---@param context CS.System.Runtime.Serialization.StreamingContext
function CS.System.Collections.Generic.Dictionary_System.Int32_System.Int32:GetObjectData(info, context) end
---@param sender CS.System.Object
function CS.System.Collections.Generic.Dictionary_System.Int32_System.Int32:OnDeserialization(sender) end
---@overload fun(key:number): boolean
---@return boolean
---@param key number
---@param value CS.System.Int32
function CS.System.Collections.Generic.Dictionary_System.Int32_System.Int32:Remove(key, value) end
---@return boolean
---@param key number
---@param value CS.System.Int32
function CS.System.Collections.Generic.Dictionary_System.Int32_System.Int32:TryGetValue(key, value) end
---@return boolean
---@param key number
---@param value number
function CS.System.Collections.Generic.Dictionary_System.Int32_System.Int32:TryAdd(key, value) end
return CS.System.Collections.Generic.Dictionary_System.Int32_System.Int32
