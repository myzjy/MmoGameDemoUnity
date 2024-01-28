---@class System.Collections.Generic.HashSet_System.Int32
---@field public Count number
---@field public Comparer System.Collections.Generic.IEqualityComparer_System.Int32

---@type System.Collections.Generic.HashSet_System.Int32
System.Collections.Generic.HashSet_System.Int32 = { }
---@overload fun(): System.Collections.Generic.HashSet_System.Int32
---@overload fun(comparer:System.Collections.Generic.IEqualityComparer_System.Int32): System.Collections.Generic.HashSet_System.Int32
---@overload fun(capacity:number): System.Collections.Generic.HashSet_System.Int32
---@overload fun(collection:System.Collections.Generic.IEnumerable_System.Int32): System.Collections.Generic.HashSet_System.Int32
---@overload fun(collection:System.Collections.Generic.IEnumerable_System.Int32, comparer:System.Collections.Generic.IEqualityComparer_System.Int32): System.Collections.Generic.HashSet_System.Int32
---@return System.Collections.Generic.HashSet_System.Int32
---@param capacity number
---@param comparer System.Collections.Generic.IEqualityComparer_System.Int32
function System.Collections.Generic.HashSet_System.Int32.New(capacity, comparer) end
function System.Collections.Generic.HashSet_System.Int32:Clear() end
---@return boolean
---@param item number
function System.Collections.Generic.HashSet_System.Int32:Contains(item) end
---@overload fun(array:System.Int32[]): void
---@overload fun(array:System.Int32[], arrayIndex:number): void
---@param array System.Int32[]
---@param arrayIndex number
---@param count number
function System.Collections.Generic.HashSet_System.Int32:CopyTo(array, arrayIndex, count) end
---@return boolean
---@param item number
function System.Collections.Generic.HashSet_System.Int32:Remove(item) end
---@return System.Collections.Generic.HashSet_System.Int32.Enumerator
function System.Collections.Generic.HashSet_System.Int32:GetEnumerator() end
---@param info System.Runtime.Serialization.SerializationInfo
---@param context System.Runtime.Serialization.StreamingContext
function System.Collections.Generic.HashSet_System.Int32:GetObjectData(info, context) end
---@param sender System.Object
function System.Collections.Generic.HashSet_System.Int32:OnDeserialization(sender) end
---@return boolean
---@param item number
function System.Collections.Generic.HashSet_System.Int32:Add(item) end
---@return boolean
---@param equalValue number
---@param actualValue System.Int32
function System.Collections.Generic.HashSet_System.Int32:TryGetValue(equalValue, actualValue) end
---@param other System.Collections.Generic.IEnumerable_System.Int32
function System.Collections.Generic.HashSet_System.Int32:UnionWith(other) end
---@param other System.Collections.Generic.IEnumerable_System.Int32
function System.Collections.Generic.HashSet_System.Int32:IntersectWith(other) end
---@param other System.Collections.Generic.IEnumerable_System.Int32
function System.Collections.Generic.HashSet_System.Int32:ExceptWith(other) end
---@param other System.Collections.Generic.IEnumerable_System.Int32
function System.Collections.Generic.HashSet_System.Int32:SymmetricExceptWith(other) end
---@return boolean
---@param other System.Collections.Generic.IEnumerable_System.Int32
function System.Collections.Generic.HashSet_System.Int32:IsSubsetOf(other) end
---@return boolean
---@param other System.Collections.Generic.IEnumerable_System.Int32
function System.Collections.Generic.HashSet_System.Int32:IsProperSubsetOf(other) end
---@return boolean
---@param other System.Collections.Generic.IEnumerable_System.Int32
function System.Collections.Generic.HashSet_System.Int32:IsSupersetOf(other) end
---@return boolean
---@param other System.Collections.Generic.IEnumerable_System.Int32
function System.Collections.Generic.HashSet_System.Int32:IsProperSupersetOf(other) end
---@return boolean
---@param other System.Collections.Generic.IEnumerable_System.Int32
function System.Collections.Generic.HashSet_System.Int32:Overlaps(other) end
---@return boolean
---@param other System.Collections.Generic.IEnumerable_System.Int32
function System.Collections.Generic.HashSet_System.Int32:SetEquals(other) end
---@return number
---@param match (fun(obj:number):boolean)
function System.Collections.Generic.HashSet_System.Int32:RemoveWhere(match) end
function System.Collections.Generic.HashSet_System.Int32:TrimExcess() end
---@return System.Collections.Generic.IEqualityComparer_System.Collections.Generic.HashSet_System.Int32
function System.Collections.Generic.HashSet_System.Int32.CreateSetComparer() end
return System.Collections.Generic.HashSet_System.Int32
