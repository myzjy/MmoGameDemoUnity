---@class UnityEngine.Camera : UnityEngine.Behaviour
---@field public onPreCull (fun(cam:UnityEngine.Camera):void)
---@field public onPreRender (fun(cam:UnityEngine.Camera):void)
---@field public onPostRender (fun(cam:UnityEngine.Camera):void)
---@field public nearClipPlane number
---@field public farClipPlane number
---@field public fieldOfView number
---@field public renderingPath number
---@field public actualRenderingPath number
---@field public allowHDR boolean
---@field public allowMSAA boolean
---@field public allowDynamicResolution boolean
---@field public forceIntoRenderTexture boolean
---@field public orthographicSize number
---@field public orthographic boolean
---@field public opaqueSortMode number
---@field public transparencySortMode number
---@field public transparencySortAxis UnityEngine.Vector3
---@field public depth number
---@field public aspect number
---@field public velocity UnityEngine.Vector3
---@field public cullingMask number
---@field public eventMask number
---@field public layerCullSpherical boolean
---@field public cameraType number
---@field public layerCullDistances System.Single[]
---@field public useOcclusionCulling boolean
---@field public cullingMatrix UnityEngine.Matrix4x4
---@field public backgroundColor UnityEngine.Color
---@field public clearFlags number
---@field public depthTextureMode number
---@field public clearStencilAfterLightingPass boolean
---@field public usePhysicalProperties boolean
---@field public sensorSize UnityEngine.Vector2
---@field public lensShift UnityEngine.Vector2
---@field public focalLength number
---@field public gateFit number
---@field public rect UnityEngine.Rect
---@field public pixelRect UnityEngine.Rect
---@field public pixelWidth number
---@field public pixelHeight number
---@field public scaledPixelWidth number
---@field public scaledPixelHeight number
---@field public targetTexture UnityEngine.RenderTexture
---@field public activeTexture UnityEngine.RenderTexture
---@field public targetDisplay number
---@field public cameraToWorldMatrix UnityEngine.Matrix4x4
---@field public worldToCameraMatrix UnityEngine.Matrix4x4
---@field public projectionMatrix UnityEngine.Matrix4x4
---@field public nonJitteredProjectionMatrix UnityEngine.Matrix4x4
---@field public useJitteredProjectionMatrixForTransparentRendering boolean
---@field public previousViewProjectionMatrix UnityEngine.Matrix4x4
---@field public main UnityEngine.Camera
---@field public current UnityEngine.Camera
---@field public scene UnityEngine.SceneManagement.Scene
---@field public stereoEnabled boolean
---@field public stereoSeparation number
---@field public stereoConvergence number
---@field public areVRStereoViewMatricesWithinSingleCullTolerance boolean
---@field public stereoTargetEye number
---@field public stereoActiveEye number
---@field public allCamerasCount number
---@field public allCameras UnityEngine.Camera[]
---@field public commandBufferCount number

