--[[
    Created by zhangjingyi.
    DateTime: 2024/7/9 下午3:11
--]]

Tools = {}

------------------------------------------------------------
---判断一个值能否通过条件
--- @generic V
--- @param inValue V
--- @param ... fun(a:V):boolean
--- @return boolean
------------------------------------------------------------
function Tools.FitConditions(inValue, ...)
    local tArgs = { ... }
    for _, tValueFunc in ipairs(tArgs) do
        local tIsExist = tValueFunc(inValue)
        if not tIsExist then
            return false
        end
    end
    return true
end

------------------------------------------------------------
---对数值进行四舍五入
--- @param inValue number 数值
--- @return number
------------------------------------------------------------
function math.RoundToInt(inValue)
    return math.floor(inValue + 0.5)
end

------------------------------------------------------------
---对数值进行取小数部分
--- @param inValue number 数值
--- @return number
------------------------------------------------------------
function math.Frac(inValue)
    return inValue - math.floor(inValue)
end

------------------------------------------------------------
---角度转弧度
--- @param inAngle number 角度
--- @return number
------------------------------------------------------------
function math.AngleToRadian(inAngle)
    return inAngle * math.pi / 180
end

------------------------------------------------------------
---弧度转角度
--- @param inRadian number 弧度
--- @return number
------------------------------------------------------------
function math.RadianToAngle(inRadian)
    return inRadian / math.pi * 180
end

------------------------------------------------------------
---检查指定的文件或目录是否存在，如果存在返回 true，否则返回 false
--- @param inFilePath string 文件路径
--- @return boolean
------------------------------------------------------------
function io.Exists(inFilePath)
    local inFile = io.open(inFilePath, "r")
    if inFile then
        io.close(inFile)
    end
    return inFile ~= nil
end

------------------------------------------------------------
---读取文件内容，返回包含文件内容的字符串，如果失败返回 nil
--- @param inFilePath string 文件路径
--- @return string|nil
------------------------------------------------------------
function io.ReadText(inFilePath)
    local inFile = io.open(inFilePath, "r")
    if not inFile then
        return nil
    end
    local tContent = inFile:read("*a")
    io.close(inFile)
    return tContent
end

------------------------------------------------------------
--- 以字符串内容写入文件，成功返回 true，失败返回 false
--- "mode 写入模式" 参数决定 io.writefile() 如何写入内容，可用的值如下：
--- -   "w+" : 覆盖文件已有内容，如果文件不存在则创建新文件
--- -   "a+" : 追加内容到文件尾部，如果文件不存在则创建文件
--- 此外，还可以在 "写入模式" 参数最后追加字符 "b" ，表示以二进制方式写入数据，这样可以避免内容写入不完整。
--- @param inFilePath string 文件路径
--- @param inContent string 内容
--- @param inMode string 模式
------------------------------------------------------------
function io.WriteText(inFilePath, inContent, inMode)
    inMode = inMode or "w+b"
    local tOpenFile = io.open(inFilePath, inMode)
    if not tOpenFile then
        return false
    end
    if tOpenFile:write(inContent) == nil then
        return false
    end
    io.close(tOpenFile)
    return true
end

------------------------------------------------------------
---拆分一个路径字符串，返回组成路径的各个部分
--- local pathinfo  = io.pathinfo("/var/app/test/abc.png")
--- pathinfo.dirname  = "/var/app/test/"
--- pathinfo.filename = "abc.png"
--- pathinfo.basename = "abc"
--- pathinfo.extname  = ".png"
--- @param  inFilePath string 要分拆的路径字符串
--- @return table
------------------------------------------------------------
function io.PathInfo(inFilePath)
    local inIndex = string.len(inFilePath)
    local inExtPos = inIndex + 1
    while inIndex > 0 do
        local tFileByte = string.byte(inFilePath, inIndex)
        if tFileByte == 46 then
            -- 46 = char "."
            inExtPos = inIndex
        elseif tFileByte == 47 then
            -- 47 = char "/"
            break
        end
        inIndex = inIndex - 1
    end

    local inDirname = string.sub(inFilePath, 1, inIndex)
    local inFilename = string.sub(inFilePath, inIndex + 1)
    inExtPos = inExtPos - inIndex
    local inBasename = string.sub(inFilename, 1, inExtPos - 1)
    local inExtname = string.sub(inFilename, inExtPos)
    return {
        dirname = inDirname,
        filename = inFilename,
        basename = inBasename,
        extname = inExtname,
    }
end

------------------------------------------------------------
---返回指定文件的大小，如果失败返回 false
--- @param  inFilePath string 文件完全路径
--- @return number|boolean
------------------------------------------------------------
function io.Filesize(inFilePath)
    --- @type number|boolean
    local tSize = false
    local tOpenFile = io.open(inFilePath, "r")
    if tOpenFile then
        local inCurrent = tOpenFile:seek()
        tSize = tOpenFile:seek("end")
        tOpenFile:seek("set", inCurrent)
        io.close(tOpenFile)
    end
    return tSize
