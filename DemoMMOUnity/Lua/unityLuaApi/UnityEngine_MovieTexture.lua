---@class UnityEngine.MovieTexture : UnityEngine.Texture
---@field public audioClip UnityEngine.AudioClip
---@field public loop boolean
---@field public isPlaying boolean
---@field public isReadyToPlay boolean
---@field public duration number

---@type UnityEngine.MovieTexture
UnityEngine.MovieTexture = { }
function UnityEngine.MovieTexture:Play() end
function UnityEngine.MovieTexture:Stop() end
function UnityEngine.MovieTexture:Pause() end
return UnityEngine.MovieTexture
