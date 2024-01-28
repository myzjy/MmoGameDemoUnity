---@class UnityEngine.Component : UnityEngine.Object
---@field public transform UnityEngine.Transform
---@field public gameObject UnityEngine.GameObject
---@field public tag string

---@type UnityEngine.Component
UnityEngine.Component = { }
---@return UnityEngine.Component
function UnityEngine.Component.New() end
---@return UnityEngine.Component
---@param t string
function UnityEngine.Component:GetComponent(t) end
---@overload fun(t:string): UnityEngine.Component
---@return UnityEngine.Component
---@param t string
---@param includeInactive boolean
function UnityEngine.Component:GetComponentInChildren(t, includeInactive) end
---@overload fun(t:string): UnityEngine.Component[]
---@return UnityEngine.Component[]
---@param t string
---@param includeInactive boolean
function UnityEngine.Component:GetComponentsInChildren(t, includeInactive) end
---@return UnityEngine.Component
---@param t string
function UnityEngine.Component:GetComponentInParent(t) end
---@overload fun(t:string): UnityEngine.Component[]
---@return UnityEngine.Component[]
---@param t string
---@param includeInactive boolean
function UnityEngine.Component:GetComponentsInParent(t, includeInactive) end
---@overload fun(t:string): UnityEngine.Component[]
---@return UnityEngine.Component[]
---@param t string
---@param results System.Collections.Generic.List_UnityEngine.Component
function UnityEngine.Component:GetComponents(t, results) end
---@return boolean
---@param tag string
function UnityEngine.Component:CompareTag(tag) end
---@overload fun(methodName:string): void
---@overload fun(methodName:string, value:System.Object): void
---@overload fun(methodName:string, options:number): void
---@param methodName string
---@param value System.Object
---@param options number
function UnityEngine.Component:SendMessageUpwards(methodName, value, options) end
---@overload fun(methodName:string): void
---@overload fun(methodName:string, value:System.Object): void
---@overload fun(methodName:string, options:number): void
---@param methodName string
---@param value System.Object
---@param options number
function UnityEngine.Component:SendMessage(methodName, value, options) end
---@overload fun(methodName:string): void
---@overload fun(methodName:string, parameter:System.Object): void
---@overload fun(methodName:string, options:number): void
---@param methodName string
---@param parameter System.Object
---@param options number
function UnityEngine.Component:BroadcastMessage(methodName, parameter, options) end
---@return UnityEngine.Component
---@param className string
function UnityEngine.Component:GetComp(className) end
return UnityEngine.Component
