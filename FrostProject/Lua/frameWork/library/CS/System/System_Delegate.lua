---@class CS.System.Delegate
---@field public Method CS.System.Reflection.MethodInfo
---@field public Target CS.System.Object
CS.System.Delegate = { }
---@overload fun(t:string, method:CS.System.Reflection.MethodInfo): CS.System.Delegate
---@overload fun(t:string, firstArgument:CS.System.Object, method:CS.System.Reflection.MethodInfo): CS.System.Delegate
---@overload fun(t:string, method:CS.System.Reflection.MethodInfo, throwOnBindFailure:boolean): CS.System.Delegate
---@overload fun(t:string, target:CS.System.Object, method:string): CS.System.Delegate
---@overload fun(t:string, target:string, method:string): CS.System.Delegate
---@overload fun(t:string, firstArgument:CS.System.Object, method:CS.System.Reflection.MethodInfo, throwOnBindFailure:boolean): CS.System.Delegate
---@overload fun(t:string, target:string, method:string, ignoreCase:boolean): CS.System.Delegate
---@overload fun(t:string, target:CS.System.Object, method:string, ignoreCase:boolean): CS.System.Delegate
---@overload fun(t:string, target:string, method:string, ignoreCase:boolean, throwOnBindFailure:boolean): CS.System.Delegate
---@return CS.System.Delegate
---@param t string
---@param target CS.System.Object
---@param method string
---@param ignoreCase boolean
---@param throwOnBindFailure boolean
function CS.System.Delegate.CreateDelegate(t, target, method, ignoreCase, throwOnBindFailure) end
---@return CS.System.Object
---@param args CS.System.Object[]
function CS.System.Delegate:DynamicInvoke(args) end
---@return CS.System.Object
function CS.System.Delegate:Clone() end
---@return boolean
---@param obj CS.System.Object
function CS.System.Delegate:Equals(obj) end
---@return number
function CS.System.Delegate:GetHashCode() end
---@param info CS.System.Runtime.Serialization.SerializationInfo
---@param context CS.System.Runtime.Serialization.StreamingContext
function CS.System.Delegate:GetObjectData(info, context) end
---@return CS.System.Delegate[]
function CS.System.Delegate:GetInvocationList() end
---@overload fun(delegates:CS.System.Delegate[]): CS.System.Delegate
---@return CS.System.Delegate
---@param a CS.System.Delegate
---@param b CS.System.Delegate
function CS.System.Delegate.Combine(a, b) end
---@return CS.System.Delegate
---@param source CS.System.Delegate
---@param value CS.System.Delegate
function CS.System.Delegate.Remove(source, value) end
---@return CS.System.Delegate
---@param source CS.System.Delegate
---@param value CS.System.Delegate
function CS.System.Delegate.RemoveAll(source, value) end
---@return boolean
---@param d1 CS.System.Delegate
---@param d2 CS.System.Delegate
function CS.System.Delegate.op_Equality(d1, d2) end
---@return boolean
---@param d1 CS.System.Delegate
---@param d2 CS.System.Delegate
function CS.System.Delegate.op_Inequality(d1, d2) end
return CS.System.Delegate
