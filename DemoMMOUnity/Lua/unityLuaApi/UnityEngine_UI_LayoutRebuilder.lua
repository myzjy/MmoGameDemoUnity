---@class UnityEngine.UI.LayoutRebuilder
---@field public transform UnityEngine.Transform

---@type UnityEngine.UI.LayoutRebuilder
UnityEngine.UI.LayoutRebuilder = { }
---@return UnityEngine.UI.LayoutRebuilder
function UnityEngine.UI.LayoutRebuilder.New() end
---@return boolean
function UnityEngine.UI.LayoutRebuilder:IsDestroyed() end
---@param layoutRoot UnityEngine.RectTransform
function UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRoot) end
---@param executing number
function UnityEngine.UI.LayoutRebuilder:Rebuild(executing) end
---@param rect UnityEngine.RectTransform
function UnityEngine.UI.LayoutRebuilder.MarkLayoutForRebuild(rect) end
function UnityEngine.UI.LayoutRebuilder:LayoutComplete() end
function UnityEngine.UI.LayoutRebuilder:GraphicUpdateComplete() end
---@return number
function UnityEngine.UI.LayoutRebuilder:GetHashCode() end
---@return boolean
---@param obj System.Object
function UnityEngine.UI.LayoutRebuilder:Equals(obj) end
---@return string
function UnityEngine.UI.LayoutRebuilder:ToString() end
return UnityEngine.UI.LayoutRebuilder
