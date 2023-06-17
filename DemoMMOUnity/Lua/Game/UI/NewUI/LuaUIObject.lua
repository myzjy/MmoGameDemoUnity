---@class LuaUIObject
LuaUIObject = class("LuaUIObject")

---@param Panel {gameObject:UnityEngine.GameObject}
function LuaUIObject:SetPanel(Panel)
    self.Panel = Panel
    self.gameObject = Panel.gameObject
    self.transform = Panel.gameObject.transform
    self.InstanceID = self.gameObject:GetInstanceID()

end

---@param value boolean 设置
function LuaUIObject:SetActive(value)
    self.GameObject:SetActive(value)
end
---@param type string 类型
function LuaUIObject:GetComponent(type)
    local component = self.transform:GetComponent(type)
    return component
end

---@param btn UnityEngine.UI.Button
---@param func function
function LuaUIObject:SetListener(btn, func)
    CS.Util.SetListener(btn, func)
end

---@param text string
---@param textUI UnityEngine.UI.Text
function LuaUIObject:SetText(textUI, text)
    textUI.text = text
end

---@param image UnityEngine.UI.Image
---@param sprite UnityEngine.Sprite
function LuaUIObject:SetSprite(image, sprite)
    image.sprite = sprite
end 