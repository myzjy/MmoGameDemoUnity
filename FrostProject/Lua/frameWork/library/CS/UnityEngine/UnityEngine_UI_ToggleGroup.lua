---@class CS.UnityEngine.UI.ToggleGroup : CS.UnityEngine.EventSystems.UIBehaviour
---@field public allowSwitchOff boolean
CS.UnityEngine.UI.ToggleGroup = { }
---@param toggle CS.UnityEngine.UI.Toggle
function CS.UnityEngine.UI.ToggleGroup:NotifyToggleOn(toggle) end
---@param toggle CS.UnityEngine.UI.Toggle
function CS.UnityEngine.UI.ToggleGroup:UnregisterToggle(toggle) end
---@param toggle CS.UnityEngine.UI.Toggle
function CS.UnityEngine.UI.ToggleGroup:RegisterToggle(toggle) end
---@return boolean
function CS.UnityEngine.UI.ToggleGroup:AnyTogglesOn() end
---@return CS.System.Collections.Generic.IEnumerable_UnityEngine.UI.Toggle
function CS.UnityEngine.UI.ToggleGroup:ActiveToggles() end
function CS.UnityEngine.UI.ToggleGroup:SetAllTogglesOff() end
return CS.UnityEngine.UI.ToggleGroup
