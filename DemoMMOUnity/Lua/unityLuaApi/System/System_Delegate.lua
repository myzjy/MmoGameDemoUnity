---@class System.Delegate
---@field public Method System.Reflection.MethodInfo
---@field public Target System.Object

---@type System.Delegate
System.Delegate = { }
---@overload fun(t:string, method:System.Reflection.MethodInfo): System.Delegate
---@overload fun(t:string, firstArgument:System.Object, method:System.Reflection.MethodInfo): System.Delegate
---@overload fun(t:string, method:System.Reflection.MethodInfo, throwOnBindFailure:boolean): System.Delegate
---@overload fun(t:string, target:System.Object, method:string): System.Delegate
---@overload fun(t:string, target:string, method:string): System.Delegate
---@overload fun(t:string, firstArgument:System.Object, method:System.Reflection.MethodInfo, throwOnBindFailure:boolean): System.Delegate
---@overload fun(t:string, target:string, method:string, ignoreCase:boolean): System.Delegate
---@overload fun(t:string, target:System.Object, method:string, ignoreCase:boolean): System.Delegate
---@overload fun(t:string, target:string, method:string, ignoreCase:boolean, throwOnBindFailure:boolean): System.Delegate
---@return System.Delegate
---@param t string
---@param target System.Object
---@param method string
---@param ignoreCase boolean
---@param throwOnBindFailure boolean
function System.Delegate.CreateDelegate(t, target, method, ignoreCase, throwOnBindFailure) end
---@return System.Object
---@param args System.Object[]
function System.Delegate:DynamicInvoke(args) end
---@return System.Object
function System.Delegate:Clone() end
---@return boolean
---@param obj System.Object
function System.Delegate:Equals(obj) end
---@return number
function System.Delegate:GetHashCode() end
---@param info System.Runtime.Serialization.SerializationInfo
---@param context System.Runtime.Serialization.StreamingContext
function System.Delegate:GetObjectData(info, context) end
---@return System.Delegate[]
function System.Delegate:GetInvocationList() end
---@overload fun(delegates:System.Delegate[]): System.Delegate
---@return System.Delegate
---@param a System.Delegate
---@param b System.Delegate
function System.Delegate.Combine(a, b) end
---@return System.Delegate
---@param source System.Delegate
---@param value System.Delegate
function System.Delegate.Remove(source, value) end
---@return System.Delegate
---@param source System.Delegate
---@param value System.Delegate
function System.Delegate.RemoveAll(source, value) end
---@return boolean
---@param d1 System.Delegate
---@param d2 System.Delegate
function System.Delegate.op_Equality(d1, d2) end
---@return boolean
---@param d1 System.Delegate
---@param d2 System.Delegate
function System.Delegate.op_Inequality(d1, d2) end
return System.Delegate
