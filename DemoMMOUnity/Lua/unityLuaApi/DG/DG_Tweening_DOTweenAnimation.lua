---@class DG.Tweening.DOTweenAnimation : DG.Tweening.Core.ABSAnimationComponent
---@field public delay number
---@field public duration number
---@field public easeType number
---@field public easeCurve UnityEngine.AnimationCurve
---@field public loopType number
---@field public loops number
---@field public id string
---@field public isRelative boolean
---@field public isFrom boolean
---@field public isIndependentUpdate boolean
---@field public autoKill boolean
---@field public isActive boolean
---@field public isValid boolean
---@field public target UnityEngine.Component
---@field public animationType number
---@field public targetType number
---@field public forcedTargetType number
---@field public autoPlay boolean
---@field public useTargetAsV3 boolean
---@field public endValueFloat number
---@field public endValueV3 UnityEngine.Vector3
---@field public endValueV2 UnityEngine.Vector2
---@field public endValueColor UnityEngine.Color
---@field public endValueString string
---@field public endValueRect UnityEngine.Rect
---@field public endValueTransform UnityEngine.Transform
---@field public optionalBool0 boolean
---@field public optionalFloat0 number
---@field public optionalInt0 number
---@field public optionalRotationMode number
---@field public optionalScrambleMode number
---@field public optionalString string

---@type DG.Tweening.DOTweenAnimation
DG.Tweening.DOTweenAnimation = { }
---@return DG.Tweening.DOTweenAnimation
function DG.Tweening.DOTweenAnimation.New() end
function DG.Tweening.DOTweenAnimation:CreateTween() end
function DG.Tweening.DOTweenAnimation:DOPlay() end
function DG.Tweening.DOTweenAnimation:DOPlayBackwards() end
function DG.Tweening.DOTweenAnimation:DOPlayForward() end
function DG.Tweening.DOTweenAnimation:DOPause() end
function DG.Tweening.DOTweenAnimation:DOTogglePause() end
function DG.Tweening.DOTweenAnimation:DORewind() end
---@param fromHere boolean
function DG.Tweening.DOTweenAnimation:DORestart(fromHere) end
function DG.Tweening.DOTweenAnimation:DOComplete() end
function DG.Tweening.DOTweenAnimation:DOKill() end
---@param id string
function DG.Tweening.DOTweenAnimation:DOPlayById(id) end
---@param id string
function DG.Tweening.DOTweenAnimation:DOPlayAllById(id) end
---@param id string
function DG.Tweening.DOTweenAnimation:DOPauseAllById(id) end
---@param id string
function DG.Tweening.DOTweenAnimation:DOPlayBackwardsById(id) end
---@param id string
function DG.Tweening.DOTweenAnimation:DOPlayBackwardsAllById(id) end
---@param id string
function DG.Tweening.DOTweenAnimation:DOPlayForwardById(id) end
---@param id string
function DG.Tweening.DOTweenAnimation:DOPlayForwardAllById(id) end
function DG.Tweening.DOTweenAnimation:DOPlayNext() end
function DG.Tweening.DOTweenAnimation:DORewindAndPlayNext() end
---@param id string
function DG.Tweening.DOTweenAnimation:DORestartById(id) end
---@param id string
function DG.Tweening.DOTweenAnimation:DORestartAllById(id) end
---@return System.Collections.Generic.List_DG.Tweening.Tween
function DG.Tweening.DOTweenAnimation:GetTweens() end
---@return number
---@param t string
function DG.Tweening.DOTweenAnimation.TypeToDOTargetType(t) end
return DG.Tweening.DOTweenAnimation