---@type UnityEngine.Camera
UnityEngine.Camera = { }
---@return UnityEngine.Camera
function UnityEngine.Camera.New() end
function UnityEngine.Camera:Reset() end
function UnityEngine.Camera:ResetTransparencySortSettings() end
function UnityEngine.Camera:ResetAspect() end
function UnityEngine.Camera:ResetCullingMatrix() end
---@param shader UnityEngine.Shader
---@param replacementTag string
function UnityEngine.Camera:SetReplacementShader(shader, replacementTag) end
function UnityEngine.Camera:ResetReplacementShader() end
---@overload fun(colorBuffer:UnityEngine.RenderBuffer, depthBuffer:UnityEngine.RenderBuffer): void
---@param colorBuffer UnityEngine.RenderBuffer[]
---@param depthBuffer UnityEngine.RenderBuffer
function UnityEngine.Camera:SetTargetBuffers(colorBuffer, depthBuffer) end
function UnityEngine.Camera:ResetWorldToCameraMatrix() end
function UnityEngine.Camera:ResetProjectionMatrix() end
---@return UnityEngine.Matrix4x4
---@param clipPlane UnityEngine.Vector4
function UnityEngine.Camera:CalculateObliqueMatrix(clipPlane) end
---@overload fun(position:UnityEngine.Vector3): UnityEngine.Vector3
---@return UnityEngine.Vector3
---@param position UnityEngine.Vector3
---@param eye number
function UnityEngine.Camera:WorldToScreenPoint(position, eye) end
---@overload fun(position:UnityEngine.Vector3): UnityEngine.Vector3
---@return UnityEngine.Vector3
---@param position UnityEngine.Vector3
---@param eye number
function UnityEngine.Camera:WorldToViewportPoint(position, eye) end
---@overload fun(position:UnityEngine.Vector3): UnityEngine.Vector3
---@return UnityEngine.Vector3
---@param position UnityEngine.Vector3
---@param eye number
function UnityEngine.Camera:ViewportToWorldPoint(position, eye) end
---@overload fun(position:UnityEngine.Vector3): UnityEngine.Vector3
---@return UnityEngine.Vector3
---@param position UnityEngine.Vector3
---@param eye number
function UnityEngine.Camera:ScreenToWorldPoint(position, eye) end
---@return UnityEngine.Vector3
---@param position UnityEngine.Vector3
function UnityEngine.Camera:ScreenToViewportPoint(position) end
---@return UnityEngine.Vector3
---@param position UnityEngine.Vector3
function UnityEngine.Camera:ViewportToScreenPoint(position) end
---@overload fun(pos:UnityEngine.Vector3): UnityEngine.Ray
---@return UnityEngine.Ray
---@param pos UnityEngine.Vector3
---@param eye number
function UnityEngine.Camera:ViewportPointToRay(pos, eye) end
---@overload fun(pos:UnityEngine.Vector3): UnityEngine.Ray
---@return UnityEngine.Ray
---@param pos UnityEngine.Vector3
---@param eye number
function UnityEngine.Camera:ScreenPointToRay(pos, eye) end
---@param viewport UnityEngine.Rect
---@param z number
---@param eye number
---@param outCorners UnityEngine.Vector3[]
function UnityEngine.Camera:CalculateFrustumCorners(viewport, z, eye, outCorners) end
---@param output UnityEngine.Matrix4x4
---@param focalLength number
---@param sensorSize UnityEngine.Vector2
---@param lensShift UnityEngine.Vector2
---@param nearClip number
---@param farClip number
---@param gateFitParameters UnityEngine.Camera.GateFitParameters
function UnityEngine.Camera.CalculateProjectionMatrixFromPhysicalProperties(output, focalLength, sensorSize, lensShift, nearClip, farClip, gateFitParameters) end
---@return number
---@param focalLength number
---@param sensorSize number
function UnityEngine.Camera.FocalLengthToFOV(focalLength, sensorSize) end
---@return number
---@param fov number
---@param sensorSize number
function UnityEngine.Camera.FOVToFocalLength(fov, sensorSize) end
---@return UnityEngine.Matrix4x4
---@param eye number
function UnityEngine.Camera:GetStereoNonJitteredProjectionMatrix(eye) end
---@return UnityEngine.Matrix4x4
---@param eye number
function UnityEngine.Camera:GetStereoViewMatrix(eye) end
---@param eye number
function UnityEngine.Camera:CopyStereoDeviceProjectionMatrixToNonJittered(eye) end
---@return UnityEngine.Matrix4x4
---@param eye number
function UnityEngine.Camera:GetStereoProjectionMatrix(eye) end
---@param eye number
---@param matrix UnityEngine.Matrix4x4
function UnityEngine.Camera:SetStereoProjectionMatrix(eye, matrix) end
function UnityEngine.Camera:ResetStereoProjectionMatrices() end
---@param eye number
---@param matrix UnityEngine.Matrix4x4
function UnityEngine.Camera:SetStereoViewMatrix(eye, matrix) end
function UnityEngine.Camera:ResetStereoViewMatrices() end
---@return number
---@param cameras UnityEngine.Camera[]
function UnityEngine.Camera.GetAllCameras(cameras) end
---@overload fun(cubemap:UnityEngine.Cubemap): boolean
---@overload fun(cubemap:UnityEngine.RenderTexture): boolean
---@overload fun(cubemap:UnityEngine.Cubemap, faceMask:number): boolean
---@overload fun(cubemap:UnityEngine.RenderTexture, faceMask:number): boolean
---@return boolean
---@param cubemap UnityEngine.RenderTexture
---@param faceMask number
---@param stereoEye number
function UnityEngine.Camera:RenderToCubemap(cubemap, faceMask, stereoEye) end
function UnityEngine.Camera:Render() end
---@param shader UnityEngine.Shader
---@param replacementTag string
function UnityEngine.Camera:RenderWithShader(shader, replacementTag) end
function UnityEngine.Camera:RenderDontRestore() end
---@param cur UnityEngine.Camera
function UnityEngine.Camera.SetupCurrent(cur) end
---@param other UnityEngine.Camera
function UnityEngine.Camera:CopyFrom(other) end
---@param evt number
function UnityEngine.Camera:RemoveCommandBuffers(evt) end
function UnityEngine.Camera:RemoveAllCommandBuffers() end
---@param evt number
---@param buffer UnityEngine.Rendering.CommandBuffer
function UnityEngine.Camera:AddCommandBuffer(evt, buffer) end
---@param evt number
---@param buffer UnityEngine.Rendering.CommandBuffer
---@param queueType number
function UnityEngine.Camera:AddCommandBufferAsync(evt, buffer, queueType) end
---@param evt number
---@param buffer UnityEngine.Rendering.CommandBuffer
function UnityEngine.Camera:RemoveCommandBuffer(evt, buffer) end
---@return UnityEngine.Rendering.CommandBuffer[]
---@param evt number
function UnityEngine.Camera:GetCommandBuffers(evt) end
return UnityEngine.Camera
