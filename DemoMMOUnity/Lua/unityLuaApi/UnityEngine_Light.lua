---@class UnityEngine.Light : UnityEngine.Behaviour
---@field public type number
---@field public spotAngle number
---@field public color UnityEngine.Color
---@field public colorTemperature number
---@field public intensity number
---@field public bounceIntensity number
---@field public shadowCustomResolution number
---@field public shadowBias number
---@field public shadowNormalBias number
---@field public shadowNearPlane number
---@field public range number
---@field public flare UnityEngine.Flare
---@field public bakingOutput UnityEngine.LightBakingOutput
---@field public cullingMask number
---@field public lightShadowCasterMode number
---@field public shadowRadius number
---@field public shadowAngle number
---@field public shadows number
---@field public shadowStrength number
---@field public shadowResolution number
---@field public layerShadowCullDistances System.Single[]
---@field public cookieSize number
---@field public cookie UnityEngine.Texture
---@field public renderMode number
---@field public areaSize UnityEngine.Vector2
---@field public lightmapBakeType number
---@field public commandBufferCount number

---@type UnityEngine.Light
UnityEngine.Light = { }
---@return UnityEngine.Light
function UnityEngine.Light.New() end
function UnityEngine.Light:Reset() end
function UnityEngine.Light:SetLightDirty() end
---@overload fun(evt:number, buffer:UnityEngine.Rendering.CommandBuffer): void
---@param evt number
---@param buffer UnityEngine.Rendering.CommandBuffer
---@param shadowPassMask number
function UnityEngine.Light:AddCommandBuffer(evt, buffer, shadowPassMask) end
---@overload fun(evt:number, buffer:UnityEngine.Rendering.CommandBuffer, queueType:number): void
---@param evt number
---@param buffer UnityEngine.Rendering.CommandBuffer
---@param shadowPassMask number
---@param queueType number
function UnityEngine.Light:AddCommandBufferAsync(evt, buffer, shadowPassMask, queueType) end
---@param evt number
---@param buffer UnityEngine.Rendering.CommandBuffer
function UnityEngine.Light:RemoveCommandBuffer(evt, buffer) end
---@param evt number
function UnityEngine.Light:RemoveCommandBuffers(evt) end
function UnityEngine.Light:RemoveAllCommandBuffers() end
---@return UnityEngine.Rendering.CommandBuffer[]
---@param evt number
function UnityEngine.Light:GetCommandBuffers(evt) end
---@return UnityEngine.Light[]
---@param t number
---@param layer number
function UnityEngine.Light.GetLights(t, layer) end
return UnityEngine.Light
