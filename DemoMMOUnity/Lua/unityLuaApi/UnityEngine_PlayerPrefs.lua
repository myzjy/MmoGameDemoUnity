---@class UnityEngine.PlayerPrefs

---@type UnityEngine.PlayerPrefs
UnityEngine.PlayerPrefs = { }
---@return UnityEngine.PlayerPrefs
function UnityEngine.PlayerPrefs.New() end
---@param key string
---@param value number
function UnityEngine.PlayerPrefs.SetInt(key, value) end
---@overload fun(key:string): number
---@return number
---@param key string
---@param defaultValue number
function UnityEngine.PlayerPrefs.GetInt(key, defaultValue) end
---@param key string
---@param value number
function UnityEngine.PlayerPrefs.SetFloat(key, value) end
---@overload fun(key:string): number
---@return number
---@param key string
---@param defaultValue number
function UnityEngine.PlayerPrefs.GetFloat(key, defaultValue) end
---@param key string
---@param value string
function UnityEngine.PlayerPrefs.SetString(key, value) end
---@overload fun(key:string): string
---@return string
---@param key string
---@param defaultValue string
function UnityEngine.PlayerPrefs.GetString(key, defaultValue) end
---@return boolean
---@param key string
function UnityEngine.PlayerPrefs.HasKey(key) end
---@param key string
function UnityEngine.PlayerPrefs.DeleteKey(key) end
function UnityEngine.PlayerPrefs.DeleteAll() end
function UnityEngine.PlayerPrefs.Save() end
return UnityEngine.PlayerPrefs
