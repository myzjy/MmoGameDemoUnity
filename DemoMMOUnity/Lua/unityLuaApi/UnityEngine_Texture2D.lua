---@class UnityEngine.Texture2D : UnityEngine.Texture
---@field public mipmapCount number
---@field public format number
---@field public whiteTexture UnityEngine.Texture2D
---@field public blackTexture UnityEngine.Texture2D
---@field public isReadable boolean
---@field public streamingMipmaps boolean
---@field public streamingMipmapsPriority number
---@field public requestedMipmapLevel number
---@field public desiredMipmapLevel number
---@field public loadingMipmapLevel number
---@field public loadedMipmapLevel number
---@field public alphaIsTransparency boolean

---@type UnityEngine.Texture2D
UnityEngine.Texture2D = { }
---@overload fun(width:number, height:number): UnityEngine.Texture2D
---@overload fun(width:number, height:number, format:number, flags:number): UnityEngine.Texture2D
---@overload fun(width:number, height:number, textureFormat:number, mipChain:boolean): UnityEngine.Texture2D
---@return UnityEngine.Texture2D
---@param width number
---@param height number
---@param textureFormat number
---@param mipChain boolean
---@param linear boolean
function UnityEngine.Texture2D.New(width, height, textureFormat, mipChain, linear) end
---@param highQuality boolean
function UnityEngine.Texture2D:Compress(highQuality) end
function UnityEngine.Texture2D:ClearRequestedMipmapLevel() end
---@return boolean
function UnityEngine.Texture2D:IsRequestedMipmapLevelLoaded() end
---@param nativeTex number
function UnityEngine.Texture2D:UpdateExternalTexture(nativeTex) end
---@return System.Byte[]
function UnityEngine.Texture2D:GetRawTextureData() end
---@overload fun(): UnityEngine.Color[]
---@overload fun(miplevel:number): UnityEngine.Color[]
---@overload fun(x:number, y:number, blockWidth:number, blockHeight:number): UnityEngine.Color[]
---@return UnityEngine.Color[]
---@param x number
---@param y number
---@param blockWidth number
---@param blockHeight number
---@param miplevel number
function UnityEngine.Texture2D:GetPixels(x, y, blockWidth, blockHeight, miplevel) end
---@overload fun(): UnityEngine.Color32[]
---@return UnityEngine.Color32[]
---@param miplevel number
function UnityEngine.Texture2D:GetPixels32(miplevel) end
---@overload fun(textures:UnityEngine.Texture2D[], padding:number): UnityEngine.Rect[]
---@overload fun(textures:UnityEngine.Texture2D[], padding:number, maximumAtlasSize:number): UnityEngine.Rect[]
---@return UnityEngine.Rect[]
---@param textures UnityEngine.Texture2D[]
---@param padding number
---@param maximumAtlasSize number
---@param makeNoLongerReadable boolean
function UnityEngine.Texture2D:PackTextures(textures, padding, maximumAtlasSize, makeNoLongerReadable) end
---@return UnityEngine.Texture2D
---@param width number
---@param height number
---@param format number
---@param mipChain boolean
---@param linear boolean
---@param nativeTex number
function UnityEngine.Texture2D.CreateExternalTexture(width, height, format, mipChain, linear, nativeTex) end
---@param x number
---@param y number
---@param color UnityEngine.Color
function UnityEngine.Texture2D:SetPixel(x, y, color) end
---@overload fun(colors:UnityEngine.Color[]): void
---@overload fun(colors:UnityEngine.Color[], miplevel:number): void
---@overload fun(x:number, y:number, blockWidth:number, blockHeight:number, colors:UnityEngine.Color[]): void
---@param x number
---@param y number
---@param blockWidth number
---@param blockHeight number
---@param colors UnityEngine.Color[]
---@param miplevel number
function UnityEngine.Texture2D:SetPixels(x, y, blockWidth, blockHeight, colors, miplevel) end
---@return UnityEngine.Color
---@param x number
---@param y number
function UnityEngine.Texture2D:GetPixel(x, y) end
---@return UnityEngine.Color
---@param x number
---@param y number
function UnityEngine.Texture2D:GetPixelBilinear(x, y) end
---@overload fun(data:System.Byte[]): void
---@param data number
---@param size number
function UnityEngine.Texture2D:LoadRawTextureData(data, size) end
---@overload fun(): void
---@overload fun(updateMipmaps:boolean): void
---@param updateMipmaps boolean
---@param makeNoLongerReadable boolean
function UnityEngine.Texture2D:Apply(updateMipmaps, makeNoLongerReadable) end
---@overload fun(width:number, height:number): boolean
---@return boolean
---@param width number
---@param height number
---@param format number
---@param hasMipMap boolean
function UnityEngine.Texture2D:Resize(width, height, format, hasMipMap) end
---@overload fun(source:UnityEngine.Rect, destX:number, destY:number): void
---@param source UnityEngine.Rect
---@param destX number
---@param destY number
---@param recalculateMipMaps boolean
function UnityEngine.Texture2D:ReadPixels(source, destX, destY, recalculateMipMaps) end
---@return boolean
---@param sizes UnityEngine.Vector2[]
---@param padding number
---@param atlasSize number
---@param results System.Collections.Generic.List_UnityEngine.Rect
function UnityEngine.Texture2D.GenerateAtlas(sizes, padding, atlasSize, results) end
---@overload fun(colors:UnityEngine.Color32[]): void
---@overload fun(colors:UnityEngine.Color32[], miplevel:number): void
---@overload fun(x:number, y:number, blockWidth:number, blockHeight:number, colors:UnityEngine.Color32[]): void
---@param x number
---@param y number
---@param blockWidth number
---@param blockHeight number
---@param colors UnityEngine.Color32[]
---@param miplevel number
function UnityEngine.Texture2D:SetPixels32(x, y, blockWidth, blockHeight, colors, miplevel) end
return UnityEngine.Texture2D
