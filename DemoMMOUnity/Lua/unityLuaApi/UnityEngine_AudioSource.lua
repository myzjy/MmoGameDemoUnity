---@class UnityEngine.AudioSource : UnityEngine.AudioBehaviour
---@field public volume number
---@field public pitch number
---@field public time number
---@field public timeSamples number
---@field public clip UnityEngine.AudioClip
---@field public outputAudioMixerGroup UnityEngine.Audio.AudioMixerGroup
---@field public isPlaying boolean
---@field public isVirtual boolean
---@field public loop boolean
---@field public ignoreListenerVolume boolean
---@field public playOnAwake boolean
---@field public ignoreListenerPause boolean
---@field public velocityUpdateMode number
---@field public panStereo number
---@field public spatialBlend number
---@field public spatialize boolean
---@field public spatializePostEffects boolean
---@field public reverbZoneMix number
---@field public bypassEffects boolean
---@field public bypassListenerEffects boolean
---@field public bypassReverbZones boolean
---@field public dopplerLevel number
---@field public spread number
---@field public priority number
---@field public mute boolean
---@field public minDistance number
---@field public maxDistance number
---@field public rolloffMode number

---@type UnityEngine.AudioSource
UnityEngine.AudioSource = { }
---@return UnityEngine.AudioSource
function UnityEngine.AudioSource.New() end
---@overload fun(): void
---@param delay uint64
function UnityEngine.AudioSource:Play(delay) end
---@param delay number
function UnityEngine.AudioSource:PlayDelayed(delay) end
---@param time number
function UnityEngine.AudioSource:PlayScheduled(time) end
---@param time number
function UnityEngine.AudioSource:SetScheduledStartTime(time) end
---@param time number
function UnityEngine.AudioSource:SetScheduledEndTime(time) end
function UnityEngine.AudioSource:Stop() end
function UnityEngine.AudioSource:Pause() end
function UnityEngine.AudioSource:UnPause() end
---@overload fun(clip:UnityEngine.AudioClip): void
---@param clip UnityEngine.AudioClip
---@param volumeScale number
function UnityEngine.AudioSource:PlayOneShot(clip, volumeScale) end
---@overload fun(clip:UnityEngine.AudioClip, position:UnityEngine.Vector3): void
---@param clip UnityEngine.AudioClip
---@param position UnityEngine.Vector3
---@param volume number
function UnityEngine.AudioSource.PlayClipAtPoint(clip, position, volume) end
---@param t number
---@param curve UnityEngine.AnimationCurve
function UnityEngine.AudioSource:SetCustomCurve(t, curve) end
---@return UnityEngine.AnimationCurve
---@param t number
function UnityEngine.AudioSource:GetCustomCurve(t) end
---@param samples System.Single[]
---@param channel number
function UnityEngine.AudioSource:GetOutputData(samples, channel) end
---@param samples System.Single[]
---@param channel number
---@param window number
function UnityEngine.AudioSource:GetSpectrumData(samples, channel, window) end
---@return boolean
---@param index number
---@param value number
function UnityEngine.AudioSource:SetSpatializerFloat(index, value) end
---@return boolean
---@param index number
---@param value System.Single
function UnityEngine.AudioSource:GetSpatializerFloat(index, value) end
---@return boolean
---@param index number
---@param value number
function UnityEngine.AudioSource:SetAmbisonicDecoderFloat(index, value) end
---@return boolean
---@param index number
---@param value System.Single
function UnityEngine.AudioSource:GetAmbisonicDecoderFloat(index, value) end
return UnityEngine.AudioSource
