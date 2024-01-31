---@class UnityEngine.UI.Text : UnityEngine.UI.MaskableGraphic
---@field public cachedTextGenerator UnityEngine.TextGenerator
---@field public cachedTextGeneratorForLayout UnityEngine.TextGenerator
---@field public mainTexture UnityEngine.Texture
---@field public font UnityEngine.Font
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

---@type UnityEngine.UI.Text
UnityEngine.UI.Text = { }
function UnityEngine.UI.Text:FontTextureChanged() end
---@return UnityEngine.TextGenerationSettings
---@param extents UnityEngine.Vector2
function UnityEngine.UI.Text:GetGenerationSettings(extents) end
---@return UnityEngine.Vector2
---@param anchor number
function UnityEngine.UI.Text.GetTextAnchorPivot(anchor) end
function UnityEngine.UI.Text:CalculateLayoutInputHorizontal() end
function UnityEngine.UI.Text:CalculateLayoutInputVertical() end
function UnityEngine.UI.Text:OnRebuildRequested() end
return UnityEngine.UI.Text
