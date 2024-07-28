---@class CS.UnityEngine.Events.UnityEvent : CS.UnityEngine.Events.UnityEventBase
CS.UnityEngine.Events.UnityEvent = { }
---@return CS.UnityEngine.Events.UnityEvent
function CS.UnityEngine.Events.UnityEvent.New() end
---@param call (fun():void)
function CS.UnityEngine.Events.UnityEvent:AddListener(call) end
---@param call (fun():void)
function CS.UnityEngine.Events.UnityEvent:RemoveListener(call) end
function CS.UnityEngine.Events.UnityEvent:Invoke() end
return CS.UnityEngine.Events.UnityEvent
