---@class CS.UnityEngine.Video.VideoPlayer : CS.UnityEngine.Behaviour
---@field public source number
---@field public url string
---@field public clip CS.UnityEngine.Video.VideoClip
---@field public renderMode number
---@field public targetCamera CS.UnityEngine.Camera
---@field public targetTexture CS.UnityEngine.RenderTexture
---@field public targetMaterialRenderer CS.UnityEngine.Renderer
---@field public targetMaterialProperty string
---@field public aspectRatio number
---@field public targetCameraAlpha number
---@field public targetCamera3DLayout number
---@field public texture CS.UnityEngine.Texture
---@field public isPrepared boolean
---@field public waitForFirstFrame boolean
---@field public playOnAwake boolean
---@field public isPlaying boolean
---@field public isPaused boolean
---@field public canSetTime boolean
---@field public time number
---@field public frame int64
---@field public clockTime number
---@field public canStep boolean
---@field public canSetPlaybackSpeed boolean
---@field public playbackSpeed number
---@field public isLooping boolean
---@field public canSetTimeSource boolean
---@field public timeSource number
---@field public timeReference number
---@field public externalReferenceTime number
---@field public canSetSkipOnDrop boolean
---@field public skipOnDrop boolean
---@field public frameCount uint64
---@field public frameRate number
---@field public length number
---@field public width number
---@field public height number
---@field public pixelAspectRatioNumerator number
---@field public pixelAspectRatioDenominator number
---@field public audioTrackCount number
---@field public controlledAudioTrackMaxCount number
---@field public controlledAudioTrackCount number
---@field public audioOutputMode number
---@field public canSetDirectAudioVolume boolean
---@field public sendFrameReadyEvents boolean

CS.UnityEngine.Video.VideoPlayer = { }
---@return CS.UnityEngine.Video.VideoPlayer
function CS.UnityEngine.Video.VideoPlayer.New() end
function CS.UnityEngine.Video.VideoPlayer:Prepare() end
function CS.UnityEngine.Video.VideoPlayer:Play() end
function CS.UnityEngine.Video.VideoPlayer:Pause() end
function CS.UnityEngine.Video.VideoPlayer:Stop() end
function CS.UnityEngine.Video.VideoPlayer:StepForward() end
---@return string
---@param trackIndex number
function CS.UnityEngine.Video.VideoPlayer:GetAudioLanguageCode(trackIndex) end
---@return number
---@param trackIndex number
function CS.UnityEngine.Video.VideoPlayer:GetAudioChannelCount(trackIndex) end
---@return number
---@param trackIndex number
function CS.UnityEngine.Video.VideoPlayer:GetAudioSampleRate(trackIndex) end
---@param trackIndex number
---@param enabled boolean
function CS.UnityEngine.Video.VideoPlayer:EnableAudioTrack(trackIndex, enabled) end
---@return boolean
---@param trackIndex number
function CS.UnityEngine.Video.VideoPlayer:IsAudioTrackEnabled(trackIndex) end
---@return number
---@param trackIndex number
function CS.UnityEngine.Video.VideoPlayer:GetDirectAudioVolume(trackIndex) end
---@param trackIndex number
---@param volume number
function CS.UnityEngine.Video.VideoPlayer:SetDirectAudioVolume(trackIndex, volume) end
---@return boolean
---@param trackIndex number
function CS.UnityEngine.Video.VideoPlayer:GetDirectAudioMute(trackIndex) end
---@param trackIndex number
---@param mute boolean
function CS.UnityEngine.Video.VideoPlayer:SetDirectAudioMute(trackIndex, mute) end
---@return CS.UnityEngine.AudioSource
---@param trackIndex number
function CS.UnityEngine.Video.VideoPlayer:GetTargetAudioSource(trackIndex) end
---@param trackIndex number
---@param source CS.UnityEngine.AudioSource
function CS.UnityEngine.Video.VideoPlayer:SetTargetAudioSource(trackIndex, source) end
---@param value (fun(source:CS.UnityEngine.Video.VideoPlayer):void)
function CS.UnityEngine.Video.VideoPlayer:add_prepareCompleted(value) end
---@param value (fun(source:CS.UnityEngine.Video.VideoPlayer):void)
function CS.UnityEngine.Video.VideoPlayer:remove_prepareCompleted(value) end
---@param value (fun(source:CS.UnityEngine.Video.VideoPlayer):void)
function CS.UnityEngine.Video.VideoPlayer:add_loopPointReached(value) end
---@param value (fun(source:CS.UnityEngine.Video.VideoPlayer):void)
function CS.UnityEngine.Video.VideoPlayer:remove_loopPointReached(value) end
---@param value (fun(source:CS.UnityEngine.Video.VideoPlayer):void)
function CS.UnityEngine.Video.VideoPlayer:add_started(value) end
---@param value (fun(source:CS.UnityEngine.Video.VideoPlayer):void)
function CS.UnityEngine.Video.VideoPlayer:remove_started(value) end
---@param value (fun(source:CS.UnityEngine.Video.VideoPlayer):void)
function CS.UnityEngine.Video.VideoPlayer:add_frameDropped(value) end
---@param value (fun(source:CS.UnityEngine.Video.VideoPlayer):void)
function CS.UnityEngine.Video.VideoPlayer:remove_frameDropped(value) end
---@param value (fun(source:CS.UnityEngine.Video.VideoPlayer, message:string):void)
function CS.UnityEngine.Video.VideoPlayer:add_errorReceived(value) end
---@param value (fun(source:CS.UnityEngine.Video.VideoPlayer, message:string):void)
function CS.UnityEngine.Video.VideoPlayer:remove_errorReceived(value) end
---@param value (fun(source:CS.UnityEngine.Video.VideoPlayer):void)
function CS.UnityEngine.Video.VideoPlayer:add_seekCompleted(value) end
---@param value (fun(source:CS.UnityEngine.Video.VideoPlayer):void)
function CS.UnityEngine.Video.VideoPlayer:remove_seekCompleted(value) end
---@param value (fun(source:CS.UnityEngine.Video.VideoPlayer, seconds:number):void)
function CS.UnityEngine.Video.VideoPlayer:add_clockResyncOccurred(value) end
---@param value (fun(source:CS.UnityEngine.Video.VideoPlayer, seconds:number):void)
function CS.UnityEngine.Video.VideoPlayer:remove_clockResyncOccurred(value) end
---@param value (fun(source:CS.UnityEngine.Video.VideoPlayer, frameIdx:int64):void)
function CS.UnityEngine.Video.VideoPlayer:add_frameReady(value) end
---@param value (fun(source:CS.UnityEngine.Video.VideoPlayer, frameIdx:int64):void)
function CS.UnityEngine.Video.VideoPlayer:remove_frameReady(value) end
return CS.UnityEngine.Video.VideoPlayer
