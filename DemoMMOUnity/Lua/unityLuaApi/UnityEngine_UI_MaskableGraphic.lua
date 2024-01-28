---@class UnityEngine.UI.MaskableGraphic : UnityEngine.UI.Graphic
---@field public onCullStateChanged UnityEngine.UI.MaskableGraphic.CullStateChangedEvent
---@field public maskable boolean

---@type UnityEngine.UI.MaskableGraphic
UnityEngine.UI.MaskableGraphic = { }
---@return UnityEngine.Material
---@param baseMaterial UnityEngine.Material
function UnityEngine.UI.MaskableGraphic:GetModifiedMaterial(baseMaterial) end
---@param clipRect UnityEngine.Rect
---@param validRect boolean
function UnityEngine.UI.MaskableGraphic:Cull(clipRect, validRect) end
---@param clipRect UnityEngine.Rect
---@param validRect boolean
function UnityEngine.UI.MaskableGraphic:SetClipRect(clipRect, validRect) end
function UnityEngine.UI.MaskableGraphic:RecalculateClipping() end
function UnityEngine.UI.MaskableGraphic:RecalculateMasking() end
return UnityEngine.UI.MaskableGraphic
