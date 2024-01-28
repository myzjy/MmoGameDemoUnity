---@class System.Collections.Generic.List_UnityEngine.RectTransform
---@field public Capacity number
---@field public Count number
---@field public Item UnityEngine.RectTransform

---@type System.Collections.Generic.List_UnityEngine.RectTransform
System.Collections.Generic.List_UnityEngine.RectTransform = { }
---@overload fun(): System.Collections.Generic.List_UnityEngine.RectTransform
---@overload fun(capacity:number): System.Collections.Generic.List_UnityEngine.RectTransform
---@return System.Collections.Generic.List_UnityEngine.RectTransform
---@param collection System.Collections.Generic.IEnumerable_UnityEngine.RectTransform
function System.Collections.Generic.List_UnityEngine.RectTransform.New(collection) end
---@param item UnityEngine.RectTransform
function System.Collections.Generic.List_UnityEngine.RectTransform:Add(item) end
---@param collection System.Collections.Generic.IEnumerable_UnityEngine.RectTransform
function System.Collections.Generic.List_UnityEngine.RectTransform:AddRange(collection) end
---@return System.Collections.ObjectModel.ReadOnlyCollection_UnityEngine.RectTransform
function System.Collections.Generic.List_UnityEngine.RectTransform:AsReadOnly() end
---@overload fun(item:UnityEngine.RectTransform): number
---@overload fun(item:UnityEngine.RectTransform, comparer:System.Collections.Generic.IComparer_UnityEngine.RectTransform): number
---@return number
---@param index number
---@param count number
---@param item UnityEngine.RectTransform
---@param comparer System.Collections.Generic.IComparer_UnityEngine.RectTransform
function System.Collections.Generic.List_UnityEngine.RectTransform:BinarySearch(index, count, item, comparer) end
function System.Collections.Generic.List_UnityEngine.RectTransform:Clear() end
---@return boolean
---@param item UnityEngine.RectTransform
function System.Collections.Generic.List_UnityEngine.RectTransform:Contains(item) end
---@overload fun(array:UnityEngine.RectTransform[]): void
---@overload fun(array:UnityEngine.RectTransform[], arrayIndex:number): void
---@param index number
---@param array UnityEngine.RectTransform[]
---@param arrayIndex number
---@param count number
function System.Collections.Generic.List_UnityEngine.RectTransform:CopyTo(index, array, arrayIndex, count) end
---@return boolean
---@param match (fun(obj:UnityEngine.RectTransform):boolean)
function System.Collections.Generic.List_UnityEngine.RectTransform:Exists(match) end
---@return UnityEngine.RectTransform
---@param match (fun(obj:UnityEngine.RectTransform):boolean)
function System.Collections.Generic.List_UnityEngine.RectTransform:Find(match) end
---@return System.Collections.Generic.List_UnityEngine.RectTransform
---@param match (fun(obj:UnityEngine.RectTransform):boolean)
function System.Collections.Generic.List_UnityEngine.RectTransform:FindAll(match) end
---@overload fun(match:(fun(obj:UnityEngine.RectTransform):boolean)): number
---@overload fun(startIndex:number, match:(fun(obj:UnityEngine.RectTransform):boolean)): number
---@return number
---@param startIndex number
---@param count number
---@param match (fun(obj:UnityEngine.RectTransform):boolean)
function System.Collections.Generic.List_UnityEngine.RectTransform:FindIndex(startIndex, count, match) end
---@return UnityEngine.RectTransform
---@param match (fun(obj:UnityEngine.RectTransform):boolean)
function System.Collections.Generic.List_UnityEngine.RectTransform:FindLast(match) end
---@overload fun(match:(fun(obj:UnityEngine.RectTransform):boolean)): number
---@overload fun(startIndex:number, match:(fun(obj:UnityEngine.RectTransform):boolean)): number
---@return number
---@param startIndex number
---@param count number
---@param match (fun(obj:UnityEngine.RectTransform):boolean)
function System.Collections.Generic.List_UnityEngine.RectTransform:FindLastIndex(startIndex, count, match) end
---@param action (fun(obj:UnityEngine.RectTransform):void)
function System.Collections.Generic.List_UnityEngine.RectTransform:ForEach(action) end
---@return System.Collections.Generic.List_UnityEngine.RectTransform.Enumerator
function System.Collections.Generic.List_UnityEngine.RectTransform:GetEnumerator() end
---@return System.Collections.Generic.List_UnityEngine.RectTransform
---@param index number
---@param count number
function System.Collections.Generic.List_UnityEngine.RectTransform:GetRange(index, count) end
---@overload fun(item:UnityEngine.RectTransform): number
---@overload fun(item:UnityEngine.RectTransform, index:number): number
---@return number
---@param item UnityEngine.RectTransform
---@param index number
---@param count number
function System.Collections.Generic.List_UnityEngine.RectTransform:IndexOf(item, index, count) end
---@param index number
---@param item UnityEngine.RectTransform
function System.Collections.Generic.List_UnityEngine.RectTransform:Insert(index, item) end
---@param index number
---@param collection System.Collections.Generic.IEnumerable_UnityEngine.RectTransform
function System.Collections.Generic.List_UnityEngine.RectTransform:InsertRange(index, collection) end
---@overload fun(item:UnityEngine.RectTransform): number
---@overload fun(item:UnityEngine.RectTransform, index:number): number
---@return number
---@param item UnityEngine.RectTransform
---@param index number
---@param count number
function System.Collections.Generic.List_UnityEngine.RectTransform:LastIndexOf(item, index, count) end
---@return boolean
---@param item UnityEngine.RectTransform
function System.Collections.Generic.List_UnityEngine.RectTransform:Remove(item) end
---@return number
---@param match (fun(obj:UnityEngine.RectTransform):boolean)
function System.Collections.Generic.List_UnityEngine.RectTransform:RemoveAll(match) end
---@param index number
function System.Collections.Generic.List_UnityEngine.RectTransform:RemoveAt(index) end
---@param index number
---@param count number
function System.Collections.Generic.List_UnityEngine.RectTransform:RemoveRange(index, count) end
---@overload fun(): void
---@param index number
---@param count number
function System.Collections.Generic.List_UnityEngine.RectTransform:Reverse(index, count) end
---@overload fun(): void
---@overload fun(comparer:System.Collections.Generic.IComparer_UnityEngine.RectTransform): void
---@overload fun(comparison:(fun(x:UnityEngine.RectTransform, y:UnityEngine.RectTransform):number)): void
---@param index number
---@param count number
---@param comparer System.Collections.Generic.IComparer_UnityEngine.RectTransform
function System.Collections.Generic.List_UnityEngine.RectTransform:Sort(index, count, comparer) end
---@return UnityEngine.RectTransform[]
function System.Collections.Generic.List_UnityEngine.RectTransform:ToArray() end
function System.Collections.Generic.List_UnityEngine.RectTransform:TrimExcess() end
---@return boolean
---@param match (fun(obj:UnityEngine.RectTransform):boolean)
function System.Collections.Generic.List_UnityEngine.RectTransform:TrueForAll(match) end
return System.Collections.Generic.List_UnityEngine.RectTransform
