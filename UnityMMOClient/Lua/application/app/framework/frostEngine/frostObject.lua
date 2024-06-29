--- 对应 Unity 侧 object 
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by Administrator.
--- DateTime: 2024/6/28 下午9:56
---

---@class FrostObject
---@field uObject UnityEngine.Object unity 侧Object
local FrostObject = class("FrostObject")

function FrostObject:ctor()
    --- 名， lua 侧有一个className 
    ---@type string
    self.name = false
end

--- 初始化
function FrostObject:Init(argument)
    self.uObject = argument.uObject
    self.name = self.uObject.name
end

----------------------------------------------------
--- @public
--- 当用于调用原点对象时，此方法返回一个正值。当用于调用实例对象时，此方法返回一个负值
---@return number 返回对象的实例ID。
----------------------------------------------------
function FrostObject:GetInstanceID()
    if not self.uObject then
        return -1
    end
    return self.uObject:GetInstanceID()
end

-----------------------------------------------------
--- 可以重写
--- @return boolean 对比值
-----------------------------------------------------
function FrostObject:vEquals(other)
    local rhs = other.uObject
    return self.uObject:Equals(rhs)
end

---@param original UnityEngine.Object
---@param position UnityEngine.Vector3
---@param rotation UnityEngine.Quaternion
---@overload fun():FrostObject
---@overload fun(original:UnityEngine.Object)
---@return FrostObject
function FrostObject:Instantiate(original, position, rotation)
    local uObject = UnityEngine.Object.Instantiate(original, position, rotation)
    local object = FrostObject()
    object:Init({
        uObject = uObject
    })
    return object
end

function FrostObject:InstantiateGameObject(gameObject)
    local uGameObject = LuaUtils.InstantiateGameObject(gameObject)
    local tGameObject = FrostGameObject()
    tGameObject:Init({
        uGameObject = uGameObject
    })

end

function FrostObject:vGetType()
    return self.uObject:GetType()
end

return FrostObject