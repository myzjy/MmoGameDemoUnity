---@class CS.UnityEngine.RectOffset
---@field public left number
---@field public right number
---@field public top number
---@field public bottom number
---@field public horizontal number
---@field public vertical number
CS.UnityEngine.RectOffset = { }
---@overload fun(): CS.UnityEngine.RectOffset
---@return CS.UnityEngine.RectOffset
---@param left number
---@param right number
---@param top number
---@param bottom number
function CS.UnityEngine.RectOffset.New(left, right, top, bottom) end
---@return CS.UnityEngine.Rect
---@param rect CS.UnityEngine.Rect
function CS.UnityEngine.RectOffset:Add(rect) end
---@return CS.UnityEngine.Rect
---@param rect CS.UnityEngine.Rect
function CS.UnityEngine.RectOffset:Remove(rect) end
---@return string
function CS.UnityEngine.RectOffset:ToString() end
return CS.UnityEngine.RectOffset
