---@class UnityEngine.AudioClip : UnityEngine.Object
---@field public length number
---@field public samples number
---@field public channels number
---@field public frequency number
---@field public loadType number
---@field public preloadAudioData boolean
---@field public ambisonic boolean
---@field public loadState number
---@field public loadInBackground boolean

---@type UnityEngine.AudioClip
UnityEngine.AudioClip = { }
---@return boolean
function UnityEngine.AudioClip:LoadAudioData() end
---@return boolean
function UnityEngine.AudioClip:UnloadAudioData() end
---@return boolean
---@param data System.Single[]
---@param offsetSamples number
function UnityEngine.AudioClip:GetData(data, offsetSamples) end
---@return boolean
---@param data System.Single[]
---@param offsetSamples number
function UnityEngine.AudioClip:SetData(data, offsetSamples) end
---@overload fun(name:string, lengthSamples:number, channels:number, frequency:number, stream:boolean): UnityEngine.AudioClip
---@overload fun(name:string, lengthSamples:number, channels:number, frequency:number, stream:boolean, pcmreadercallback:(fun(data:System.Single[]):void)): UnityEngine.AudioClip
---@return UnityEngine.AudioClip
---@param name string
---@param lengthSamples number
---@param channels number
---@param frequency number
---@param stream boolean
---@param pcmreadercallback (fun(data:System.Single[]):void)
---@param pcmsetpositioncallback (fun(position:number):void)
function UnityEngine.AudioClip.Create(name, lengthSamples, channels, frequency, stream, pcmreadercallback, pcmsetpositioncallback) end
return UnityEngine.AudioClip
