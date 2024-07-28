---@class CS.UnityEngine.UI.Button : CS.UnityEngine.UI.Selectable
---@field public onClick CS.UnityEngine.UI.Button.ButtonClickedEvent
CS.UnityEngine.UI.Button = { }
---@param eventData CS.UnityEngine.EventSystems.PointerEventData
function CS.UnityEngine.UI.Button:OnPointerClick(eventData) end
---@param eventData CS.UnityEngine.EventSystems.BaseEventData
function CS.UnityEngine.UI.Button:OnSubmit(eventData) end
return CS.UnityEngine.UI.Button
