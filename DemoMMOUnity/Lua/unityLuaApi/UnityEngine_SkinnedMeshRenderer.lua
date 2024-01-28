---@class UnityEngine.SkinnedMeshRenderer : UnityEngine.Renderer
---@field public quality number
---@field public updateWhenOffscreen boolean
---@field public forceMatrixRecalculationPerRender boolean
---@field public rootBone UnityEngine.Transform
---@field public bones UnityEngine.Transform[]
---@field public sharedMesh UnityEngine.Mesh
---@field public skinnedMotionVectors boolean
---@field public localBounds UnityEngine.Bounds

---@type UnityEngine.SkinnedMeshRenderer
UnityEngine.SkinnedMeshRenderer = { }
---@return UnityEngine.SkinnedMeshRenderer
function UnityEngine.SkinnedMeshRenderer.New() end
---@return number
---@param index number
function UnityEngine.SkinnedMeshRenderer:GetBlendShapeWeight(index) end
---@param index number
---@param value number
function UnityEngine.SkinnedMeshRenderer:SetBlendShapeWeight(index, value) end
---@param mesh UnityEngine.Mesh
function UnityEngine.SkinnedMeshRenderer:BakeMesh(mesh) end
return UnityEngine.SkinnedMeshRenderer
