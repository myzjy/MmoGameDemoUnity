---@class CS.UnityEngine.Events.UnityEventBase
CS.UnityEngine.Events.UnityEventBase = { }
---@return number
function CS.UnityEngine.Events.UnityEventBase:GetPersistentEventCount() end
---@return CS.UnityEngine.Object
---@param index number
function CS.UnityEngine.Events.UnityEventBase:GetPersistentTarget(index) end
---@return string
---@param index number
function CS.UnityEngine.Events.UnityEventBase:GetPersistentMethodName(index) end
---@param index number
---@param state number
function CS.UnityEngine.Events.UnityEventBase:SetPersistentListenerState(index, state) end
function CS.UnityEngine.Events.UnityEventBase:RemoveAllListeners() end
---@return string
function CS.UnityEngine.Events.UnityEventBase:ToString() end
---@return CS.System.Reflection.MethodInfo
---@param obj CS.System.Object
---@param functionName string
---@param argumentTypes CS.System.Type[]
function CS.UnityEngine.Events.UnityEventBase.GetValidMethodInfo(obj, functionName, argumentTypes) end
return CS.UnityEngine.Events.UnityEventBase
