---@class CS.System.Collections.Generic.HashSet_System.Int32
---@field public Count number
---@field public Comparer CS.System.Collections.Generic.IEqualityComparer_System.Int32
CS.System.Collections.Generic.HashSet_System.Int32 = { }
---@overload fun(): CS.System.Collections.Generic.HashSet_System.Int32
---@overload fun(comparer:CS.System.Collections.Generic.IEqualityComparer_System.Int32): CS.System.Collections.Generic.HashSet_System.Int32
---@overload fun(capacity:number): CS.System.Collections.Generic.HashSet_System.Int32
---@overload fun(collection:CS.System.Collections.Generic.IEnumerable_System.Int32): CS.System.Collections.Generic.HashSet_System.Int32
---@overload fun(collection:CS.System.Collections.Generic.IEnumerable_System.Int32, comparer:CS.System.Collections.Generic.IEqualityComparer_System.Int32): CS.System.Collections.Generic.HashSet_System.Int32
---@return CS.System.Collections.Generic.HashSet_System.Int32
---@param capacity number
---@param comparer CS.System.Collections.Generic.IEqualityComparer_System.Int32
function CS.System.Collections.Generic.HashSet_System.Int32.New(capacity, comparer) end
function CS.System.Collections.Generic.HashSet_System.Int32:Clear() end
---@return boolean
---@param item number
function CS.System.Collections.Generic.HashSet_System.Int32:Contains(item) end
---@overload fun(array:CS.System.Int32[]): void
---@overload fun(array:CS.System.Int32[], arrayIndex:number): void
---@param array CS.System.Int32[]
---@param arrayIndex number
---@param count number
function CS.System.Collections.Generic.HashSet_System.Int32:CopyTo(array, arrayIndex, count) end
---@return boolean
---@param item number
function CS.System.Collections.Generic.HashSet_System.Int32:Remove(item) end
---@return CS.System.Collections.Generic.HashSet_System.Int32.Enumerator
function CS.System.Collections.Generic.HashSet_System.Int32:GetEnumerator() end
---@param info CS.System.Runtime.Serialization.SerializationInfo
---@param context CS.System.Runtime.Serialization.StreamingContext
function CS.System.Collections.Generic.HashSet_System.Int32:GetObjectData(info, context) end
---@param sender CS.System.Object
function CS.System.Collections.Generic.HashSet_System.Int32:OnDeserialization(sender) end
---@return boolean
---@param item number
function CS.System.Collections.Generic.HashSet_System.Int32:Add(item) end
---@return boolean
---@param equalValue number
---@param actualValue CS.System.Int32
function CS.System.Collections.Generic.HashSet_System.Int32:TryGetValue(equalValue, actualValue) end
---@param other CS.System.Collections.Generic.IEnumerable_System.Int32
function CS.System.Collections.Generic.HashSet_System.Int32:UnionWith(other) end
---@param other CS.System.Collections.Generic.IEnumerable_System.Int32
function CS.System.Collections.Generic.HashSet_System.Int32:IntersectWith(other) end
---@param other CS.System.Collections.Generic.IEnumerable_System.Int32
function CS.System.Collections.Generic.HashSet_System.Int32:ExceptWith(other) end
---@param other CS.System.Collections.Generic.IEnumerable_System.Int32
function CS.System.Collections.Generic.HashSet_System.Int32:SymmetricExceptWith(other) end
---@return boolean
---@param other CS.System.Collections.Generic.IEnumerable_System.Int32
function CS.System.Collections.Generic.HashSet_System.Int32:IsSubsetOf(other) end
---@return boolean
---@param other CS.System.Collections.Generic.IEnumerable_System.Int32
function CS.System.Collections.Generic.HashSet_System.Int32:IsProperSubsetOf(other) end
---@return boolean
---@param other CS.System.Collections.Generic.IEnumerable_System.Int32
function CS.System.Collections.Generic.HashSet_System.Int32:IsSupersetOf(other) end
---@return boolean
---@param other CS.System.Collections.Generic.IEnumerable_System.Int32
function CS.System.Collections.Generic.HashSet_System.Int32:IsProperSupersetOf(other) end
---@return boolean
---@param other CS.System.Collections.Generic.IEnumerable_System.Int32
function CS.System.Collections.Generic.HashSet_System.Int32:Overlaps(other) end
---@return boolean
---@param other CS.System.Collections.Generic.IEnumerable_System.Int32
function CS.System.Collections.Generic.HashSet_System.Int32:SetEquals(other) end
---@return number
---@param match (fun(obj:number):boolean)
function CS.System.Collections.Generic.HashSet_System.Int32:RemoveWhere(match) end
function CS.System.Collections.Generic.HashSet_System.Int32:TrimExcess() end
---@return CS.System.Collections.Generic.IEqualityComparer_System.Collections.Generic.HashSet_System.Int32
function CS.System.Collections.Generic.HashSet_System.Int32.CreateSetComparer() end
return CS.System.Collections.Generic.HashSet_System.Int32
