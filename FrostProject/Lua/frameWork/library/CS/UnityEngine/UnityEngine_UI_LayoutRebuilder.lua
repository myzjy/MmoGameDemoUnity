---@class CS.UnityEngine.UI.LayoutRebuilder
---@field public transform CS.UnityEngine.Transform
CS.UnityEngine.UI.LayoutRebuilder = { }
---@return CS.UnityEngine.UI.LayoutRebuilder
function CS.UnityEngine.UI.LayoutRebuilder.New() end
---@return boolean
function CS.UnityEngine.UI.LayoutRebuilder:IsDestroyed() end
---@param layoutRoot CS.UnityEngine.RectTransform
function CS.UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRoot) end
---@param executing number
function CS.UnityEngine.UI.LayoutRebuilder:Rebuild(executing) end
---@param rect CS.UnityEngine.RectTransform
function CS.UnityEngine.UI.LayoutRebuilder.MarkLayoutForRebuild(rect) end
function CS.UnityEngine.UI.LayoutRebuilder:LayoutComplete() end
function CS.UnityEngine.UI.LayoutRebuilder:GraphicUpdateComplete() end
---@return number
function CS.UnityEngine.UI.LayoutRebuilder:GetHashCode() end
---@return boolean
---@param obj CS.System.Object
function CS.UnityEngine.UI.LayoutRebuilder:Equals(obj) end
---@return string
function CS.UnityEngine.UI.LayoutRebuilder:ToString() end
return CS.UnityEngine.UI.LayoutRebuilder
