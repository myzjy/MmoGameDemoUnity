---@class CS.UnityEngine.UI.Text : CS.UnityEngine.UI.MaskableGraphic
---@field public cachedTextGenerator CS.UnityEngine.TextGenerator
---@field public cachedTextGeneratorForLayout CS.UnityEngine.TextGenerator
---@field public mainTexture CS.UnityEngine.Texture
---@field public font CS.UnityEngine.Font
---@field public text string
---@field public supportRichText boolean
---@field public resizeTextForBestFit boolean
---@field public resizeTextMinSize number
---@field public resizeTextMaxSize number
---@field public alignment number
---@field public alignByGeometry boolean
---@field public fontSize number
---@field public horizontalOverflow number
---@field public verticalOverflow number
---@field public lineSpacing number
---@field public fontStyle number
---@field public pixelsPerUnit number
---@field public minWidth number
---@field public preferredWidth number
---@field public flexibleWidth number
---@field public minHeight number
---@field public preferredHeight number
---@field public flexibleHeight number
---@field public layoutPriority number
CS.UnityEngine.UI.Text = { }
function CS.UnityEngine.UI.Text:FontTextureChanged() end
---@return CS.UnityEngine.TextCore.Text.TextGenerationSettings
---@param extents CS.UnityEngine.Vector2
function CS.UnityEngine.UI.Text:GetGenerationSettings(extents) end
---@return CS.UnityEngine.Vector2
---@param anchor number
function CS.UnityEngine.UI.Text.GetTextAnchorPivot(anchor) end
function CS.UnityEngine.UI.Text:CalculateLayoutInputHorizontal() end
function CS.UnityEngine.UI.Text:CalculateLayoutInputVertical() end
function CS.UnityEngine.UI.Text:OnRebuildRequested() end
return CS.UnityEngine.UI.Text
