---@class CS.UnityEngine.Resources
CS.UnityEngine.Resources = { }
---@return CS.UnityEngine.Resources
function CS.UnityEngine.Resources.New() end
---@return CS.UnityEngine.Object[]
---@param t string
function CS.UnityEngine.Resources.FindObjectsOfTypeAll(t) end
---@overload fun(path:string): CS.UnityEngine.Object
---@return CS.UnityEngine.Object
---@param path string
---@param systemTypeInstance string
function CS.UnityEngine.Resources.Load(path, systemTypeInstance) end
---@overload fun(path:string): CS.UnityEngine.ResourceRequest
---@return CS.UnityEngine.ResourceRequest
---@param path string
---@param t string
function CS.UnityEngine.Resources.LoadAsync(path, t) end
---@overload fun(path:string): CS.UnityEngine.Object[]
---@return CS.UnityEngine.Object[]
---@param path string
---@param systemTypeInstance string
function CS.UnityEngine.Resources.LoadAll(path, systemTypeInstance) end
---@return CS.UnityEngine.Object
---@param t string
---@param path string
function CS.UnityEngine.Resources.GetBuiltinResource(t, path) end
---@param assetToUnload CS.UnityEngine.Object
function CS.UnityEngine.Resources.UnloadAsset(assetToUnload) end
---@return CS.UnityEngine.AsyncOperation
function CS.UnityEngine.Resources.UnloadUnusedAssets() end
return CS.UnityEngine.Resources
