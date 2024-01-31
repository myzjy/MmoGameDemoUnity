---@class UnityEngine.UI.LayoutElement : UnityEngine.EventSystems.UIBehaviour
---@field public ignoreLayout boolean
---@field public minWidth number
---@field public minHeight number
---@field public preferredWidth number
---@field public preferredHeight number
---@field public flexibleWidth number
---@field public flexibleHeight number
---@field public layoutPriority number

---@type UnityEngine.UI.LayoutElement
UnityEngine.UI.LayoutElement = { }
function UnityEngine.UI.LayoutElement:CalculateLayoutInputHorizontal() end
function UnityEngine.UI.LayoutElement:CalculateLayoutInputVertical() end
return UnityEngine.UI.LayoutElement
