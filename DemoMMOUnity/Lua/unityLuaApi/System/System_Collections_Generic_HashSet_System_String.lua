---@class System.Collections.Generic.HashSet_System.String
---@field public Count number
---@field public Comparer System.Collections.Generic.IEqualityComparer_System.String

---@type System.Collections.Generic.HashSet_System.String
System.Collections.Generic.HashSet_System.String = { }
---@overload fun(): System.Collections.Generic.HashSet_System.String
---@overload fun(comparer:System.Collections.Generic.IEqualityComparer_System.String): System.Collections.Generic.HashSet_System.String
---@overload fun(capacity:number): System.Collections.Generic.HashSet_System.String
---@overload fun(collection:System.Collections.Generic.IEnumerable_System.String): System.Collections.Generic.HashSet_System.String
---@overload fun(collection:System.Collections.Generic.IEnumerable_System.String, comparer:System.Collections.Generic.IEqualityComparer_System.String): System.Collections.Generic.HashSet_System.String
---@return System.Collections.Generic.HashSet_System.String
---@param capacity number
---@param comparer System.Collections.Generic.IEqualityComparer_System.String
function System.Collections.Generic.HashSet_System.String.New(capacity, comparer) end
function System.Collections.Generic.HashSet_System.String:Clear() end
---@return boolean
---@param item string
function System.Collections.Generic.HashSet_System.String:Contains(item) end
---@overload fun(array:System.String[]): void
---@overload fun(array:System.String[], arrayIndex:number): void
---@param array System.String[]
---@param arrayIndex number
---@param count number
function System.Collections.Generic.HashSet_System.String:CopyTo(array, arrayIndex, count) end
---@return boolean
---@param item string
function System.Collections.Generic.HashSet_System.String:Remove(item) end
---@return System.Collections.Generic.HashSet_System.String.Enumerator
function System.Collections.Generic.HashSet_System.String:GetEnumerator() end
---@param info System.Runtime.Serialization.SerializationInfo
---@param context System.Runtime.Serialization.StreamingContext
function System.Collections.Generic.HashSet_System.String:GetObjectData(info, context) end
---@param sender System.Object
function System.Collections.Generic.HashSet_System.String:OnDeserialization(sender) end
---@return boolean
---@param item string
function System.Collections.Generic.HashSet_System.String:Add(item) end
---@return boolean
---@param equalValue string
---@param actualValue System.String
function System.Collections.Generic.HashSet_System.String:TryGetValue(equalValue, actualValue) end
---@param other System.Collections.Generic.IEnumerable_System.String
function System.Collections.Generic.HashSet_System.String:UnionWith(other) end
---@param other System.Collections.Generic.IEnumerable_System.String
function System.Collections.Generic.HashSet_System.String:IntersectWith(other) end
---@param other System.Collections.Generic.IEnumerable_System.String
function System.Collections.Generic.HashSet_System.String:ExceptWith(other) end
---@param other System.Collections.Generic.IEnumerable_System.String
function System.Collections.Generic.HashSet_System.String:SymmetricExceptWith(other) end
---@return boolean
---@param other System.Collections.Generic.IEnumerable_System.String
function System.Collections.Generic.HashSet_System.String:IsSubsetOf(other) end
---@return boolean
---@param other System.Collections.Generic.IEnumerable_System.String
function System.Collections.Generic.HashSet_System.String:IsProperSubsetOf(other) end
---@return boolean
---@param other System.Collections.Generic.IEnumerable_System.String
function System.Collections.Generic.HashSet_System.String:IsSupersetOf(other) end
---@return boolean
---@param other System.Collections.Generic.IEnumerable_System.String
function System.Collections.Generic.HashSet_System.String:IsProperSupersetOf(other) end
---@return boolean
---@param other System.Collections.Generic.IEnumerable_System.String
function System.Collections.Generic.HashSet_System.String:Overlaps(other) end
---@return boolean
---@param other System.Collections.Generic.IEnumerable_System.String
function System.Collections.Generic.HashSet_System.String:SetEquals(other) end
---@return number
---@param match (fun(obj:string):boolean)
function System.Collections.Generic.HashSet_System.String:RemoveWhere(match) end
function System.Collections.Generic.HashSet_System.String:TrimExcess() end
---@return System.Collections.Generic.IEqualityComparer_System.Collections.Generic.HashSet_System.String
function System.Collections.Generic.HashSet_System.String.CreateSetComparer() end
return System.Collections.Generic.HashSet_System.String
