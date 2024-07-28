---@class CS.UnityEngine.UI.Graphic : CS.UnityEngine.EventSystems.UIBehaviour
---@field public defaultGraphicMaterial CS.UnityEngine.Material
---@field public color CS.UnityEngine.Color
---@field public raycastTarget boolean
---@field public depth number
---@field public rectTransform CS.UnityEngine.RectTransform
---@field public canvas CS.UnityEngine.Canvas
---@field public canvasRenderer CS.UnityEngine.CanvasRenderer
---@field public defaultMaterial CS.UnityEngine.Material
---@field public material CS.UnityEngine.Material
---@field public materialForRendering CS.UnityEngine.Material
---@field public mainTexture CS.UnityEngine.Texture
CS.UnityEngine.UI.Graphic = { }
function CS.UnityEngine.UI.Graphic:SetAllDirty() end
function CS.UnityEngine.UI.Graphic:SetLayoutDirty() end
function CS.UnityEngine.UI.Graphic:SetVerticesDirty() end
function CS.UnityEngine.UI.Graphic:SetMaterialDirty() end
function CS.UnityEngine.UI.Graphic:OnCullingChanged() end
---@param update number
function CS.UnityEngine.UI.Graphic:Rebuild(update) end
function CS.UnityEngine.UI.Graphic:LayoutComplete() end
function CS.UnityEngine.UI.Graphic:GraphicUpdateComplete() end
function CS.UnityEngine.UI.Graphic:OnRebuildRequested() end
function CS.UnityEngine.UI.Graphic:SetNativeSize() end
---@return boolean
---@param sp CS.UnityEngine.Vector2
---@param eventCamera CS.UnityEngine.Camera
function CS.UnityEngine.UI.Graphic:Raycast(sp, eventCamera) end
---@return CS.UnityEngine.Vector2
---@param point CS.UnityEngine.Vector2
function CS.UnityEngine.UI.Graphic:PixelAdjustPoint(point) end
---@return CS.UnityEngine.Rect
function CS.UnityEngine.UI.Graphic:GetPixelAdjustedRect() end
---@overload fun(targetColor:CS.UnityEngine.Color, duration:number, ignoreTimeScale:boolean, useAlpha:boolean): void
---@param targetColor CS.UnityEngine.Color
---@param duration number
---@param ignoreTimeScale boolean
---@param useAlpha boolean
---@param useRGB boolean
function CS.UnityEngine.UI.Graphic:CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha, useRGB) end
---@param alpha number
---@param duration number
---@param ignoreTimeScale boolean
function CS.UnityEngine.UI.Graphic:CrossFadeAlpha(alpha, duration, ignoreTimeScale) end
---@param action (fun():void)
function CS.UnityEngine.UI.Graphic:RegisterDirtyLayoutCallback(action) end
---@param action (fun():void)
function CS.UnityEngine.UI.Graphic:UnregisterDirtyLayoutCallback(action) end
---@param action (fun():void)
function CS.UnityEngine.UI.Graphic:RegisterDirtyVerticesCallback(action) end
---@param action (fun():void)
function CS.UnityEngine.UI.Graphic:UnregisterDirtyVerticesCallback(action) end
---@param action (fun():void)
function CS.UnityEngine.UI.Graphic:RegisterDirtyMaterialCallback(action) end
---@param action (fun():void)
function CS.UnityEngine.UI.Graphic:UnregisterDirtyMaterialCallback(action) end

