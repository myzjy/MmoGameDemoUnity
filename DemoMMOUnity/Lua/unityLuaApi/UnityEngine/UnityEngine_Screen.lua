---@class UnityEngine.Screen
---@field public width number
---@field public height number
---@field public dpi number
---@field public orientation number
---@field public sleepTimeout number
---@field public autorotateToPortrait boolean
---@field public autorotateToPortraitUpsideDown boolean
---@field public autorotateToLandscapeLeft boolean
---@field public autorotateToLandscapeRight boolean
---@field public currentResolution UnityEngine.Resolution
---@field public fullScreen boolean
---@field public fullScreenMode number
---@field public safeArea UnityEngine.Rect
---@field public resolutions UnityEngine.Resolution[]

---@type UnityEngine.Screen
UnityEngine.Screen = { }
---@return UnityEngine.Screen
function UnityEngine.Screen.New() end
---@overload fun(width:number, height:number, fullscreenMode:number): void
---@overload fun(width:number, height:number, fullscreen:boolean): void
---@overload fun(width:number, height:number, fullscreenMode:number, preferredRefreshRate:number): void
---@param width number
---@param height number
---@param fullscreen boolean
---@param preferredRefreshRate number
function UnityEngine.Screen.SetResolution(width, height, fullscreen, preferredRefreshRate) end
return UnityEngine.Screen
