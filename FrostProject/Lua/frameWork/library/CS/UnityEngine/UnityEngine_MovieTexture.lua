---@class CS.UnityEngine.MovieTexture : CS.UnityEngine.Texture
---@field public audioClip CS.UnityEngine.AudioClip
---@field public loop boolean
---@field public isPlaying boolean
---@field public isReadyToPlay boolean
---@field public duration number
CS.UnityEngine.MovieTexture = { }
function CS.UnityEngine.MovieTexture:Play() end
function CS.UnityEngine.MovieTexture:Stop() end
function CS.UnityEngine.MovieTexture:Pause() end
return CS.UnityEngine.MovieTexture
