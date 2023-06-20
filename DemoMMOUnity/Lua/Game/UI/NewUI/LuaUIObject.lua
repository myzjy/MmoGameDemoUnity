---@class LuaUIObject
LuaUIObject = class("LuaUIObject")

---@param Panel 
function LuaUIObject:SetPanel(Panel)
    self.Panel = Panel
    self.gameObject = Panel.gameObject
    self.transform = Panel.gameObject.transform
    self.InstanceID = self.gameObject:GetInstanceID()
    self.UIView=self:GetComponent("UIView")
    
end

---@param value boolean 设置
function LuaUIObject:SetActive(value)
    self.GameObject:SetActive(value)
end

---@param type string 类型
function LuaUIObject:GetComponent(type)
    --local component = self.transform:GetComponent(type)
    return self.transform:GetComponent(type)
end

---@param btn 
---@param func function
function LuaUIObject:SetListener(btn, func)
    CS.Util.SetListener(btn, func)
end

---@param text string
---@param textUI 
function LuaUIObject:SetText(textUI, text)
    textUI.text = text
end

---@param image 
---@param sprite 
function LuaUIObject:SetSprite(image, sprite)
    image.sprite = sprite
end

