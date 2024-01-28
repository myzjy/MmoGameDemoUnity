---@class DG.Tweening.DOTween
---@field public Version string
---@field public useSafeMode boolean
---@field public showUnityEditorReport boolean
---@field public timeScale number
---@field public useSmoothDeltaTime boolean
---@field public maxSmoothUnscaledTime number
---@field public drawGizmos boolean
---@field public defaultUpdateType number
---@field public defaultTimeScaleIndependent boolean
---@field public defaultAutoPlay number
---@field public defaultAutoKill boolean
---@field public defaultLoopType number
---@field public defaultRecyclable boolean
---@field public defaultEaseType number
---@field public defaultEaseOvershootOrAmplitude number
---@field public defaultEasePeriod number
---@field public logBehaviour number

---@type DG.Tweening.DOTween
DG.Tweening.DOTween = { }
---@return DG.Tweening.DOTween
function DG.Tweening.DOTween.New() end
---@return DG.Tweening.IDOTweenInit
---@param recycleAllByDefault System.Nullable_System.Boolean
---@param useSafeMode System.Nullable_System.Boolean
---@param logBehaviour System.Nullable_DG.Tweening.LogBehaviour
function DG.Tweening.DOTween.Init(recycleAllByDefault, useSafeMode, logBehaviour) end
---@param tweenersCapacity number
---@param sequencesCapacity number
function DG.Tweening.DOTween.SetTweensCapacity(tweenersCapacity, sequencesCapacity) end
---@param destroy boolean
function DG.Tweening.DOTween.Clear(destroy) end
function DG.Tweening.DOTween.ClearCachedTweens() end
---@return number
function DG.Tweening.DOTween.Validate() end
---@param deltaTime number
---@param unscaledDeltaTime number
function DG.Tweening.DOTween.ManualUpdate(deltaTime, unscaledDeltaTime) end
---@overload fun(getter:(fun():number), setter:(fun(pNewValue:number):void), endValue:number, duration:number): DG.Tweening.Core.TweenerCore_System.Single_System.Single_DG.Tweening.Plugins.Options.FloatOptions
---@overload fun(getter:(fun():number), setter:(fun(pNewValue:number):void), endValue:number, duration:number): DG.Tweening.Core.TweenerCore_System.Double_System.Double_DG.Tweening.Plugins.Options.NoOptions
---@overload fun(getter:(fun():number), setter:(fun(pNewValue:number):void), endValue:number, duration:number): DG.Tweening.Tweener
---@overload fun(getter:(fun():number), setter:(fun(pNewValue:number):void), endValue:number, duration:number): DG.Tweening.Tweener
---@overload fun(getter:(fun():int64), setter:(fun(pNewValue:int64):void), endValue:int64, duration:number): DG.Tweening.Tweener
---@overload fun(getter:(fun():uint64), setter:(fun(pNewValue:uint64):void), endValue:uint64, duration:number): DG.Tweening.Tweener
---@overload fun(getter:(fun():string), setter:(fun(pNewValue:string):void), endValue:string, duration:number): DG.Tweening.Core.TweenerCore_System.String_System.String_DG.Tweening.Plugins.Options.StringOptions
---@overload fun(getter:(fun():UnityEngine.Vector2), setter:(fun(pNewValue:UnityEngine.Vector2):void), endValue:UnityEngine.Vector2, duration:number): DG.Tweening.Core.TweenerCore_UnityEngine.Vector2_UnityEngine.Vector2_DG.Tweening.Plugins.Options.VectorOptions
---@overload fun(getter:(fun():UnityEngine.Vector3), setter:(fun(pNewValue:UnityEngine.Vector3):void), endValue:UnityEngine.Vector3, duration:number): DG.Tweening.Core.TweenerCore_UnityEngine.Vector3_UnityEngine.Vector3_DG.Tweening.Plugins.Options.VectorOptions
---@overload fun(getter:(fun():UnityEngine.Vector4), setter:(fun(pNewValue:UnityEngine.Vector4):void), endValue:UnityEngine.Vector4, duration:number): DG.Tweening.Core.TweenerCore_UnityEngine.Vector4_UnityEngine.Vector4_DG.Tweening.Plugins.Options.VectorOptions
---@overload fun(getter:(fun():UnityEngine.Quaternion), setter:(fun(pNewValue:UnityEngine.Quaternion):void), endValue:UnityEngine.Vector3, duration:number): DG.Tweening.Core.TweenerCore_UnityEngine.Quaternion_UnityEngine.Vector3_DG.Tweening.Plugins.Options.QuaternionOptions
---@overload fun(getter:(fun():UnityEngine.Color), setter:(fun(pNewValue:UnityEngine.Color):void), endValue:UnityEngine.Color, duration:number): DG.Tweening.Core.TweenerCore_UnityEngine.Color_UnityEngine.Color_DG.Tweening.Plugins.Options.ColorOptions
---@overload fun(getter:(fun():UnityEngine.Rect), setter:(fun(pNewValue:UnityEngine.Rect):void), endValue:UnityEngine.Rect, duration:number): DG.Tweening.Core.TweenerCore_UnityEngine.Rect_UnityEngine.Rect_DG.Tweening.Plugins.Options.RectOptions
---@overload fun(getter:(fun():UnityEngine.RectOffset), setter:(fun(pNewValue:UnityEngine.RectOffset):void), endValue:UnityEngine.RectOffset, duration:number): DG.Tweening.Tweener
---@return DG.Tweening.Core.TweenerCore_System.Single_System.Single_DG.Tweening.Plugins.Options.FloatOptions
---@param setter (fun(pNewValue:number):void)
---@param startValue number
---@param endValue number
---@param duration number
function DG.Tweening.DOTween.To(setter, startValue, endValue, duration) end
---@return DG.Tweening.Core.TweenerCore_UnityEngine.Vector3_UnityEngine.Vector3_DG.Tweening.Plugins.Options.VectorOptions
---@param getter (fun():UnityEngine.Vector3)
---@param setter (fun(pNewValue:UnityEngine.Vector3):void)
---@param endValue number
---@param duration number
---@param axisConstraint number
function DG.Tweening.DOTween.ToAxis(getter, setter, endValue, duration, axisConstraint) end
---@return DG.Tweening.Tweener
---@param getter (fun():UnityEngine.Color)
---@param setter (fun(pNewValue:UnityEngine.Color):void)
---@param endValue number
---@param duration number
function DG.Tweening.DOTween.ToAlpha(getter, setter, endValue, duration) end
---@return DG.Tweening.Core.TweenerCore_UnityEngine.Vector3_UnityEngine.Vector3_Array_DG.Tweening.Plugins.Options.Vector3ArrayOptions
---@param getter (fun():UnityEngine.Vector3)
---@param setter (fun(pNewValue:UnityEngine.Vector3):void)
---@param direction UnityEngine.Vector3
---@param duration number
---@param vibrato number
---@param elasticity number
function DG.Tweening.DOTween.Punch(getter, setter, direction, duration, vibrato, elasticity) end
---@overload fun(getter:(fun():UnityEngine.Vector3), setter:(fun(pNewValue:UnityEngine.Vector3):void), duration:number, strength:UnityEngine.Vector3, vibrato:number, randomness:number, fadeOut:boolean): DG.Tweening.Core.TweenerCore_UnityEngine.Vector3_UnityEngine.Vector3_Array_DG.Tweening.Plugins.Options.Vector3ArrayOptions
---@return DG.Tweening.Core.TweenerCore_UnityEngine.Vector3_UnityEngine.Vector3_Array_DG.Tweening.Plugins.Options.Vector3ArrayOptions
---@param getter (fun():UnityEngine.Vector3)
---@param setter (fun(pNewValue:UnityEngine.Vector3):void)
---@param duration number
---@param strength number
---@param vibrato number
---@param randomness number
---@param ignoreZAxis boolean
---@param fadeOut boolean
function DG.Tweening.DOTween.Shake(getter, setter, duration, strength, vibrato, randomness, ignoreZAxis, fadeOut) end
---@return DG.Tweening.Core.TweenerCore_UnityEngine.Vector3_UnityEngine.Vector3_Array_DG.Tweening.Plugins.Options.Vector3ArrayOptions
---@param getter (fun():UnityEngine.Vector3)
---@param setter (fun(pNewValue:UnityEngine.Vector3):void)
---@param endValues UnityEngine.Vector3[]
---@param durations System.Single[]
function DG.Tweening.DOTween.ToArray(getter, setter, endValues, durations) end
---@return DG.Tweening.Sequence
function DG.Tweening.DOTween.Sequence() end
---@return number
---@param withCallbacks boolean
function DG.Tweening.DOTween.CompleteAll(withCallbacks) end
---@return number
---@param targetOrId System.Object
---@param withCallbacks boolean
function DG.Tweening.DOTween.Complete(targetOrId, withCallbacks) end
---@return number
function DG.Tweening.DOTween.FlipAll() end
---@return number
---@param targetOrId System.Object
function DG.Tweening.DOTween.Flip(targetOrId) end
---@return number
---@param to number
---@param andPlay boolean
function DG.Tweening.DOTween.GotoAll(to, andPlay) end
---@return number
---@param targetOrId System.Object
---@param to number
---@param andPlay boolean
function DG.Tweening.DOTween.Goto(targetOrId, to, andPlay) end
---@overload fun(complete:boolean): number
---@return number
---@param complete boolean
---@param idsOrTargetsToExclude System.Object[]
function DG.Tweening.DOTween.KillAll(complete, idsOrTargetsToExclude) end
---@return number
---@param targetOrId System.Object
---@param complete boolean
function DG.Tweening.DOTween.Kill(targetOrId, complete) end
---@return number
function DG.Tweening.DOTween.PauseAll() end
---@return number
---@param targetOrId System.Object
function DG.Tweening.DOTween.Pause(targetOrId) end
---@return number
function DG.Tweening.DOTween.PlayAll() end
---@overload fun(targetOrId:System.Object): number
---@return number
---@param target System.Object
---@param id System.Object
function DG.Tweening.DOTween.Play(target, id) end
---@return number
function DG.Tweening.DOTween.PlayBackwardsAll() end
---@overload fun(targetOrId:System.Object): number
---@return number
---@param target System.Object
---@param id System.Object
function DG.Tweening.DOTween.PlayBackwards(target, id) end
---@return number
function DG.Tweening.DOTween.PlayForwardAll() end
---@overload fun(targetOrId:System.Object): number
---@return number
---@param target System.Object
---@param id System.Object
function DG.Tweening.DOTween.PlayForward(target, id) end
---@return number
---@param includeDelay boolean
function DG.Tweening.DOTween.RestartAll(includeDelay) end
---@overload fun(targetOrId:System.Object, includeDelay:boolean, changeDelayTo:number): number
---@return number
---@param target System.Object
---@param id System.Object
---@param includeDelay boolean
---@param changeDelayTo number
function DG.Tweening.DOTween.Restart(target, id, includeDelay, changeDelayTo) end
---@return number
---@param includeDelay boolean
function DG.Tweening.DOTween.RewindAll(includeDelay) end
---@return number
---@param targetOrId System.Object
---@param includeDelay boolean
function DG.Tweening.DOTween.Rewind(targetOrId, includeDelay) end
---@return number
function DG.Tweening.DOTween.SmoothRewindAll() end
---@return number
---@param targetOrId System.Object
function DG.Tweening.DOTween.SmoothRewind(targetOrId) end
---@return number
function DG.Tweening.DOTween.TogglePauseAll() end
---@return number
---@param targetOrId System.Object
function DG.Tweening.DOTween.TogglePause(targetOrId) end
---@return boolean
---@param targetOrId System.Object
---@param alsoCheckIfIsPlaying boolean
function DG.Tweening.DOTween.IsTweening(targetOrId, alsoCheckIfIsPlaying) end
---@return number
function DG.Tweening.DOTween.TotalPlayingTweens() end
---@return System.Collections.Generic.List_DG.Tweening.Tween
function DG.Tweening.DOTween.PlayingTweens() end
---@return System.Collections.Generic.List_DG.Tweening.Tween
function DG.Tweening.DOTween.PausedTweens() end
---@return System.Collections.Generic.List_DG.Tweening.Tween
---@param id System.Object
---@param playingOnly boolean
function DG.Tweening.DOTween.TweensById(id, playingOnly) end
---@return System.Collections.Generic.List_DG.Tweening.Tween
---@param target System.Object
---@param playingOnly boolean
function DG.Tweening.DOTween.TweensByTarget(target, playingOnly) end
return DG.Tweening.DOTween
