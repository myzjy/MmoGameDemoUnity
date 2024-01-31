---@class UnityEngine.Mathf : System.ValueType
---@field public PI number
---@field public Infinity number
---@field public NegativeInfinity number
---@field public Deg2Rad number
---@field public Rad2Deg number
---@field public Epsilon number

---@type UnityEngine.Mathf
UnityEngine.Mathf = { }
---@return number
---@param value number
function UnityEngine.Mathf.ClosestPowerOfTwo(value) end
---@return boolean
---@param value number
function UnityEngine.Mathf.IsPowerOfTwo(value) end
---@return number
---@param value number
function UnityEngine.Mathf.NextPowerOfTwo(value) end
---@return number
---@param value number
function UnityEngine.Mathf.GammaToLinearSpace(value) end
---@return number
---@param value number
function UnityEngine.Mathf.LinearToGammaSpace(value) end
---@return UnityEngine.Color
---@param kelvin number
function UnityEngine.Mathf.CorrelatedColorTemperatureToRGB(kelvin) end
---@return number
---@param val number
function UnityEngine.Mathf.FloatToHalf(val) end
---@return number
---@param val number
function UnityEngine.Mathf.HalfToFloat(val) end
---@return number
---@param x number
---@param y number
function UnityEngine.Mathf.PerlinNoise(x, y) end
---@return number
---@param f number
function UnityEngine.Mathf.Sin(f) end
---@return number
---@param f number
function UnityEngine.Mathf.Cos(f) end
---@return number
---@param f number
function UnityEngine.Mathf.Tan(f) end
---@return number
---@param f number
function UnityEngine.Mathf.Asin(f) end
---@return number
---@param f number
function UnityEngine.Mathf.Acos(f) end
---@return number
---@param f number
function UnityEngine.Mathf.Atan(f) end
---@return number
---@param y number
---@param x number
function UnityEngine.Mathf.Atan2(y, x) end
---@return number
---@param f number
function UnityEngine.Mathf.Sqrt(f) end
---@overload fun(f:number): number
---@return number
---@param value number
function UnityEngine.Mathf.Abs(value) end
---@overload fun(values:System.Single[]): number
---@overload fun(values:System.Int32[]): number
---@overload fun(a:number, b:number): number
---@return number
---@param a number
---@param b number
function UnityEngine.Mathf.Min(a, b) end
---@overload fun(values:System.Single[]): number
---@overload fun(values:System.Int32[]): number
---@overload fun(a:number, b:number): number
---@return number
---@param a number
---@param b number
function UnityEngine.Mathf.Max(a, b) end
---@return number
---@param f number
---@param p number
function UnityEngine.Mathf.Pow(f, p) end
---@return number
---@param power number
function UnityEngine.Mathf.Exp(power) end
---@overload fun(f:number): number
---@return number
---@param f number
---@param p number
function UnityEngine.Mathf.Log(f, p) end
---@return number
---@param f number
function UnityEngine.Mathf.Log10(f) end
---@return number
---@param f number
function UnityEngine.Mathf.Ceil(f) end
---@return number
---@param f number
function UnityEngine.Mathf.Floor(f) end
---@return number
---@param f number
function UnityEngine.Mathf.Round(f) end
---@return number
---@param f number
function UnityEngine.Mathf.CeilToInt(f) end
---@return number
---@param f number
function UnityEngine.Mathf.FloorToInt(f) end
---@return number
---@param f number
function UnityEngine.Mathf.RoundToInt(f) end
---@return number
---@param f number
function UnityEngine.Mathf.Sign(f) end
---@overload fun(value:number, min:number, max:number): number
---@return number
---@param value number
---@param min number
---@param max number
function UnityEngine.Mathf.Clamp(value, min, max) end
---@return number
---@param value number
function UnityEngine.Mathf.Clamp01(value) end
---@return number
---@param a number
---@param b number
---@param t number
function UnityEngine.Mathf.Lerp(a, b, t) end
---@return number
---@param a number
---@param b number
---@param t number
function UnityEngine.Mathf.LerpUnclamped(a, b, t) end
---@return number
---@param a number
---@param b number
---@param t number
function UnityEngine.Mathf.LerpAngle(a, b, t) end
---@return number
---@param current number
---@param target number
---@param maxDelta number
function UnityEngine.Mathf.MoveTowards(current, target, maxDelta) end
---@return number
---@param current number
---@param target number
---@param maxDelta number
function UnityEngine.Mathf.MoveTowardsAngle(current, target, maxDelta) end
---@return number
---@param from number
---@param to number
---@param t number
function UnityEngine.Mathf.SmoothStep(from, to, t) end
---@return number
---@param value number
---@param absmax number
---@param gamma number
function UnityEngine.Mathf.Gamma(value, absmax, gamma) end
---@return boolean
---@param a number
---@param b number
function UnityEngine.Mathf.Approximately(a, b) end
---@overload fun(current:number, target:number, currentVelocity:System.Single, smoothTime:number): number
---@overload fun(current:number, target:number, currentVelocity:System.Single, smoothTime:number, maxSpeed:number): number
---@return number
---@param current number
---@param target number
---@param currentVelocity System.Single
---@param smoothTime number
---@param maxSpeed number
---@param deltaTime number
function UnityEngine.Mathf.SmoothDamp(current, target, currentVelocity, smoothTime, maxSpeed, deltaTime) end
---@overload fun(current:number, target:number, currentVelocity:System.Single, smoothTime:number): number
---@overload fun(current:number, target:number, currentVelocity:System.Single, smoothTime:number, maxSpeed:number): number
---@return number
---@param current number
---@param target number
---@param currentVelocity System.Single
---@param smoothTime number
---@param maxSpeed number
---@param deltaTime number
function UnityEngine.Mathf.SmoothDampAngle(current, target, currentVelocity, smoothTime, maxSpeed, deltaTime) end
---@return number
---@param t number
---@param length number
function UnityEngine.Mathf.Repeat(t, length) end
---@return number
---@param t number
---@param length number
function UnityEngine.Mathf.PingPong(t, length) end
---@return number
---@param a number
---@param b number
---@param value number
function UnityEngine.Mathf.InverseLerp(a, b, value) end
---@return number
---@param current number
---@param target number
function UnityEngine.Mathf.DeltaAngle(current, target) end
return UnityEngine.Mathf
