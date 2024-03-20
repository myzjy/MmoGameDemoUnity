LuaUtils = {}
function LuaUtils.GetUISerializableKeyObject(gameObject)
    local sKeyObj = gameObject:GetComponent("UISerializableKeyObject") or
        ZJYFrameWork.UISerializable.UISerializableKeyObject
    return sKeyObj
end

---获取 Image 组件
---@param gameObject UnityEngine.GameObject
---@return UnityEngine.UI.Image
function LuaUtils.GetUIImage(gameObject)
    local sKeyObj = gameObject:GetComponent("Image") or UnityEngine.UI.Image
    return sKeyObj
end

---@param gameObject UnityEngine.GameObject
function LuaUtils.GetUIButton(gameObject)
    local sKeyObj = gameObject:GetComponent("Button") or UnityEngine.UI.Button
    return sKeyObj
end

--- 获取 UISerializableKeyObject 组件 之后 更具 objName 属性 获取 Grid 组件
---@param gameObject UnityEngine.GameObject
---@param objName any
---@return UnityEngine.UI.GridLayoutGroup
function LuaUtils.GetKeyUIGrid(gameObject, objName)
    local sKeyObj = LuaUtils.GetUISerializableKeyObject(gameObject)
    local obj = sKeyObj:GetObjTypeStr(objName) or UnityEngine.UI.GridLayoutGroup
    return obj
end

function LuaUtils.GetKeyGameObject(gameObject, objName)
    local sKeyObj = LuaUtils.GetUISerializableKeyObject(gameObject)
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

function LuaUtils.GetUICanvaGroup(gameObject, objName)
    local sKeyObj = LuaUtils.GetUISerializableKeyObject(gameObject)
    local canvasGroup = sKeyObj:GetObjTypeStr(objName) or UnityEngine.CanvasGroup
    return canvasGroup
end

function LuaUtils.GetKeyUIImage(gameObject, objeName)
    local sKeyObj = LuaUtils.GetUISerializableKeyObject(gameObject)
    local image = sKeyObj:GetObjTypeStr(objeName) or UnityEngine.UI.Image
    return image
end

function LuaUtils.Get()

end

--- 根据 名字获取 gameObject 上面 Image 组件
---@param gameObject UnityEngine.GameObject
---@param objName string
---@return UnityEngine.UI.Image
function LuaUtils.GetKeyImageGameOject(gameObject, objName)
    local obj = LuaUtils.GetKeyGameObject(gameObject, objName)
    local __uiImage = LuaUtils.GetUIImage(obj)
    return __uiImage
end
