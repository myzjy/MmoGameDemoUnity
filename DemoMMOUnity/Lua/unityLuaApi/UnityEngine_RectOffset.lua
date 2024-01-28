---@class UnityEngine.RectOffset
---@field public left number
---@field public right number
---@field public top number
---@field public bottom number
---@field public horizontal number
---@field public vertical number

---@type UnityEngine.RectOffset
UnityEngine.RectOffset = { }
---@overload fun(): UnityEngine.RectOffset
---@return UnityEngine.RectOffset
---@param left number
---@param right number
---@param top number
---@param bottom number
function UnityEngine.RectOffset.New(left, right, top, bottom) end
---@return UnityEngine.Rect
---@param rect UnityEngine.Rect
function UnityEngine.RectOffset:Add(rect) end
---@return UnityEngine.Rect
---@param rect UnityEngine.Rect
function UnityEngine.RectOffset:Remove(rect) end
---@return string
function UnityEngine.RectOffset:ToString() end
return UnityEngine.RectOffset
