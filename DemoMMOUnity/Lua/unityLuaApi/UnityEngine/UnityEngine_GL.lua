---@class UnityEngine.GL
---@field public TRIANGLES number
---@field public TRIANGLE_STRIP number
---@field public QUADS number
---@field public LINES number
---@field public LINE_STRIP number
---@field public wireframe boolean
---@field public sRGBWrite boolean
---@field public invertCulling boolean
---@field public modelview UnityEngine.Matrix4x4

---@type UnityEngine.GL
UnityEngine.GL = { }
---@return UnityEngine.GL
function UnityEngine.GL.New() end
---@param x number
---@param y number
---@param z number
function UnityEngine.GL.Vertex3(x, y, z) end
---@param v UnityEngine.Vector3
function UnityEngine.GL.Vertex(v) end
---@param x number
---@param y number
---@param z number
function UnityEngine.GL.TexCoord3(x, y, z) end
---@param v UnityEngine.Vector3
function UnityEngine.GL.TexCoord(v) end
---@param x number
---@param y number
function UnityEngine.GL.TexCoord2(x, y) end
---@param unit number
---@param x number
---@param y number
---@param z number
function UnityEngine.GL.MultiTexCoord3(unit, x, y, z) end
---@param unit number
---@param v UnityEngine.Vector3
function UnityEngine.GL.MultiTexCoord(unit, v) end
---@param unit number
---@param x number
---@param y number
function UnityEngine.GL.MultiTexCoord2(unit, x, y) end
---@param c UnityEngine.Color
function UnityEngine.GL.Color(c) end
function UnityEngine.GL.Flush() end
function UnityEngine.GL.RenderTargetBarrier() end
---@param m UnityEngine.Matrix4x4
function UnityEngine.GL.MultMatrix(m) end
function UnityEngine.GL.PushMatrix() end
function UnityEngine.GL.PopMatrix() end
function UnityEngine.GL.LoadIdentity() end
function UnityEngine.GL.LoadOrtho() end
---@overload fun(): void
---@param left number
---@param right number
---@param bottom number
---@param top number
function UnityEngine.GL.LoadPixelMatrix(left, right, bottom, top) end
---@param mat UnityEngine.Matrix4x4
function UnityEngine.GL.LoadProjectionMatrix(mat) end
function UnityEngine.GL.InvalidateState() end
---@return UnityEngine.Matrix4x4
---@param proj UnityEngine.Matrix4x4
---@param renderIntoTexture boolean
function UnityEngine.GL.GetGPUProjectionMatrix(proj, renderIntoTexture) end
---@param callback number
---@param eventID number
function UnityEngine.GL.IssuePluginEvent(callback, eventID) end
---@param mode number
function UnityEngine.GL.Begin(mode) end
function UnityEngine.GL.End() end
---@overload fun(clearDepth:boolean, clearColor:boolean, backgroundColor:UnityEngine.Color): void
---@param clearDepth boolean
---@param clearColor boolean
---@param backgroundColor UnityEngine.Color
---@param depth number
function UnityEngine.GL.Clear(clearDepth, clearColor, backgroundColor, depth) end
---@param pixelRect UnityEngine.Rect
function UnityEngine.GL.Viewport(pixelRect) end
---@param clearDepth boolean
---@param camera UnityEngine.Camera
function UnityEngine.GL.ClearWithSkybox(clearDepth, camera) end
return UnityEngine.GL
