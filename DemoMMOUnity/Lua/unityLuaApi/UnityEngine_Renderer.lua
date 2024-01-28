---@class UnityEngine.Renderer : UnityEngine.Component
---@field public bounds UnityEngine.Bounds
---@field public enabled boolean
---@field public isVisible boolean
---@field public shadowCastingMode number
---@field public receiveShadows boolean
---@field public motionVectorGenerationMode number
---@field public lightProbeUsage number
---@field public reflectionProbeUsage number
---@field public renderingLayerMask number
---@field public rendererPriority number
---@field public sortingLayerName string
---@field public sortingLayerID number
---@field public sortingOrder number
---@field public allowOcclusionWhenDynamic boolean
---@field public isPartOfStaticBatch boolean
---@field public worldToLocalMatrix UnityEngine.Matrix4x4
---@field public localToWorldMatrix UnityEngine.Matrix4x4
---@field public lightProbeProxyVolumeOverride UnityEngine.GameObject
---@field public probeAnchor UnityEngine.Transform
---@field public lightmapIndex number
---@field public realtimeLightmapIndex number
---@field public lightmapScaleOffset UnityEngine.Vector4
---@field public realtimeLightmapScaleOffset UnityEngine.Vector4
---@field public materials UnityEngine.Material[]
---@field public material UnityEngine.Material
---@field public sharedMaterial UnityEngine.Material
---@field public sharedMaterials UnityEngine.Material[]

---@type UnityEngine.Renderer
UnityEngine.Renderer = { }
---@return UnityEngine.Renderer
function UnityEngine.Renderer.New() end
---@return boolean
function UnityEngine.Renderer:HasPropertyBlock() end
---@overload fun(properties:UnityEngine.MaterialPropertyBlock): void
---@param properties UnityEngine.MaterialPropertyBlock
---@param materialIndex number
function UnityEngine.Renderer:SetPropertyBlock(properties, materialIndex) end
---@overload fun(properties:UnityEngine.MaterialPropertyBlock): void
---@param properties UnityEngine.MaterialPropertyBlock
---@param materialIndex number
function UnityEngine.Renderer:GetPropertyBlock(properties, materialIndex) end
---@param m System.Collections.Generic.List_UnityEngine.Material
function UnityEngine.Renderer:GetMaterials(m) end
---@param m System.Collections.Generic.List_UnityEngine.Material
function UnityEngine.Renderer:GetSharedMaterials(m) end
---@param result System.Collections.Generic.List_UnityEngine.Rendering.ReflectionProbeBlendInfo
function UnityEngine.Renderer:GetClosestReflectionProbes(result) end
return UnityEngine.Renderer
