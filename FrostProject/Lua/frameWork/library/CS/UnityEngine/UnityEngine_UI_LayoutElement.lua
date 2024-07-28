---@class CS.UnityEngine.UI.LayoutElement : CS.UnityEngine.EventSystems.UIBehaviour
---@field public ignoreLayout boolean
---@field public minWidth number
---@field public minHeight number
---@field public preferredWidth number
---@field public preferredHeight number
---@field public flexibleWidth number
---@field public flexibleHeight number
---@field public layoutPriority number
CS.UnityEngine.UI.LayoutElement = { }
function CS.UnityEngine.UI.LayoutElement:CalculateLayoutInputHorizontal() end
function CS.UnityEngine.UI.LayoutElement:CalculateLayoutInputVertical() end
return CS.UnityEngine.UI.LayoutElement
