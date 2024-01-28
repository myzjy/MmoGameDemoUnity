---@class UnityEngine.RectTransformUtility

---@type UnityEngine.RectTransformUtility
UnityEngine.RectTransformUtility = { }
---@overload fun(rect:UnityEngine.RectTransform, screenPoint:UnityEngine.Vector2): boolean
---@return boolean
---@param rect UnityEngine.RectTransform
---@param screenPoint UnityEngine.Vector2
---@param cam UnityEngine.Camera
function UnityEngine.RectTransformUtility.RectangleContainsScreenPoint(rect, screenPoint, cam) end
---@return boolean
---@param rect UnityEngine.RectTransform
---@param screenPoint UnityEngine.Vector2
---@param cam UnityEngine.Camera
---@param worldPoint UnityEngine.Vector3
function UnityEngine.RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, screenPoint, cam, worldPoint) end
---@return boolean
---@param rect UnityEngine.RectTransform
---@param screenPoint UnityEngine.Vector2
---@param cam UnityEngine.Camera
---@param localPoint UnityEngine.Vector2
function UnityEngine.RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPoint, cam, localPoint) end
---@return UnityEngine.Ray
---@param cam UnityEngine.Camera
---@param screenPos UnityEngine.Vector2
function UnityEngine.RectTransformUtility.ScreenPointToRay(cam, screenPos) end
---@return UnityEngine.Vector2
---@param cam UnityEngine.Camera
---@param worldPoint UnityEngine.Vector3
function UnityEngine.RectTransformUtility.WorldToScreenPoint(cam, worldPoint) end
---@overload fun(trans:UnityEngine.Transform): UnityEngine.Bounds
---@return UnityEngine.Bounds
---@param root UnityEngine.Transform
---@param child UnityEngine.Transform
function UnityEngine.RectTransformUtility.CalculateRelativeRectTransformBounds(root, child) end
---@param rect UnityEngine.RectTransform
---@param axis number
---@param keepPositioning boolean
---@param recursive boolean
function UnityEngine.RectTransformUtility.FlipLayoutOnAxis(rect, axis, keepPositioning, recursive) end
---@param rect UnityEngine.RectTransform
---@param keepPositioning boolean
---@param recursive boolean
function UnityEngine.RectTransformUtility.FlipLayoutAxes(rect, keepPositioning, recursive) end
---@return UnityEngine.Vector2
---@param point UnityEngine.Vector2
---@param elementTransform UnityEngine.Transform
---@param canvas UnityEngine.Canvas
function UnityEngine.RectTransformUtility.PixelAdjustPoint(point, elementTransform, canvas) end
---@return UnityEngine.Rect
---@param rectTransform UnityEngine.RectTransform
---@param canvas UnityEngine.Canvas
function UnityEngine.RectTransformUtility.PixelAdjustRect(rectTransform, canvas) end
return UnityEngine.RectTransformUtility
