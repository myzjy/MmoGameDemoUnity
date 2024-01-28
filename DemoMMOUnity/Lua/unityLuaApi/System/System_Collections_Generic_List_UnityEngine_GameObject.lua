---@class System.Collections.Generic.List_UnityEngine.GameObject
---@field public Capacity number
---@field public Count number
---@field public Item UnityEngine.GameObject

---@type System.Collections.Generic.List_UnityEngine.GameObject
System.Collections.Generic.List_UnityEngine.GameObject = { }
---@overload fun(): System.Collections.Generic.List_UnityEngine.GameObject
---@overload fun(capacity:number): System.Collections.Generic.List_UnityEngine.GameObject
---@return System.Collections.Generic.List_UnityEngine.GameObject
---@param collection System.Collections.Generic.IEnumerable_UnityEngine.GameObject
function System.Collections.Generic.List_UnityEngine.GameObject.New(collection) end
---@param item UnityEngine.GameObject
function System.Collections.Generic.List_UnityEngine.GameObject:Add(item) end
---@param collection System.Collections.Generic.IEnumerable_UnityEngine.GameObject
function System.Collections.Generic.List_UnityEngine.GameObject:AddRange(collection) end
---@return System.Collections.ObjectModel.ReadOnlyCollection_UnityEngine.GameObject
function System.Collections.Generic.List_UnityEngine.GameObject:AsReadOnly() end
---@overload fun(item:UnityEngine.GameObject): number
---@overload fun(item:UnityEngine.GameObject, comparer:System.Collections.Generic.IComparer_UnityEngine.GameObject): number
---@return number
---@param index number
---@param count number
---@param item UnityEngine.GameObject
---@param comparer System.Collections.Generic.IComparer_UnityEngine.GameObject
function System.Collections.Generic.List_UnityEngine.GameObject:BinarySearch(index, count, item, comparer) end
function System.Collections.Generic.List_UnityEngine.GameObject:Clear() end
---@return boolean
---@param item UnityEngine.GameObject
function System.Collections.Generic.List_UnityEngine.GameObject:Contains(item) end
---@overload fun(array:UnityEngine.GameObject[]): void
---@overload fun(array:UnityEngine.GameObject[], arrayIndex:number): void
---@param index number
---@param array UnityEngine.GameObject[]
---@param arrayIndex number
---@param count number
function System.Collections.Generic.List_UnityEngine.GameObject:CopyTo(index, array, arrayIndex, count) end
---@return boolean
---@param match (fun(obj:UnityEngine.GameObject):boolean)
function System.Collections.Generic.List_UnityEngine.GameObject:Exists(match) end
---@return UnityEngine.GameObject
---@param match (fun(obj:UnityEngine.GameObject):boolean)
function System.Collections.Generic.List_UnityEngine.GameObject:Find(match) end
---@return System.Collections.Generic.List_UnityEngine.GameObject
---@param match (fun(obj:UnityEngine.GameObject):boolean)
function System.Collections.Generic.List_UnityEngine.GameObject:FindAll(match) end
---@overload fun(match:(fun(obj:UnityEngine.GameObject):boolean)): number
---@overload fun(startIndex:number, match:(fun(obj:UnityEngine.GameObject):boolean)): number
---@return number
---@param startIndex number
---@param count number
---@param match (fun(obj:UnityEngine.GameObject):boolean)
function System.Collections.Generic.List_UnityEngine.GameObject:FindIndex(startIndex, count, match) end
---@return UnityEngine.GameObject
---@param match (fun(obj:UnityEngine.GameObject):boolean)
function System.Collections.Generic.List_UnityEngine.GameObject:FindLast(match) end
---@overload fun(match:(fun(obj:UnityEngine.GameObject):boolean)): number
---@overload fun(startIndex:number, match:(fun(obj:UnityEngine.GameObject):boolean)): number
---@return number
---@param startIndex number
---@param count number
---@param match (fun(obj:UnityEngine.GameObject):boolean)
function System.Collections.Generic.List_UnityEngine.GameObject:FindLastIndex(startIndex, count, match) end
---@param action (fun(obj:UnityEngine.GameObject):void)
function System.Collections.Generic.List_UnityEngine.GameObject:ForEach(action) end
---@return System.Collections.Generic.List_UnityEngine.GameObject.Enumerator
function System.Collections.Generic.List_UnityEngine.GameObject:GetEnumerator() end
---@return System.Collections.Generic.List_UnityEngine.GameObject
---@param index number
---@param count number
function System.Collections.Generic.List_UnityEngine.GameObject:GetRange(index, count) end
---@overload fun(item:UnityEngine.GameObject): number
---@overload fun(item:UnityEngine.GameObject, index:number): number
---@return number
---@param item UnityEngine.GameObject
---@param index number
---@param count number
function System.Collections.Generic.List_UnityEngine.GameObject:IndexOf(item, index, count) end
---@param index number
---@param item UnityEngine.GameObject
function System.Collections.Generic.List_UnityEngine.GameObject:Insert(index, item) end
---@param index number
---@param collection System.Collections.Generic.IEnumerable_UnityEngine.GameObject
function System.Collections.Generic.List_UnityEngine.GameObject:InsertRange(index, collection) end
---@overload fun(item:UnityEngine.GameObject): number
---@overload fun(item:UnityEngine.GameObject, index:number): number
---@return number
---@param item UnityEngine.GameObject
---@param index number
---@param count number
function System.Collections.Generic.List_UnityEngine.GameObject:LastIndexOf(item, index, count) end
---@return boolean
---@param item UnityEngine.GameObject
function System.Collections.Generic.List_UnityEngine.GameObject:Remove(item) end
---@return number
---@param match (fun(obj:UnityEngine.GameObject):boolean)
function System.Collections.Generic.List_UnityEngine.GameObject:RemoveAll(match) end
---@param index number
function System.Collections.Generic.List_UnityEngine.GameObject:RemoveAt(index) end
---@param index number
---@param count number
function System.Collections.Generic.List_UnityEngine.GameObject:RemoveRange(index, count) end
---@overload fun(): void
---@param index number
---@param count number
function System.Collections.Generic.List_UnityEngine.GameObject:Reverse(index, count) end
---@overload fun(): void
---@overload fun(comparer:System.Collections.Generic.IComparer_UnityEngine.GameObject): void
---@overload fun(comparison:(fun(x:UnityEngine.GameObject, y:UnityEngine.GameObject):number)): void
---@param index number
---@param count number
---@param comparer System.Collections.Generic.IComparer_UnityEngine.GameObject
function System.Collections.Generic.List_UnityEngine.GameObject:Sort(index, count, comparer) end
---@return UnityEngine.GameObject[]
function System.Collections.Generic.List_UnityEngine.GameObject:ToArray() end
function System.Collections.Generic.List_UnityEngine.GameObject:TrimExcess() end
---@return boolean
---@param match (fun(obj:UnityEngine.GameObject):boolean)
function System.Collections.Generic.List_UnityEngine.GameObject:TrueForAll(match) end
return System.Collections.Generic.List_UnityEngine.GameObject
