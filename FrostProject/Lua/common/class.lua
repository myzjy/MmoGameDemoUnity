--[[
   author:   zhangjingyi.
   DateTime: 2024/7/9 下午2:37
--]]

null = nil
nullTbl = {}
CS = CS
TypeOf = CS.typeof

--- @class LuaDataType Lua数据类型枚举
LuaDataType = {
    Number = "number",
    String = "string",
    Nil = "nil",
    Booean = "boolean",
    Function = "function",
    Table = "table",
    UserData = "userdata",
    Thread = "thread",
}

---@class UnitType 元对象类型枚举
UnitType = {
    Unit = "Unit",
    Type = "Type",
    Instance = "Instance",
}

---@class ClassType class类型
ClassType = {
    Lua = 0,              ---纯Lua
    CreateFirst = 1,      --需要先调用__createfirst创建实例对象
    ExtendCSInstance = 2, --扩展CSharp实例
}

---@class Unit 元
---@field __unitType UnitType|string 元类型
---@field __super Unit|nil 基类型                       __unitType = UnitType.Type 存在
---@field __classname string 类型名称                   __unitType = UnitType.Type 存在
---@field __classType ClassType|number 类型种类         __unitType = UnitType.Type 存在
---@field __firstCreate function|Unit|nil 创建CS实例    __classType = ClassType.CreateFirst &&  __unitType = UnitType.Type存在
---@field ctor function|nil 构造函数 自动调用
---@field __type Unit 所属类型 ，是一个 {}                __unitType == UnitType.Instance 存在
---@field __object any CS 对象                          __classType = ClassType.ExtendCSInstance|ClassType.CreateFirst存在
---@field IsSubClassOf function|nil
Unit = {
    __unitType = UnitType.Unit,
    __classname = "Unit",
    __super = nil,
    __classType = nil,
    __type = nil,
    __object = nil,
    __firstCreate = nil,
    ctor = nil,
    IsSubClassOf = nil,
}

-----------------------------------------------------------
---@param inSuper string|Unit 名字或者一张由 class 创建出的表
---@return boolean  是否为父类
-----------------------------------------------------------
function Unit:IsSubClassOf(inSuper)
    --- @type Unit
    local tType
    --- @type string|Unit
    local tClassName
    tType = self.__unitType == UnitType.Instance and self.__type or self
    tClassName = type(inSuper) == LuaDataType.String and inSuper or inSuper.__classname
    local tmp = tType
    while tmp ~= nil do
        if tmp.__classname == tClassName then
            return true
        end
        tmp = tmp.__super
    end
    return false
end

--------------------------------------------
--- 将 父类 中的 所有父类 查询后之后的方法被替换 copy
--- @param inInstance Unit 生成的class
--- @param inSuperType Unit class
--- @return Unit copy的 class
--------------------------------------------
local function CopySuper(inInstance, inSuperType)
    local tmp = inSuperType
    local supers = {}
    while tmp ~= nil do
        table.insert(supers, 1, tmp)
        tmp = tmp.__super
    end
    for _, superItem in pairs(supers) do
        for k, v in pairs(superItem) do
            if k ~= "__firstCreate" and k ~= "__unitType" then
                inInstance[k] = v
            end
        end
    end
    supers = nil
    return inInstance
end

----------------------------------------------
--- 执行 CS 生成
---@param inInstance Unit 元函数
----------------------------------------------
local function ExtendCSInstance(inInstance)
    inInstance.__unitType = UnitType.Instance
    local tMeta = {}
    tMeta.__call = function(_, ...)
        error(_.__classname .. " is a instance extend from cs instance ")
    end
    tMeta.__index = function(_t, k)
        local tSelfField = rawget(_t, k)
        if tSelfField then
            return tSelfField
        else
            local tFromCs = _t.__object[k]
            if type(tFromCs) == LuaDataType.Function then
                return function(...)
                    local args = { ... }
                    if not table.IsEmpty(args) then
                        table.remove(args, 1)
                    end
                    tFromCs(_t.__object, table.unpack(args))
                end
            else
                return tFromCs
            end
        end
    end
    tMeta.__newindex = function(_t, k, v)
        local tValueType = type(v)
        if tValueType == LuaDataType.Function then
            rawset(_t, k, v)
        else
            if _t.__object[k] then
                _t.__object[k] = v
            else
                rawset(_t, k, v)
            end
        end
    end
    setmetatable(inInstance, tMeta)
    return inInstance
