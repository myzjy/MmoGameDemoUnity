---@class CS.UnityEngine.EventSystems.BaseEventData : CS.UnityEngine.EventSystems.AbstractEventData
---@field public currentInputModule CS.UnityEngine.EventSystems.BaseInputModule
---@field public selectedObject CS.UnityEngine.GameObject
CS.UnityEngine.EventSystems.BaseEventData = { }
---@return CS.UnityEngine.EventSystems.BaseEventData
---@param eventSystem CS.UnityEngine.EventSystems.EventSystem
function CS.UnityEngine.EventSystems.BaseEventData.New(eventSystem) end
return CS.UnityEngine.EventSystems.BaseEventData
