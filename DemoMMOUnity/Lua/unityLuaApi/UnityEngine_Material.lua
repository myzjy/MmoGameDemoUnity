---@class UnityEngine.Material : UnityEngine.Object
---@field public shader UnityEngine.Shader
---@field public color UnityEngine.Color
---@field public mainTexture UnityEngine.Texture
---@field public mainTextureOffset UnityEngine.Vector2
---@field public mainTextureScale UnityEngine.Vector2
---@field public renderQueue number
---@field public globalIlluminationFlags number
---@field public doubleSidedGI boolean
---@field public enableInstancing boolean
---@field public passCount number
---@field public shaderKeywords System.String[]

---@type UnityEngine.Material
UnityEngine.Material = { }
---@overload fun(shader:UnityEngine.Shader): UnityEngine.Material
---@overload fun(source:UnityEngine.Material): UnityEngine.Material
---@return UnityEngine.Material
---@param contents string
function UnityEngine.Material.New(contents) end
---@overload fun(nameID:number): boolean
---@return boolean
---@param name string
function UnityEngine.Material:HasProperty(name) end
---@param keyword string
function UnityEngine.Material:EnableKeyword(keyword) end
---@param keyword string
function UnityEngine.Material:DisableKeyword(keyword) end
---@return boolean
---@param keyword string
function UnityEngine.Material:IsKeywordEnabled(keyword) end
---@param passName string
---@param enabled boolean
function UnityEngine.Material:SetShaderPassEnabled(passName, enabled) end
---@return boolean
---@param passName string
function UnityEngine.Material:GetShaderPassEnabled(passName) end
---@return string
---@param pass number
function UnityEngine.Material:GetPassName(pass) end
---@return number
---@param passName string
function UnityEngine.Material:FindPass(passName) end
---@param tag string
---@param val string
function UnityEngine.Material:SetOverrideTag(tag, val) end
---@overload fun(tag:string, searchFallbacks:boolean): string
---@return string
---@param tag string
---@param searchFallbacks boolean
---@param defaultValue string
function UnityEngine.Material:GetTag(tag, searchFallbacks, defaultValue) end
---@param start UnityEngine.Material
---@param ed UnityEngine.Material
---@param t number
function UnityEngine.Material:Lerp(start, ed, t) end
---@return boolean
---@param pass number
function UnityEngine.Material:SetPass(pass) end
---@param mat UnityEngine.Material
function UnityEngine.Material:CopyPropertiesFromMaterial(mat) end
---@overload fun(): System.String[]
---@return System.String[]
---@param outNames System.Collections.Generic.List_System.String
function UnityEngine.Material:GetTexturePropertyNames(outNames) end
---@overload fun(): System.Int32[]
---@return System.Int32[]
---@param outNames System.Collections.Generic.List_System.Int32
function UnityEngine.Material:GetTexturePropertyNameIDs(outNames) end
---@overload fun(name:string, value:number): void
---@param nameID number
---@param value number
function UnityEngine.Material:SetFloat(nameID, value) end
---@overload fun(name:string, value:number): void
---@param nameID number
---@param value number
function UnityEngine.Material:SetInt(nameID, value) end
---@overload fun(name:string, value:UnityEngine.Color): void
---@param nameID number
---@param value UnityEngine.Color
function UnityEngine.Material:SetColor(nameID, value) end
---@overload fun(name:string, value:UnityEngine.Vector4): void
---@param nameID number
---@param value UnityEngine.Vector4
function UnityEngine.Material:SetVector(nameID, value) end
---@overload fun(name:string, value:UnityEngine.Matrix4x4): void
---@param nameID number
---@param value UnityEngine.Matrix4x4
function UnityEngine.Material:SetMatrix(nameID, value) end
---@overload fun(name:string, value:UnityEngine.Texture): void
---@param nameID number
---@param value UnityEngine.Texture
function UnityEngine.Material:SetTexture(nameID, value) end
---@overload fun(name:string, value:UnityEngine.ComputeBuffer): void
---@param nameID number
---@param value UnityEngine.ComputeBuffer
function UnityEngine.Material:SetBuffer(nameID, value) end
---@overload fun(name:string, values:System.Collections.Generic.List_System.Single): void
---@overload fun(nameID:number, values:System.Collections.Generic.List_System.Single): void
---@overload fun(name:string, values:System.Single[]): void
---@param nameID number
---@param values System.Single[]
function UnityEngine.Material:SetFloatArray(nameID, values) end
---@overload fun(name:string, values:System.Collections.Generic.List_UnityEngine.Color): void
---@overload fun(nameID:number, values:System.Collections.Generic.List_UnityEngine.Color): void
---@overload fun(name:string, values:UnityEngine.Color[]): void
---@param nameID number
---@param values UnityEngine.Color[]
function UnityEngine.Material:SetColorArray(nameID, values) end
---@overload fun(name:string, values:System.Collections.Generic.List_UnityEngine.Vector4): void
---@overload fun(nameID:number, values:System.Collections.Generic.List_UnityEngine.Vector4): void
---@overload fun(name:string, values:UnityEngine.Vector4[]): void
---@param nameID number
---@param values UnityEngine.Vector4[]
function UnityEngine.Material:SetVectorArray(nameID, values) end
---@overload fun(name:string, values:System.Collections.Generic.List_UnityEngine.Matrix4x4): void
---@overload fun(nameID:number, values:System.Collections.Generic.List_UnityEngine.Matrix4x4): void
---@overload fun(name:string, values:UnityEngine.Matrix4x4[]): void
---@param nameID number
---@param values UnityEngine.Matrix4x4[]
function UnityEngine.Material:SetMatrixArray(nameID, values) end
---@overload fun(name:string): number
---@return number
---@param nameID number
function UnityEngine.Material:GetFloat(nameID) end
---@overload fun(name:string): number
---@return number
---@param nameID number
function UnityEngine.Material:GetInt(nameID) end
---@overload fun(name:string): UnityEngine.Color
---@return UnityEngine.Color
---@param nameID number
function UnityEngine.Material:GetColor(nameID) end
---@overload fun(name:string): UnityEngine.Vector4
---@return UnityEngine.Vector4
---@param nameID number
function UnityEngine.Material:GetVector(nameID) end
---@overload fun(name:string): UnityEngine.Matrix4x4
---@return UnityEngine.Matrix4x4
---@param nameID number
function UnityEngine.Material:GetMatrix(nameID) end
---@overload fun(name:string): UnityEngine.Texture
---@return UnityEngine.Texture
---@param nameID number
function UnityEngine.Material:GetTexture(nameID) end
---@overload fun(name:string): System.Single[]
---@overload fun(nameID:number): System.Single[]
---@overload fun(name:string, values:System.Collections.Generic.List_System.Single): void
---@return System.Single[]
---@param nameID number
---@param values System.Collections.Generic.List_System.Single
function UnityEngine.Material:GetFloatArray(nameID, values) end
---@overload fun(name:string): UnityEngine.Color[]
---@overload fun(nameID:number): UnityEngine.Color[]
---@overload fun(name:string, values:System.Collections.Generic.List_UnityEngine.Color): void
---@return UnityEngine.Color[]
---@param nameID number
---@param values System.Collections.Generic.List_UnityEngine.Color
function UnityEngine.Material:GetColorArray(nameID, values) end
---@overload fun(name:string): UnityEngine.Vector4[]
---@overload fun(nameID:number): UnityEngine.Vector4[]
---@overload fun(name:string, values:System.Collections.Generic.List_UnityEngine.Vector4): void
---@return UnityEngine.Vector4[]
---@param nameID number
---@param values System.Collections.Generic.List_UnityEngine.Vector4
function UnityEngine.Material:GetVectorArray(nameID, values) end
---@overload fun(name:string): UnityEngine.Matrix4x4[]
---@overload fun(nameID:number): UnityEngine.Matrix4x4[]
---@overload fun(name:string, values:System.Collections.Generic.List_UnityEngine.Matrix4x4): void
---@return UnityEngine.Matrix4x4[]
---@param nameID number
---@param values System.Collections.Generic.List_UnityEngine.Matrix4x4
function UnityEngine.Material:GetMatrixArray(nameID, values) end
---@overload fun(name:string, value:UnityEngine.Vector2): void
---@param nameID number
---@param value UnityEngine.Vector2
function UnityEngine.Material:SetTextureOffset(nameID, value) end
---@overload fun(name:string, value:UnityEngine.Vector2): void
---@param nameID number
---@param value UnityEngine.Vector2
function UnityEngine.Material:SetTextureScale(nameID, value) end
---@overload fun(name:string): UnityEngine.Vector2
---@return UnityEngine.Vector2
---@param nameID number
function UnityEngine.Material:GetTextureOffset(nameID) end
---@overload fun(name:string): UnityEngine.Vector2
---@return UnityEngine.Vector2
---@param nameID number
function UnityEngine.Material:GetTextureScale(nameID) end
return UnityEngine.Material
