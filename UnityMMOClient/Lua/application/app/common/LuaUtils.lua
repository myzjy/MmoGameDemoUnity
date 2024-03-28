LuaUtils = {}

--------------------------------------------------------------------------------------------
---```
--- 进入 从 GameObject上获取 相关组件事件
---```


function LuaUtils.GetUISerializableKeyObject(gameObject)
    local sKeyObj = gameObject:GetComponent("UISerializableKeyObject") or
        ZJYFrameWork.UISerializable.UISerializableKeyObject
    return sKeyObj
end

---获取 Image 组件
---@param gameObject UnityEngine.GameObject
---@return UnityEngine.UI.Image
function LuaUtils.GetUIImage(gameObject)
    ---@type UnityEngine.UI.Image
    local sKeyObj = gameObject:GetComponent("Image") or UnityEngine.UI.Image
    return sKeyObj
end
function LuaUtils.GetUIText(gameObject)
    local sKeyObj = gameObject:GetComponent("Text") or UnityEngine.UI.Text
    return sKeyObj
end

---@param gameObject UnityEngine.GameObject
---@return UnityEngine.UI.Button
function LuaUtils.GetUIButton(gameObject)
    ---@type UnityEngine.UI.Button
    local sKeyObj = gameObject:GetComponent("Button") or UnityEngine.UI.Button
    
    return sKeyObj
end
--- 获取
---@param gameObject UnityEngine.GameObject
---@return UnityEngine.CanvasGroup
function LuaUtils.GetUICanvaGroup(gameObject)
    local sKeyObj = gameObject:GetComponent("CanvasGroup") or  UnityEngine.CanvasGroup
    return sKeyObj
end

--------------------------------------------------------------------------------------------


--------------------------------------------------------------------------------------------
---```
--- 进入 从 UISerializableKeyObject 组件 上获取 相关组件事件
---```

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

function LuaUtils.GetKeyUICanvasGroup(gameObject, objName)
    local sKeyObj = LuaUtils.GetUISerializableKeyObject(gameObject)
    local canvasGroup = sKeyObj:GetObjTypeStr(objName) or UnityEngine.CanvasGroup
    return canvasGroup
end

function LuaUtils.GetKeyUIImage(gameObject, objName)
    local sKeyObj = LuaUtils.GetUISerializableKeyObject(gameObject)
    local image = sKeyObj:GetObjTypeStr(objName) or UnityEngine.UI.Image
    return image
end

---@param gameObject UnityEngine.GameObject
---@param objName string
---@return UnityEngine.UI.Text
function LuaUtils.GetKeyUIText(gameObject,objName)
    local sKeyObj = LuaUtils.GetUISerializableKeyObject(gameObject)
    local __text= sKeyObj:GetObjTypeStr(objName) or UnityEngine.UI.Text
    return __text
end
function LuaUtils.GetKeyUITextGameObject(gameObject,objName)
    local obj = LuaUtils.GetKeyGameObject(gameObject, objName)
    local __uiText = LuaUtils.GetUIText(obj)
    return __uiText
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

---@param gameObejct UnityEngine.GameObject
---@param objName string
---@return UnityEngine.UI.Button
function LuaUtils.GetKeyButtonGameObject(gameObejct,objName)
    local obj=LuaUtils.GetKeyGameObject(gameObejct,objName)
    local __uiButton = LuaUtils.GetUIButton(obj)
    return __uiButton
end
function LuaUtils.GetKeyCanvaGroupGameObject(gameObejct,objName)
    local obj=LuaUtils.GetKeyGameObject(gameObejct,objName)
    local __uiButton = LuaUtils.GetUICanvaGroup(obj)
    return __uiButton
end