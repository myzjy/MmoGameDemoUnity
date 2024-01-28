---@class UnityEngine.CanvasGroup : UnityEngine.Behaviour
---@field public alpha number
---@field public interactable boolean
---@field public blocksRaycasts boolean
---@field public ignoreParentGroups boolean

---@type UnityEngine.CanvasGroup
UnityEngine.CanvasGroup = { }
---@return UnityEngine.CanvasGroup
function UnityEngine.CanvasGroup.New() end
---@return boolean
---@param sp UnityEngine.Vector2
---@param eventCamera UnityEngine.Camera
function UnityEngine.CanvasGroup:IsRaycastLocationValid(sp, eventCamera) end
return UnityEngine.CanvasGroup