end

------------------------------------------------------------
--- 深拷贝一个table
---@param inTab table
---@return table
------------------------------------------------------------
function table.DeepCopy(inTab)
    local tSearchTable = {}
    local function tFunc(inObject)
        if type(inObject) ~= LuaDataType.Table then
            return inObject
        end
        local tNewTable = {}
        tSearchTable[inObject] = tNewTable
        for tIndex, tObjValue in pairs(inObject) do
            tNewTable[tFunc(tIndex)] = tFunc(tObjValue)
        end
        return setmetatable(tNewTable, getmetatable(inObject))
    end

    return tFunc(inTab)
end

------------------------------------------------------------
--- 清空一个Tabl
---@param inTab table|nil
------------------------------------------------------------
function table.Clear(inTab)
    if inTab == nil then
        return
    end
    for _, v in pairs(inTab) do
        v = nil
    end
    inTab = nil
end

------------------------------------------------------------
--- 计算表格包含的字段数量
--- Lua table 的 "#" 操作只对依次排序的数值下标数组有效，table.nums() 则计算 table 中所有不为 nil 的值的个数。
---@param inTab table
---@return number
------------------------------------------------------------
function table.GetCount(inTab)
    local count = 0
    for _, _ in pairs(inTab) do
        count = count + 1
    end
    return count
end

