---@class DG.Tweening.Tween : DG.Tweening.Core.ABSSequentiable
---@field public timeScale number
---@field public isBackwards boolean
---@field public id System.Object
---@field public target System.Object
---@field public onPlay (fun():void)
---@field public onPause (fun():void)
---@field public onRewind (fun():void)
---@field public onUpdate (fun():void)
---@field public onStepComplete (fun():void)
---@field public onComplete (fun():void)
---@field public onKill (fun():void)
---@field public onWaypointChange (fun(value:number):void)
---@field public easeOvershootOrAmplitude number
---@field public easePeriod number
---@field public fullPosition number

---@type DG.Tweening.Tween
DG.Tweening.Tween = { }
return DG.Tweening.Tween
