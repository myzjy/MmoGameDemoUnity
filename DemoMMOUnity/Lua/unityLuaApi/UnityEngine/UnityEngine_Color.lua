---@class UnityEngine.Color : System.ValueType
---@field public r number
---@field public g number
---@field public b number
---@field public a number
---@field public red UnityEngine.Color
---@field public green UnityEngine.Color
---@field public blue UnityEngine.Color
---@field public white UnityEngine.Color
---@field public black UnityEngine.Color
---@field public yellow UnityEngine.Color
---@field public cyan UnityEngine.Color
---@field public magenta UnityEngine.Color
---@field public gray UnityEngine.Color
---@field public grey UnityEngine.Color
---@field public clear UnityEngine.Color
---@field public grayscale number
---@field public linear UnityEngine.Color
---@field public gamma UnityEngine.Color
---@field public maxColorComponent number
---@field public Item number

---@type UnityEngine.Color
UnityEngine.Color = { }
---@overload fun(r:number, g:number, b:number): UnityEngine.Color
---@return UnityEngine.Color
---@param r number
---@param g number
---@param b number
---@param a number
function UnityEngine.Color.New(r, g, b, a) end
---@overload fun(): string
---@return string
---@param format string
function UnityEngine.Color:ToString(format) end
---@return number
function UnityEngine.Color:GetHashCode() end
---@overload fun(other:System.Object): boolean
---@return boolean
---@param other UnityEngine.Color
function UnityEngine.Color:Equals(other) end
---@return UnityEngine.Color
---@param a UnityEngine.Color
---@param b UnityEngine.Color
function UnityEngine.Color.op_Addition(a, b) end
---@return UnityEngine.Color
---@param a UnityEngine.Color
---@param b UnityEngine.Color
function UnityEngine.Color.op_Subtraction(a, b) end
---@overload fun(a:UnityEngine.Color, b:UnityEngine.Color): UnityEngine.Color
---@overload fun(a:UnityEngine.Color, b:number): UnityEngine.Color
---@return UnityEngine.Color
---@param b number
---@param a UnityEngine.Color
function UnityEngine.Color.op_Multiply(b, a) end
---@return UnityEngine.Color
---@param a UnityEngine.Color
---@param b number
function UnityEngine.Color.op_Division(a, b) end
---@return boolean
---@param lhs UnityEngine.Color
---@param rhs UnityEngine.Color
function UnityEngine.Color.op_Equality(lhs, rhs) end
---@return boolean
---@param lhs UnityEngine.Color
---@param rhs UnityEngine.Color
function UnityEngine.Color.op_Inequality(lhs, rhs) end
---@return UnityEngine.Color
---@param a UnityEngine.Color
---@param b UnityEngine.Color
---@param t number
function UnityEngine.Color.Lerp(a, b, t) end
---@return UnityEngine.Color
---@param a UnityEngine.Color
---@param b UnityEngine.Color
---@param t number
function UnityEngine.Color.LerpUnclamped(a, b, t) end
---@overload fun(c:UnityEngine.Color): UnityEngine.Vector4
---@return UnityEngine.Vector4
---@param v UnityEngine.Vector4
function UnityEngine.Color.op_Implicit(v) end
---@param rgbColor UnityEngine.Color
---@param H System.Single
---@param S System.Single
---@param V System.Single
function UnityEngine.Color.RGBToHSV(rgbColor, H, S, V) end
---@overload fun(H:number, S:number, V:number): UnityEngine.Color
---@return UnityEngine.Color
---@param H number
---@param S number
---@param V number
---@param hdr boolean
function UnityEngine.Color.HSVToRGB(H, S, V, hdr) end
return UnityEngine.Color
