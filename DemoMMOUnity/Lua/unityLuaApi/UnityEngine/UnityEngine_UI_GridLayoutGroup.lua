---@class UnityEngine.UI.GridLayoutGroup : UnityEngine.UI.LayoutGroup
---@field public startCorner number
---@field public startAxis number
---@field public cellSize UnityEngine.Vector2
---@field public spacing UnityEngine.Vector2
---@field public constraint number
---@field public constraintCount number

---@type UnityEngine.UI.GridLayoutGroup
UnityEngine.UI.GridLayoutGroup = { }
function UnityEngine.UI.GridLayoutGroup:CalculateLayoutInputHorizontal() end
function UnityEngine.UI.GridLayoutGroup:CalculateLayoutInputVertical() end
function UnityEngine.UI.GridLayoutGroup:SetLayoutHorizontal() end
function UnityEngine.UI.GridLayoutGroup:SetLayoutVertical() end
return UnityEngine.UI.GridLayoutGroup
