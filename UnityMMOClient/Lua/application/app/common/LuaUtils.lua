LuaUtils = {}


---@param gameObject UnityEngine.GameObject
---@param objName any
function LuaUtils.GetUIGrid(gameObject, objName)
    local sKeyObj = gameObject:GetComponent("UISerializableKeyObject") or
        ZJYFrameWork.UISerializable.UISerializableKeyObject
    local obj = sKeyObj:GetObjTypeStr(objName) or UnityEngine.UI.GridLayoutGroup
    return obj
end

function LuaUtils.GetGameObject(gameObject, objName)
    local sKeyObj = gameObject:GetComponent("UISerializableKeyObject") or
        ZJYFrameWork.UISerializable.UISerializableKeyObject
    local obj = sKeyObj:GetObjTypeStr(objName) or UnityEngine.GameObject
    return obj
end

function LuaUtils:Set()
    
end
