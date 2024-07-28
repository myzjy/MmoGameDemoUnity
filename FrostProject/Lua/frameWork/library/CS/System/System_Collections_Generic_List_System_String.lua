---@class CS.System.Collections.Generic.List_System.String
---@field public Capacity number
---@field public Count number
---@field public Item string
CS.System.Collections.Generic.List_System.String = { }
---@overload fun(): CS.System.Collections.Generic.List_System.String
---@overload fun(capacity:number): CS.System.Collections.Generic.List_System.String
---@return CS.System.Collections.Generic.List_System.String
---@param collection CS.System.Collections.Generic.IEnumerable_System.String
function CS.System.Collections.Generic.List_System.String.New(collection) end
---@param item string
function CS.System.Collections.Generic.List_System.String:Add(item) end
---@param collection CS.System.Collections.Generic.IEnumerable_System.String
function CS.System.Collections.Generic.List_System.String:AddRange(collection) end
---@return CS.System.Collections.ObjectModel.ReadOnlyCollection_System.String
function CS.System.Collections.Generic.List_System.String:AsReadOnly() end
---@overload fun(item:string): number
---@overload fun(item:string, comparer:CS.System.Collections.Generic.IComparer_System.String): number
---@return number
---@param index number
---@param count number
---@param item string
---@param comparer CS.System.Collections.Generic.IComparer_System.String
function CS.System.Collections.Generic.List_System.String:BinarySearch(index, count, item, comparer) end
function CS.System.Collections.Generic.List_System.String:Clear() end
---@return boolean
---@param item string
function CS.System.Collections.Generic.List_System.String:Contains(item) end
---@overload fun(array:CS.System.String[]): void
---@overload fun(array:CS.System.String[], arrayIndex:number): void
---@param index number
---@param array CS.System.String[]
---@param arrayIndex number
---@param count number
function CS.System.Collections.Generic.List_System.String:CopyTo(index, array, arrayIndex, count) end
---@return boolean
---@param match (fun(obj:string):boolean)
function CS.System.Collections.Generic.List_System.String:Exists(match) end
---@return string
---@param match (fun(obj:string):boolean)
function CS.System.Collections.Generic.List_System.String:Find(match) end
---@return CS.System.Collections.Generic.List_System.String
---@param match (fun(obj:string):boolean)
function CS.System.Collections.Generic.List_System.String:FindAll(match) end
---@overload fun(match:(fun(obj:string):boolean)): number
---@overload fun(startIndex:number, match:(fun(obj:string):boolean)): number
---@return number
---@param startIndex number
---@param count number
---@param match (fun(obj:string):boolean)
function CS.System.Collections.Generic.List_System.String:FindIndex(startIndex, count, match) end
---@return string
---@param match (fun(obj:string):boolean)
function CS.System.Collections.Generic.List_System.String:FindLast(match) end
---@overload fun(match:(fun(obj:string):boolean)): number
---@overload fun(startIndex:number, match:(fun(obj:string):boolean)): number
---@return number
---@param startIndex number
---@param count number
---@param match (fun(obj:string):boolean)
function CS.System.Collections.Generic.List_System.String:FindLastIndex(startIndex, count, match) end
---@param action (fun(obj:string):void)
function CS.System.Collections.Generic.List_System.String:ForEach(action) end
---@return CS.System.Collections.Generic.List_System.String.Enumerator
function CS.System.Collections.Generic.List_System.String:GetEnumerator() end
---@return CS.System.Collections.Generic.List_System.String
---@param index number
---@param count number
function CS.System.Collections.Generic.List_System.String:GetRange(index, count) end
---@overload fun(item:string): number
---@overload fun(item:string, index:number): number
---@return number
---@param item string
---@param index number
---@param count number
function CS.System.Collections.Generic.List_System.String:IndexOf(item, index, count) end
---@param index number
---@param item string
function CS.System.Collections.Generic.List_System.String:Insert(index, item) end
---@param index number
---@param collection CS.System.Collections.Generic.IEnumerable_System.String
function CS.System.Collections.Generic.List_System.String:InsertRange(index, collection) end
---@overload fun(item:string): number
---@overload fun(item:string, index:number): number
---@return number
---@param item string
---@param index number
---@param count number
function CS.System.Collections.Generic.List_System.String:LastIndexOf(item, index, count) end
---@return boolean
---@param item string
function CS.System.Collections.Generic.List_System.String:Remove(item) end
---@return number
---@param match (fun(obj:string):boolean)
function CS.System.Collections.Generic.List_System.String:RemoveAll(match) end
---@param index number
function CS.System.Collections.Generic.List_System.String:RemoveAt(index) end
---@param index number
---@param count number
function CS.System.Collections.Generic.List_System.String:RemoveRange(index, count) end
---@overload fun(): void
---@param index number
---@param count number
function CS.System.Collections.Generic.List_System.String:Reverse(index, count) end
---@overload fun(): void
---@overload fun(comparer:CS.System.Collections.Generic.IComparer_System.String): void
---@overload fun(comparison:(fun(x:string, y:string):number)): void
---@param index number
---@param count number
---@param comparer CS.System.Collections.Generic.IComparer_System.String
function CS.System.Collections.Generic.List_System.String:Sort(index, count, comparer) end
---@return CS.System.String[]
function CS.System.Collections.Generic.List_System.String:ToArray() end
function CS.System.Collections.Generic.List_System.String:TrimExcess() end
---@return boolean
---@param match (fun(obj:string):boolean)
function CS.System.Collections.Generic.List_System.String:TrueForAll(match) end
return CS.System.Collections.Generic.List_System.String
