---@class CS.UnityEngine.ParticleSystem : CS.UnityEngine.Component
---@field public isPlaying boolean
---@field public isEmitting boolean
---@field public isStopped boolean
---@field public isPaused boolean
---@field public particleCount number
---@field public time number
---@field public randomSeed number
---@field public useAutoRandomSeed boolean
---@field public proceduralSimulationSupported boolean
---@field public main CS.UnityEngine.ParticleSystem.MainModule
---@field public emission CS.UnityEngine.ParticleSystem.EmissionModule
---@field public shape CS.UnityEngine.ParticleSystem.ShapeModule
---@field public velocityOverLifetime CS.UnityEngine.ParticleSystem.VelocityOverLifetimeModule
---@field public limitVelocityOverLifetime CS.UnityEngine.ParticleSystem.LimitVelocityOverLifetimeModule
---@field public inheritVelocity CS.UnityEngine.ParticleSystem.InheritVelocityModule
---@field public forceOverLifetime CS.UnityEngine.ParticleSystem.ForceOverLifetimeModule
---@field public colorOverLifetime CS.UnityEngine.ParticleSystem.ColorOverLifetimeModule
---@field public colorBySpeed CS.UnityEngine.ParticleSystem.ColorBySpeedModule
---@field public sizeOverLifetime CS.UnityEngine.ParticleSystem.SizeOverLifetimeModule
---@field public sizeBySpeed CS.UnityEngine.ParticleSystem.SizeBySpeedModule
---@field public rotationOverLifetime CS.UnityEngine.ParticleSystem.RotationOverLifetimeModule
---@field public rotationBySpeed CS.UnityEngine.ParticleSystem.RotationBySpeedModule
---@field public externalForces CS.UnityEngine.ParticleSystem.ExternalForcesModule
---@field public noise CS.UnityEngine.ParticleSystem.NoiseModule
---@field public collision CS.UnityEngine.ParticleSystem.CollisionModule
---@field public trigger CS.UnityEngine.ParticleSystem.TriggerModule
---@field public subEmitters CS.UnityEngine.ParticleSystem.SubEmittersModule
---@field public textureSheetAnimation CS.UnityEngine.ParticleSystem.TextureSheetAnimationModule
---@field public lights CS.UnityEngine.ParticleSystem.LightsModule
---@field public trails CS.UnityEngine.ParticleSystem.TrailModule
---@field public customData CS.UnityEngine.ParticleSystem.CustomDataModule
CS.UnityEngine.ParticleSystem = { }
---@return CS.UnityEngine.ParticleSystem
function CS.UnityEngine.ParticleSystem.New() end
---@param customData CS.System.Collections.Generic.List_UnityEngine.Vector4
---@param streamIndex number
function CS.UnityEngine.ParticleSystem:SetCustomParticleData(customData, streamIndex) end
---@return number
---@param customData CS.System.Collections.Generic.List_UnityEngine.Vector4
---@param streamIndex number
function CS.UnityEngine.ParticleSystem:GetCustomParticleData(customData, streamIndex) end
---@overload fun(subEmitterIndex:number): void
---@overload fun(subEmitterIndex:number, particle:CS.UnityEngine.ParticleSystem.Particle): void
---@param subEmitterIndex number
---@param particles CS.System.Collections.Generic.List_UnityEngine.ParticleSystem.Particle
function CS.UnityEngine.ParticleSystem:TriggerSubEmitter(subEmitterIndex, particles) end
---@overload fun(particles:CS.UnityEngine.ParticleSystem.Particle[]): void
---@overload fun(particles:CS.UnityEngine.ParticleSystem.Particle[], size:number): void
---@param particles CS.UnityEngine.ParticleSystem.Particle[]
---@param size number
---@param offset number
function CS.UnityEngine.ParticleSystem:SetParticles(particles, size, offset) end
---@overload fun(particles:CS.UnityEngine.ParticleSystem.Particle[]): number
---@overload fun(particles:CS.UnityEngine.ParticleSystem.Particle[], size:number): number
---@return number
---@param particles CS.UnityEngine.ParticleSystem.Particle[]
---@param size number
---@param offset number
function CS.UnityEngine.ParticleSystem:GetParticles(particles, size, offset) end
---@overload fun(t:number): void
---@overload fun(t:number, withChildren:boolean): void
---@overload fun(t:number, withChildren:boolean, restart:boolean): void
---@param t number
---@param withChildren boolean
---@param restart boolean
---@param fixedTimeStep boolean
function CS.UnityEngine.ParticleSystem:Simulate(t, withChildren, restart, fixedTimeStep) end
---@overload fun(): void
---@param withChildren boolean
function CS.UnityEngine.ParticleSystem:Play(withChildren) end
---@overload fun(): void
---@param withChildren boolean
function CS.UnityEngine.ParticleSystem:Pause(withChildren) end
---@overload fun(): void
---@overload fun(withChildren:boolean): void
---@param withChildren boolean
---@param stopBehavior number
function CS.UnityEngine.ParticleSystem:Stop(withChildren, stopBehavior) end
---@overload fun(): void
---@param withChildren boolean
function CS.UnityEngine.ParticleSystem:Clear(withChildren) end
---@overload fun(): boolean
---@return boolean
---@param withChildren boolean
function CS.UnityEngine.ParticleSystem:IsAlive(withChildren) end
---@overload fun(count:number): void
---@param emitParams CS.UnityEngine.ParticleSystem.EmitParams
---@param count number
function CS.UnityEngine.ParticleSystem:Emit(emitParams, count) end
function CS.UnityEngine.ParticleSystem.ResetPreMappedBufferMemory() end
return CS.UnityEngine.ParticleSystem
