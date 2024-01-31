---@class UnityEngine.Resources

---@type UnityEngine.Resources
UnityEngine.Resources = { }
---@return UnityEngine.Resources
function UnityEngine.Resources.New() end
---@return UnityEngine.Object[]
---@param t string
function UnityEngine.Resources.FindObjectsOfTypeAll(t) end
---@overload fun(path:string): UnityEngine.Object
---@return UnityEngine.Object
---@param path string
---@param systemTypeInstance string
function UnityEngine.Resources.Load(path, systemTypeInstance) end
---@overload fun(path:string): UnityEngine.ResourceRequest
---@return UnityEngine.ResourceRequest
---@param path string
---@param t string
function UnityEngine.Resources.LoadAsync(path, t) end
---@overload fun(path:string): UnityEngine.Object[]
---@return UnityEngine.Object[]
---@param path string
---@param systemTypeInstance string
function UnityEngine.Resources.LoadAll(path, systemTypeInstance) end
---@return UnityEngine.Object
---@param t string
---@param path string
function UnityEngine.Resources.GetBuiltinResource(t, path) end
---@param assetToUnload UnityEngine.Object
function UnityEngine.Resources.UnloadAsset(assetToUnload) end
---@return UnityEngine.AsyncOperation
function UnityEngine.Resources.UnloadUnusedAssets() end
return UnityEngine.Resources
