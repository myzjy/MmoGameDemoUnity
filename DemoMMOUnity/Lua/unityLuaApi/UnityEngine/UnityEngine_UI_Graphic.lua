---@class UnityEngine.UI.Graphic : UnityEngine.EventSystems.UIBehaviour
---@field public defaultGraphicMaterial UnityEngine.Material
---@field public color UnityEngine.Color
---@field public raycastTarget boolean
---@field public depth number
---@field public rectTransform UnityEngine.RectTransform
---@field public canvas UnityEngine.Canvas
---@field public canvasRenderer UnityEngine.CanvasRenderer
---@field public defaultMaterial UnityEngine.Material
---@field public material UnityEngine.Material
---@field public materialForRendering UnityEngine.Material
---@field public mainTexture UnityEngine.Texture

---@type UnityEngine.UI.Graphic
UnityEngine.UI.Graphic = { }
function UnityEngine.UI.Graphic:SetAllDirty() end
function UnityEngine.UI.Graphic:SetLayoutDirty() end
function UnityEngine.UI.Graphic:SetVerticesDirty() end
function UnityEngine.UI.Graphic:SetMaterialDirty() end
function UnityEngine.UI.Graphic:OnCullingChanged() end
---@param update number
function UnityEngine.UI.Graphic:Rebuild(update) end
function UnityEngine.UI.Graphic:LayoutComplete() end
function UnityEngine.UI.Graphic:GraphicUpdateComplete() end
function UnityEngine.UI.Graphic:OnRebuildRequested() end
function UnityEngine.UI.Graphic:SetNativeSize() end
---@return boolean
---@param sp UnityEngine.Vector2
---@param eventCamera UnityEngine.Camera
function UnityEngine.UI.Graphic:Raycast(sp, eventCamera) end
---@return UnityEngine.Vector2
---@param point UnityEngine.Vector2
function UnityEngine.UI.Graphic:PixelAdjustPoint(point) end
---@return UnityEngine.Rect
function UnityEngine.UI.Graphic:GetPixelAdjustedRect() end
---@overload fun(targetColor:UnityEngine.Color, duration:number, ignoreTimeScale:boolean, useAlpha:boolean): void
---@param targetColor UnityEngine.Color
---@param duration number
---@param ignoreTimeScale boolean
---@param useAlpha boolean
---@param useRGB boolean
function UnityEngine.UI.Graphic:CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha, useRGB) end
---@param alpha number
---@param duration number
---@param ignoreTimeScale boolean
function UnityEngine.UI.Graphic:CrossFadeAlpha(alpha, duration, ignoreTimeScale) end
---@param action (fun():void)
function UnityEngine.UI.Graphic:RegisterDirtyLayoutCallback(action) end
---@param action (fun():void)
function UnityEngine.UI.Graphic:UnregisterDirtyLayoutCallback(action) end
---@param action (fun():void)
function UnityEngine.UI.Graphic:RegisterDirtyVerticesCallback(action) end
---@param action (fun():void)
function UnityEngine.UI.Graphic:UnregisterDirtyVerticesCallback(action) end
---@param action (fun():void)
function UnityEngine.UI.Graphic:RegisterDirtyMaterialCallback(action) end
---@param action (fun():void)
function UnityEngine.UI.Graphic:UnregisterDirtyMaterialCallback(action) end
return UnityEngine.UI.Graphic
