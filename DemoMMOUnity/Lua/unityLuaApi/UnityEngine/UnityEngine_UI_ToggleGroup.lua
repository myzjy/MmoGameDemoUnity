---@class UnityEngine.UI.ToggleGroup : UnityEngine.EventSystems.UIBehaviour
---@field public allowSwitchOff boolean

---@type UnityEngine.UI.ToggleGroup
UnityEngine.UI.ToggleGroup = { }
---@param toggle UnityEngine.UI.Toggle
function UnityEngine.UI.ToggleGroup:NotifyToggleOn(toggle) end
---@param toggle UnityEngine.UI.Toggle
function UnityEngine.UI.ToggleGroup:UnregisterToggle(toggle) end
---@param toggle UnityEngine.UI.Toggle
function UnityEngine.UI.ToggleGroup:RegisterToggle(toggle) end
---@return boolean
function UnityEngine.UI.ToggleGroup:AnyTogglesOn() end
---@return System.Collections.Generic.IEnumerable_UnityEngine.UI.Toggle
function UnityEngine.UI.ToggleGroup:ActiveToggles() end
function UnityEngine.UI.ToggleGroup:SetAllTogglesOff() end
return UnityEngine.UI.ToggleGroup
