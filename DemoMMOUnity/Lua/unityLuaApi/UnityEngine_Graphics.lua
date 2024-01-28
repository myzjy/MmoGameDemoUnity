---@class UnityEngine.Graphics
---@field public activeColorGamut number
---@field public activeTier number
---@field public activeColorBuffer UnityEngine.RenderBuffer
---@field public activeDepthBuffer UnityEngine.RenderBuffer

---@type UnityEngine.Graphics
UnityEngine.Graphics = { }
---@return UnityEngine.Graphics
function UnityEngine.Graphics.New() end
function UnityEngine.Graphics.ClearRandomWriteTargets() end
---@param buffer UnityEngine.Rendering.CommandBuffer
function UnityEngine.Graphics.ExecuteCommandBuffer(buffer) end
---@param buffer UnityEngine.Rendering.CommandBuffer
---@param queueType number
function UnityEngine.Graphics.ExecuteCommandBufferAsync(buffer, queueType) end
---@overload fun(setup:UnityEngine.RenderTargetSetup): void
---@overload fun(rt:UnityEngine.RenderTexture): void
---@overload fun(colorBuffers:UnityEngine.RenderBuffer[], depthBuffer:UnityEngine.RenderBuffer): void
---@overload fun(rt:UnityEngine.RenderTexture, mipLevel:number): void
---@overload fun(colorBuffer:UnityEngine.RenderBuffer, depthBuffer:UnityEngine.RenderBuffer): void
---@overload fun(rt:UnityEngine.RenderTexture, mipLevel:number, face:number): void
---@overload fun(colorBuffer:UnityEngine.RenderBuffer, depthBuffer:UnityEngine.RenderBuffer, mipLevel:number): void
---@overload fun(rt:UnityEngine.RenderTexture, mipLevel:number, face:number, depthSlice:number): void
---@overload fun(colorBuffer:UnityEngine.RenderBuffer, depthBuffer:UnityEngine.RenderBuffer, mipLevel:number, face:number): void
---@param colorBuffer UnityEngine.RenderBuffer
---@param depthBuffer UnityEngine.RenderBuffer
---@param mipLevel number
---@param face number
---@param depthSlice number
function UnityEngine.Graphics.SetRenderTarget(colorBuffer, depthBuffer, mipLevel, face, depthSlice) end
---@overload fun(index:number, uav:UnityEngine.RenderTexture): void
---@overload fun(index:number, uav:UnityEngine.ComputeBuffer): void
---@param index number
---@param uav UnityEngine.ComputeBuffer
---@param preserveCounterValue boolean
function UnityEngine.Graphics.SetRandomWriteTarget(index, uav, preserveCounterValue) end
---@overload fun(src:UnityEngine.Texture, dst:UnityEngine.Texture): void
---@overload fun(src:UnityEngine.Texture, srcElement:number, dst:UnityEngine.Texture, dstElement:number): void
---@overload fun(src:UnityEngine.Texture, srcElement:number, srcMip:number, dst:UnityEngine.Texture, dstElement:number, dstMip:number): void
---@param src UnityEngine.Texture
---@param srcElement number
---@param srcMip number
---@param srcX number
---@param srcY number
---@param srcWidth number
---@param srcHeight number
---@param dst UnityEngine.Texture
---@param dstElement number
---@param dstMip number
---@param dstX number
---@param dstY number
function UnityEngine.Graphics.CopyTexture(src, srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, dst, dstElement, dstMip, dstX, dstY) end
---@overload fun(src:UnityEngine.Texture, dst:UnityEngine.Texture): boolean
---@return boolean
---@param src UnityEngine.Texture
---@param srcElement number
---@param dst UnityEngine.Texture
---@param dstElement number
function UnityEngine.Graphics.ConvertTexture(src, srcElement, dst, dstElement) end
---@overload fun(): UnityEngine.Rendering.GPUFence
---@return UnityEngine.Rendering.GPUFence
---@param stage number
function UnityEngine.Graphics.CreateGPUFence(stage) end
---@overload fun(fence:UnityEngine.Rendering.GPUFence): void
---@param fence UnityEngine.Rendering.GPUFence
---@param stage number
function UnityEngine.Graphics.WaitOnGPUFence(fence, stage) end
---@overload fun(screenRect:UnityEngine.Rect, texture:UnityEngine.Texture): void
---@overload fun(screenRect:UnityEngine.Rect, texture:UnityEngine.Texture, mat:UnityEngine.Material): void
---@overload fun(screenRect:UnityEngine.Rect, texture:UnityEngine.Texture, mat:UnityEngine.Material, pass:number): void
---@overload fun(screenRect:UnityEngine.Rect, texture:UnityEngine.Texture, leftBorder:number, rightBorder:number, topBorder:number, bottomBorder:number): void
---@overload fun(screenRect:UnityEngine.Rect, texture:UnityEngine.Texture, sourceRect:UnityEngine.Rect, leftBorder:number, rightBorder:number, topBorder:number, bottomBorder:number): void
---@overload fun(screenRect:UnityEngine.Rect, texture:UnityEngine.Texture, leftBorder:number, rightBorder:number, topBorder:number, bottomBorder:number, mat:UnityEngine.Material): void
---@overload fun(screenRect:UnityEngine.Rect, texture:UnityEngine.Texture, leftBorder:number, rightBorder:number, topBorder:number, bottomBorder:number, mat:UnityEngine.Material, pass:number): void
---@overload fun(screenRect:UnityEngine.Rect, texture:UnityEngine.Texture, sourceRect:UnityEngine.Rect, leftBorder:number, rightBorder:number, topBorder:number, bottomBorder:number, color:UnityEngine.Color): void
---@overload fun(screenRect:UnityEngine.Rect, texture:UnityEngine.Texture, sourceRect:UnityEngine.Rect, leftBorder:number, rightBorder:number, topBorder:number, bottomBorder:number, mat:UnityEngine.Material): void
---@overload fun(screenRect:UnityEngine.Rect, texture:UnityEngine.Texture, sourceRect:UnityEngine.Rect, leftBorder:number, rightBorder:number, topBorder:number, bottomBorder:number, mat:UnityEngine.Material, pass:number): void
---@overload fun(screenRect:UnityEngine.Rect, texture:UnityEngine.Texture, sourceRect:UnityEngine.Rect, leftBorder:number, rightBorder:number, topBorder:number, bottomBorder:number, color:UnityEngine.Color, mat:UnityEngine.Material): void
---@param screenRect UnityEngine.Rect
---@param texture UnityEngine.Texture
---@param sourceRect UnityEngine.Rect
---@param leftBorder number
---@param rightBorder number
---@param topBorder number
---@param bottomBorder number
---@param color UnityEngine.Color
---@param mat UnityEngine.Material
---@param pass number
function UnityEngine.Graphics.DrawTexture(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, color, mat, pass) end
---@overload fun(mesh:UnityEngine.Mesh, matrix:UnityEngine.Matrix4x4): void
---@overload fun(mesh:UnityEngine.Mesh, matrix:UnityEngine.Matrix4x4, materialIndex:number): void
---@overload fun(mesh:UnityEngine.Mesh, position:UnityEngine.Vector3, rotation:UnityEngine.Quaternion): void
---@param mesh UnityEngine.Mesh
---@param position UnityEngine.Vector3
---@param rotation UnityEngine.Quaternion
---@param materialIndex number
function UnityEngine.Graphics.DrawMeshNow(mesh, position, rotation, materialIndex) end
---@overload fun(mesh:UnityEngine.Mesh, matrix:UnityEngine.Matrix4x4, material:UnityEngine.Material, layer:number): void
---@overload fun(mesh:UnityEngine.Mesh, matrix:UnityEngine.Matrix4x4, material:UnityEngine.Material, layer:number, camera:UnityEngine.Camera): void
---@overload fun(mesh:UnityEngine.Mesh, position:UnityEngine.Vector3, rotation:UnityEngine.Quaternion, material:UnityEngine.Material, layer:number): void
---@overload fun(mesh:UnityEngine.Mesh, matrix:UnityEngine.Matrix4x4, material:UnityEngine.Material, layer:number, camera:UnityEngine.Camera, submeshIndex:number): void
---@overload fun(mesh:UnityEngine.Mesh, position:UnityEngine.Vector3, rotation:UnityEngine.Quaternion, material:UnityEngine.Material, layer:number, camera:UnityEngine.Camera): void
---@overload fun(mesh:UnityEngine.Mesh, position:UnityEngine.Vector3, rotation:UnityEngine.Quaternion, material:UnityEngine.Material, layer:number, camera:UnityEngine.Camera, submeshIndex:number): void
---@overload fun(mesh:UnityEngine.Mesh, matrix:UnityEngine.Matrix4x4, material:UnityEngine.Material, layer:number, camera:UnityEngine.Camera, submeshIndex:number, properties:UnityEngine.MaterialPropertyBlock): void
---@overload fun(mesh:UnityEngine.Mesh, matrix:UnityEngine.Matrix4x4, material:UnityEngine.Material, layer:number, camera:UnityEngine.Camera, submeshIndex:number, properties:UnityEngine.MaterialPropertyBlock, castShadows:boolean): void
---@overload fun(mesh:UnityEngine.Mesh, position:UnityEngine.Vector3, rotation:UnityEngine.Quaternion, material:UnityEngine.Material, layer:number, camera:UnityEngine.Camera, submeshIndex:number, properties:UnityEngine.MaterialPropertyBlock): void
---@overload fun(mesh:UnityEngine.Mesh, matrix:UnityEngine.Matrix4x4, material:UnityEngine.Material, layer:number, camera:UnityEngine.Camera, submeshIndex:number, properties:UnityEngine.MaterialPropertyBlock, castShadows:number): void
---@overload fun(mesh:UnityEngine.Mesh, matrix:UnityEngine.Matrix4x4, material:UnityEngine.Material, layer:number, camera:UnityEngine.Camera, submeshIndex:number, properties:UnityEngine.MaterialPropertyBlock, castShadows:boolean, receiveShadows:boolean): void
---@overload fun(mesh:UnityEngine.Mesh, position:UnityEngine.Vector3, rotation:UnityEngine.Quaternion, material:UnityEngine.Material, layer:number, camera:UnityEngine.Camera, submeshIndex:number, properties:UnityEngine.MaterialPropertyBlock, castShadows:boolean): void
---@overload fun(mesh:UnityEngine.Mesh, matrix:UnityEngine.Matrix4x4, material:UnityEngine.Material, layer:number, camera:UnityEngine.Camera, submeshIndex:number, properties:UnityEngine.MaterialPropertyBlock, castShadows:number, receiveShadows:boolean): void
---@overload fun(mesh:UnityEngine.Mesh, position:UnityEngine.Vector3, rotation:UnityEngine.Quaternion, material:UnityEngine.Material, layer:number, camera:UnityEngine.Camera, submeshIndex:number, properties:UnityEngine.MaterialPropertyBlock, castShadows:number): void
---@overload fun(mesh:UnityEngine.Mesh, position:UnityEngine.Vector3, rotation:UnityEngine.Quaternion, material:UnityEngine.Material, layer:number, camera:UnityEngine.Camera, submeshIndex:number, properties:UnityEngine.MaterialPropertyBlock, castShadows:boolean, receiveShadows:boolean): void
---@overload fun(mesh:UnityEngine.Mesh, matrix:UnityEngine.Matrix4x4, material:UnityEngine.Material, layer:number, camera:UnityEngine.Camera, submeshIndex:number, properties:UnityEngine.MaterialPropertyBlock, castShadows:boolean, receiveShadows:boolean, useLightProbes:boolean): void
---@overload fun(mesh:UnityEngine.Mesh, matrix:UnityEngine.Matrix4x4, material:UnityEngine.Material, layer:number, camera:UnityEngine.Camera, submeshIndex:number, properties:UnityEngine.MaterialPropertyBlock, castShadows:number, receiveShadows:boolean, probeAnchor:UnityEngine.Transform): void
---@overload fun(mesh:UnityEngine.Mesh, position:UnityEngine.Vector3, rotation:UnityEngine.Quaternion, material:UnityEngine.Material, layer:number, camera:UnityEngine.Camera, submeshIndex:number, properties:UnityEngine.MaterialPropertyBlock, castShadows:number, receiveShadows:boolean): void
---@overload fun(mesh:UnityEngine.Mesh, position:UnityEngine.Vector3, rotation:UnityEngine.Quaternion, material:UnityEngine.Material, layer:number, camera:UnityEngine.Camera, submeshIndex:number, properties:UnityEngine.MaterialPropertyBlock, castShadows:boolean, receiveShadows:boolean, useLightProbes:boolean): void
---@overload fun(mesh:UnityEngine.Mesh, matrix:UnityEngine.Matrix4x4, material:UnityEngine.Material, layer:number, camera:UnityEngine.Camera, submeshIndex:number, properties:UnityEngine.MaterialPropertyBlock, castShadows:number, receiveShadows:boolean, probeAnchor:UnityEngine.Transform, useLightProbes:boolean): void
---@overload fun(mesh:UnityEngine.Mesh, position:UnityEngine.Vector3, rotation:UnityEngine.Quaternion, material:UnityEngine.Material, layer:number, camera:UnityEngine.Camera, submeshIndex:number, properties:UnityEngine.MaterialPropertyBlock, castShadows:number, receiveShadows:boolean, probeAnchor:UnityEngine.Transform): void
---@overload fun(mesh:UnityEngine.Mesh, matrix:UnityEngine.Matrix4x4, material:UnityEngine.Material, layer:number, camera:UnityEngine.Camera, submeshIndex:number, properties:UnityEngine.MaterialPropertyBlock, castShadows:number, receiveShadows:boolean, probeAnchor:UnityEngine.Transform, lightProbeUsage:number): void
---@overload fun(mesh:UnityEngine.Mesh, matrix:UnityEngine.Matrix4x4, material:UnityEngine.Material, layer:number, camera:UnityEngine.Camera, submeshIndex:number, properties:UnityEngine.MaterialPropertyBlock, castShadows:number, receiveShadows:boolean, probeAnchor:UnityEngine.Transform, lightProbeUsage:number, lightProbeProxyVolume:UnityEngine.LightProbeProxyVolume): void
---@param mesh UnityEngine.Mesh
---@param position UnityEngine.Vector3
---@param rotation UnityEngine.Quaternion
---@param material UnityEngine.Material
---@param layer number
---@param camera UnityEngine.Camera
---@param submeshIndex number
---@param properties UnityEngine.MaterialPropertyBlock
---@param castShadows number
---@param receiveShadows boolean
---@param probeAnchor UnityEngine.Transform
---@param useLightProbes boolean
function UnityEngine.Graphics.DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, useLightProbes) end
---@overload fun(mesh:UnityEngine.Mesh, submeshIndex:number, material:UnityEngine.Material, matrices:UnityEngine.Matrix4x4[]): void
---@overload fun(mesh:UnityEngine.Mesh, submeshIndex:number, material:UnityEngine.Material, matrices:System.Collections.Generic.List_UnityEngine.Matrix4x4): void
---@overload fun(mesh:UnityEngine.Mesh, submeshIndex:number, material:UnityEngine.Material, matrices:UnityEngine.Matrix4x4[], count:number): void
---@overload fun(mesh:UnityEngine.Mesh, submeshIndex:number, material:UnityEngine.Material, matrices:System.Collections.Generic.List_UnityEngine.Matrix4x4, properties:UnityEngine.MaterialPropertyBlock): void
---@overload fun(mesh:UnityEngine.Mesh, submeshIndex:number, material:UnityEngine.Material, matrices:UnityEngine.Matrix4x4[], count:number, properties:UnityEngine.MaterialPropertyBlock): void
---@overload fun(mesh:UnityEngine.Mesh, submeshIndex:number, material:UnityEngine.Material, matrices:System.Collections.Generic.List_UnityEngine.Matrix4x4, properties:UnityEngine.MaterialPropertyBlock, castShadows:number): void
---@overload fun(mesh:UnityEngine.Mesh, submeshIndex:number, material:UnityEngine.Material, matrices:UnityEngine.Matrix4x4[], count:number, properties:UnityEngine.MaterialPropertyBlock, castShadows:number): void
---@overload fun(mesh:UnityEngine.Mesh, submeshIndex:number, material:UnityEngine.Material, matrices:System.Collections.Generic.List_UnityEngine.Matrix4x4, properties:UnityEngine.MaterialPropertyBlock, castShadows:number, receiveShadows:boolean): void
---@overload fun(mesh:UnityEngine.Mesh, submeshIndex:number, material:UnityEngine.Material, matrices:System.Collections.Generic.List_UnityEngine.Matrix4x4, properties:UnityEngine.MaterialPropertyBlock, castShadows:number, receiveShadows:boolean, layer:number): void
---@overload fun(mesh:UnityEngine.Mesh, submeshIndex:number, material:UnityEngine.Material, matrices:UnityEngine.Matrix4x4[], count:number, properties:UnityEngine.MaterialPropertyBlock, castShadows:number, receiveShadows:boolean): void
---@overload fun(mesh:UnityEngine.Mesh, submeshIndex:number, material:UnityEngine.Material, matrices:UnityEngine.Matrix4x4[], count:number, properties:UnityEngine.MaterialPropertyBlock, castShadows:number, receiveShadows:boolean, layer:number): void
---@overload fun(mesh:UnityEngine.Mesh, submeshIndex:number, material:UnityEngine.Material, matrices:System.Collections.Generic.List_UnityEngine.Matrix4x4, properties:UnityEngine.MaterialPropertyBlock, castShadows:number, receiveShadows:boolean, layer:number, camera:UnityEngine.Camera): void
---@overload fun(mesh:UnityEngine.Mesh, submeshIndex:number, material:UnityEngine.Material, matrices:UnityEngine.Matrix4x4[], count:number, properties:UnityEngine.MaterialPropertyBlock, castShadows:number, receiveShadows:boolean, layer:number, camera:UnityEngine.Camera): void
---@overload fun(mesh:UnityEngine.Mesh, submeshIndex:number, material:UnityEngine.Material, matrices:System.Collections.Generic.List_UnityEngine.Matrix4x4, properties:UnityEngine.MaterialPropertyBlock, castShadows:number, receiveShadows:boolean, layer:number, camera:UnityEngine.Camera, lightProbeUsage:number): void
---@overload fun(mesh:UnityEngine.Mesh, submeshIndex:number, material:UnityEngine.Material, matrices:System.Collections.Generic.List_UnityEngine.Matrix4x4, properties:UnityEngine.MaterialPropertyBlock, castShadows:number, receiveShadows:boolean, layer:number, camera:UnityEngine.Camera, lightProbeUsage:number, lightProbeProxyVolume:UnityEngine.LightProbeProxyVolume): void
---@overload fun(mesh:UnityEngine.Mesh, submeshIndex:number, material:UnityEngine.Material, matrices:UnityEngine.Matrix4x4[], count:number, properties:UnityEngine.MaterialPropertyBlock, castShadows:number, receiveShadows:boolean, layer:number, camera:UnityEngine.Camera, lightProbeUsage:number): void
---@param mesh UnityEngine.Mesh
---@param submeshIndex number
---@param material UnityEngine.Material
---@param matrices UnityEngine.Matrix4x4[]
---@param count number
---@param properties UnityEngine.MaterialPropertyBlock
---@param castShadows number
---@param receiveShadows boolean
---@param layer number
---@param camera UnityEngine.Camera
---@param lightProbeUsage number
---@param lightProbeProxyVolume UnityEngine.LightProbeProxyVolume
function UnityEngine.Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, castShadows, receiveShadows, layer, camera, lightProbeUsage, lightProbeProxyVolume) end
---@overload fun(mesh:UnityEngine.Mesh, submeshIndex:number, material:UnityEngine.Material, bounds:UnityEngine.Bounds, bufferWithArgs:UnityEngine.ComputeBuffer): void
---@overload fun(mesh:UnityEngine.Mesh, submeshIndex:number, material:UnityEngine.Material, bounds:UnityEngine.Bounds, bufferWithArgs:UnityEngine.ComputeBuffer, argsOffset:number): void
---@overload fun(mesh:UnityEngine.Mesh, submeshIndex:number, material:UnityEngine.Material, bounds:UnityEngine.Bounds, bufferWithArgs:UnityEngine.ComputeBuffer, argsOffset:number, properties:UnityEngine.MaterialPropertyBlock): void
---@overload fun(mesh:UnityEngine.Mesh, submeshIndex:number, material:UnityEngine.Material, bounds:UnityEngine.Bounds, bufferWithArgs:UnityEngine.ComputeBuffer, argsOffset:number, properties:UnityEngine.MaterialPropertyBlock, castShadows:number): void
---@overload fun(mesh:UnityEngine.Mesh, submeshIndex:number, material:UnityEngine.Material, bounds:UnityEngine.Bounds, bufferWithArgs:UnityEngine.ComputeBuffer, argsOffset:number, properties:UnityEngine.MaterialPropertyBlock, castShadows:number, receiveShadows:boolean): void
---@overload fun(mesh:UnityEngine.Mesh, submeshIndex:number, material:UnityEngine.Material, bounds:UnityEngine.Bounds, bufferWithArgs:UnityEngine.ComputeBuffer, argsOffset:number, properties:UnityEngine.MaterialPropertyBlock, castShadows:number, receiveShadows:boolean, layer:number): void
---@overload fun(mesh:UnityEngine.Mesh, submeshIndex:number, material:UnityEngine.Material, bounds:UnityEngine.Bounds, bufferWithArgs:UnityEngine.ComputeBuffer, argsOffset:number, properties:UnityEngine.MaterialPropertyBlock, castShadows:number, receiveShadows:boolean, layer:number, camera:UnityEngine.Camera): void
---@overload fun(mesh:UnityEngine.Mesh, submeshIndex:number, material:UnityEngine.Material, bounds:UnityEngine.Bounds, bufferWithArgs:UnityEngine.ComputeBuffer, argsOffset:number, properties:UnityEngine.MaterialPropertyBlock, castShadows:number, receiveShadows:boolean, layer:number, camera:UnityEngine.Camera, lightProbeUsage:number): void
---@param mesh UnityEngine.Mesh
---@param submeshIndex number
---@param material UnityEngine.Material
---@param bounds UnityEngine.Bounds
---@param bufferWithArgs UnityEngine.ComputeBuffer
---@param argsOffset number
---@param properties UnityEngine.MaterialPropertyBlock
---@param castShadows number
---@param receiveShadows boolean
---@param layer number
---@param camera UnityEngine.Camera
---@param lightProbeUsage number
---@param lightProbeProxyVolume UnityEngine.LightProbeProxyVolume
function UnityEngine.Graphics.DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows, layer, camera, lightProbeUsage, lightProbeProxyVolume) end
---@overload fun(topology:number, vertexCount:number): void
---@param topology number
---@param vertexCount number
---@param instanceCount number
function UnityEngine.Graphics.DrawProcedural(topology, vertexCount, instanceCount) end
---@overload fun(topology:number, bufferWithArgs:UnityEngine.ComputeBuffer): void
---@param topology number
---@param bufferWithArgs UnityEngine.ComputeBuffer
---@param argsOffset number
function UnityEngine.Graphics.DrawProceduralIndirect(topology, bufferWithArgs, argsOffset) end
---@overload fun(source:UnityEngine.Texture, dest:UnityEngine.RenderTexture): void
---@overload fun(source:UnityEngine.Texture, mat:UnityEngine.Material): void
---@overload fun(source:UnityEngine.Texture, dest:UnityEngine.RenderTexture, mat:UnityEngine.Material): void
---@overload fun(source:UnityEngine.Texture, mat:UnityEngine.Material, pass:number): void
---@overload fun(source:UnityEngine.Texture, dest:UnityEngine.RenderTexture, scale:UnityEngine.Vector2, offset:UnityEngine.Vector2): void
---@param source UnityEngine.Texture
---@param dest UnityEngine.RenderTexture
---@param mat UnityEngine.Material
---@param pass number
function UnityEngine.Graphics.Blit(source, dest, mat, pass) end
---@param source UnityEngine.Texture
---@param dest UnityEngine.RenderTexture
---@param mat UnityEngine.Material
---@param offsets UnityEngine.Vector2[]
function UnityEngine.Graphics.BlitMultiTap(source, dest, mat, offsets) end
return UnityEngine.Graphics
