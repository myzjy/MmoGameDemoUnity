---@class CS.System.Collections.Generic.HashSet_System.String
---@field public Count number
---@field public Comparer CS.System.Collections.Generic.IEqualityComparer_System.String
CS.System.Collections.Generic.HashSet_System.String = { }
---@overload fun(): CS.System.Collections.Generic.HashSet_System.String
---@overload fun(comparer:CS.System.Collections.Generic.IEqualityComparer_System.String): CS.System.Collections.Generic.HashSet_System.String
---@overload fun(capacity:number): CS.System.Collections.Generic.HashSet_System.String
---@overload fun(collection:CS.System.Collections.Generic.IEnumerable_System.String): CS.System.Collections.Generic.HashSet_System.String
---@overload fun(collection:CS.System.Collections.Generic.IEnumerable_System.String, comparer:CS.System.Collections.Generic.IEqualityComparer_System.String): CS.System.Collections.Generic.HashSet_System.String
---@return CS.System.Collections.Generic.HashSet_System.String
---@param capacity number
---@param comparer CS.System.Collections.Generic.IEqualityComparer_System.String
function CS.System.Collections.Generic.HashSet_System.String.New(capacity, comparer) end
function CS.System.Collections.Generic.HashSet_System.String:Clear() end
---@return boolean
---@param item string
function CS.System.Collections.Generic.HashSet_System.String:Contains(item) end
---@overload fun(array:CS.System.String[]): void
---@overload fun(array:CS.System.String[], arrayIndex:number): void
---@param array CS.System.String[]
---@param arrayIndex number
---@param count number
function CS.System.Collections.Generic.HashSet_System.String:CopyTo(array, arrayIndex, count) end
---@return boolean
---@param item string
function CS.System.Collections.Generic.HashSet_System.String:Remove(item) end
---@return CS.System.Collections.Generic.HashSet_System.String.Enumerator
function CS.System.Collections.Generic.HashSet_System.String:GetEnumerator() end
---@param info CS.System.Runtime.Serialization.SerializationInfo
---@param context CS.System.Runtime.Serialization.StreamingContext
function CS.System.Collections.Generic.HashSet_System.String:GetObjectData(info, context) end
---@param sender CS.System.Object
function CS.System.Collections.Generic.HashSet_System.String:OnDeserialization(sender) end
---@return boolean
---@param item string
function CS.System.Collections.Generic.HashSet_System.String:Add(item) end
---@return boolean
---@param equalValue string
---@param actualValue CS.System.String
function CS.System.Collections.Generic.HashSet_System.String:TryGetValue(equalValue, actualValue) end
---@param other CS.System.Collections.Generic.IEnumerable_System.String
function CS.System.Collections.Generic.HashSet_System.String:UnionWith(other) end
---@param other CS.System.Collections.Generic.IEnumerable_System.String
function CS.System.Collections.Generic.HashSet_System.String:IntersectWith(other) end
---@param other CS.System.Collections.Generic.IEnumerable_System.String
function CS.System.Collections.Generic.HashSet_System.String:ExceptWith(other) end
---@param other CS.System.Collections.Generic.IEnumerable_System.String
function CS.System.Collections.Generic.HashSet_System.String:SymmetricExceptWith(other) end
---@return boolean
---@param other CS.System.Collections.Generic.IEnumerable_System.String
function CS.System.Collections.Generic.HashSet_System.String:IsSubsetOf(other) end
---@return boolean
---@param other CS.System.Collections.Generic.IEnumerable_System.String
function CS.System.Collections.Generic.HashSet_System.String:IsProperSubsetOf(other) end
---@return boolean
---@param other CS.System.Collections.Generic.IEnumerable_System.String
function CS.System.Collections.Generic.HashSet_System.String:IsSupersetOf(other) end
---@return boolean
---@param other CS.System.Collections.Generic.IEnumerable_System.String
function CS.System.Collections.Generic.HashSet_System.String:IsProperSupersetOf(other) end
---@return boolean
---@param other CS.System.Collections.Generic.IEnumerable_System.String
function CS.System.Collections.Generic.HashSet_System.String:Overlaps(other) end
---@return boolean
---@param other CS.System.Collections.Generic.IEnumerable_System.String
function CS.System.Collections.Generic.HashSet_System.String:SetEquals(other) end
---@return number
---@param match (fun(obj:string):boolean)
function CS.System.Collections.Generic.HashSet_System.String:RemoveWhere(match) end
function CS.System.Collections.Generic.HashSet_System.String:TrimExcess() end
---@return CS.System.Collections.Generic.IEqualityComparer_System.Collections.Generic.HashSet_System.String
function CS.System.Collections.Generic.HashSet_System.String.CreateSetComparer() end
return CS.System.Collections.Generic.HashSet_System.String
