---@class UnityEngine.UI.Toggle : UnityEngine.UI.Selectable
---@field public toggleTransition number
---@field public graphic UnityEngine.UI.Graphic
---@field public onValueChanged UnityEngine.UI.Toggle.ToggleEvent
---@field public group UnityEngine.UI.ToggleGroup
---@field public isOn boolean

---@type UnityEngine.UI.Toggle
UnityEngine.UI.Toggle = { }
---@param executing number
function UnityEngine.UI.Toggle:Rebuild(executing) end
function UnityEngine.UI.Toggle:LayoutComplete() end
function UnityEngine.UI.Toggle:GraphicUpdateComplete() end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.UI.Toggle:OnPointerClick(eventData) end
---@param eventData UnityEngine.EventSystems.BaseEventData
function UnityEngine.UI.Toggle:OnSubmit(eventData) end
return UnityEngine.UI.Toggle
