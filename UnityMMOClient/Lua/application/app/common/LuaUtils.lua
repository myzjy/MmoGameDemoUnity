LuaUtils = {}


---@param gameObject UnityEngine.GameObject
---@param objName any
function LuaUtils.GetKeyUIGrid(gameObject, objName)
    local sKeyObj = gameObject:GetComponent("UISerializableKeyObject") or
        ZJYFrameWork.UISerializable.UISerializableKeyObject
    local obj = sKeyObj:GetObjTypeStr(objName) or UnityEngine.UI.GridLayoutGroup
    return obj
end

function LuaUtils.GetKeyGameObject(gameObject, objName)
    local sKeyObj = gameObject:GetComponent("UISerializableKeyObject") or
        ZJYFrameWork.UISerializable.UISerializableKeyObject
    local obj = sKeyObj:GetObjTypeStr(objName) or UnityEngine.GameObject
    return obj
end

function LuaUtils.SetActive(item, value)
    item:SetActive(value)
end

---@param gameObject UnityEngine.GameObject
---@param vec UnityEngine.Vector3
function LuaUtils.SetScale(gameObject, vec)
    gameObject.transform.localScale = vec
end
