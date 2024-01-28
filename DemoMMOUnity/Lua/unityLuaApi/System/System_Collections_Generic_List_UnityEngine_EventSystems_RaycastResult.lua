---@class System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult
---@field public Capacity number
---@field public Count number
---@field public Item UnityEngine.EventSystems.RaycastResult

---@type System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult
System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult = { }
---@overload fun(): System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult
---@overload fun(capacity:number): System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult
---@return System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult
---@param collection System.Collections.Generic.IEnumerable_UnityEngine.EventSystems.RaycastResult
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult.New(collection) end
---@param item UnityEngine.EventSystems.RaycastResult
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:Add(item) end
---@param collection System.Collections.Generic.IEnumerable_UnityEngine.EventSystems.RaycastResult
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:AddRange(collection) end
---@return System.Collections.ObjectModel.ReadOnlyCollection_UnityEngine.EventSystems.RaycastResult
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:AsReadOnly() end
---@overload fun(item:UnityEngine.EventSystems.RaycastResult): number
---@overload fun(item:UnityEngine.EventSystems.RaycastResult, comparer:System.Collections.Generic.IComparer_UnityEngine.EventSystems.RaycastResult): number
---@return number
---@param index number
---@param count number
---@param item UnityEngine.EventSystems.RaycastResult
---@param comparer System.Collections.Generic.IComparer_UnityEngine.EventSystems.RaycastResult
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:BinarySearch(index, count, item, comparer) end
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:Clear() end
---@return boolean
---@param item UnityEngine.EventSystems.RaycastResult
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:Contains(item) end
---@overload fun(array:UnityEngine.EventSystems.RaycastResult[]): void
---@overload fun(array:UnityEngine.EventSystems.RaycastResult[], arrayIndex:number): void
---@param index number
---@param array UnityEngine.EventSystems.RaycastResult[]
---@param arrayIndex number
---@param count number
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:CopyTo(index, array, arrayIndex, count) end
---@return boolean
---@param match (fun(obj:UnityEngine.EventSystems.RaycastResult):boolean)
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:Exists(match) end
---@return UnityEngine.EventSystems.RaycastResult
---@param match (fun(obj:UnityEngine.EventSystems.RaycastResult):boolean)
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:Find(match) end
---@return System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult
---@param match (fun(obj:UnityEngine.EventSystems.RaycastResult):boolean)
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:FindAll(match) end
---@overload fun(match:(fun(obj:UnityEngine.EventSystems.RaycastResult):boolean)): number
---@overload fun(startIndex:number, match:(fun(obj:UnityEngine.EventSystems.RaycastResult):boolean)): number
---@return number
---@param startIndex number
---@param count number
---@param match (fun(obj:UnityEngine.EventSystems.RaycastResult):boolean)
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:FindIndex(startIndex, count, match) end
---@return UnityEngine.EventSystems.RaycastResult
---@param match (fun(obj:UnityEngine.EventSystems.RaycastResult):boolean)
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:FindLast(match) end
---@overload fun(match:(fun(obj:UnityEngine.EventSystems.RaycastResult):boolean)): number
---@overload fun(startIndex:number, match:(fun(obj:UnityEngine.EventSystems.RaycastResult):boolean)): number
---@return number
---@param startIndex number
---@param count number
---@param match (fun(obj:UnityEngine.EventSystems.RaycastResult):boolean)
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:FindLastIndex(startIndex, count, match) end
---@param action (fun(obj:UnityEngine.EventSystems.RaycastResult):void)
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:ForEach(action) end
---@return System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult.Enumerator
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:GetEnumerator() end
---@return System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult
---@param index number
---@param count number
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:GetRange(index, count) end
---@overload fun(item:UnityEngine.EventSystems.RaycastResult): number
---@overload fun(item:UnityEngine.EventSystems.RaycastResult, index:number): number
---@return number
---@param item UnityEngine.EventSystems.RaycastResult
---@param index number
---@param count number
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:IndexOf(item, index, count) end
---@param index number
---@param item UnityEngine.EventSystems.RaycastResult
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:Insert(index, item) end
---@param index number
---@param collection System.Collections.Generic.IEnumerable_UnityEngine.EventSystems.RaycastResult
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:InsertRange(index, collection) end
---@overload fun(item:UnityEngine.EventSystems.RaycastResult): number
---@overload fun(item:UnityEngine.EventSystems.RaycastResult, index:number): number
---@return number
---@param item UnityEngine.EventSystems.RaycastResult
---@param index number
---@param count number
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:LastIndexOf(item, index, count) end
---@return boolean
---@param item UnityEngine.EventSystems.RaycastResult
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:Remove(item) end
---@return number
---@param match (fun(obj:UnityEngine.EventSystems.RaycastResult):boolean)
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:RemoveAll(match) end
---@param index number
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:RemoveAt(index) end
---@param index number
---@param count number
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:RemoveRange(index, count) end
---@overload fun(): void
---@param index number
---@param count number
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:Reverse(index, count) end
---@overload fun(): void
---@overload fun(comparer:System.Collections.Generic.IComparer_UnityEngine.EventSystems.RaycastResult): void
---@overload fun(comparison:(fun(x:UnityEngine.EventSystems.RaycastResult, y:UnityEngine.EventSystems.RaycastResult):number)): void
---@param index number
---@param count number
---@param comparer System.Collections.Generic.IComparer_UnityEngine.EventSystems.RaycastResult
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:Sort(index, count, comparer) end
---@return UnityEngine.EventSystems.RaycastResult[]
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:ToArray() end
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:TrimExcess() end
---@return boolean
---@param match (fun(obj:UnityEngine.EventSystems.RaycastResult):boolean)
function System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:TrueForAll(match) end
return System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult
