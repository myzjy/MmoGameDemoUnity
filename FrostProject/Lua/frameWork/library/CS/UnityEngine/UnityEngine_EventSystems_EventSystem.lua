---@class CS.UnityEngine.EventSystems.EventSystem : CS.UnityEngine.EventSystems.UIBehaviour
---@field public current CS.UnityEngine.EventSystems.EventSystem
---@field public sendNavigationEvents boolean
---@field public pixelDragThreshold number
---@field public currentInputModule CS.UnityEngine.EventSystems.BaseInputModule
---@field public firstSelectedGameObject CS.UnityEngine.GameObject
---@field public currentSelectedGameObject CS.UnityEngine.GameObject
---@field public isFocused boolean
---@field public alreadySelecting boolean
CS.UnityEngine.EventSystems.EventSystem = { }
function CS.UnityEngine.EventSystems.EventSystem:UpdateModules() end
---@overload fun(selected:CS.UnityEngine.GameObject): void
---@param selected CS.UnityEngine.GameObject
---@param pointer CS.UnityEngine.EventSystems.BaseEventData
function CS.UnityEngine.EventSystems.EventSystem:SetSelectedGameObject(selected, pointer) end
---@param eventData CS.UnityEngine.EventSystems.PointerEventData
---@param raycastResults CS.System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult
function CS.UnityEngine.EventSystems.EventSystem:RaycastAll(eventData, raycastResults) end
---@overload fun(): boolean
---@return boolean
---@param pointerId number
function CS.UnityEngine.EventSystems.EventSystem:IsPointerOverGameObject(pointerId) end
---@return string
function CS.UnityEngine.EventSystems.EventSystem:ToString() end
return CS.UnityEngine.EventSystems.EventSystem
