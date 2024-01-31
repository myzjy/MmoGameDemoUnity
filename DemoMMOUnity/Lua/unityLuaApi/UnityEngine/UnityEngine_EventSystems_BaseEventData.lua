---@class UnityEngine.EventSystems.BaseEventData : UnityEngine.EventSystems.AbstractEventData
---@field public currentInputModule UnityEngine.EventSystems.BaseInputModule
---@field public selectedObject UnityEngine.GameObject

---@type UnityEngine.EventSystems.BaseEventData
UnityEngine.EventSystems.BaseEventData = { }
---@return UnityEngine.EventSystems.BaseEventData
---@param eventSystem UnityEngine.EventSystems.EventSystem
function UnityEngine.EventSystems.BaseEventData.New(eventSystem) end
return UnityEngine.EventSystems.BaseEventData