end

-----------------------------------------------------------
--- 执行 初始化函数时调用
--- @param inInstance Unit
--- @param inType Unit
--- @param ... any
--- @return Unit
-----------------------------------------------------------
local function CallCtor(inInstance, inType, ...)
    local ctorTable = {}
    local tmp = inType
    while tmp ~= nil do
        table.insert(ctorTable, 1, tmp)
        tmp = tmp.__super
    end
    for _, v in pairs(ctorTable) do
        local ctor = rawget(v, "ctor")
        if ctor then
            ctor(inInstance, ...)
        end
    end
    ctorTable = nil
    return inInstance
end
local function OnUserFirstExCSCallCtor(inType, ...)
    local tInstance = {}
    tInstance.__type = inType
    tInstance.__object = inType.__firstCreate(...)
    return CallCtor(CopySuper(ExtendCSInstance(tInstance), inType), inType, ...)
end

local function OnFirstExCSCallCtor(inType, ...)
    local tInstance = {}
    tInstance.__unitType = UnitType.Instance
    tInstance.__type = inType
    local tInstanceMeta = {}
    tInstanceMeta.__index = inType
    tInstanceMeta.__call = function(_, ...)
        FrostLogE( _.__classname, "this is a Instance of " , _.__classname)
    end
    setmetatable(tInstance, tInstanceMeta)
    return CallCtor(tInstance, inType, ...)
end

---创建一个类
---@generic T:Unit
---@param inClassName string  类名
---@param inSuper any|nil
---@return T
Class = function(inClassName, inSuper)
    assert(type(inClassName) == LuaDataType.String and #inClassName > 0)
    ---@type Unit
    local tUnitType = {
        __unitType = UnitType.Unit,
        __classname = "Unit",
        __super = nil,
        __classType = nil,
        __type = nil,
        __object = nil,
        __firstCreate = nil,
        ctor = nil,
        IsSubClassOf = nil,
    }
    local tSuperType = type(inSuper)
    local tIsCSType = inSuper and tSuperType == LuaDataType.Table and typeof(inSuper)      --判断是否是C#类
    local tIsCSInstance = inSuper and tSuperType == LuaDataType.UserData                 --判断是否为C#实例
    local tIsExCSInsAgain = inSuper and inSuper.__classType == ClassType.ExtendCSInstance --再次扩展C#实例
    if tIsExCSInsAgain then
        FrostLogE('cannot extends a c# instance multiple times.')
    end
    local tIsFirstExCSType = tIsCSType and inSuper and (not inSuper.__classType) or tSuperType == LuaDataType.Function                       --首次继承C#类
    local tIsExCSTypeAgain = inSuper and inSuper.__classType == ClassType.CreateFirst --再次扩展C#类
    tUnitType.__classname = inClassName
    tUnitType.__type = Unit
    tUnitType.__unitType = tIsCSInstance and UnitType.Instance or UnitType.Type
    tUnitType.__object = tIsCSInstance and inSuper or nil
    tUnitType.ctor = tUnitType.ctor or function(...)
    end
    tUnitType.__classType = (tIsCSInstance and ClassType.ExtendCSInstance)
            or ((tIsFirstExCSType or tIsExCSTypeAgain) and ClassType.CreateFirst) or ClassType.Lua
    tUnitType.__super = ((tIsCSInstance or tIsFirstExCSType) and Unit) or (tIsExCSTypeAgain and inSuper) or (inSuper == nil and Unit or inSuper)
    tUnitType.__firstCreate = (tIsExCSTypeAgain and inSuper and inSuper.__firstCreate)
            or ((tIsCSType and inSuper and not inSuper.__classType)
            and function(...) return inSuper(...) end)
            or((tSuperType == LuaDataType.Function) and inSuper) or nil
    if tIsCSInstance then
        return ExtendCSInstance(tUnitType)
    end
    local tMeta = {}
    tMeta.__index = inSuper == nil and Unit or inSuper
    local __call
    if tIsFirstExCSType or tIsExCSTypeAgain then
        __call = OnUserFirstExCSCallCtor
    else
        __call = OnFirstExCSCallCtor
    end
    tMeta.__call = __call
    setmetatable(tUnitType, tMeta)
    -- 注册 全局变量
    ClassLibraryMap[inClassName] = tUnitType
    return tUnitType
end