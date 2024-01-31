---@class UnityEngine.WWW : UnityEngine.CustomYieldInstruction
---@field public assetBundle UnityEngine.AssetBundle
---@field public bytes System.Byte[]
---@field public bytesDownloaded number
---@field public error string
---@field public isDone boolean
---@field public progress number
---@field public responseHeaders System.Collections.Generic.Dictionary_System.String_System.String
---@field public text string
---@field public texture UnityEngine.Texture2D
---@field public textureNonReadable UnityEngine.Texture2D
---@field public threadPriority number
---@field public uploadProgress number
---@field public url string
---@field public keepWaiting boolean

---@type UnityEngine.WWW
UnityEngine.WWW = { }
---@overload fun(url:string): UnityEngine.WWW
---@overload fun(url:string, form:UnityEngine.WWWForm): UnityEngine.WWW
---@overload fun(url:string, postData:System.Byte[]): UnityEngine.WWW
---@overload fun(url:string, postData:System.Byte[], headers:System.Collections.Hashtable): UnityEngine.WWW
---@return UnityEngine.WWW
---@param url string
---@param postData System.Byte[]
---@param headers System.Collections.Generic.Dictionary_System.String_System.String
function UnityEngine.WWW.New(url, postData, headers) end
---@overload fun(s:string): string
---@return string
---@param s string
---@param e System.Text.Encoding
function UnityEngine.WWW.EscapeURL(s, e) end
---@overload fun(s:string): string
---@return string
---@param s string
---@param e System.Text.Encoding
function UnityEngine.WWW.UnEscapeURL(s, e) end
---@overload fun(url:string, version:number): UnityEngine.WWW
---@overload fun(url:string, hash:UnityEngine.Hash128): UnityEngine.WWW
---@overload fun(url:string, version:number, crc:number): UnityEngine.WWW
---@overload fun(url:string, hash:UnityEngine.Hash128, crc:number): UnityEngine.WWW
---@return UnityEngine.WWW
---@param url string
---@param cachedBundle UnityEngine.CachedAssetBundle
---@param crc number
function UnityEngine.WWW.LoadFromCacheOrDownload(url, cachedBundle, crc) end
---@param texture UnityEngine.Texture2D
function UnityEngine.WWW:LoadImageIntoTexture(texture) end
function UnityEngine.WWW:Dispose() end
---@overload fun(): UnityEngine.AudioClip
---@overload fun(threeD:boolean): UnityEngine.AudioClip
---@overload fun(threeD:boolean, stream:boolean): UnityEngine.AudioClip
---@return UnityEngine.AudioClip
---@param threeD boolean
---@param stream boolean
---@param audioType number
function UnityEngine.WWW:GetAudioClip(threeD, stream, audioType) end
---@overload fun(): UnityEngine.AudioClip
---@overload fun(threeD:boolean): UnityEngine.AudioClip
---@return UnityEngine.AudioClip
---@param threeD boolean
---@param audioType number
function UnityEngine.WWW:GetAudioClipCompressed(threeD, audioType) end
return UnityEngine.WWW
