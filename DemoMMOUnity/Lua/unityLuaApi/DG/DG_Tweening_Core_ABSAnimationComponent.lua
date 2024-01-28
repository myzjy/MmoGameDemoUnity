---@class DG.Tweening.Core.ABSAnimationComponent : UnityEngine.MonoBehaviour
---@field public updateType number
---@field public isSpeedBased boolean
---@field public hasOnStart boolean
---@field public hasOnPlay boolean
---@field public hasOnUpdate boolean
---@field public hasOnStepComplete boolean
---@field public hasOnComplete boolean
---@field public hasOnTweenCreated boolean
---@field public hasOnRewind boolean
---@field public onStart UnityEngine.Events.UnityEvent
---@field public onPlay UnityEngine.Events.UnityEvent
---@field public onUpdate UnityEngine.Events.UnityEvent
---@field public onStepComplete UnityEngine.Events.UnityEvent
---@field public onComplete UnityEngine.Events.UnityEvent
---@field public onTweenCreated UnityEngine.Events.UnityEvent
---@field public onRewind UnityEngine.Events.UnityEvent
---@field public tween DG.Tweening.Tween

---@type DG.Tweening.Core.ABSAnimationComponent
DG.Tweening.Core.ABSAnimationComponent = { }
function DG.Tweening.Core.ABSAnimationComponent:DOPlay() end
function DG.Tweening.Core.ABSAnimationComponent:DOPlayBackwards() end
function DG.Tweening.Core.ABSAnimationComponent:DOPlayForward() end
function DG.Tweening.Core.ABSAnimationComponent:DOPause() end
function DG.Tweening.Core.ABSAnimationComponent:DOTogglePause() end
function DG.Tweening.Core.ABSAnimationComponent:DORewind() end
---@param fromHere boolean
function DG.Tweening.Core.ABSAnimationComponent:DORestart(fromHere) end
function DG.Tweening.Core.ABSAnimationComponent:DOComplete() end
function DG.Tweening.Core.ABSAnimationComponent:DOKill() end
return DG.Tweening.Core.ABSAnimationComponent
