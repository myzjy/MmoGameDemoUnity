---@class UnityEngine.Vector2Int : System.ValueType
---@field public x number
---@field public y number
---@field public Item number
---@field public magnitude number
---@field public sqrMagnitude number
---@field public zero UnityEngine.Vector2Int
---@field public one UnityEngine.Vector2Int
---@field public up UnityEngine.Vector2Int
---@field public down UnityEngine.Vector2Int
---@field public left UnityEngine.Vector2Int
---@field public right UnityEngine.Vector2Int

---@type UnityEngine.Vector2Int
UnityEngine.Vector2Int = { }
---@return UnityEngine.Vector2Int
---@param x number
---@param y number
function UnityEngine.Vector2Int.New(x, y) end
---@param x number
---@param y number
function UnityEngine.Vector2Int:Set(x, y) end
---@return number
---@param a UnityEngine.Vector2Int
---@param b UnityEngine.Vector2Int
function UnityEngine.Vector2Int.Distance(a, b) end
---@return UnityEngine.Vector2Int
---@param lhs UnityEngine.Vector2Int
---@param rhs UnityEngine.Vector2Int
function UnityEngine.Vector2Int.Min(lhs, rhs) end
---@return UnityEngine.Vector2Int
---@param lhs UnityEngine.Vector2Int
---@param rhs UnityEngine.Vector2Int
function UnityEngine.Vector2Int.Max(lhs, rhs) end
---@overload fun(scale:UnityEngine.Vector2Int): void
---@param a UnityEngine.Vector2Int
---@param b UnityEngine.Vector2Int
function UnityEngine.Vector2Int:Scale(a, b) end
---@param min UnityEngine.Vector2Int
---@param max UnityEngine.Vector2Int
function UnityEngine.Vector2Int:Clamp(min, max) end
---@return UnityEngine.Vector2
---@param v UnityEngine.Vector2Int
function UnityEngine.Vector2Int.op_Implicit(v) end
---@return UnityEngine.Vector3Int
---@param v UnityEngine.Vector2Int
function UnityEngine.Vector2Int.op_Explicit(v) end
---@return UnityEngine.Vector2Int
---@param v UnityEngine.Vector2
function UnityEngine.Vector2Int.FloorToInt(v) end
---@return UnityEngine.Vector2Int
---@param v UnityEngine.Vector2
function UnityEngine.Vector2Int.CeilToInt(v) end
---@return UnityEngine.Vector2Int
---@param v UnityEngine.Vector2
function UnityEngine.Vector2Int.RoundToInt(v) end
---@return UnityEngine.Vector2Int
---@param a UnityEngine.Vector2Int
---@param b UnityEngine.Vector2Int
function UnityEngine.Vector2Int.op_Addition(a, b) end
---@return UnityEngine.Vector2Int
---@param a UnityEngine.Vector2Int
---@param b UnityEngine.Vector2Int
function UnityEngine.Vector2Int.op_Subtraction(a, b) end
---@overload fun(a:UnityEngine.Vector2Int, b:UnityEngine.Vector2Int): UnityEngine.Vector2Int
---@return UnityEngine.Vector2Int
---@param a UnityEngine.Vector2Int
---@param b number
function UnityEngine.Vector2Int.op_Multiply(a, b) end
---@return boolean
---@param lhs UnityEngine.Vector2Int
---@param rhs UnityEngine.Vector2Int
function UnityEngine.Vector2Int.op_Equality(lhs, rhs) end
---@return boolean
---@param lhs UnityEngine.Vector2Int
---@param rhs UnityEngine.Vector2Int
function UnityEngine.Vector2Int.op_Inequality(lhs, rhs) end
---@overload fun(other:System.Object): boolean
---@return boolean
---@param other UnityEngine.Vector2Int
function UnityEngine.Vector2Int:Equals(other) end
---@return number
function UnityEngine.Vector2Int:GetHashCode() end
---@return string
function UnityEngine.Vector2Int:ToString() end
return UnityEngine.Vector2Int
