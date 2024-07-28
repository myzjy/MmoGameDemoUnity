---@class CS.UnityEngine.UI.Image : CS.UnityEngine.UI.MaskableGraphic
---@field public sprite CS.UnityEngine.Sprite
---@field public overrideSprite CS.UnityEngine.Sprite
---@field public type number
---@field public preserveAspect boolean
---@field public fillCenter boolean
---@field public fillMethod number
---@field public fillAmount number
---@field public fillClockwise boolean
---@field public fillOrigin number
---@field public alphaHitTestMinimumThreshold number
---@field public useSpriteMesh boolean
---@field public defaultETC1GraphicMaterial CS.UnityEngine.Material
---@field public mainTexture CS.UnityEngine.Texture
---@field public hasBorder boolean
---@field public pixelsPerUnit number
---@field public material CS.UnityEngine.Material
---@field public minWidth number
---@field public preferredWidth number
---@field public flexibleWidth number
---@field public minHeight number
---@field public preferredHeight number
---@field public flexibleHeight number
---@field public layoutPriority number
CS.UnityEngine.UI.Image = { }
function CS.UnityEngine.UI.Image:OnBeforeSerialize() end
function CS.UnityEngine.UI.Image:OnAfterDeserialize() end
function CS.UnityEngine.UI.Image:SetNativeSize() end
function CS.UnityEngine.UI.Image:CalculateLayoutInputHorizontal() end
function CS.UnityEngine.UI.Image:CalculateLayoutInputVertical() end
---@return boolean
---@param screenPoint CS.UnityEngine.Vector2
---@param eventCamera CS.UnityEngine.Camera
function CS.UnityEngine.UI.Image:IsRaycastLocationValid(screenPoint, eventCamera) end
return CS.UnityEngine.UI.Image
