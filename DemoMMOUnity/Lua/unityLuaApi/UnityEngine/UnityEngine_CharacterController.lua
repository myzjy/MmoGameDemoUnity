---@class UnityEngine.CharacterController : UnityEngine.Collider
---@field public velocity UnityEngine.Vector3
---@field public isGrounded boolean
---@field public collisionFlags number
---@field public radius number
---@field public height number
---@field public center UnityEngine.Vector3
---@field public slopeLimit number
---@field public stepOffset number
---@field public skinWidth number
---@field public minMoveDistance number
---@field public detectCollisions boolean
---@field public enableOverlapRecovery boolean

---@type UnityEngine.CharacterController
UnityEngine.CharacterController = { }
---@return UnityEngine.CharacterController
function UnityEngine.CharacterController.New() end
---@return boolean
---@param speed UnityEngine.Vector3
function UnityEngine.CharacterController:SimpleMove(speed) end
---@return number
---@param motion UnityEngine.Vector3
function UnityEngine.CharacterController:Move(motion) end
return UnityEngine.CharacterController
