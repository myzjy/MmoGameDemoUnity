---@class UnityEngine.Events.UnityEventBase

---@type UnityEngine.Events.UnityEventBase
UnityEngine.Events.UnityEventBase = { }
---@return number
function UnityEngine.Events.UnityEventBase:GetPersistentEventCount() end
---@return UnityEngine.Object
---@param index number
function UnityEngine.Events.UnityEventBase:GetPersistentTarget(index) end
---@return string
---@param index number
function UnityEngine.Events.UnityEventBase:GetPersistentMethodName(index) end
---@param index number
---@param state number
function UnityEngine.Events.UnityEventBase:SetPersistentListenerState(index, state) end
function UnityEngine.Events.UnityEventBase:RemoveAllListeners() end
---@return string
function UnityEngine.Events.UnityEventBase:ToString() end
---@return System.Reflection.MethodInfo
---@param obj System.Object
---@param functionName string
---@param argumentTypes System.Type[]
function UnityEngine.Events.UnityEventBase.GetValidMethodInfo(obj, functionName, argumentTypes) end
return UnityEngine.Events.UnityEventBase
