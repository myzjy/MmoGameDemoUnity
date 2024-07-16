-------------------------------------------------------------------------------
--     common functions       
--  
-- @author zjy
-- @DateTime 2024/7/8 下午11:17
-------------------------------------------------------------------------------
Log = {
    
}
local lua_type = type
local lua_tostring = tostring
local lua_pairs = pairs
local lua_ipairs = ipairs
local format = string.format
local strsub = string.sub
local tonumber = tonumber
FrostLuaLog = CS.FrostEngine.Debug.Log
FrostLuaLogE = CS.FrostEngine.Debug.LogError
FrostLuaLogW = CS.FrostEngine.Debug.LogWarning

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

 Log = {
     
 }

function Log:PrintLog(inTag, inScript, inFmt)
    local t = {
        "[",
        string.upper(lua_tostring(inTag)),
        "] ",
        inScript,
        lua_tostring(inFmt)
    }

    FrostLuaLog(table.concat(t))
end

function Log:PrintLogE(inTag, inScript, inFmt)
    local t = {
        "[",
        string.upper(lua_tostring(inTag)),
        "] ",
        inScript,
        lua_tostring(inFmt)
    }

    FrostLuaLogE(table.concat(t))
end

function Log:PrintLogW(inTag, inScript, inFmt)
    local t = {
        "[",
        string.upper(lua_tostring(inTag)),
        "] ",
        inScript,
        lua_tostring(inFmt)
    }

    FrostLuaLogW(table.concat(t))
end

function FrostLogD(...)
    local tArguments = {...}
    local tFmt = table.concat(tArguments,"\t")
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
    Log:PrintLog("DBG", d, Log:SetLogColor(CONSOLE_COLOR.Light_Blue_Green, tFmt))
end


function FrostLogE(...)
    local tArguments = {...}
    local tFmt = table.concat(tArguments,"\t")
    local d = ""
    if Debug >= 1 then
        local info = debug.getinfo(2, "Sln");
        if info ~= nil and info.short_src ~= nil and info.currentline ~= nil then
            d = info.short_src .. ":" .. info.currentline .. ":"
        end
    end

    Log:PrintLogE("ERR", d, Log:SetLogColor(CONSOLE_COLOR.Light_Yellow, tFmt))
end

function FrostLogW(...)
    if Debug < 1 then
        return
    end
    local tArguments = {...}
    local tFmt = table.concat(tArguments,"\t")
    local d = ""
    if Debug >= 1 then
        local info = debug.getinfo(2, "Sln");
        if info ~= nil and info.short_src ~= nil and info.currentline ~= nil then
            d = info.short_src .. ":" .. info.currentline .. ":"
        end
    end
    Log:PrintLogW("WARN", d, Log:SetLogColor(CONSOLE_COLOR.Light_Yellow, tFmt))
end


function FrostLogI(inFmt, ...)
    if Debug < 1 then
        return
    end
    local tArguments = {...}
    local tFmt = table.concat(tArguments,"\t")
    local d = ""
    if Debug >= 1 then
        local info = debug.getinfo(2, "Sln");
        if info ~= nil and info.short_src ~= nil and info.currentline ~= nil then
            d = info.short_src .. ":" .. info.currentline .. ":"
        end
    end
    Log:PrintLog("INFO", d, Log:SetLogColor(CONSOLE_COLOR.Light_Green, tFmt))
end

function Log:SetLogColor(nColor, inFmt)
    return "<color=" .. nColor .. ">" .. inFmt .. "</color>"
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


------------------------------------------------------------
function _DisableLuaLogInterface(...) 
    local tArguments = { ... }
    
end

print = function(...)  end
error = function(...)  end
warn =  function(...)  end
