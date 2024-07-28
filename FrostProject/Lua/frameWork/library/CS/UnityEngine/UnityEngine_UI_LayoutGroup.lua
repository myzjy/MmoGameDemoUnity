---@class CS.UnityEngine.UI.LayoutGroup : CS.UnityEngine.EventSystems.UIBehaviour
---@field public padding CS.UnityEngine.RectOffset
---@field public childAlignment number
---@field public minWidth number
---@field public preferredWidth number
---@field public flexibleWidth number
---@field public minHeight number
---@field public preferredHeight number
---@field public flexibleHeight number
---@field public layoutPriority number
CS.UnityEngine.UI.LayoutGroup = { }
function CS.UnityEngine.UI.LayoutGroup:CalculateLayoutInputHorizontal() end
function CS.UnityEngine.UI.LayoutGroup:CalculateLayoutInputVertical() end
function CS.UnityEngine.UI.LayoutGroup:SetLayoutHorizontal() end
function CS.UnityEngine.UI.LayoutGroup:SetLayoutVertical() end
return CS.UnityEngine.UI.LayoutGroup
