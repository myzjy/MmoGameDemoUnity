---@class UnityEngine.UI.LayoutGroup : UnityEngine.EventSystems.UIBehaviour
---@field public padding UnityEngine.RectOffset
---@field public childAlignment number
---@field public minWidth number
---@field public preferredWidth number
---@field public flexibleWidth number
---@field public minHeight number
---@field public preferredHeight number
---@field public flexibleHeight number
---@field public layoutPriority number

---@type UnityEngine.UI.LayoutGroup
UnityEngine.UI.LayoutGroup = { }
function UnityEngine.UI.LayoutGroup:CalculateLayoutInputHorizontal() end
function UnityEngine.UI.LayoutGroup:CalculateLayoutInputVertical() end
function UnityEngine.UI.LayoutGroup:SetLayoutHorizontal() end
function UnityEngine.UI.LayoutGroup:SetLayoutVertical() end
return UnityEngine.UI.LayoutGroup
