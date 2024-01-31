---@class UnityEngine.Canvas : UnityEngine.Behaviour
---@field public renderMode number
---@field public isRootCanvas boolean
---@field public pixelRect UnityEngine.Rect
---@field public scaleFactor number
---@field public referencePixelsPerUnit number
---@field public overridePixelPerfect boolean
---@field public pixelPerfect boolean
---@field public planeDistance number
---@field public renderOrder number
---@field public overrideSorting boolean
---@field public sortingOrder number
---@field public targetDisplay number
---@field public sortingLayerID number
---@field public cachedSortingLayerValue number
---@field public additionalShaderChannels number
---@field public sortingLayerName string
---@field public rootCanvas UnityEngine.Canvas
---@field public worldCamera UnityEngine.Camera
---@field public normalizedSortingGridSize number

---@type UnityEngine.Canvas
UnityEngine.Canvas = { }
---@return UnityEngine.Canvas
function UnityEngine.Canvas.New() end
---@param value (fun():void)
function UnityEngine.Canvas.add_willRenderCanvases(value) end
---@param value (fun():void)
function UnityEngine.Canvas.remove_willRenderCanvases(value) end
---@return UnityEngine.Material
function UnityEngine.Canvas.GetDefaultCanvasMaterial() end
---@return UnityEngine.Material
function UnityEngine.Canvas.GetETC1SupportedCanvasMaterial() end
function UnityEngine.Canvas.ForceUpdateCanvases() end
return UnityEngine.Canvas
