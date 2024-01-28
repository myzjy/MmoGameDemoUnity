---@class UnityEngine.Sprite : UnityEngine.Object
---@field public bounds UnityEngine.Bounds
---@field public rect UnityEngine.Rect
---@field public border UnityEngine.Vector4
---@field public texture UnityEngine.Texture2D
---@field public pixelsPerUnit number
---@field public associatedAlphaSplitTexture UnityEngine.Texture2D
---@field public pivot UnityEngine.Vector2
---@field public packed boolean
---@field public packingMode number
---@field public packingRotation number
---@field public textureRect UnityEngine.Rect
---@field public textureRectOffset UnityEngine.Vector2
---@field public vertices UnityEngine.Vector2[]
---@field public triangles System.UInt16[]
---@field public uv UnityEngine.Vector2[]

---@type UnityEngine.Sprite
UnityEngine.Sprite = { }
---@return number
function UnityEngine.Sprite:GetPhysicsShapeCount() end
---@return number
---@param shapeIdx number
function UnityEngine.Sprite:GetPhysicsShapePointCount(shapeIdx) end
---@return number
---@param shapeIdx number
---@param physicsShape System.Collections.Generic.List_UnityEngine.Vector2
function UnityEngine.Sprite:GetPhysicsShape(shapeIdx, physicsShape) end
---@param physicsShapes System.Collections.Generic.IList_UnityEngine.Vector2_Array
function UnityEngine.Sprite:OverridePhysicsShape(physicsShapes) end
---@param vertices UnityEngine.Vector2[]
---@param triangles System.UInt16[]
function UnityEngine.Sprite:OverrideGeometry(vertices, triangles) end
---@overload fun(texture:UnityEngine.Texture2D, rect:UnityEngine.Rect, pivot:UnityEngine.Vector2): UnityEngine.Sprite
---@overload fun(texture:UnityEngine.Texture2D, rect:UnityEngine.Rect, pivot:UnityEngine.Vector2, pixelsPerUnit:number): UnityEngine.Sprite
---@overload fun(texture:UnityEngine.Texture2D, rect:UnityEngine.Rect, pivot:UnityEngine.Vector2, pixelsPerUnit:number, extrude:number): UnityEngine.Sprite
---@overload fun(texture:UnityEngine.Texture2D, rect:UnityEngine.Rect, pivot:UnityEngine.Vector2, pixelsPerUnit:number, extrude:number, meshType:number): UnityEngine.Sprite
---@overload fun(texture:UnityEngine.Texture2D, rect:UnityEngine.Rect, pivot:UnityEngine.Vector2, pixelsPerUnit:number, extrude:number, meshType:number, border:UnityEngine.Vector4): UnityEngine.Sprite
---@return UnityEngine.Sprite
---@param texture UnityEngine.Texture2D
---@param rect UnityEngine.Rect
---@param pivot UnityEngine.Vector2
---@param pixelsPerUnit number
---@param extrude number
---@param meshType number
---@param border UnityEngine.Vector4
---@param generateFallbackPhysicsShape boolean
function UnityEngine.Sprite.Create(texture, rect, pivot, pixelsPerUnit, extrude, meshType, border, generateFallbackPhysicsShape) end
return UnityEngine.Sprite
