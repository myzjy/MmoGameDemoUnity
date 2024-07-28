---@class CS.UnityEngine.UI.Selectable : CS.UnityEngine.EventSystems.UIBehaviour
---@field public allSelectables CS.System.Collections.Generic.List_UnityEngine.UI.Selectable
---@field public navigation CS.UnityEngine.UI.Navigation
---@field public transition number
---@field public colors CS.UnityEngine.UI.ColorBlock
---@field public spriteState CS.UnityEngine.UI.SpriteState
---@field public animationTriggers CS.UnityEngine.UI.AnimationTriggers
---@field public targetGraphic CS.UnityEngine.UI.Graphic
---@field public interactable boolean
---@field public image CS.UnityEngine.UI.Image
---@field public animator CS.UnityEngine.Animator
CS.UnityEngine.UI.Selectable = { }
---@return boolean
function CS.UnityEngine.UI.Selectable:IsInteractable() end
---@return CS.UnityEngine.UI.Selectable
---@param dir CS.UnityEngine.Vector3
function CS.UnityEngine.UI.Selectable:FindSelectable(dir) end
---@return CS.UnityEngine.UI.Selectable
function CS.UnityEngine.UI.Selectable:FindSelectableOnLeft() end
---@return CS.UnityEngine.UI.Selectable
function CS.UnityEngine.UI.Selectable:FindSelectableOnRight() end
---@return CS.UnityEngine.UI.Selectable
function CS.UnityEngine.UI.Selectable:FindSelectableOnUp() end
---@return CS.UnityEngine.UI.Selectable
function CS.UnityEngine.UI.Selectable:FindSelectableOnDown() end
---@param eventData CS.UnityEngine.EventSystems.AxisEventData
function CS.UnityEngine.UI.Selectable:OnMove(eventData) end
---@param eventData CS.UnityEngine.EventSystems.PointerEventData
function CS.UnityEngine.UI.Selectable:OnPointerDown(eventData) end
---@param eventData CS.UnityEngine.EventSystems.PointerEventData
function CS.UnityEngine.UI.Selectable:OnPointerUp(eventData) end
---@param eventData CS.UnityEngine.EventSystems.PointerEventData
function CS.UnityEngine.UI.Selectable:OnPointerEnter(eventData) end
---@param eventData CS.UnityEngine.EventSystems.PointerEventData
function CS.UnityEngine.UI.Selectable:OnPointerExit(eventData) end
---@param eventData CS.UnityEngine.EventSystems.BaseEventData
function CS.UnityEngine.UI.Selectable:OnSelect(eventData) end
---@param eventData CS.UnityEngine.EventSystems.BaseEventData
function CS.UnityEngine.UI.Selectable:OnDeselect(eventData) end
function CS.UnityEngine.UI.Selectable:Select() end
return CS.UnityEngine.UI.Selectable
