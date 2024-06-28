---@class FrostGameObject:FrostObject
---@field uTransform UnityEngine.Transform
---@field uGameObject UnityEngine.GameObject
---@field active boolean
---@field layer number
local FrostGameObject = class("FrostGameObject",FrostObject)

function FrostGameObject:ctor()

end
function FrostGameObject:Init(argument)
    self.uObject = argument.uGameObject
    self.uGameObject = argument.uGameObject
end

function FrostGameObject:GetComponent(componentStr)

end

function FrostGameObject:SetActive(value)
    self.uGameObject:Instantiate()
end


return FrostGameObject