------------------------------------------------------------
--- 返回指定表格中的所有键
--- local tHashtable = {a = 1, b = 2, c = 3}
--- local keys = table.GetKeys(tHashtable)
--- keys = {"a", "b", "c"}
---@param inHashtable table
---@return any[]
------------------------------------------------------------
function table.GetKeys(inHashtable)
    local keys = {}
    for k, _ in pairs(inHashtable) do
        keys[#keys + 1] = k
    end
    return keys
end

------------------------------------------------------------
---@param inHashtable table
---@param inKey string
---@return boolean
------------------------------------------------------------
function table.ContainsKey(inHashtable, inKey)
    local tTable = type(inHashtable)
    return (tTable == LuaDataType.Table or tTable == LuaDataType.UserData) and inHashtable[inKey] ~= nil
end

------------------------------------------------------------
--- 返回指定表格中的所有值
--- local inHashtable = {a = 1, b = 2, c = 3}
--- local tValues = table.GetValues(inHashtable)
--- tValues = {1, 2, 3}
---@param inHashtable table
---@return any[]
------------------------------------------------------------
function table.GetValues(inHashtable)
    local tValues = {}
    for _, v in pairs(inHashtable) do
        tValues[#tValues + 1] = v
    end
    return tValues
end

------------------------------------------------------------
--- 将来源表格中所有键及其值复制到目标表格对象中，如果存在同名键，则覆盖其值
--- local tDest = {a = 1, b = 2}
--- local tSrc  = {c = 3, d = 4}
--- table.merge(tDest, tSrc)
--- tDest = {a = 1, b = 2, c = 3, d = 4}
---@param inDest table
---@param inSrc table
------------------------------------------------------------
function table.Merge(inDest, inSrc)
    for k, v in pairs(inSrc) do
        inDest[k] = v
    end
end

------------------------------------------------------------
--- 在目标表格的指定位置插入来源表格，如果没有指定位置则连接两个表格
--- local tDest = {1, 2, 3}
--- local tSrc  = {4, 5, 6}
--- table.Insert(tDest, tSrc)
--- tDest = {1, 2, 3, 4, 5, 6}
--- tDest = {1, 2, 3}
--- table.Insert(tDest, tSrc, 5)
--- tDest = {1, 2, 3, nil, 4, 5, 6}
---@generic V
---@param inDest V[]
---@param inSrcTab V[]
---@param inBegin number
------------------------------------------------------------
function table.Insert(inDest, inSrcTab, inBegin)
    local bo, _begin = math.RoundToInt(inBegin)
    if not bo then
        _begin = 0
    end
    if _begin <= 0 then
        _begin = #inDest + 1
    end
    local len = #inSrcTab
    for i = 0, len - 1 do
        inDest[i + _begin] = inSrcTab[i + 1]
    end
end

------------------------------------------------------------
---@param inTabs table
---@param inValue any
------------------------------------------------------------
function table.GetKey(inTabs, inValue)
    for tIndex, tValue in pairs(inTabs) do
        if tValue == inValue then
            return tIndex
        end
    end
    return false
end

------------------------------------------------------------
---筛选所有符合条件的数据
---@generic K
---@generic V
---@param inTabs table<K,V>
---@param ... fun(a:V):boolean
---@param inReturnDic boolean 返回值是否采用原有的key
---@return table<K,V>|V[]
------------------------------------------------------------
function table.FindAll(inTabs, inReturnDic, ...)
    local tResultTabs = {}
    for tKey, tValue in pairs(inTabs) do
        local bo = Tools.FitConditions(tValue, ...)
        if bo then
            if inReturnDic then
                tResultTabs[tKey] = tValue
            else
                table.insert(tResultTabs, tValue)
            end
        end
    end
    return tResultTabs
end

------------------------------------------------------------
---筛选第一个符合条件的数据
---@generic K
---@generic V
---@param inTabs table<K,V>
---@param ... fun(a:V):boolean
---@return V
------------------------------------------------------------
function table.Find(inTabs, ...)
    for _, tValue in pairs(inTabs) do
        local bo = Tools.FitConditions(tValue, ...)
        if bo then
            return tValue
        end
    end
    return nil
end

------------------------------------------------------------
--- 对表格中每一个值执行一次指定的函数，并用函数返回值更新表格内容
---@generic K,V
---@param inTab table<K,V>
---@param inFunc fun(tKey:K,Value:V):V
------------------------------------------------------------
function table.Map(inTab, inFunc)
    for k, v in pairs(inTab) do
        inTab[k] = inFunc(k, v)
    end
end

------------------------------------------------------------
--- 对表格中每一个值执行一次指定的函数
---@generic K
---@generic V
---@generic K,V
---@param inTab table<K,V>
---@param func fun(tKey,Value)
------------------------------------------------------------
function table.Walk(inTab, inFunc)
    for k, v in pairs(inTab) do
        inFunc(k, v)
    end
end

------------------------------------------------------------
--- 对表格中每一个值执行一次指定的函数，如果该函数返回 true，则对应的值会从表格中删除
---@generic K,V
---@param inTab table<K,V>
---@param inFunc fun(tKey:K,tValue:V):boolean
------------------------------------------------------------
function table.RemoveAll(inTab, inFunc)
    for k, v in pairs(inTab) do
        if inFunc(k, v) then
            inTab[k] = nil
        end
    end
end

------------------------------------------------------------
---遍历表格，确保其中的值唯一
---@generic V
---@param inTab table
---@param inIsArray boolean 是否转成数组
---@return table<any,V>|V[]
------------------------------------------------------------
function table.Distinct(inTab, inIsArray)
    local tCheckTab = {}
    local tTmpTab = {}
    local idx = 1
    for k, v in pairs(inTab) do
        if not tCheckTab[v] then
            if inIsArray then
                tTmpTab[idx] = v
                idx = idx + 1
            else
                tTmpTab[k] = v
            end
            tCheckTab[v] = true
        end
    end
    return tTmpTab
end

------------------------------------------------------------
---判断一个table 是不是 空的
---@param inTabs table
---@return boolean
------------------------------------------------------------
function table.IsEmpty(inTabs)
    return not next(inTabs)
end

------------------------------------------------------------
---反转table
---@generic V
---@param array V[]
---@return V[]
------------------------------------------------------------
function table.Reverse(array)
    local var = {}
    for i = 1, #array do
        var[i] = table.remove(array)
    end
    return var
end

------------------------------------------------------------
---交换俩个元素
---@param array table
---@param i any
---@param j any
------------------------------------------------------------
function table.Swap(array, i, j)
    if i and j and not table.IsEmpty(array) then
        local tmp = array[i]
        array[i] = array[j]
        array[j] = tmp
    end
end

------------------------------------------------------------
---多条件排序 第一个条件无法判断就判断后一个
---@generic V
---@param inTabs V[]
---@param ... fun(a:V, b:V):number 比较下一个0 ;成立 1;不成立-1
------------------------------------------------------------
function table.Sort(inTabs, ...)
    local tArgs = { ... }
    if table.IsEmpty(tArgs) then
        return
    end
    local inCompareFunc = function(inA, inB)
        for _, tCondition in ipairs(tArgs) do
            local _result = tCondition(inA, inB)
            if _result > 0 then
                return true
            elseif _result < 0 then
                return false
            end
        end
        return false
    end
    table.sort(inTabs, inCompareFunc)
end

------------------------------------------------------------
---转化一个table
---@generic K
---@generic V
---@generic Convert
---@param inTabs table<K,V>
---@param inFunc fun(a:V):Convert
---@param inReturnDic boolean
---@return table<K,Convert>|Convert[]
------------------------------------------------------------
function table.Convert(inTabs, inFunc, inReturnDic)
    local tResultTabs = {}
    for tKey, tValue in pairs(inTabs) do
        local convert = inFunc(tValue)
        if inReturnDic then
            tResultTabs[tKey] = convert
        else
            table.insert(tResultTabs, convert)
        end
    end
    return tResultTabs
end

------------------------------------------------------------
---@generic K
---@generic V
---@param inTabs table<K,V>
------------------------------------------------------------
function table.getMaxIndex(inTabs)
    local tMaxIndex = 0

    for tIndex, _ in pairs(inTabs) do
        if type(tIndex) == "number" and math.floor(tIndex) == tIndex and tIndex > tMaxIndex then
            tMaxIndex = tIndex
        end
    end

    return tMaxIndex
end

------------------------------------------------------------------------------