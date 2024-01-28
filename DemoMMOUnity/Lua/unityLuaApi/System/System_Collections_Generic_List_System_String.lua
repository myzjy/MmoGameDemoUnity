---@class System.Collections.Generic.List_System.String
---@field public Capacity number
---@field public Count number
---@field public Item string

---@type System.Collections.Generic.List_System.String
System.Collections.Generic.List_System.String = { }
---@overload fun(): System.Collections.Generic.List_System.String
---@overload fun(capacity:number): System.Collections.Generic.List_System.String
---@return System.Collections.Generic.List_System.String
---@param collection System.Collections.Generic.IEnumerable_System.String
function System.Collections.Generic.List_System.String.New(collection) end
---@param item string
function System.Collections.Generic.List_System.String:Add(item) end
---@param collection System.Collections.Generic.IEnumerable_System.String
function System.Collections.Generic.List_System.String:AddRange(collection) end
---@return System.Collections.ObjectModel.ReadOnlyCollection_System.String
function System.Collections.Generic.List_System.String:AsReadOnly() end
---@overload fun(item:string): number
---@overload fun(item:string, comparer:System.Collections.Generic.IComparer_System.String): number
---@return number
---@param index number
---@param count number
---@param item string
---@param comparer System.Collections.Generic.IComparer_System.String
function System.Collections.Generic.List_System.String:BinarySearch(index, count, item, comparer) end
function System.Collections.Generic.List_System.String:Clear() end
---@return boolean
---@param item string
function System.Collections.Generic.List_System.String:Contains(item) end
---@overload fun(array:System.String[]): void
---@overload fun(array:System.String[], arrayIndex:number): void
---@param index number
---@param array System.String[]
---@param arrayIndex number
---@param count number
function System.Collections.Generic.List_System.String:CopyTo(index, array, arrayIndex, count) end
---@return boolean
---@param match (fun(obj:string):boolean)
function System.Collections.Generic.List_System.String:Exists(match) end
---@return string
---@param match (fun(obj:string):boolean)
function System.Collections.Generic.List_System.String:Find(match) end
---@return System.Collections.Generic.List_System.String
---@param match (fun(obj:string):boolean)
function System.Collections.Generic.List_System.String:FindAll(match) end
---@overload fun(match:(fun(obj:string):boolean)): number
---@overload fun(startIndex:number, match:(fun(obj:string):boolean)): number
---@return number
---@param startIndex number
---@param count number
---@param match (fun(obj:string):boolean)
function System.Collections.Generic.List_System.String:FindIndex(startIndex, count, match) end
---@return string
---@param match (fun(obj:string):boolean)
function System.Collections.Generic.List_System.String:FindLast(match) end
---@overload fun(match:(fun(obj:string):boolean)): number
---@overload fun(startIndex:number, match:(fun(obj:string):boolean)): number
---@return number
---@param startIndex number
---@param count number
---@param match (fun(obj:string):boolean)
function System.Collections.Generic.List_System.String:FindLastIndex(startIndex, count, match) end
---@param action (fun(obj:string):void)
function System.Collections.Generic.List_System.String:ForEach(action) end
---@return System.Collections.Generic.List_System.String.Enumerator
function System.Collections.Generic.List_System.String:GetEnumerator() end
---@return System.Collections.Generic.List_System.String
---@param index number
---@param count number
function System.Collections.Generic.List_System.String:GetRange(index, count) end
---@overload fun(item:string): number
---@overload fun(item:string, index:number): number
---@return number
---@param item string
---@param index number
---@param count number
function System.Collections.Generic.List_System.String:IndexOf(item, index, count) end
---@param index number
---@param item string
function System.Collections.Generic.List_System.String:Insert(index, item) end
---@param index number
---@param collection System.Collections.Generic.IEnumerable_System.String
function System.Collections.Generic.List_System.String:InsertRange(index, collection) end
---@overload fun(item:string): number
---@overload fun(item:string, index:number): number
---@return number
---@param item string
---@param index number
---@param count number
function System.Collections.Generic.List_System.String:LastIndexOf(item, index, count) end
---@return boolean
---@param item string
function System.Collections.Generic.List_System.String:Remove(item) end
---@return number
---@param match (fun(obj:string):boolean)
function System.Collections.Generic.List_System.String:RemoveAll(match) end
---@param index number
function System.Collections.Generic.List_System.String:RemoveAt(index) end
---@param index number
---@param count number
function System.Collections.Generic.List_System.String:RemoveRange(index, count) end
---@overload fun(): void
---@param index number
---@param count number
function System.Collections.Generic.List_System.String:Reverse(index, count) end
---@overload fun(): void
---@overload fun(comparer:System.Collections.Generic.IComparer_System.String): void
---@overload fun(comparison:(fun(x:string, y:string):number)): void
---@param index number
---@param count number
---@param comparer System.Collections.Generic.IComparer_System.String
function System.Collections.Generic.List_System.String:Sort(index, count, comparer) end
---@return System.String[]
function System.Collections.Generic.List_System.String:ToArray() end
function System.Collections.Generic.List_System.String:TrimExcess() end
---@return boolean
---@param match (fun(obj:string):boolean)
function System.Collections.Generic.List_System.String:TrueForAll(match) end
return System.Collections.Generic.List_System.String
