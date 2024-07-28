---@class CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData
---@field public Capacity number
---@field public Count number
---@field public Item ZJYFrameWork.UISerializable.UIKeyObjectData
CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData = {}

---@overload fun(): CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData
---@overload fun(capacity:number): CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData
---@return CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData
---@param collection CS.System.Collections.Generic.IEnumerable_ZJYFrameWork.UISerializable.UIKeyObjectData
function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData.New(collection) end

---@param item ZJYFrameWork.UISerializable.UIKeyObjectData
function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:Add(item) end

---@param collection CS.System.Collections.Generic.IEnumerable_ZJYFrameWork.UISerializable.UIKeyObjectData
function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:AddRange(collection) end

---@return CS.System.Collections.ObjectModel.ReadOnlyCollection_ZJYFrameWork.UISerializable.UIKeyObjectData
function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:AsReadOnly() end

---@overload fun(item:ZJYFrameWork.UISerializable.UIKeyObjectData): number
---@overload fun(item:ZJYFrameWork.UISerializable.UIKeyObjectData, comparer:CS.System.Collections.Generic.IComparer_ZJYFrameWork.UISerializable.UIKeyObjectData): number
---@return number
---@param index number
---@param count number
---@param item ZJYFrameWork.UISerializable.UIKeyObjectData
---@param comparer CS.System.Collections.Generic.IComparer_ZJYFrameWork.UISerializable.UIKeyObjectData
function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:BinarySearch(index, count, item, comparer) end

function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:Clear() end

---@return boolean
---@param item ZJYFrameWork.UISerializable.UIKeyObjectData
function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:Contains(item) end

---@overload fun(array:ZJYFrameWork.UISerializable.UIKeyObjectData[]): void
---@overload fun(array:ZJYFrameWork.UISerializable.UIKeyObjectData[], arrayIndex:number): void
---@param index number
---@param array ZJYFrameWork.UISerializable.UIKeyObjectData[]
---@param arrayIndex number
---@param count number
function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:CopyTo(index, array, arrayIndex, count) end

---@return boolean
---@param match (fun(obj:ZJYFrameWork.UISerializable.UIKeyObjectData):boolean)
function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:Exists(match) end

---@return ZJYFrameWork.UISerializable.UIKeyObjectData
---@param match (fun(obj:ZJYFrameWork.UISerializable.UIKeyObjectData):boolean)
function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:Find(match) end

---@return CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData
---@param match (fun(obj:ZJYFrameWork.UISerializable.UIKeyObjectData):boolean)
function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:FindAll(match) end

---@overload fun(match:(fun(obj:ZJYFrameWork.UISerializable.UIKeyObjectData):boolean)): number
---@overload fun(startIndex:number, match:(fun(obj:ZJYFrameWork.UISerializable.UIKeyObjectData):boolean)): number
---@return number
---@param startIndex number
---@param count number
---@param match (fun(obj:ZJYFrameWork.UISerializable.UIKeyObjectData):boolean)
function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:FindIndex(startIndex, count, match) end

---@return ZJYFrameWork.UISerializable.UIKeyObjectData
---@param match (fun(obj:ZJYFrameWork.UISerializable.UIKeyObjectData):boolean)
function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:FindLast(match) end

---@overload fun(match:(fun(obj:ZJYFrameWork.UISerializable.UIKeyObjectData):boolean)): number
---@overload fun(startIndex:number, match:(fun(obj:ZJYFrameWork.UISerializable.UIKeyObjectData):boolean)): number
---@return number
---@param startIndex number
---@param count number
---@param match (fun(obj:ZJYFrameWork.UISerializable.UIKeyObjectData):boolean)
function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:FindLastIndex(startIndex, count, match) end

---@param action (fun(obj:ZJYFrameWork.UISerializable.UIKeyObjectData):void)
function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:ForEach(action) end

---@return CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData.Enumerator
function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:GetEnumerator() end

---@return CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData
---@param index number
---@param count number
function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:GetRange(index, count) end

---@overload fun(item:ZJYFrameWork.UISerializable.UIKeyObjectData): number
---@overload fun(item:ZJYFrameWork.UISerializable.UIKeyObjectData, index:number): number
---@return number
---@param item ZJYFrameWork.UISerializable.UIKeyObjectData
---@param index number
---@param count number
function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:IndexOf(item, index, count) end

---@param index number
---@param item ZJYFrameWork.UISerializable.UIKeyObjectData
function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:Insert(index, item) end

---@param index number
---@param collection CS.System.Collections.Generic.IEnumerable_ZJYFrameWork.UISerializable.UIKeyObjectData
function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:InsertRange(index, collection) end

---@overload fun(item:ZJYFrameWork.UISerializable.UIKeyObjectData): number
---@overload fun(item:ZJYFrameWork.UISerializable.UIKeyObjectData, index:number): number
---@return number
---@param item ZJYFrameWork.UISerializable.UIKeyObjectData
---@param index number
---@param count number
function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:LastIndexOf(item, index, count) end

---@return boolean
---@param item ZJYFrameWork.UISerializable.UIKeyObjectData
function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:Remove(item) end

---@return number
---@param match (fun(obj:ZJYFrameWork.UISerializable.UIKeyObjectData):boolean)
function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:RemoveAll(match) end

---@param index number
function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:RemoveAt(index) end

---@param index number
---@param count number
function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:RemoveRange(index, count) end

---@overload fun(): void
---@param index number
---@param count number
function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:Reverse(index, count) end

---@overload fun(): void
---@overload fun(comparer:CS.System.Collections.Generic.IComparer_ZJYFrameWork.UISerializable.UIKeyObjectData): void
---@overload fun(comparison:(fun(x:ZJYFrameWork.UISerializable.UIKeyObjectData, y:ZJYFrameWork.UISerializable.UIKeyObjectData):number)): void
---@param index number
---@param count number
---@param comparer CS.System.Collections.Generic.IComparer_ZJYFrameWork.UISerializable.UIKeyObjectData
function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:Sort(index, count, comparer) end

---@return ZJYFrameWork.UISerializable.UIKeyObjectData[]
function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:ToArray() end

function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:TrimExcess() end

---@return boolean
---@param match (fun(obj:ZJYFrameWork.UISerializable.UIKeyObjectData):boolean)
function CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData:TrueForAll(match) end

return CS.System.Collections.Generic.List_ZJYFrameWork.UISerializable.UIKeyObjectData
