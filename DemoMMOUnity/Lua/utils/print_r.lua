---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by Administrator.
--- DateTime: 2023/5/20 17:52
---

require "utils.str"



local lua_type = type
local lua_print = print
local tconcat = table.concat
local tinsert = table.insert
local srep = string.rep
local type = type
local tostring = tostring
local next = next
local format = string.format
local isbin = string.isbin

local tapcnt = 0
function global.print_(...)
    if tapcnt == 0 then
        lua_print(...)
    else
        lua_print(srep("| ", tapcnt), ...)
    end
end

function global.print_begin(...)
    lua_print(srep("| ", tapcnt) .. '+------------------------------')
    tapcnt = tapcnt + 1
    print_(...)
end

function global.print_end(...)
    print_(...)
    tapcnt = tapcnt - 1
    lua_print(srep("| ", tapcnt) .. '+------------------------------')
end

function global.print_r(root, lvl, deep)
    lvl = lvl or 2 -- verbose
    if DEBUG < lvl then return end
    local tb = string.split(debug.traceback("", 2), "\n")
    local str = dumps(root, lua_type(root) == "table", deep)
    local str0 = '++++------------------------------'
    lua_print("\n" .. str0 .. "\n" .. "dump from: " .. string.trim(tb[3]) .. "\n" .. str .. "\n" .. str0)
end

function global.print_r_deep(root, deep)
    return print_r(root, 2, deep)
end

function global.print_hex(s)
    local function hexadump(s)
        return (s:gsub('.', function(c) return format('%02X ', c:byte()) end))
    end
    lua_print(hexadump(s))
end

