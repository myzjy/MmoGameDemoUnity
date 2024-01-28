---@class UnityEngine.Rect : System.ValueType
---@field public zero UnityEngine.Rect
---@field public x number
---@field public y number
---@field public position UnityEngine.Vector2
---@field public center UnityEngine.Vector2
---@field public min UnityEngine.Vector2
---@field public max UnityEngine.Vector2
---@field public width number
---@field public height number
---@field public size UnityEngine.Vector2
---@field public xMin number
---@field public yMin number
---@field public xMax number
---@field public yMax number

---@type UnityEngine.Rect
UnityEngine.Rect = { }
---@overload fun(source:UnityEngine.Rect): UnityEngine.Rect
---@overload fun(position:UnityEngine.Vector2, size:UnityEngine.Vector2): UnityEngine.Rect
---@return UnityEngine.Rect
---@param x number
---@param y number
---@param width number
---@param height number
function UnityEngine.Rect.New(x, y, width, height) end
---@return UnityEngine.Rect
---@param xmin number
---@param ymin number
---@param xmax number
---@param ymax number
function UnityEngine.Rect.MinMaxRect(xmin, ymin, xmax, ymax) end
---@param x number
---@param y number
---@param width number
---@param height number
function UnityEngine.Rect:Set(x, y, width, height) end
---@overload fun(point:UnityEngine.Vector2): boolean
---@overload fun(point:UnityEngine.Vector3): boolean
---@return boolean
---@param point UnityEngine.Vector3
---@param allowInverse boolean
function UnityEngine.Rect:Contains(point, allowInverse) end
---@overload fun(other:UnityEngine.Rect): boolean
---@return boolean
---@param other UnityEngine.Rect
---@param allowInverse boolean
function UnityEngine.Rect:Overlaps(other, allowInverse) end
---@return UnityEngine.Vector2
---@param rectangle UnityEngine.Rect
---@param normalizedRectCoordinates UnityEngine.Vector2
function UnityEngine.Rect.NormalizedToPoint(rectangle, normalizedRectCoordinates) end
---@return UnityEngine.Vector2
---@param rectangle UnityEngine.Rect
---@param point UnityEngine.Vector2
function UnityEngine.Rect.PointToNormalized(rectangle, point) end
---@return boolean
---@param lhs UnityEngine.Rect
---@param rhs UnityEngine.Rect
function UnityEngine.Rect.op_Inequality(lhs, rhs) end
---@return boolean
---@param lhs UnityEngine.Rect
---@param rhs UnityEngine.Rect
function UnityEngine.Rect.op_Equality(lhs, rhs) end
---@return number
function UnityEngine.Rect:GetHashCode() end
---@overload fun(other:System.Object): boolean
---@return boolean
---@param other UnityEngine.Rect
function UnityEngine.Rect:Equals(other) end
---@overload fun(): string
---@return string
---@param format string
function UnityEngine.Rect:ToString(format) end
return UnityEngine.Rect
