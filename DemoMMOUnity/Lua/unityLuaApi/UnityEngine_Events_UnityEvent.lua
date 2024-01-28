---@class UnityEngine.Events.UnityEvent : UnityEngine.Events.UnityEventBase

---@type UnityEngine.Events.UnityEvent
UnityEngine.Events.UnityEvent = { }
---@return UnityEngine.Events.UnityEvent
function UnityEngine.Events.UnityEvent.New() end
---@param call (fun():void)
function UnityEngine.Events.UnityEvent:AddListener(call) end
---@param call (fun():void)
function UnityEngine.Events.UnityEvent:RemoveListener(call) end
function UnityEngine.Events.UnityEvent:Invoke() end
return UnityEngine.Events.UnityEvent
