-------------------------------------------------------------------------------
--     common functions       
--  
-- @author zjy
-- @DateTime 2024/7/8 下午11:17
-------------------------------------------------------------------------------

local lua_type = type
local lua_tostring = tostring
local lua_pairs = pairs
local lua_ipairs = ipairs
local format = string.format
local strsub = string.sub
local tonumber = tonumber
FrostLog = CS.FrostEngine.Debug.Log
FrostLogE = CS.FrostEngine.Debug.LogError
FrostLogW = CS.FrostEngine.Debug.LogWarning

CONSOLE_COLOR = {
    Dark_black = "#000000",
    Dark_Blue = "#00008b",
    Dark_Green = "#013220",
    Dark_Blue_Green = "#0d98ba",
    Dark_Red = "#ff0000",
    Dark_Purple = "#800080",
    Dark_Yellow = "#ffff00",
    Default = "#000000",
    Light_Black = "#000000",
    Light_Blue = "#00008b",
    Light_Green = "#013220",
    Light_Blue_Green = "#0d98ba",
    Light_Red = "#ff0000",
    Light_Purple = "#800080",
    Light_Yellow = "#ffff00",
    Light_White = "#ffffff"
}

function PrintLog(tag, script, fmt, ...)
    local t = {
        "[",
        string.upper(lua_tostring(tag)),
        "] ",
        script,
        format(lua_tostring(fmt), ...)
    }

    FrostLog(table.concat(t))
end

function PrintLogE(tag, script, fmt, ...)
    local t = {
        "[",
        string.upper(lua_tostring(tag)),
        "] ",
        script,
        format(lua_tostring(fmt), ...)
    }

    FrostLogE(table.concat(t))
end

function PrintLogW(tag, script, fmt, ...)
    local t = {
        "[",
        string.upper(lua_tostring(tag)),
        "] ",
        script,
        format(lua_tostring(fmt), ...)
    }

    FrostLogW(table.concat(t))
end

function PrintError(fmt, ...)
    local d = ""
    if Debug >= 1 then
        local info = debug.getinfo(2, "Sln");
        if info ~= nil and info.short_src ~= nil and info.currentline ~= nil then
            d = info.short_src .. ":" .. info.currentline .. ":"
        end
    end
  
    PrintLogE("ERR", d, SetLogColor(CONSOLE_COLOR.Light_Yellow, fmt), ...)
end

function PrintWarn(fmt, ...)
    if Debug < 1 then
        return
    end
    local d = ""
    if Debug >= 1 then
        local info = debug.getinfo(2, "Sln");
        if info ~= nil and info.short_src ~= nil and info.currentline ~= nil then
            d = info.short_src .. ":" .. info.currentline .. ":"
        end
    end
    PrintLogW("WARN", d, SetLogColor(CONSOLE_COLOR.Light_Yellow, fmt), ...)
end

function PrintWarnStack(fmt, ...)
    if Debug < 1 then
        return
    end
    local d = ""
    if Debug >= 1 then
        local info = debug.getinfo(2, "Sln");
        if info ~= nil and info.short_src ~= nil and info.currentline ~= nil then
            d = info.short_src .. ":" .. info.currentline .. ":"
        end
    end
    PrintLog("WARN", d, SetLogColor(CONSOLE_COLOR.Light_Yellow, fmt), ...)
    print(debug.traceback("", 2))
end

function PrintInfo(fmt, ...)
    if Debug < 1 then
        return
    end
    local d = ""
    if Debug >= 1 then
        local info = debug.getinfo(2, "Sln");
        if info ~= nil and info.short_src ~= nil and info.currentline ~= nil then
            d = info.short_src .. ":" .. info.currentline .. ":"
        end
    end
    PrintLog("INFO", d, SetLogColor(CONSOLE_COLOR.Light_Green, fmt), ...)
end

function PrintDebug(fmt, ...)
    local d = ""
    if Debug < 1 then
        return
    end
    if Debug >= 1 then
        local info = debug.getinfo(2, "Sln");
        if info ~= nil and info.short_src ~= nil and info.currentline ~= nil then
            d = info.short_src .. ":" .. info.currentline .. ":"
        end
    end
    PrintLog("DBG", d, SetLogColor(CONSOLE_COLOR.Light_Blue_Green, fmt), ...)
end

function SetLogColor(nColor, fmt)
    return "<color=" .. nColor .. ">" .. fmt .. "</color>"
end

function Handle(method, obj, ...)
    local args = { ... }
    local newHandler = function(...)
        local argsInvoke = { ... };
        for __idx = 1, #args do
            table.insert(argsInvoke, args[__idx]);
        end
        if obj then
            return method(obj, table.unpack(argsInvoke));
        else
            return method(table.unpack(argsInvoke));
        end
    end
    return newHandler
end
