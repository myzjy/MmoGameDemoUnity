---@class CS.UnityEngine.UI.GridLayoutGroup : CS.UnityEngine.UI.LayoutGroup
---@field public startCorner number
---@field public startAxis number
---@field public cellSize CS.UnityEngine.Vector2
---@field public spacing CS.UnityEngine.Vector2
---@field public constraint number
---@field public constraintCount number
CS.UnityEngine.UI.GridLayoutGroup = { }
function CS.UnityEngine.UI.GridLayoutGroup:CalculateLayoutInputHorizontal() end
function CS.UnityEngine.UI.GridLayoutGroup:CalculateLayoutInputVertical() end
function CS.UnityEngine.UI.GridLayoutGroup:SetLayoutHorizontal() end
function CS.UnityEngine.UI.GridLayoutGroup:SetLayoutVertical() end
return CS.UnityEngine.UI.GridLayoutGroup
