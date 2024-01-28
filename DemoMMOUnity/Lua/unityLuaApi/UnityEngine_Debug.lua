---@class UnityEngine.Debug
---@field public unityLogger UnityEngine.ILogger
---@field public developerConsoleVisible boolean
---@field public isDebugBuild boolean

---@type UnityEngine.Debug
UnityEngine.Debug = { }
---@return UnityEngine.Debug
function UnityEngine.Debug.New() end
---@overload fun(start:UnityEngine.Vector3, ed:UnityEngine.Vector3): void
---@overload fun(start:UnityEngine.Vector3, ed:UnityEngine.Vector3, color:UnityEngine.Color): void
---@overload fun(start:UnityEngine.Vector3, ed:UnityEngine.Vector3, color:UnityEngine.Color, duration:number): void
---@param start UnityEngine.Vector3
---@param ed UnityEngine.Vector3
---@param color UnityEngine.Color
---@param duration number
---@param depthTest boolean
function UnityEngine.Debug.DrawLine(start, ed, color, duration, depthTest) end
---@overload fun(start:UnityEngine.Vector3, dir:UnityEngine.Vector3): void
---@overload fun(start:UnityEngine.Vector3, dir:UnityEngine.Vector3, color:UnityEngine.Color): void
---@overload fun(start:UnityEngine.Vector3, dir:UnityEngine.Vector3, color:UnityEngine.Color, duration:number): void
---@param start UnityEngine.Vector3
---@param dir UnityEngine.Vector3
---@param color UnityEngine.Color
---@param duration number
---@param depthTest boolean
function UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest) end
function UnityEngine.Debug.Break() end
function UnityEngine.Debug.DebugBreak() end
---@overload fun(message:System.Object): void
---@param message System.Object
---@param context UnityEngine.Object
function UnityEngine.Debug.Log(message, context) end
---@overload fun(format:string, args:System.Object[]): void
---@param context UnityEngine.Object
---@param format string
---@param args System.Object[]
function UnityEngine.Debug.LogFormat(context, format, args) end
---@overload fun(message:System.Object): void
---@param message System.Object
---@param context UnityEngine.Object
function UnityEngine.Debug.LogError(message, context) end
---@overload fun(format:string, args:System.Object[]): void
---@param context UnityEngine.Object
---@param format string
---@param args System.Object[]
function UnityEngine.Debug.LogErrorFormat(context, format, args) end
function UnityEngine.Debug.ClearDeveloperConsole() end
---@overload fun(exception:System.Exception): void
---@param exception System.Exception
---@param context UnityEngine.Object
function UnityEngine.Debug.LogException(exception, context) end
---@overload fun(message:System.Object): void
---@param message System.Object
---@param context UnityEngine.Object
function UnityEngine.Debug.LogWarning(message, context) end
---@overload fun(format:string, args:System.Object[]): void
---@param context UnityEngine.Object
---@param format string
---@param args System.Object[]
function UnityEngine.Debug.LogWarningFormat(context, format, args) end
---@overload fun(condition:boolean): void
---@overload fun(condition:boolean, context:UnityEngine.Object): void
---@overload fun(condition:boolean, message:System.Object): void
---@overload fun(condition:boolean, message:string): void
---@overload fun(condition:boolean, message:System.Object, context:UnityEngine.Object): void
---@param condition boolean
---@param message string
---@param context UnityEngine.Object
function UnityEngine.Debug.Assert(condition, message, context) end
---@overload fun(condition:boolean, format:string, args:System.Object[]): void
---@param condition boolean
---@param context UnityEngine.Object
---@param format string
---@param args System.Object[]
function UnityEngine.Debug.AssertFormat(condition, context, format, args) end
---@overload fun(message:System.Object): void
---@param message System.Object
---@param context UnityEngine.Object
function UnityEngine.Debug.LogAssertion(message, context) end
---@overload fun(format:string, args:System.Object[]): void
---@param context UnityEngine.Object
---@param format string
---@param args System.Object[]
function UnityEngine.Debug.LogAssertionFormat(context, format, args) end
return UnityEngine.Debug
