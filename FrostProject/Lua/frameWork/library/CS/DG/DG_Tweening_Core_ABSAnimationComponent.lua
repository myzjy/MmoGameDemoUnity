---@class DG.Tweening.Core.ABSAnimationComponent : CS.UnityEngine.MonoBehaviour
---@field public updateType number
---@field public isSpeedBased boolean
---@field public hasOnStart boolean
---@field public hasOnPlay boolean
---@field public hasOnUpdate boolean
---@field public hasOnStepComplete boolean
---@field public hasOnComplete boolean
---@field public hasOnTweenCreated boolean
---@field public hasOnRewind boolean
---@field public onStart CS.UnityEngine.Events.UnityEvent
---@field public onPlay CS.UnityEngine.Events.UnityEvent
---@field public onUpdate CS.UnityEngine.Events.UnityEvent
---@field public onStepComplete CS.UnityEngine.Events.UnityEvent
---@field public onComplete CS.UnityEngine.Events.UnityEvent
---@field public onTweenCreated CS.UnityEngine.Events.UnityEvent
---@field public onRewind CS.UnityEngine.Events.UnityEvent
---@field public tween DG.Tweening.Tween
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
