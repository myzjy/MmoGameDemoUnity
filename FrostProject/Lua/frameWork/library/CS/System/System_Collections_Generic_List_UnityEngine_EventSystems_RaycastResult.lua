---@class CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult
---@field public Capacity number
---@field public Count number
---@field public Item CS.UnityEngine.EventSystems.RaycastResult
CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult = { }
---@overload fun(): CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult
---@overload fun(capacity:number): CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult
---@return CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult
---@param collection CS.System.Collections.Generic.IEnumerable_UnityEngine.EventSystems.RaycastResult
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult.New(collection) end
---@param item CS.UnityEngine.EventSystems.RaycastResult
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:Add(item) end
---@param collection CS.System.Collections.Generic.IEnumerable_UnityEngine.EventSystems.RaycastResult
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:AddRange(collection) end
---@return CS.System.Collections.ObjectModel.ReadOnlyCollection_UnityEngine.EventSystems.RaycastResult
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:AsReadOnly() end
---@overload fun(item:CS.UnityEngine.EventSystems.RaycastResult): number
---@overload fun(item:CS.UnityEngine.EventSystems.RaycastResult, comparer:CS.System.Collections.Generic.IComparer_UnityEngine.EventSystems.RaycastResult): number
---@return number
---@param index number
---@param count number
---@param item CS.UnityEngine.EventSystems.RaycastResult
---@param comparer CS.System.Collections.Generic.IComparer_UnityEngine.EventSystems.RaycastResult
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:BinarySearch(index, count, item, comparer) end
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:Clear() end
---@return boolean
---@param item CS.UnityEngine.EventSystems.RaycastResult
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:Contains(item) end
---@overload fun(array:CS.UnityEngine.EventSystems.RaycastResult[]): void
---@overload fun(array:CS.UnityEngine.EventSystems.RaycastResult[], arrayIndex:number): void
---@param index number
---@param array CS.UnityEngine.EventSystems.RaycastResult[]
---@param arrayIndex number
---@param count number
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:CopyTo(index, array, arrayIndex, count) end
---@return boolean
---@param match (fun(obj:CS.UnityEngine.EventSystems.RaycastResult):boolean)
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:Exists(match) end
---@return CS.UnityEngine.EventSystems.RaycastResult
---@param match (fun(obj:CS.UnityEngine.EventSystems.RaycastResult):boolean)
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:Find(match) end
---@return CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult
---@param match (fun(obj:CS.UnityEngine.EventSystems.RaycastResult):boolean)
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:FindAll(match) end
---@overload fun(match:(fun(obj:CS.UnityEngine.EventSystems.RaycastResult):boolean)): number
---@overload fun(startIndex:number, match:(fun(obj:CS.UnityEngine.EventSystems.RaycastResult):boolean)): number
---@return number
---@param startIndex number
---@param count number
---@param match (fun(obj:CS.UnityEngine.EventSystems.RaycastResult):boolean)
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:FindIndex(startIndex, count, match) end
---@return CS.UnityEngine.EventSystems.RaycastResult
---@param match (fun(obj:CS.UnityEngine.EventSystems.RaycastResult):boolean)
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:FindLast(match) end
---@overload fun(match:(fun(obj:CS.UnityEngine.EventSystems.RaycastResult):boolean)): number
---@overload fun(startIndex:number, match:(fun(obj:CS.UnityEngine.EventSystems.RaycastResult):boolean)): number
---@return number
---@param startIndex number
---@param count number
---@param match (fun(obj:CS.UnityEngine.EventSystems.RaycastResult):boolean)
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:FindLastIndex(startIndex, count, match) end
---@param action (fun(obj:CS.UnityEngine.EventSystems.RaycastResult):void)
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:ForEach(action) end
---@return CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult.Enumerator
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:GetEnumerator() end
---@return CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult
---@param index number
---@param count number
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:GetRange(index, count) end
---@overload fun(item:CS.UnityEngine.EventSystems.RaycastResult): number
---@overload fun(item:CS.UnityEngine.EventSystems.RaycastResult, index:number): number
---@return number
---@param item CS.UnityEngine.EventSystems.RaycastResult
---@param index number
---@param count number
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:IndexOf(item, index, count) end
---@param index number
---@param item CS.UnityEngine.EventSystems.RaycastResult
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:Insert(index, item) end
---@param index number
---@param collection CS.System.Collections.Generic.IEnumerable_UnityEngine.EventSystems.RaycastResult
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:InsertRange(index, collection) end
---@overload fun(item:CS.UnityEngine.EventSystems.RaycastResult): number
---@overload fun(item:CS.UnityEngine.EventSystems.RaycastResult, index:number): number
---@return number
---@param item CS.UnityEngine.EventSystems.RaycastResult
---@param index number
---@param count number
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:LastIndexOf(item, index, count) end
---@return boolean
---@param item CS.UnityEngine.EventSystems.RaycastResult
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:Remove(item) end
---@return number
---@param match (fun(obj:CS.UnityEngine.EventSystems.RaycastResult):boolean)
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:RemoveAll(match) end
---@param index number
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:RemoveAt(index) end
---@param index number
---@param count number
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:RemoveRange(index, count) end
---@overload fun(): void
---@param index number
---@param count number
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:Reverse(index, count) end
---@overload fun(): void
---@overload fun(comparer:CS.System.Collections.Generic.IComparer_UnityEngine.EventSystems.RaycastResult): void
---@overload fun(comparison:(fun(x:CS.UnityEngine.EventSystems.RaycastResult, y:CS.UnityEngine.EventSystems.RaycastResult):number)): void
---@param index number
---@param count number
---@param comparer CS.System.Collections.Generic.IComparer_UnityEngine.EventSystems.RaycastResult
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:Sort(index, count, comparer) end
---@return CS.UnityEngine.EventSystems.RaycastResult[]
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:ToArray() end
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:TrimExcess() end
---@return boolean
---@param match (fun(obj:CS.UnityEngine.EventSystems.RaycastResult):boolean)
function CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult:TrueForAll(match) end
return CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult
