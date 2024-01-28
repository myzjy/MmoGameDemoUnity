---@class UnityEngine.AnimationState : UnityEngine.TrackedReference
---@field public enabled boolean
---@field public weight number
---@field public wrapMode number
---@field public time number
---@field public normalizedTime number
---@field public speed number
---@field public normalizedSpeed number
---@field public length number
---@field public layer number
---@field public clip UnityEngine.AnimationClip
---@field public name string
---@field public blendMode number

---@type UnityEngine.AnimationState
UnityEngine.AnimationState = { }
---@return UnityEngine.AnimationState
function UnityEngine.AnimationState.New() end
---@overload fun(mix:UnityEngine.Transform): void
---@param mix UnityEngine.Transform
---@param recursive boolean
function UnityEngine.AnimationState:AddMixingTransform(mix, recursive) end
---@param mix UnityEngine.Transform
function UnityEngine.AnimationState:RemoveMixingTransform(mix) end
return UnityEngine.AnimationState
