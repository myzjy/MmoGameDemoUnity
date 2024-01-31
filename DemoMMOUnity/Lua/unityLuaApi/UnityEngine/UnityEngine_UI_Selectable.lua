---@class UnityEngine.UI.Selectable : UnityEngine.EventSystems.UIBehaviour
---@field public allSelectables System.Collections.Generic.List_UnityEngine.UI.Selectable
---@field public navigation UnityEngine.UI.Navigation
---@field public transition number
---@field public colors UnityEngine.UI.ColorBlock
---@field public spriteState UnityEngine.UI.SpriteState
---@field public animationTriggers UnityEngine.UI.AnimationTriggers
---@field public targetGraphic UnityEngine.UI.Graphic
---@field public interactable boolean
---@field public image UnityEngine.UI.Image
---@field public animator UnityEngine.Animator

---@type UnityEngine.UI.Selectable
UnityEngine.UI.Selectable = { }
---@return boolean
function UnityEngine.UI.Selectable:IsInteractable() end
---@return UnityEngine.UI.Selectable
---@param dir UnityEngine.Vector3
function UnityEngine.UI.Selectable:FindSelectable(dir) end
---@return UnityEngine.UI.Selectable
function UnityEngine.UI.Selectable:FindSelectableOnLeft() end
---@return UnityEngine.UI.Selectable
function UnityEngine.UI.Selectable:FindSelectableOnRight() end
---@return UnityEngine.UI.Selectable
function UnityEngine.UI.Selectable:FindSelectableOnUp() end
---@return UnityEngine.UI.Selectable
function UnityEngine.UI.Selectable:FindSelectableOnDown() end
---@param eventData UnityEngine.EventSystems.AxisEventData
function UnityEngine.UI.Selectable:OnMove(eventData) end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.UI.Selectable:OnPointerDown(eventData) end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.UI.Selectable:OnPointerUp(eventData) end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.UI.Selectable:OnPointerEnter(eventData) end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.UI.Selectable:OnPointerExit(eventData) end
---@param eventData UnityEngine.EventSystems.BaseEventData
function UnityEngine.UI.Selectable:OnSelect(eventData) end
---@param eventData UnityEngine.EventSystems.BaseEventData
function UnityEngine.UI.Selectable:OnDeselect(eventData) end
function UnityEngine.UI.Selectable:Select() end
return UnityEngine.UI.Selectable
