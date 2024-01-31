---@class UnityEngine.Rigidbody : UnityEngine.Component
---@field public velocity UnityEngine.Vector3
---@field public angularVelocity UnityEngine.Vector3
---@field public drag number
---@field public angularDrag number
---@field public mass number
---@field public useGravity boolean
---@field public maxDepenetrationVelocity number
---@field public isKinematic boolean
---@field public freezeRotation boolean
---@field public constraints number
---@field public collisionDetectionMode number
---@field public centerOfMass UnityEngine.Vector3
---@field public worldCenterOfMass UnityEngine.Vector3
---@field public inertiaTensorRotation UnityEngine.Quaternion
---@field public inertiaTensor UnityEngine.Vector3
---@field public detectCollisions boolean
---@field public position UnityEngine.Vector3
---@field public rotation UnityEngine.Quaternion
---@field public interpolation number
---@field public solverIterations number
---@field public sleepThreshold number
---@field public maxAngularVelocity number
---@field public solverVelocityIterations number

---@type UnityEngine.Rigidbody
UnityEngine.Rigidbody = { }
---@return UnityEngine.Rigidbody
function UnityEngine.Rigidbody.New() end
---@param density number
function UnityEngine.Rigidbody:SetDensity(density) end
---@param position UnityEngine.Vector3
function UnityEngine.Rigidbody:MovePosition(position) end
---@param rot UnityEngine.Quaternion
function UnityEngine.Rigidbody:MoveRotation(rot) end
function UnityEngine.Rigidbody:Sleep() end
---@return boolean
function UnityEngine.Rigidbody:IsSleeping() end
function UnityEngine.Rigidbody:WakeUp() end
function UnityEngine.Rigidbody:ResetCenterOfMass() end
function UnityEngine.Rigidbody:ResetInertiaTensor() end
---@return UnityEngine.Vector3
---@param relativePoint UnityEngine.Vector3
function UnityEngine.Rigidbody:GetRelativePointVelocity(relativePoint) end
---@return UnityEngine.Vector3
---@param worldPoint UnityEngine.Vector3
function UnityEngine.Rigidbody:GetPointVelocity(worldPoint) end
---@overload fun(force:UnityEngine.Vector3): void
---@overload fun(force:UnityEngine.Vector3, mode:number): void
---@overload fun(x:number, y:number, z:number): void
---@param x number
---@param y number
---@param z number
---@param mode number
function UnityEngine.Rigidbody:AddForce(x, y, z, mode) end
---@overload fun(force:UnityEngine.Vector3): void
---@overload fun(force:UnityEngine.Vector3, mode:number): void
---@overload fun(x:number, y:number, z:number): void
---@param x number
---@param y number
---@param z number
---@param mode number
function UnityEngine.Rigidbody:AddRelativeForce(x, y, z, mode) end
---@overload fun(torque:UnityEngine.Vector3): void
---@overload fun(torque:UnityEngine.Vector3, mode:number): void
---@overload fun(x:number, y:number, z:number): void
---@param x number
---@param y number
---@param z number
---@param mode number
function UnityEngine.Rigidbody:AddTorque(x, y, z, mode) end
---@overload fun(torque:UnityEngine.Vector3): void
---@overload fun(torque:UnityEngine.Vector3, mode:number): void
---@overload fun(x:number, y:number, z:number): void
---@param x number
---@param y number
---@param z number
---@param mode number
function UnityEngine.Rigidbody:AddRelativeTorque(x, y, z, mode) end
---@overload fun(force:UnityEngine.Vector3, position:UnityEngine.Vector3): void
---@param force UnityEngine.Vector3
---@param position UnityEngine.Vector3
---@param mode number
function UnityEngine.Rigidbody:AddForceAtPosition(force, position, mode) end
---@overload fun(explosionForce:number, explosionPosition:UnityEngine.Vector3, explosionRadius:number): void
---@overload fun(explosionForce:number, explosionPosition:UnityEngine.Vector3, explosionRadius:number, upwardsModifier:number): void
---@param explosionForce number
---@param explosionPosition UnityEngine.Vector3
---@param explosionRadius number
---@param upwardsModifier number
---@param mode number
function UnityEngine.Rigidbody:AddExplosionForce(explosionForce, explosionPosition, explosionRadius, upwardsModifier, mode) end
---@return UnityEngine.Vector3
---@param position UnityEngine.Vector3
function UnityEngine.Rigidbody:ClosestPointOnBounds(position) end
---@overload fun(direction:UnityEngine.Vector3, hitInfo:UnityEngine.RaycastHit): boolean
---@overload fun(direction:UnityEngine.Vector3, hitInfo:UnityEngine.RaycastHit, maxDistance:number): boolean
---@return boolean
---@param direction UnityEngine.Vector3
---@param hitInfo UnityEngine.RaycastHit
---@param maxDistance number
---@param queryTriggerInteraction number
function UnityEngine.Rigidbody:SweepTest(direction, hitInfo, maxDistance, queryTriggerInteraction) end
---@overload fun(direction:UnityEngine.Vector3): UnityEngine.RaycastHit[]
---@overload fun(direction:UnityEngine.Vector3, maxDistance:number): UnityEngine.RaycastHit[]
---@return UnityEngine.RaycastHit[]
---@param direction UnityEngine.Vector3
---@param maxDistance number
---@param queryTriggerInteraction number
function UnityEngine.Rigidbody:SweepTestAll(direction, maxDistance, queryTriggerInteraction) end
return UnityEngine.Rigidbody
