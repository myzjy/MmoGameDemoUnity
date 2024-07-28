--[[
-- added by wsh @ 2017-12-18
-- string扩展工具类，对string不支持的功能执行扩展
--]]



-- 字符串分割
-- @split_string：被分割的字符串
-- @pattern：分隔符，可以为模式匹配
-- @init：起始位置
-- @plain：为true禁用pattern模式匹配；为false则开启模式匹配
local function split(split_string, pattern, search_pos_begin, plain)
    assert(type(split_string) == "string")
    assert(type(pattern) == "string" and #pattern > 0)
    search_pos_begin = search_pos_begin or 1
    plain = plain or true
    local split_result = {}

    while true do
        local find_pos_begin, find_pos_end = string.find(split_string, pattern, search_pos_begin, plain)
        if not find_pos_begin then
            break
        end
        local cur_str = ""
        if find_pos_begin > search_pos_begin then
            cur_str = string.sub(split_string, search_pos_begin, find_pos_begin - 1)
        end
        split_result[#split_result + 1] = cur_str
        search_pos_begin = find_pos_end + 1
    end

    if search_pos_begin < string.len(split_string) then
        split_result[#split_result + 1] = string.sub(split_string, search_pos_begin)
    else
        split_result[#split_result + 1] = ""
    end

    return split_result
end

-- 字符串连接
local function join(join_table, joiner)
    if #join_table == 0 then
        return ""
    end

    local fmt = "%s"
    for i = 2, #join_table do
        fmt = fmt .. joiner .. "%s"
    end

    return string.format(fmt, table.unpack(join_table))
end

-- 是否包含
-- 注意：plain为true时，关闭模式匹配机制，此时函数仅做直接的 “查找子串”的操作
local function contains(target_string, pattern, plain)
    plain = plain or true
    local find_pos_begin, find_pos_end = string.find(target_string, pattern, 1, plain)
    return find_pos_begin ~= nil
end

-- 以某个字符串开始
local function startswith(target_string, start_pattern, plain)
    plain = plain or true
    local find_pos_begin, find_pos_end = string.find(target_string, start_pattern, 1, plain)
    return find_pos_begin == 1
end

-- 以某个字符串结尾
local function endswith(target_string, start_pattern, plain)
    plain = plain or true
    local find_pos_begin, find_pos_end = string.find(target_string, start_pattern, - #start_pattern, plain)
    return find_pos_end == #target_string
end
string.empty = ""
-------------------------------------------------------------------------------------------
-- 返回字符串是否为空
-- @param source(string) 需要检查的字符串
-- @return 如果字符串为 nil 或者 长度为 0， 返回true
-------------------------------------------------------------------------------------------
function string.IsNullOrEmpty(source)
    return not source or string.len(source) == 0
end

---去除头部空格
---@param  source string
---@return string
function string.TrimHead(source)
    local gs = string.gsub(source, "[ \t\n\r]+$", string.empty)
    return gs
end

--- 去除输入字符串尾部的空白字符，返回结果
---@param  source string
---@return string
function string.TrimTail(source)
    local gs = string.gsub(source, "[ \t\n\r]+$", string.empty)
    return gs
end

--- 去除输入字符串 尾部+头部 的空白字符，返回结果
---@param  source string
---@return string
function string.Trim(source)
    source = string.TrimHead(source)
    return string.TrimTail(source)
end

--- 将字符串的第一个字符转为大写，返回结果
---@param  source string
---@return string
function string.UpperFirst(source)
    return string.upper(string.sub(source, 1, 1)) .. string.sub(source, 2)
end

---用指定字符或字符串分割输入字符串，返回包含分割结果的数组
---@param source string 源字符串
---@param delimiter string 分割符
---@return string[]|boolean
function string.Split(source, delimiter)
    source    = tostring(source)
    delimiter = tostring(delimiter)
    if string.IsNullOrEmpty(delimiter) then return false end
    local pos, arr = 0, {}
    -- for each divider found
    for st, sp in function()
        return string.find(source, delimiter, pos, true)
    end do
        table.insert(arr, string.sub(source, pos, st - 1))
        pos = sp + 1
    end
    table.insert(arr, string.sub(source, pos))
    return arr
end

-- 计算 UTF8 字符串的长度，每一个中文算一个字符
-- local input = "你好World"
-- string.UTF8Length(input)
-- 输出 7
---@param  source string
---@return number
function string.UTF8Length(source)
    local len  = string.len(source)
    local left = len
    local cnt  = 0
    local arr  = { 0, 0xc0, 0xe0, 0xf0, 0xf8, 0xfc }
    while left ~= 0 do
        local tmp = string.byte(source, -left)
        local i = #arr
        while arr[i] do
            if tmp >= arr[i] then
                left = left - i
                break
            end
            i = i - 1
        end
        cnt = cnt + 1
    end
    return cnt
end

------------------------------------------------------------------------------

local format = string.format
local find = string.find
local sub = string.sub
local concat = table.concat
local remove = table.remove

-- 首字母大写
function string.caption(s)
    return string.upper(sub(s, 1, 1)) .. sub(s, 2)
end

-- 扩展 string.format, 第一个参数是 table 进行对应值传递 ('{month}月{day}日', {day=1, month=2})
function string.formatex(str, ...)
    local args = {...}
    if type(args[1]) == "table" then
        local t = clone(args[1])
        for k, v in pairs(t) do
            -- 转译 % 内容为显示项
            t[k] = string.gsub(v, "%%", "%%%%")
        end
        local s = {}
        local p = find(str, "{")
        local q = find(str, "}", p)
        while p and q do
            local key = sub(str, p+1, q-1)
            if t[key] then
                s[#s + 1] = sub(str, 1, p-1)
                s[#s + 1] = t[key]
            else
                s[#s + 1] = sub(str, 1, q)
            end
            str = sub(str, q+1)
            p = find(str, "{")
            q = find(str, "}", p)
        end
        s[#s + 1] = str
        str = concat(s)
        remove(args, 1)
    end
    return format(str, unpack(args))
end

-- 限制字数，多余裁剪
-- @param isWord 默认按字符数限制，true按字设置
function string.utf8limit(input, length, isWord)
    local idx, pos, total = 1, 0, 0
    while idx <= #input do
        local curByte = string.byte(input, idx)
        local num = string.utf8charlen(curByte)
        total = total + (isWord and 1 or num)
        if total > length then
            return string.sub(input, 1, idx-1), total
        end
        idx = idx + num
    end
    return input, total
end

local first = {
    {{0x00, 0x7F}, 0},
    {{0xC0, 0xDF}, 1},
    {{0xE0, 0xEF}, 2},
    {{0xF0, 0xF7}, 3},
}

function string.isbin(s)
    local remain = 0
    for i = 1, #s do
        local b = s:byte(i)
        if remain == 0 then
            local flag = false
            for _, t in ipairs(first) do
                if t[1][1] <= b and b <= t[1][2] then
                    remain = t[2]
                    flag = true
                    break
                end
            end
            if not flag then
                return true
            end
        else
            if b < 0x80 or b > 0xBF then
                return true
            end
            remain = remain - 1
        end
    end
    if remain == 0 then
        return false
    end
    return true
end

local isbin = string.isbin
function string.isobjectid(s)
    return #s == 12 and isbin(s)
end

string.split = split
string.join = join
string.contains = contains
string.startswith = startswith
string.endswith = endswith
