---@class UnityEngine.EventSystems.EventSystem : UnityEngine.EventSystems.UIBehaviour
---@field public current UnityEngine.EventSystems.EventSystem
---@field public sendNavigationEvents boolean
---@field public pixelDragThreshold number
---@field public currentInputModule UnityEngine.EventSystems.BaseInputModule
---@field public firstSelectedGameObject UnityEngine.GameObject
---@field public currentSelectedGameObject UnityEngine.GameObject
---@field public isFocused boolean
---@field public alreadySelecting boolean

---@type UnityEngine.EventSystems.EventSystem
UnityEngine.EventSystems.EventSystem = { }
function UnityEngine.EventSystems.EventSystem:UpdateModules() end
---@overload fun(selected:UnityEngine.GameObject): void
---@param selected UnityEngine.GameObject
---@param pointer UnityEngine.EventSystems.BaseEventData
function UnityEngine.EventSystems.EventSystem:SetSelectedGameObject(selected, pointer) end
---@param eventData UnityEngine.EventSystems.PointerEventData
---@param raycastResults System.Collections.Generic.List_UnityEngine.EventSystems.RaycastResult
function UnityEngine.EventSystems.EventSystem:RaycastAll(eventData, raycastResults) end
---@overload fun(): boolean
---@return boolean
---@param pointerId number
function UnityEngine.EventSystems.EventSystem:IsPointerOverGameObject(pointerId) end
---@return string
function UnityEngine.EventSystems.EventSystem:ToString() end
return UnityEngine.EventSystems.EventSystem
