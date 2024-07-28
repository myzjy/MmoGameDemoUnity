---@class CS.UnityEngine.Texture2D : CS.UnityEngine.Texture
---@field public mipmapCount number
---@field public format number
---@field public whiteTexture CS.UnityEngine.Texture2D
---@field public blackTexture CS.UnityEngine.Texture2D
---@field public isReadable boolean
---@field public streamingMipmaps boolean
---@field public streamingMipmapsPriority number
---@field public requestedMipmapLevel number
---@field public desiredMipmapLevel number
---@field public loadingMipmapLevel number
---@field public loadedMipmapLevel number
---@field public alphaIsTransparency boolean
CS.UnityEngine.Texture2D = { }
---@overload fun(width:number, height:number): CS.UnityEngine.Texture2D
---@overload fun(width:number, height:number, format:number, flags:number): CS.UnityEngine.Texture2D
---@overload fun(width:number, height:number, textureFormat:number, mipChain:boolean): CS.UnityEngine.Texture2D
---@return CS.UnityEngine.Texture2D
---@param width number
---@param height number
---@param textureFormat number
---@param mipChain boolean
---@param linear boolean
function CS.UnityEngine.Texture2D.New(width, height, textureFormat, mipChain, linear) end
---@param highQuality boolean
function CS.UnityEngine.Texture2D:Compress(highQuality) end
function CS.UnityEngine.Texture2D:ClearRequestedMipmapLevel() end
---@return boolean
function CS.UnityEngine.Texture2D:IsRequestedMipmapLevelLoaded() end
---@param nativeTex number
function CS.UnityEngine.Texture2D:UpdateExternalTexture(nativeTex) end
---@return CS.System.Byte[]
function CS.UnityEngine.Texture2D:GetRawTextureData() end
---@overload fun(): CS.UnityEngine.Color[]
---@overload fun(miplevel:number): CS.UnityEngine.Color[]
---@overload fun(x:number, y:number, blockWidth:number, blockHeight:number): CS.UnityEngine.Color[]
---@return CS.UnityEngine.Color[]
---@param x number
---@param y number
---@param blockWidth number
---@param blockHeight number
---@param miplevel number
function CS.UnityEngine.Texture2D:GetPixels(x, y, blockWidth, blockHeight, miplevel) end
---@overload fun(): CS.UnityEngine.Color32[]
---@return CS.UnityEngine.Color32[]
---@param miplevel number
function CS.UnityEngine.Texture2D:GetPixels32(miplevel) end
---@overload fun(textures:CS.UnityEngine.Texture2D[], padding:number): CS.UnityEngine.Rect[]
---@overload fun(textures:CS.UnityEngine.Texture2D[], padding:number, maximumAtlasSize:number): CS.UnityEngine.Rect[]
---@return CS.UnityEngine.Rect[]
---@param textures CS.UnityEngine.Texture2D[]
---@param padding number
---@param maximumAtlasSize number
---@param makeNoLongerReadable boolean
function CS.UnityEngine.Texture2D:PackTextures(textures, padding, maximumAtlasSize, makeNoLongerReadable) end
---@return CS.UnityEngine.Texture2D
---@param width number
---@param height number
---@param format number
---@param mipChain boolean
---@param linear boolean
---@param nativeTex number
function CS.UnityEngine.Texture2D.CreateExternalTexture(width, height, format, mipChain, linear, nativeTex) end
---@param x number
---@param y number
---@param color CS.UnityEngine.Color
function CS.UnityEngine.Texture2D:SetPixel(x, y, color) end
---@overload fun(colors:CS.UnityEngine.Color[]): void
---@overload fun(colors:CS.UnityEngine.Color[], miplevel:number): void
---@overload fun(x:number, y:number, blockWidth:number, blockHeight:number, colors:CS.UnityEngine.Color[]): void
---@param x number
---@param y number
---@param blockWidth number
---@param blockHeight number
---@param colors CS.UnityEngine.Color[]
---@param miplevel number
function CS.UnityEngine.Texture2D:SetPixels(x, y, blockWidth, blockHeight, colors, miplevel) end
---@return CS.UnityEngine.Color
---@param x number
---@param y number
function CS.UnityEngine.Texture2D:GetPixel(x, y) end
---@return CS.UnityEngine.Color
---@param x number
---@param y number
function CS.UnityEngine.Texture2D:GetPixelBilinear(x, y) end
---@overload fun(data:CS.System.Byte[]): void
---@param data number
---@param size number
function CS.UnityEngine.Texture2D:LoadRawTextureData(data, size) end
---@overload fun(): void
---@overload fun(updateMipmaps:boolean): void
---@param updateMipmaps boolean
---@param makeNoLongerReadable boolean
function CS.UnityEngine.Texture2D:Apply(updateMipmaps, makeNoLongerReadable) end
---@overload fun(width:number, height:number): boolean
---@return boolean
---@param width number
---@param height number
---@param format number
---@param hasMipMap boolean
function CS.UnityEngine.Texture2D:Resize(width, height, format, hasMipMap) end
---@overload fun(source:CS.UnityEngine.Rect, destX:number, destY:number): void
---@param source CS.UnityEngine.Rect
---@param destX number
---@param destY number
---@param recalculateMipMaps boolean
function CS.UnityEngine.Texture2D:ReadPixels(source, destX, destY, recalculateMipMaps) end
---@return boolean
---@param sizes CS.UnityEngine.Vector2[]
---@param padding number
---@param atlasSize number
---@param results CS.System.Collections.Generic.List_UnityEngine.Rect
function CS.UnityEngine.Texture2D.GenerateAtlas(sizes, padding, atlasSize, results) end
---@overload fun(colors:CS.UnityEngine.Color32[]): void
---@overload fun(colors:CS.UnityEngine.Color32[], miplevel:number): void
---@overload fun(x:number, y:number, blockWidth:number, blockHeight:number, colors:CS.UnityEngine.Color32[]): void
---@param x number
---@param y number
---@param blockWidth number
---@param blockHeight number
---@param colors CS.UnityEngine.Color32[]
---@param miplevel number
function CS.UnityEngine.Texture2D:SetPixels32(x, y, blockWidth, blockHeight, colors, miplevel) end
return CS.UnityEngine.Texture2D
