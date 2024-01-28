---@class UnityEngine.AnimationClip : UnityEngine.Motion
---@field public events UnityEngine.AnimationEvent[]
---@field public length number
---@field public frameRate number
---@field public wrapMode number
---@field public localBounds UnityEngine.Bounds
---@field public legacy boolean
---@field public humanMotion boolean
---@field public empty boolean
---@field public hasGenericRootTransform boolean
---@field public hasMotionFloatCurves boolean
---@field public hasMotionCurves boolean
---@field public hasRootCurves boolean

---@type UnityEngine.AnimationClip
UnityEngine.AnimationClip = { }
---@return UnityEngine.AnimationClip
function UnityEngine.AnimationClip.New() end
---@param evt UnityEngine.AnimationEvent
function UnityEngine.AnimationClip:AddEvent(evt) end
---@param go UnityEngine.GameObject
---@param time number
function UnityEngine.AnimationClip:SampleAnimation(go, time) end
---@param relativePath string
---@param t string
---@param propertyName string
---@param curve UnityEngine.AnimationCurve
function UnityEngine.AnimationClip:SetCurve(relativePath, t, propertyName, curve) end
function UnityEngine.AnimationClip:EnsureQuaternionContinuity() end
function UnityEngine.AnimationClip:ClearCurves() end
return UnityEngine.AnimationClip
