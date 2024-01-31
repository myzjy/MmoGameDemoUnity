---@class UnityEngine.Video.VideoPlayer : UnityEngine.Behaviour
---@field public source number
---@field public url string
---@field public clip UnityEngine.Video.VideoClip
---@field public renderMode number
---@field public targetCamera UnityEngine.Camera
---@field public targetTexture UnityEngine.RenderTexture
---@field public targetMaterialRenderer UnityEngine.Renderer
---@field public targetMaterialProperty string
---@field public aspectRatio number
---@field public targetCameraAlpha number
---@field public targetCamera3DLayout number
---@field public texture UnityEngine.Texture
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

---@type UnityEngine.Video.VideoPlayer
UnityEngine.Video.VideoPlayer = { }
---@return UnityEngine.Video.VideoPlayer
function UnityEngine.Video.VideoPlayer.New() end
function UnityEngine.Video.VideoPlayer:Prepare() end
function UnityEngine.Video.VideoPlayer:Play() end
function UnityEngine.Video.VideoPlayer:Pause() end
function UnityEngine.Video.VideoPlayer:Stop() end
function UnityEngine.Video.VideoPlayer:StepForward() end
---@return string
---@param trackIndex number
function UnityEngine.Video.VideoPlayer:GetAudioLanguageCode(trackIndex) end
---@return number
---@param trackIndex number
function UnityEngine.Video.VideoPlayer:GetAudioChannelCount(trackIndex) end
---@return number
---@param trackIndex number
function UnityEngine.Video.VideoPlayer:GetAudioSampleRate(trackIndex) end
---@param trackIndex number
---@param enabled boolean
function UnityEngine.Video.VideoPlayer:EnableAudioTrack(trackIndex, enabled) end
---@return boolean
---@param trackIndex number
function UnityEngine.Video.VideoPlayer:IsAudioTrackEnabled(trackIndex) end
---@return number
---@param trackIndex number
function UnityEngine.Video.VideoPlayer:GetDirectAudioVolume(trackIndex) end
---@param trackIndex number
---@param volume number
function UnityEngine.Video.VideoPlayer:SetDirectAudioVolume(trackIndex, volume) end
---@return boolean
---@param trackIndex number
function UnityEngine.Video.VideoPlayer:GetDirectAudioMute(trackIndex) end
---@param trackIndex number
---@param mute boolean
function UnityEngine.Video.VideoPlayer:SetDirectAudioMute(trackIndex, mute) end
---@return UnityEngine.AudioSource
---@param trackIndex number
function UnityEngine.Video.VideoPlayer:GetTargetAudioSource(trackIndex) end
---@param trackIndex number
---@param source UnityEngine.AudioSource
function UnityEngine.Video.VideoPlayer:SetTargetAudioSource(trackIndex, source) end
---@param value (fun(source:UnityEngine.Video.VideoPlayer):void)
function UnityEngine.Video.VideoPlayer:add_prepareCompleted(value) end
---@param value (fun(source:UnityEngine.Video.VideoPlayer):void)
function UnityEngine.Video.VideoPlayer:remove_prepareCompleted(value) end
---@param value (fun(source:UnityEngine.Video.VideoPlayer):void)
function UnityEngine.Video.VideoPlayer:add_loopPointReached(value) end
---@param value (fun(source:UnityEngine.Video.VideoPlayer):void)
function UnityEngine.Video.VideoPlayer:remove_loopPointReached(value) end
---@param value (fun(source:UnityEngine.Video.VideoPlayer):void)
function UnityEngine.Video.VideoPlayer:add_started(value) end
---@param value (fun(source:UnityEngine.Video.VideoPlayer):void)
function UnityEngine.Video.VideoPlayer:remove_started(value) end
---@param value (fun(source:UnityEngine.Video.VideoPlayer):void)
function UnityEngine.Video.VideoPlayer:add_frameDropped(value) end
---@param value (fun(source:UnityEngine.Video.VideoPlayer):void)
function UnityEngine.Video.VideoPlayer:remove_frameDropped(value) end
---@param value (fun(source:UnityEngine.Video.VideoPlayer, message:string):void)
function UnityEngine.Video.VideoPlayer:add_errorReceived(value) end
---@param value (fun(source:UnityEngine.Video.VideoPlayer, message:string):void)
function UnityEngine.Video.VideoPlayer:remove_errorReceived(value) end
---@param value (fun(source:UnityEngine.Video.VideoPlayer):void)
function UnityEngine.Video.VideoPlayer:add_seekCompleted(value) end
---@param value (fun(source:UnityEngine.Video.VideoPlayer):void)
function UnityEngine.Video.VideoPlayer:remove_seekCompleted(value) end
---@param value (fun(source:UnityEngine.Video.VideoPlayer, seconds:number):void)
function UnityEngine.Video.VideoPlayer:add_clockResyncOccurred(value) end
---@param value (fun(source:UnityEngine.Video.VideoPlayer, seconds:number):void)
function UnityEngine.Video.VideoPlayer:remove_clockResyncOccurred(value) end
---@param value (fun(source:UnityEngine.Video.VideoPlayer, frameIdx:int64):void)
function UnityEngine.Video.VideoPlayer:add_frameReady(value) end
---@param value (fun(source:UnityEngine.Video.VideoPlayer, frameIdx:int64):void)
function UnityEngine.Video.VideoPlayer:remove_frameReady(value) end
return UnityEngine.Video.VideoPlayer
