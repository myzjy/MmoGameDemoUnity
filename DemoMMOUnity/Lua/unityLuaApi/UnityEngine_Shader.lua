---@class UnityEngine.Shader : UnityEngine.Object
---@field public maximumLOD number
---@field public globalMaximumLOD number
---@field public isSupported boolean
---@field public globalRenderPipeline string
---@field public renderQueue number

---@type UnityEngine.Shader
UnityEngine.Shader = { }
---@return UnityEngine.Shader
---@param name string
function UnityEngine.Shader.Find(name) end
---@param keyword string
function UnityEngine.Shader.EnableKeyword(keyword) end
---@param keyword string
function UnityEngine.Shader.DisableKeyword(keyword) end
---@return boolean
---@param keyword string
function UnityEngine.Shader.IsKeywordEnabled(keyword) end
function UnityEngine.Shader.WarmupAllShaders() end
---@return number
---@param name string
function UnityEngine.Shader.PropertyToID(name) end
---@overload fun(name:string, value:number): void
---@param nameID number
---@param value number
function UnityEngine.Shader.SetGlobalFloat(nameID, value) end
---@overload fun(name:string, value:number): void
---@param nameID number
---@param value number
function UnityEngine.Shader.SetGlobalInt(nameID, value) end
---@overload fun(name:string, value:UnityEngine.Vector4): void
---@param nameID number
---@param value UnityEngine.Vector4
function UnityEngine.Shader.SetGlobalVector(nameID, value) end
---@overload fun(name:string, value:UnityEngine.Color): void
---@param nameID number
---@param value UnityEngine.Color
function UnityEngine.Shader.SetGlobalColor(nameID, value) end
---@overload fun(name:string, value:UnityEngine.Matrix4x4): void
---@param nameID number
---@param value UnityEngine.Matrix4x4
function UnityEngine.Shader.SetGlobalMatrix(nameID, value) end
---@overload fun(name:string, value:UnityEngine.Texture): void
---@param nameID number
---@param value UnityEngine.Texture
function UnityEngine.Shader.SetGlobalTexture(nameID, value) end
---@overload fun(name:string, value:UnityEngine.ComputeBuffer): void
---@param nameID number
---@param value UnityEngine.ComputeBuffer
function UnityEngine.Shader.SetGlobalBuffer(nameID, value) end
---@overload fun(name:string, values:System.Collections.Generic.List_System.Single): void
---@overload fun(nameID:number, values:System.Collections.Generic.List_System.Single): void
---@overload fun(name:string, values:System.Single[]): void
---@param nameID number
---@param values System.Single[]
function UnityEngine.Shader.SetGlobalFloatArray(nameID, values) end
---@overload fun(name:string, values:System.Collections.Generic.List_UnityEngine.Vector4): void
---@overload fun(nameID:number, values:System.Collections.Generic.List_UnityEngine.Vector4): void
---@overload fun(name:string, values:UnityEngine.Vector4[]): void
---@param nameID number
---@param values UnityEngine.Vector4[]
function UnityEngine.Shader.SetGlobalVectorArray(nameID, values) end
---@overload fun(name:string, values:System.Collections.Generic.List_UnityEngine.Matrix4x4): void
---@overload fun(nameID:number, values:System.Collections.Generic.List_UnityEngine.Matrix4x4): void
---@overload fun(name:string, values:UnityEngine.Matrix4x4[]): void
---@param nameID number
---@param values UnityEngine.Matrix4x4[]
function UnityEngine.Shader.SetGlobalMatrixArray(nameID, values) end
---@overload fun(name:string): number
---@return number
---@param nameID number
function UnityEngine.Shader.GetGlobalFloat(nameID) end
---@overload fun(name:string): number
---@return number
---@param nameID number
function UnityEngine.Shader.GetGlobalInt(nameID) end
---@overload fun(name:string): UnityEngine.Vector4
---@return UnityEngine.Vector4
---@param nameID number
function UnityEngine.Shader.GetGlobalVector(nameID) end
---@overload fun(name:string): UnityEngine.Color
---@return UnityEngine.Color
---@param nameID number
function UnityEngine.Shader.GetGlobalColor(nameID) end
---@overload fun(name:string): UnityEngine.Matrix4x4
---@return UnityEngine.Matrix4x4
---@param nameID number
function UnityEngine.Shader.GetGlobalMatrix(nameID) end
---@overload fun(name:string): UnityEngine.Texture
---@return UnityEngine.Texture
---@param nameID number
function UnityEngine.Shader.GetGlobalTexture(nameID) end
---@overload fun(name:string): System.Single[]
---@overload fun(nameID:number): System.Single[]
---@overload fun(name:string, values:System.Collections.Generic.List_System.Single): void
---@return System.Single[]
---@param nameID number
---@param values System.Collections.Generic.List_System.Single
function UnityEngine.Shader.GetGlobalFloatArray(nameID, values) end
---@overload fun(name:string): UnityEngine.Vector4[]
---@overload fun(nameID:number): UnityEngine.Vector4[]
---@overload fun(name:string, values:System.Collections.Generic.List_UnityEngine.Vector4): void
---@return UnityEngine.Vector4[]
---@param nameID number
---@param values System.Collections.Generic.List_UnityEngine.Vector4
function UnityEngine.Shader.GetGlobalVectorArray(nameID, values) end
---@overload fun(name:string): UnityEngine.Matrix4x4[]
---@overload fun(nameID:number): UnityEngine.Matrix4x4[]
---@overload fun(name:string, values:System.Collections.Generic.List_UnityEngine.Matrix4x4): void
---@return UnityEngine.Matrix4x4[]
---@param nameID number
---@param values System.Collections.Generic.List_UnityEngine.Matrix4x4
function UnityEngine.Shader.GetGlobalMatrixArray(nameID, values) end
return UnityEngine.Shader
