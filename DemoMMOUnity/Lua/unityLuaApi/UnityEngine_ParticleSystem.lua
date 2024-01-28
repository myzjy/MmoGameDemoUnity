---@class UnityEngine.ParticleSystem : UnityEngine.Component
---@field public isPlaying boolean
---@field public isEmitting boolean
---@field public isStopped boolean
---@field public isPaused boolean
---@field public particleCount number
---@field public time number
---@field public randomSeed number
---@field public useAutoRandomSeed boolean
---@field public proceduralSimulationSupported boolean
---@field public main UnityEngine.ParticleSystem.MainModule
---@field public emission UnityEngine.ParticleSystem.EmissionModule
---@field public shape UnityEngine.ParticleSystem.ShapeModule
---@field public velocityOverLifetime UnityEngine.ParticleSystem.VelocityOverLifetimeModule
---@field public limitVelocityOverLifetime UnityEngine.ParticleSystem.LimitVelocityOverLifetimeModule
---@field public inheritVelocity UnityEngine.ParticleSystem.InheritVelocityModule
---@field public forceOverLifetime UnityEngine.ParticleSystem.ForceOverLifetimeModule
---@field public colorOverLifetime UnityEngine.ParticleSystem.ColorOverLifetimeModule
---@field public colorBySpeed UnityEngine.ParticleSystem.ColorBySpeedModule
---@field public sizeOverLifetime UnityEngine.ParticleSystem.SizeOverLifetimeModule
---@field public sizeBySpeed UnityEngine.ParticleSystem.SizeBySpeedModule
---@field public rotationOverLifetime UnityEngine.ParticleSystem.RotationOverLifetimeModule
---@field public rotationBySpeed UnityEngine.ParticleSystem.RotationBySpeedModule
---@field public externalForces UnityEngine.ParticleSystem.ExternalForcesModule
---@field public noise UnityEngine.ParticleSystem.NoiseModule
---@field public collision UnityEngine.ParticleSystem.CollisionModule
---@field public trigger UnityEngine.ParticleSystem.TriggerModule
---@field public subEmitters UnityEngine.ParticleSystem.SubEmittersModule
---@field public textureSheetAnimation UnityEngine.ParticleSystem.TextureSheetAnimationModule
---@field public lights UnityEngine.ParticleSystem.LightsModule
---@field public trails UnityEngine.ParticleSystem.TrailModule
---@field public customData UnityEngine.ParticleSystem.CustomDataModule

---@type UnityEngine.ParticleSystem
UnityEngine.ParticleSystem = { }
---@return UnityEngine.ParticleSystem
function UnityEngine.ParticleSystem.New() end
---@param customData System.Collections.Generic.List_UnityEngine.Vector4
---@param streamIndex number
function UnityEngine.ParticleSystem:SetCustomParticleData(customData, streamIndex) end
---@return number
---@param customData System.Collections.Generic.List_UnityEngine.Vector4
---@param streamIndex number
function UnityEngine.ParticleSystem:GetCustomParticleData(customData, streamIndex) end
---@overload fun(subEmitterIndex:number): void
---@overload fun(subEmitterIndex:number, particle:UnityEngine.ParticleSystem.Particle): void
---@param subEmitterIndex number
---@param particles System.Collections.Generic.List_UnityEngine.ParticleSystem.Particle
function UnityEngine.ParticleSystem:TriggerSubEmitter(subEmitterIndex, particles) end
---@overload fun(particles:UnityEngine.ParticleSystem.Particle[]): void
---@overload fun(particles:UnityEngine.ParticleSystem.Particle[], size:number): void
---@param particles UnityEngine.ParticleSystem.Particle[]
---@param size number
---@param offset number
function UnityEngine.ParticleSystem:SetParticles(particles, size, offset) end
---@overload fun(particles:UnityEngine.ParticleSystem.Particle[]): number
---@overload fun(particles:UnityEngine.ParticleSystem.Particle[], size:number): number
---@return number
---@param particles UnityEngine.ParticleSystem.Particle[]
---@param size number
---@param offset number
function UnityEngine.ParticleSystem:GetParticles(particles, size, offset) end
---@overload fun(t:number): void
---@overload fun(t:number, withChildren:boolean): void
---@overload fun(t:number, withChildren:boolean, restart:boolean): void
---@param t number
---@param withChildren boolean
---@param restart boolean
---@param fixedTimeStep boolean
function UnityEngine.ParticleSystem:Simulate(t, withChildren, restart, fixedTimeStep) end
---@overload fun(): void
---@param withChildren boolean
function UnityEngine.ParticleSystem:Play(withChildren) end
---@overload fun(): void
---@param withChildren boolean
function UnityEngine.ParticleSystem:Pause(withChildren) end
---@overload fun(): void
---@overload fun(withChildren:boolean): void
---@param withChildren boolean
---@param stopBehavior number
function UnityEngine.ParticleSystem:Stop(withChildren, stopBehavior) end
---@overload fun(): void
---@param withChildren boolean
function UnityEngine.ParticleSystem:Clear(withChildren) end
---@overload fun(): boolean
---@return boolean
---@param withChildren boolean
function UnityEngine.ParticleSystem:IsAlive(withChildren) end
---@overload fun(count:number): void
---@param emitParams UnityEngine.ParticleSystem.EmitParams
---@param count number
function UnityEngine.ParticleSystem:Emit(emitParams, count) end
function UnityEngine.ParticleSystem.ResetPreMappedBufferMemory() end
return UnityEngine.ParticleSystem
