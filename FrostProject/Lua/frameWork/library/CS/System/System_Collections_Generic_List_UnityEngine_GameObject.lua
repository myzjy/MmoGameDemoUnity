---@class CS.System.Collections.Generic.List_UnityEngine.GameObject
---@field public Capacity number
---@field public Count number
---@field public Item CS.UnityEngine.GameObject
CS.System.Collections.Generic.List_UnityEngine.GameObject = { }
---@overload fun(): CS.System.Collections.Generic.List_UnityEngine.GameObject
---@overload fun(capacity:number): CS.System.Collections.Generic.List_UnityEngine.GameObject
---@return CS.System.Collections.Generic.List_UnityEngine.GameObject
---@param collection CS.System.Collections.Generic.IEnumerable_UnityEngine.GameObject
function CS.System.Collections.Generic.List_UnityEngine.GameObject.New(collection) end
---@param item CS.UnityEngine.GameObject
function CS.System.Collections.Generic.List_UnityEngine.GameObject:Add(item) end
---@param collection CS.System.Collections.Generic.IEnumerable_UnityEngine.GameObject
function CS.System.Collections.Generic.List_UnityEngine.GameObject:AddRange(collection) end
---@return CS.System.Collections.ObjectModel.ReadOnlyCollection_UnityEngine.GameObject
function CS.System.Collections.Generic.List_UnityEngine.GameObject:AsReadOnly() end
---@overload fun(item:CS.UnityEngine.GameObject): number
---@overload fun(item:CS.UnityEngine.GameObject, comparer:CS.System.Collections.Generic.IComparer_UnityEngine.GameObject): number
---@return number
---@param index number
---@param count number
---@param item CS.UnityEngine.GameObject
---@param comparer CS.System.Collections.Generic.IComparer_UnityEngine.GameObject
function CS.System.Collections.Generic.List_UnityEngine.GameObject:BinarySearch(index, count, item, comparer) end
function CS.System.Collections.Generic.List_UnityEngine.GameObject:Clear() end
---@return boolean
---@param item CS.UnityEngine.GameObject
function CS.System.Collections.Generic.List_UnityEngine.GameObject:Contains(item) end
---@overload fun(array:CS.UnityEngine.GameObject[]): void
---@overload fun(array:CS.UnityEngine.GameObject[], arrayIndex:number): void
---@param index number
---@param array CS.UnityEngine.GameObject[]
---@param arrayIndex number
---@param count number
function CS.System.Collections.Generic.List_UnityEngine.GameObject:CopyTo(index, array, arrayIndex, count) end
---@return boolean
---@param match (fun(obj:CS.UnityEngine.GameObject):boolean)
function CS.System.Collections.Generic.List_UnityEngine.GameObject:Exists(match) end
---@return CS.UnityEngine.GameObject
---@param match (fun(obj:CS.UnityEngine.GameObject):boolean)
function CS.System.Collections.Generic.List_UnityEngine.GameObject:Find(match) end
---@return CS.System.Collections.Generic.List_UnityEngine.GameObject
---@param match (fun(obj:CS.UnityEngine.GameObject):boolean)
function CS.System.Collections.Generic.List_UnityEngine.GameObject:FindAll(match) end
---@overload fun(match:(fun(obj:CS.UnityEngine.GameObject):boolean)): number
---@overload fun(startIndex:number, match:(fun(obj:CS.UnityEngine.GameObject):boolean)): number
---@return number
---@param startIndex number
---@param count number
---@param match (fun(obj:CS.UnityEngine.GameObject):boolean)
function CS.System.Collections.Generic.List_UnityEngine.GameObject:FindIndex(startIndex, count, match) end
---@return CS.UnityEngine.GameObject
---@param match (fun(obj:CS.UnityEngine.GameObject):boolean)
function CS.System.Collections.Generic.List_UnityEngine.GameObject:FindLast(match) end
---@overload fun(match:(fun(obj:CS.UnityEngine.GameObject):boolean)): number
---@overload fun(startIndex:number, match:(fun(obj:CS.UnityEngine.GameObject):boolean)): number
---@return number
---@param startIndex number
---@param count number
---@param match (fun(obj:CS.UnityEngine.GameObject):boolean)
function CS.System.Collections.Generic.List_UnityEngine.GameObject:FindLastIndex(startIndex, count, match) end
---@param action (fun(obj:CS.UnityEngine.GameObject):void)
function CS.System.Collections.Generic.List_UnityEngine.GameObject:ForEach(action) end
---@return CS.System.Collections.Generic.List_UnityEngine.GameObject.Enumerator
function CS.System.Collections.Generic.List_UnityEngine.GameObject:GetEnumerator() end
---@return CS.System.Collections.Generic.List_UnityEngine.GameObject
---@param index number
---@param count number
function CS.System.Collections.Generic.List_UnityEngine.GameObject:GetRange(index, count) end
---@overload fun(item:CS.UnityEngine.GameObject): number
---@overload fun(item:CS.UnityEngine.GameObject, index:number): number
---@return number
---@param item CS.UnityEngine.GameObject
---@param index number
---@param count number
function CS.System.Collections.Generic.List_UnityEngine.GameObject:IndexOf(item, index, count) end
---@param index number
---@param item CS.UnityEngine.GameObject
function CS.System.Collections.Generic.List_UnityEngine.GameObject:Insert(index, item) end
---@param index number
---@param collection CS.System.Collections.Generic.IEnumerable_UnityEngine.GameObject
function CS.System.Collections.Generic.List_UnityEngine.GameObject:InsertRange(index, collection) end
---@overload fun(item:CS.UnityEngine.GameObject): number
---@overload fun(item:CS.UnityEngine.GameObject, index:number): number
---@return number
---@param item CS.UnityEngine.GameObject
---@param index number
---@param count number
function CS.System.Collections.Generic.List_UnityEngine.GameObject:LastIndexOf(item, index, count) end
---@return boolean
---@param item CS.UnityEngine.GameObject
function CS.System.Collections.Generic.List_UnityEngine.GameObject:Remove(item) end
---@return number
---@param match (fun(obj:CS.UnityEngine.GameObject):boolean)
function CS.System.Collections.Generic.List_UnityEngine.GameObject:RemoveAll(match) end
---@param index number
function CS.System.Collections.Generic.List_UnityEngine.GameObject:RemoveAt(index) end
---@param index number
---@param count number
function CS.System.Collections.Generic.List_UnityEngine.GameObject:RemoveRange(index, count) end
---@overload fun(): void
---@param index number
---@param count number
function CS.System.Collections.Generic.List_UnityEngine.GameObject:Reverse(index, count) end
---@overload fun(): void
---@overload fun(comparer:CS.System.Collections.Generic.IComparer_UnityEngine.GameObject): void
---@overload fun(comparison:(fun(x:CS.UnityEngine.GameObject, y:CS.UnityEngine.GameObject):number)): void
---@param index number
---@param count number
---@param comparer CS.System.Collections.Generic.IComparer_UnityEngine.GameObject
function CS.System.Collections.Generic.List_UnityEngine.GameObject:Sort(index, count, comparer) end
---@return CS.UnityEngine.GameObject[]
function CS.System.Collections.Generic.List_UnityEngine.GameObject:ToArray() end
function CS.System.Collections.Generic.List_UnityEngine.GameObject:TrimExcess() end
---@return boolean
---@param match (fun(obj:CS.UnityEngine.GameObject):boolean)
function CS.System.Collections.Generic.List_UnityEngine.GameObject:TrueForAll(match) end
return CS.System.Collections.Generic.List_UnityEngine.GameObject
