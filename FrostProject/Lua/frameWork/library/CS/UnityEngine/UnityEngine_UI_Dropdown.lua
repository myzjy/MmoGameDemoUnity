---@class CS.UnityEngine.UI.Dropdown : CS.UnityEngine.UI.Selectable
---@field public template CS.UnityEngine.RectTransform
---@field public captionText CS.UnityEngine.UI.Text
---@field public captionImage CS.UnityEngine.UI.Image
---@field public itemText CS.UnityEngine.UI.Text
---@field public itemImage CS.UnityEngine.UI.Image
---@field public options CS.System.Collections.Generic.List_UnityEngine.UI.Dropdown.OptionData
---@field public onValueChanged CS.UnityEngine.UI.Dropdown.DropdownEvent
---@field public value number
CS.UnityEngine.UI.Dropdown = { }
function CS.UnityEngine.UI.Dropdown:RefreshShownValue() end
---@overload fun(options:CS.System.Collections.Generic.List_UnityEngine.UI.Dropdown.OptionData): void
---@overload fun(options:CS.System.Collections.Generic.List_System.String): void
---@param options CS.System.Collections.Generic.List_UnityEngine.Sprite
function CS.UnityEngine.UI.Dropdown:AddOptions(options) end
function CS.UnityEngine.UI.Dropdown:ClearOptions() end
---@param eventData CS.UnityEngine.EventSystems.PointerEventData
function CS.UnityEngine.UI.Dropdown:OnPointerClick(eventData) end
---@param eventData CS.UnityEngine.EventSystems.BaseEventData
function CS.UnityEngine.UI.Dropdown:OnSubmit(eventData) end
---@param eventData CS.UnityEngine.EventSystems.BaseEventData
function CS.UnityEngine.UI.Dropdown:OnCancel(eventData) end
function CS.UnityEngine.UI.Dropdown:Show() end
function CS.UnityEngine.UI.Dropdown:Hide() end