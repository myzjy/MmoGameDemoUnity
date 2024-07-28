---@class CS.UnityEngine.UI.Toggle : CS.UnityEngine.UI.Selectable
---@field public toggleTransition number
---@field public graphic CS.UnityEngine.UI.Graphic
---@field public onValueChanged CS.UnityEngine.UI.Toggle.ToggleEvent
---@field public group CS.UnityEngine.UI.ToggleGroup
---@field public isOn boolean
CS.UnityEngine.UI.Toggle = { }
---@param executing number
function CS.UnityEngine.UI.Toggle:Rebuild(executing) end
function CS.UnityEngine.UI.Toggle:LayoutComplete() end
function CS.UnityEngine.UI.Toggle:GraphicUpdateComplete() end
---@param eventData CS.UnityEngine.EventSystems.PointerEventData
function CS.UnityEngine.UI.Toggle:OnPointerClick(eventData) end
---@param eventData CS.UnityEngine.EventSystems.BaseEventData
function CS.UnityEngine.UI.Toggle:OnSubmit(eventData) end
return CS.UnityEngine.UI.Toggle
