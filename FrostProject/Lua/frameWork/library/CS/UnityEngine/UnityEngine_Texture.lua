---@class CS.UnityEngine.Texture : CS.UnityEngine.Object
---@field public masterTextureLimit number
---@field public anisotropicFiltering number
---@field public width number
---@field public height number
---@field public dimension number
---@field public isReadable boolean
---@field public wrapMode number
---@field public wrapModeU number
---@field public wrapModeV number
---@field public wrapModeW number
---@field public filterMode number
---@field public anisoLevel number
---@field public mipMapBias number
---@field public texelSize CS.UnityEngine.Vector2
---@field public updateCount number
---@field public imageContentsHash CS.UnityEngine.Hash128
---@field public totalTextureMemory uint64
---@field public desiredTextureMemory uint64
---@field public targetTextureMemory uint64
---@field public currentTextureMemory uint64
---@field public nonStreamingTextureMemory uint64
---@field public streamingMipmapUploadCount uint64
---@field public streamingRendererCount uint64
---@field public streamingTextureCount uint64
---@field public nonStreamingTextureCount uint64
---@field public streamingTexturePendingLoadCount uint64
---@field public streamingTextureLoadingCount uint64
---@field public streamingTextureForceLoadAll boolean
---@field public streamingTextureDiscardUnusedMips boolean
CS.UnityEngine.Texture = { }
---@param forcedMin number
---@param globalMax number
function CS.UnityEngine.Texture.SetGlobalAnisotropicFilteringLimits(forcedMin, globalMax) end
---@return number
function CS.UnityEngine.Texture:GetNativeTexturePtr() end
function CS.UnityEngine.Texture:IncrementUpdateCount() end
function CS.UnityEngine.Texture.SetStreamingTextureMaterialDebugProperties() end
return CS.UnityEngine.Texture
