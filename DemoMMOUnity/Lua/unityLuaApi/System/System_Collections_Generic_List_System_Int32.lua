---@class System.Collections.Generic.List_System.Int32
---@field public Capacity number
---@field public Count number
---@field public Item number

---@type System.Collections.Generic.List_System.Int32
System.Collections.Generic.List_System.Int32 = { }
---@overload fun(): System.Collections.Generic.List_System.Int32
---@overload fun(capacity:number): System.Collections.Generic.List_System.Int32
---@return System.Collections.Generic.List_System.Int32
---@param collection System.Collections.Generic.IEnumerable_System.Int32
function System.Collections.Generic.List_System.Int32.New(collection) end
---@param item number
function System.Collections.Generic.List_System.Int32:Add(item) end
---@param collection System.Collections.Generic.IEnumerable_System.Int32
function System.Collections.Generic.List_System.Int32:AddRange(collection) end
---@return System.Collections.ObjectModel.ReadOnlyCollection_System.Int32
function System.Collections.Generic.List_System.Int32:AsReadOnly() end
---@overload fun(item:number): number
---@overload fun(item:number, comparer:System.Collections.Generic.IComparer_System.Int32): number
---@return number
---@param index number
---@param count number
---@param item number
---@param comparer System.Collections.Generic.IComparer_System.Int32
function System.Collections.Generic.List_System.Int32:BinarySearch(index, count, item, comparer) end
function System.Collections.Generic.List_System.Int32:Clear() end
---@return boolean
---@param item number
function System.Collections.Generic.List_System.Int32:Contains(item) end
---@overload fun(array:System.Int32[]): void
---@overload fun(array:System.Int32[], arrayIndex:number): void
---@param index number
---@param array System.Int32[]
---@param arrayIndex number
---@param count number
function System.Collections.Generic.List_System.Int32:CopyTo(index, array, arrayIndex, count) end
---@return boolean
---@param match (fun(obj:number):boolean)
function System.Collections.Generic.List_System.Int32:Exists(match) end
---@return number
---@param match (fun(obj:number):boolean)
function System.Collections.Generic.List_System.Int32:Find(match) end
---@return System.Collections.Generic.List_System.Int32
---@param match (fun(obj:number):boolean)
function System.Collections.Generic.List_System.Int32:FindAll(match) end
---@overload fun(match:(fun(obj:number):boolean)): number
---@overload fun(startIndex:number, match:(fun(obj:number):boolean)): number
---@return number
---@param startIndex number
---@param count number
---@param match (fun(obj:number):boolean)
function System.Collections.Generic.List_System.Int32:FindIndex(startIndex, count, match) end
---@return number
---@param match (fun(obj:number):boolean)
function System.Collections.Generic.List_System.Int32:FindLast(match) end
---@overload fun(match:(fun(obj:number):boolean)): number
---@overload fun(startIndex:number, match:(fun(obj:number):boolean)): number
---@return number
---@param startIndex number
---@param count number
---@param match (fun(obj:number):boolean)
function System.Collections.Generic.List_System.Int32:FindLastIndex(startIndex, count, match) end
---@param action (fun(obj:number):void)
function System.Collections.Generic.List_System.Int32:ForEach(action) end
---@return System.Collections.Generic.List_System.Int32.Enumerator
function System.Collections.Generic.List_System.Int32:GetEnumerator() end
---@return System.Collections.Generic.List_System.Int32
---@param index number
---@param count number
function System.Collections.Generic.List_System.Int32:GetRange(index, count) end
---@overload fun(item:number): number
---@overload fun(item:number, index:number): number
---@return number
---@param item number
---@param index number
---@param count number
function System.Collections.Generic.List_System.Int32:IndexOf(item, index, count) end
---@param index number
---@param item number
function System.Collections.Generic.List_System.Int32:Insert(index, item) end
---@param index number
---@param collection System.Collections.Generic.IEnumerable_System.Int32
function System.Collections.Generic.List_System.Int32:InsertRange(index, collection) end
---@overload fun(item:number): number
---@overload fun(item:number, index:number): number
---@return number
---@param item number
---@param index number
---@param count number
function System.Collections.Generic.List_System.Int32:LastIndexOf(item, index, count) end
---@return boolean
---@param item number
function System.Collections.Generic.List_System.Int32:Remove(item) end
---@return number
---@param match (fun(obj:number):boolean)
function System.Collections.Generic.List_System.Int32:RemoveAll(match) end
---@param index number
function System.Collections.Generic.List_System.Int32:RemoveAt(index) end
---@param index number
---@param count number
function System.Collections.Generic.List_System.Int32:RemoveRange(index, count) end
---@overload fun(): void
---@param index number
---@param count number
function System.Collections.Generic.List_System.Int32:Reverse(index, count) end
---@overload fun(): void
---@overload fun(comparer:System.Collections.Generic.IComparer_System.Int32): void
---@overload fun(comparison:(fun(x:number, y:number):number)): void
---@param index number
---@param count number
---@param comparer System.Collections.Generic.IComparer_System.Int32
function System.Collections.Generic.List_System.Int32:Sort(index, count, comparer) end
---@return System.Int32[]
function System.Collections.Generic.List_System.Int32:ToArray() end
function System.Collections.Generic.List_System.Int32:TrimExcess() end
---@return boolean
---@param match (fun(obj:number):boolean)
function System.Collections.Generic.List_System.Int32:TrueForAll(match) end
return System.Collections.Generic.List_System.Int32
