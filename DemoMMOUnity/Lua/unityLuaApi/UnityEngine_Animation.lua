---@class UnityEngine.Animation : UnityEngine.Behaviour
---@field public clip UnityEngine.AnimationClip
---@field public playAutomatically boolean
---@field public wrapMode number
---@field public isPlaying boolean
---@field public Item UnityEngine.AnimationState
---@field public animatePhysics boolean
---@field public cullingType number
---@field public localBounds UnityEngine.Bounds

---@type UnityEngine.Animation
UnityEngine.Animation = { }
---@return UnityEngine.Animation
function UnityEngine.Animation.New() end
---@overload fun(): void
---@param name string
function UnityEngine.Animation:Stop(name) end
---@overload fun(): void
---@param name string
function UnityEngine.Animation:Rewind(name) end
function UnityEngine.Animation:Sample() end
---@return boolean
---@param name string
function UnityEngine.Animation:IsPlaying(name) end
---@overload fun(): boolean
---@overload fun(mode:number): boolean
---@overload fun(animation:string): boolean
---@return boolean
---@param animation string
---@param mode number
function UnityEngine.Animation:Play(animation, mode) end
---@overload fun(animation:string): void
---@overload fun(animation:string, fadeLength:number): void
---@param animation string
---@param fadeLength number
---@param mode number
function UnityEngine.Animation:CrossFade(animation, fadeLength, mode) end
---@overload fun(animation:string): void
---@overload fun(animation:string, targetWeight:number): void
---@param animation string
---@param targetWeight number
---@param fadeLength number
function UnityEngine.Animation:Blend(animation, targetWeight, fadeLength) end
---@overload fun(animation:string): UnityEngine.AnimationState
---@overload fun(animation:string, fadeLength:number): UnityEngine.AnimationState
---@overload fun(animation:string, fadeLength:number, queue:number): UnityEngine.AnimationState
---@return UnityEngine.AnimationState
---@param animation string
---@param fadeLength number
---@param queue number
---@param mode number
function UnityEngine.Animation:CrossFadeQueued(animation, fadeLength, queue, mode) end
---@overload fun(animation:string): UnityEngine.AnimationState
---@overload fun(animation:string, queue:number): UnityEngine.AnimationState
---@return UnityEngine.AnimationState
---@param animation string
---@param queue number
---@param mode number
function UnityEngine.Animation:PlayQueued(animation, queue, mode) end
---@overload fun(clip:UnityEngine.AnimationClip, newName:string): void
---@overload fun(clip:UnityEngine.AnimationClip, newName:string, firstFrame:number, lastFrame:number): void
---@param clip UnityEngine.AnimationClip
---@param newName string
---@param firstFrame number
---@param lastFrame number
---@param addLoopFrame boolean
function UnityEngine.Animation:AddClip(clip, newName, firstFrame, lastFrame, addLoopFrame) end
---@overload fun(clip:UnityEngine.AnimationClip): void
---@param clipName string
function UnityEngine.Animation:RemoveClip(clipName) end
---@return number
function UnityEngine.Animation:GetClipCount() end
---@param layer number
function UnityEngine.Animation:SyncLayer(layer) end
---@return System.Collections.IEnumerator
function UnityEngine.Animation:GetEnumerator() end
---@return UnityEngine.AnimationClip
---@param name string
function UnityEngine.Animation:GetClip(name) end
return UnityEngine.Animation
