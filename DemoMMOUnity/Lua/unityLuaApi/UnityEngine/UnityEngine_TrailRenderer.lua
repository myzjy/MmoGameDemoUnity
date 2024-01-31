---@class UnityEngine.TrailRenderer : UnityEngine.Renderer
---@field public time number
---@field public startWidth number
---@field public endWidth number
---@field public widthMultiplier number
---@field public autodestruct boolean
---@field public emitting boolean
---@field public numCornerVertices number
---@field public numCapVertices number
---@field public minVertexDistance number
---@field public startColor UnityEngine.Color
---@field public endColor UnityEngine.Color
---@field public positionCount number
---@field public shadowBias number
---@field public generateLightingData boolean
---@field public textureMode number
---@field public alignment number
---@field public widthCurve UnityEngine.AnimationCurve
---@field public colorGradient UnityEngine.Gradient

---@type UnityEngine.TrailRenderer
UnityEngine.TrailRenderer = { }
---@return UnityEngine.TrailRenderer
function UnityEngine.TrailRenderer.New() end
---@param index number
---@param position UnityEngine.Vector3
function UnityEngine.TrailRenderer:SetPosition(index, position) end
---@return UnityEngine.Vector3
---@param index number
function UnityEngine.TrailRenderer:GetPosition(index) end
function UnityEngine.TrailRenderer:Clear() end
---@overload fun(mesh:UnityEngine.Mesh, useTransform:boolean): void
---@param mesh UnityEngine.Mesh
---@param camera UnityEngine.Camera
---@param useTransform boolean
function UnityEngine.TrailRenderer:BakeMesh(mesh, camera, useTransform) end
---@return number
---@param positions UnityEngine.Vector3[]
function UnityEngine.TrailRenderer:GetPositions(positions) end
---@param positions UnityEngine.Vector3[]
function UnityEngine.TrailRenderer:SetPositions(positions) end
---@param position UnityEngine.Vector3
function UnityEngine.TrailRenderer:AddPosition(position) end
---@param positions UnityEngine.Vector3[]
function UnityEngine.TrailRenderer:AddPositions(positions) end
return UnityEngine.TrailRenderer
