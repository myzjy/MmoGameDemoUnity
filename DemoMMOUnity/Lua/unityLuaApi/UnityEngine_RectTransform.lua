---@class UnityEngine.RectTransform : UnityEngine.Transform
---@field public rect UnityEngine.Rect
---@field public anchorMin UnityEngine.Vector2
---@field public anchorMax UnityEngine.Vector2
---@field public anchoredPosition UnityEngine.Vector2
---@field public sizeDelta UnityEngine.Vector2
---@field public pivot UnityEngine.Vector2
---@field public anchoredPosition3D UnityEngine.Vector3
---@field public offsetMin UnityEngine.Vector2
---@field public offsetMax UnityEngine.Vector2

---@type UnityEngine.RectTransform
UnityEngine.RectTransform = { }
---@return UnityEngine.RectTransform
function UnityEngine.RectTransform.New() end
---@param value (fun(driven:UnityEngine.RectTransform):void)
function UnityEngine.RectTransform.add_reapplyDrivenProperties(value) end
---@param value (fun(driven:UnityEngine.RectTransform):void)
function UnityEngine.RectTransform.remove_reapplyDrivenProperties(value) end
function UnityEngine.RectTransform:ForceUpdateRectTransforms() end
---@param fourCornersArray UnityEngine.Vector3[]
function UnityEngine.RectTransform:GetLocalCorners(fourCornersArray) end
---@param fourCornersArray UnityEngine.Vector3[]
function UnityEngine.RectTransform:GetWorldCorners(fourCornersArray) end
---@param edge number
---@param inset number
---@param size number
function UnityEngine.RectTransform:SetInsetAndSizeFromParentEdge(edge, inset, size) end
---@param axis number
---@param size number
function UnityEngine.RectTransform:SetSizeWithCurrentAnchors(axis, size) end
return UnityEngine.RectTransform
