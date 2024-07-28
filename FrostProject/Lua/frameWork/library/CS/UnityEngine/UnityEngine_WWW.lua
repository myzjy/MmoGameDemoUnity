---@class CS.UnityEngine.WWW : CS.UnityEngine.CustomYieldInstruction
---@field public assetBundle CS.UnityEngine.AssetBundle
---@field public bytes CS.System.Byte[]
---@field public bytesDownloaded number
---@field public error string
---@field public isDone boolean
---@field public progress number
---@field public responseHeaders CS.System.Collections.Generic.Dictionary_System.String_System.String
---@field public text string
---@field public texture CS.UnityEngine.Texture2D
---@field public textureNonReadable CS.UnityEngine.Texture2D
---@field public threadPriority number
---@field public uploadProgress number
---@field public url string
---@field public keepWaiting boolean
CS.UnityEngine.WWW = { }
---@overload fun(url:string): CS.UnityEngine.WWW
---@overload fun(url:string, form:CS.UnityEngine.WWWForm): CS.UnityEngine.WWW
---@overload fun(url:string, postData:CS.System.Byte[]): CS.UnityEngine.WWW
---@overload fun(url:string, postData:CS.System.Byte[], headers:CS.System.Collections.Hashtable): CS.UnityEngine.WWW
---@return CS.UnityEngine.WWW
---@param url string
---@param postData CS.System.Byte[]
---@param headers CS.System.Collections.Generic.Dictionary_System.String_System.String
function CS.UnityEngine.WWW.New(url, postData, headers) end
---@overload fun(s:string): string
---@return string
---@param s string
---@param e CS.System.Text.Encoding
function CS.UnityEngine.WWW.EscapeURL(s, e) end
---@overload fun(s:string): string
---@return string
---@param s string
---@param e CS.System.Text.Encoding
function CS.UnityEngine.WWW.UnEscapeURL(s, e) end
---@overload fun(url:string, version:number): CS.UnityEngine.WWW
---@overload fun(url:string, hash:CS.UnityEngine.Hash128): CS.UnityEngine.WWW
---@overload fun(url:string, version:number, crc:number): CS.UnityEngine.WWW
---@overload fun(url:string, hash:CS.UnityEngine.Hash128, crc:number): CS.UnityEngine.WWW
---@return CS.UnityEngine.WWW
---@param url string
---@param cachedBundle CS.UnityEngine.CachedAssetBundle
---@param crc number
function CS.UnityEngine.WWW.LoadFromCacheOrDownload(url, cachedBundle, crc) end
---@param texture CS.UnityEngine.Texture2D
function CS.UnityEngine.WWW:LoadImageIntoTexture(texture) end
function CS.UnityEngine.WWW:Dispose() end
---@overload fun(): CS.UnityEngine.AudioClip
---@overload fun(threeD:boolean): CS.UnityEngine.AudioClip
---@overload fun(threeD:boolean, stream:boolean): CS.UnityEngine.AudioClip
---@return CS.UnityEngine.AudioClip
---@param threeD boolean
---@param stream boolean
---@param audioType number
function CS.UnityEngine.WWW:GetAudioClip(threeD, stream, audioType) end
---@overload fun(): CS.UnityEngine.AudioClip
---@overload fun(threeD:boolean): CS.UnityEngine.AudioClip
---@return CS.UnityEngine.AudioClip
---@param threeD boolean
---@param audioType number
function CS.UnityEngine.WWW:GetAudioClipCompressed(threeD, audioType) end
return CS.UnityEngine.WWW
