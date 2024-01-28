---@class UnityEngine.UI.Image : UnityEngine.UI.MaskableGraphic
---@field public sprite UnityEngine.Sprite
---@field public overrideSprite UnityEngine.Sprite
---@field public type number
---@field public preserveAspect boolean
---@field public fillCenter boolean
---@field public fillMethod number
---@field public fillAmount number
---@field public fillClockwise boolean
---@field public fillOrigin number
---@field public alphaHitTestMinimumThreshold number
---@field public useSpriteMesh boolean
---@field public defaultETC1GraphicMaterial UnityEngine.Material
---@field public mainTexture UnityEngine.Texture
---@field public hasBorder boolean
---@field public pixelsPerUnit number
---@field public material UnityEngine.Material
---@field public minWidth number
---@field public preferredWidth number
---@field public flexibleWidth number
---@field public minHeight number
---@field public preferredHeight number
---@field public flexibleHeight number
---@field public layoutPriority number

---@type UnityEngine.UI.Image
UnityEngine.UI.Image = { }
function UnityEngine.UI.Image:OnBeforeSerialize() end
function UnityEngine.UI.Image:OnAfterDeserialize() end
function UnityEngine.UI.Image:SetNativeSize() end
function UnityEngine.UI.Image:CalculateLayoutInputHorizontal() end
function UnityEngine.UI.Image:CalculateLayoutInputVertical() end
---@return boolean
---@param screenPoint UnityEngine.Vector2
---@param eventCamera UnityEngine.Camera
function UnityEngine.UI.Image:IsRaycastLocationValid(screenPoint, eventCamera) end
return UnityEngine.UI.Image
