---@class UnityEngine.RenderTexture : UnityEngine.Texture
---@field public width number
---@field public height number
---@field public dimension number
---@field public useMipMap boolean
---@field public sRGB boolean
---@field public format number
---@field public vrUsage number
---@field public memorylessMode number
---@field public autoGenerateMips boolean
---@field public volumeDepth number
---@field public antiAliasing number
---@field public bindTextureMS boolean
---@field public enableRandomWrite boolean
---@field public useDynamicScale boolean
---@field public isPowerOfTwo boolean
---@field public active UnityEngine.RenderTexture
---@field public colorBuffer UnityEngine.RenderBuffer
---@field public depthBuffer UnityEngine.RenderBuffer
---@field public depth number
---@field public descriptor UnityEngine.RenderTextureDescriptor

---@type UnityEngine.RenderTexture
UnityEngine.RenderTexture = { }
---@overload fun(desc:UnityEngine.RenderTextureDescriptor): UnityEngine.RenderTexture
---@overload fun(textureToCopy:UnityEngine.RenderTexture): UnityEngine.RenderTexture
---@overload fun(width:number, height:number, depth:number): UnityEngine.RenderTexture
---@overload fun(width:number, height:number, depth:number, format:number): UnityEngine.RenderTexture
---@overload fun(width:number, height:number, depth:number, format:number): UnityEngine.RenderTexture
---@return UnityEngine.RenderTexture
---@param width number
---@param height number
---@param depth number
---@param format number
---@param readWrite number
function UnityEngine.RenderTexture.New(width, height, depth, format, readWrite) end
---@return number
function UnityEngine.RenderTexture:GetNativeDepthBufferPtr() end
---@overload fun(): void
---@param discardColor boolean
---@param discardDepth boolean
function UnityEngine.RenderTexture:DiscardContents(discardColor, discardDepth) end
function UnityEngine.RenderTexture:MarkRestoreExpected() end
---@overload fun(): void
---@param target UnityEngine.RenderTexture
function UnityEngine.RenderTexture:ResolveAntiAliasedSurface(target) end
---@param propertyName string
function UnityEngine.RenderTexture:SetGlobalShaderProperty(propertyName) end
---@return boolean
function UnityEngine.RenderTexture:Create() end
function UnityEngine.RenderTexture:Release() end
---@return boolean
function UnityEngine.RenderTexture:IsCreated() end
function UnityEngine.RenderTexture:GenerateMips() end
---@param equirect UnityEngine.RenderTexture
---@param eye number
function UnityEngine.RenderTexture:ConvertToEquirect(equirect, eye) end
---@return boolean
---@param rt UnityEngine.RenderTexture
function UnityEngine.RenderTexture.SupportsStencil(rt) end
---@param temp UnityEngine.RenderTexture
function UnityEngine.RenderTexture.ReleaseTemporary(temp) end
---@overload fun(desc:UnityEngine.RenderTextureDescriptor): UnityEngine.RenderTexture
---@overload fun(width:number, height:number): UnityEngine.RenderTexture
---@overload fun(width:number, height:number, depthBuffer:number): UnityEngine.RenderTexture
---@overload fun(width:number, height:number, depthBuffer:number, format:number): UnityEngine.RenderTexture
---@overload fun(width:number, height:number, depthBuffer:number, format:number, readWrite:number): UnityEngine.RenderTexture
---@overload fun(width:number, height:number, depthBuffer:number, format:number, readWrite:number, antiAliasing:number): UnityEngine.RenderTexture
---@overload fun(width:number, height:number, depthBuffer:number, format:number, readWrite:number, antiAliasing:number, memorylessMode:number): UnityEngine.RenderTexture
---@overload fun(width:number, height:number, depthBuffer:number, format:number, readWrite:number, antiAliasing:number, memorylessMode:number, vrUsage:number): UnityEngine.RenderTexture
---@return UnityEngine.RenderTexture
---@param width number
---@param height number
---@param depthBuffer number
---@param format number
---@param readWrite number
---@param antiAliasing number
---@param memorylessMode number
---@param vrUsage number
---@param useDynamicScale boolean
function UnityEngine.RenderTexture.GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing, memorylessMode, vrUsage, useDynamicScale) end
return UnityEngine.RenderTexture
