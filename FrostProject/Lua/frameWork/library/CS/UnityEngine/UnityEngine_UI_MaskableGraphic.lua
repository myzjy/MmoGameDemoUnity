---@class CS.UnityEngine.UI.MaskableGraphic : CS.UnityEngine.UI.Graphic
---@field public onCullStateChanged CS.UnityEngine.UI.MaskableGraphic.CullStateChangedEvent
---@field public maskable boolean
CS.UnityEngine.UI.MaskableGraphic = { }
---@return CS.UnityEngine.Material
---@param baseMaterial CS.UnityEngine.Material
function CS.UnityEngine.UI.MaskableGraphic:GetModifiedMaterial(baseMaterial) end
---@param clipRect CS.UnityEngine.Rect
---@param validRect boolean
function CS.UnityEngine.UI.MaskableGraphic:Cull(clipRect, validRect) end
---@param clipRect CS.UnityEngine.Rect
---@param validRect boolean
function CS.UnityEngine.UI.MaskableGraphic:SetClipRect(clipRect, validRect) end
function CS.UnityEngine.UI.MaskableGraphic:RecalculateClipping() end
function CS.UnityEngine.UI.MaskableGraphic:RecalculateMasking() end
return CS.UnityEngine.UI.MaskableGraphic
