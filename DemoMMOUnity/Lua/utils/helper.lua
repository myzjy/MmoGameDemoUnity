---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by Administrator.
--- DateTime: 2023/5/20 17:54
---

local tinsert = table.insert
local tconcat = table.concat
local srep = string.rep
local type = type
local tostring = tostring
local next = next
local format = string.format

require "utils.str"

local isobjectid = string.isobjectid

function global.dumps(t, tree, deepmax)
    if t == nil then return "nil" end
    deepmax = deepmax or 9999
    local cache = {[t] = "."}
    local function _dump(t,name,deep)
        if type(t) ~= "table" then return tostring(t) end
        local mt = getmetatable(t)
        if deep ~= deepmax and mt and mt.__tostring then
            return tostring(t)
        end
        local temp = {}
        for k,v in pairs(t) do
            local key = tostring(k)
            if isobjectid(key) then
                key = stringz.bintohex(key)
            end
            if cache[v] then
                tinsert(temp,key .. "={" .. cache[v].."}")
            elseif type(v) == "table" then
                local new_key = name .. "." .. key
                cache[v] = new_key
                tinsert(temp,key .. "=".. _dump(v,new_key,deep-1))
            elseif type(v) == "string" and isobjectid(v) then
                tinsert(temp,key .. "=".. stringz.bintohex(v))
            else
                tinsert(temp,key .. "=".. tostring(v))
            end
        end
        return "{" .. tconcat(temp,", ") .. "}"
    end
    local function _tree_dump(t,space,name,deep)
        local temp = {}
        if deep <= 0 then
            return ' ... [table]'
        end
        for k,v in pairs(t) do
            local key = tostring(k)
            if isobjectid(key) then
                key = "objectid(" .. stringz.bintohex(key) .. ")"
            end
            if cache[v] then
                tinsert(temp,"+" .. key .. " {" .. cache[v].."}")
            elseif type(v) == "table" then
                local new_key = name .. "." .. key
                cache[v] = new_key
                tinsert(temp,"+" .. key .. _tree_dump(v,space .. (next(t,k) and "|" or " " ).. srep(" ",#key),new_key,deep-1))
            elseif type(v) == "function" then
                tinsert(temp,"+" .. key .. " [".. tostring(v).."]")
            elseif type(v) == "string" and isobjectid(v) then
                tinsert(temp,"+" .. key .. " [".. "objectid" .. " " .. stringz.bintohex(v).."]")
            else
                tinsert(temp,"+" .. key .. " [".. type(v) .. " " .. tostring(v).."]")
            end
        end
        return tconcat(temp,"\n"..space)
    end
    return tree and _tree_dump(t, "", "", deepmax) or _dump(t, "", deepmax)
end

-- 判断图片是否存在，不要用cc.FileUtils:getInstance():isFileExist
-- png优先找pvr.ccz
function global.isImageExist(path)
    return nil ~= display.textureCache:checkFullPath(path)
end