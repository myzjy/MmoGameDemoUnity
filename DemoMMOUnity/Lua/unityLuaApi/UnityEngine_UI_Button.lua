---@class UnityEngine.UI.Button : UnityEngine.UI.Selectable
---@field public onClick UnityEngine.UI.Button.ButtonClickedEvent

---@type UnityEngine.UI.Button
UnityEngine.UI.Button = { }
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.UI.Button:OnPointerClick(eventData) end
---@param eventData UnityEngine.EventSystems.BaseEventData
function UnityEngine.UI.Button:OnSubmit(eventData) end
return UnityEngine.UI.Button
