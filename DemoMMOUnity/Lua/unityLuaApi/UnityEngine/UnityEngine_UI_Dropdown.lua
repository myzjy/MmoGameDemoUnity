---@class UnityEngine.UI.Dropdown : UnityEngine.UI.Selectable
---@field public template UnityEngine.RectTransform
---@field public captionText UnityEngine.UI.Text
---@field public captionImage UnityEngine.UI.Image
---@field public itemText UnityEngine.UI.Text
---@field public itemImage UnityEngine.UI.Image
---@field public options System.Collections.Generic.List_UnityEngine.UI.Dropdown.OptionData
---@field public onValueChanged UnityEngine.UI.Dropdown.DropdownEvent
---@field public value number

---@type UnityEngine.UI.Dropdown
UnityEngine.UI.Dropdown = { }
function UnityEngine.UI.Dropdown:RefreshShownValue() end
---@overload fun(options:System.Collections.Generic.List_UnityEngine.UI.Dropdown.OptionData): void
---@overload fun(options:System.Collections.Generic.List_System.String): void
---@param options System.Collections.Generic.List_UnityEngine.Sprite
function UnityEngine.UI.Dropdown:AddOptions(options) end
function UnityEngine.UI.Dropdown:ClearOptions() end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.UI.Dropdown:OnPointerClick(eventData) end
---@param eventData UnityEngine.EventSystems.BaseEventData
function UnityEngine.UI.Dropdown:OnSubmit(eventData) end
---@param eventData UnityEngine.EventSystems.BaseEventData
function UnityEngine.UI.Dropdown:OnCancel(eventData) end
function UnityEngine.UI.Dropdown:Show() end
function UnityEngine.UI.Dropdown:Hide() end
return UnityEngine.UI.Dropdown
