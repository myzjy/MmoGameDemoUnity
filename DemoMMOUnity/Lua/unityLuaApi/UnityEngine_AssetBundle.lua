---@class UnityEngine.AssetBundle : UnityEngine.Object
---@field public isStreamedSceneAssetBundle boolean

---@type UnityEngine.AssetBundle
UnityEngine.AssetBundle = { }
---@param unloadAllObjects boolean
function UnityEngine.AssetBundle.UnloadAllAssetBundles(unloadAllObjects) end
---@return System.Collections.Generic.IEnumerable_UnityEngine.AssetBundle
function UnityEngine.AssetBundle.GetAllLoadedAssetBundles() end
---@overload fun(path:string): UnityEngine.AssetBundleCreateRequest
---@overload fun(path:string, crc:number): UnityEngine.AssetBundleCreateRequest
---@return UnityEngine.AssetBundleCreateRequest
---@param path string
---@param crc number
---@param offset uint64
function UnityEngine.AssetBundle.LoadFromFileAsync(path, crc, offset) end
---@overload fun(path:string): UnityEngine.AssetBundle
---@overload fun(path:string, crc:number): UnityEngine.AssetBundle
---@return UnityEngine.AssetBundle
---@param path string
---@param crc number
---@param offset uint64
function UnityEngine.AssetBundle.LoadFromFile(path, crc, offset) end
---@overload fun(binary:System.Byte[]): UnityEngine.AssetBundleCreateRequest
---@return UnityEngine.AssetBundleCreateRequest
---@param binary System.Byte[]
---@param crc number
function UnityEngine.AssetBundle.LoadFromMemoryAsync(binary, crc) end
---@overload fun(binary:System.Byte[]): UnityEngine.AssetBundle
---@return UnityEngine.AssetBundle
---@param binary System.Byte[]
---@param crc number
function UnityEngine.AssetBundle.LoadFromMemory(binary, crc) end
---@overload fun(stream:System.IO.Stream): UnityEngine.AssetBundleCreateRequest
---@overload fun(stream:System.IO.Stream, crc:number): UnityEngine.AssetBundleCreateRequest
---@return UnityEngine.AssetBundleCreateRequest
---@param stream System.IO.Stream
---@param crc number
---@param managedReadBufferSize number
function UnityEngine.AssetBundle.LoadFromStreamAsync(stream, crc, managedReadBufferSize) end
---@overload fun(stream:System.IO.Stream): UnityEngine.AssetBundle
---@overload fun(stream:System.IO.Stream, crc:number): UnityEngine.AssetBundle
---@return UnityEngine.AssetBundle
---@param stream System.IO.Stream
---@param crc number
---@param managedReadBufferSize number
function UnityEngine.AssetBundle.LoadFromStream(stream, crc, managedReadBufferSize) end
---@return boolean
---@param name string
function UnityEngine.AssetBundle:Contains(name) end
---@overload fun(name:string): UnityEngine.Object
---@return UnityEngine.Object
---@param name string
---@param t string
function UnityEngine.AssetBundle:LoadAsset(name, t) end
---@overload fun(name:string): UnityEngine.AssetBundleRequest
---@return UnityEngine.AssetBundleRequest
---@param name string
---@param t string
function UnityEngine.AssetBundle:LoadAssetAsync(name, t) end
---@overload fun(name:string): UnityEngine.Object[]
---@return UnityEngine.Object[]
---@param name string
---@param t string
function UnityEngine.AssetBundle:LoadAssetWithSubAssets(name, t) end
---@overload fun(name:string): UnityEngine.AssetBundleRequest
---@return UnityEngine.AssetBundleRequest
---@param name string
---@param t string
function UnityEngine.AssetBundle:LoadAssetWithSubAssetsAsync(name, t) end
---@overload fun(): UnityEngine.Object[]
---@return UnityEngine.Object[]
---@param t string
function UnityEngine.AssetBundle:LoadAllAssets(t) end
---@overload fun(): UnityEngine.AssetBundleRequest
---@return UnityEngine.AssetBundleRequest
---@param t string
function UnityEngine.AssetBundle:LoadAllAssetsAsync(t) end
---@param unloadAllLoadedObjects boolean
function UnityEngine.AssetBundle:Unload(unloadAllLoadedObjects) end
---@return System.String[]
function UnityEngine.AssetBundle:GetAllAssetNames() end
---@return System.String[]
function UnityEngine.AssetBundle:GetAllScenePaths() end
---@return UnityEngine.AssetBundleRecompressOperation
---@param inputPath string
---@param outputPath string
---@param method UnityEngine.BuildCompression
---@param expectedCRC number
---@param priority number
function UnityEngine.AssetBundle.RecompressAssetBundleAsync(inputPath, outputPath, method, expectedCRC, priority) end
return UnityEngine.AssetBundle
