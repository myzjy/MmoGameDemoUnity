---@class CS.UnityEngine.Component : CS.UnityEngine.Object
---@field public transform CS.UnityEngine.Transform
---@field public gameObject CS.UnityEngine.GameObject
---@field public tag string
CS.UnityEngine.Component = {}
---@return CS.UnityEngine.Component
function CS.UnityEngine.Component.New() end
---@return CS.UnityEngine.Component
---@param t string
function CS.UnityEngine.Component:GetComponent(t) end
---@overload fun(t:string): CS.UnityEngine.Component
---@return CS.UnityEngine.Component
---@param t string
---@param includeInactive boolean
function CS.UnityEngine.Component:GetComponentInChildren(t, includeInactive) end
---@overload fun(t:string): CS.UnityEngine.Component[]
---@return CS.UnityEngine.Component[]
---@param t string
---@param includeInactive boolean
function CS.UnityEngine.Component:GetComponentsInChildren(t, includeInactive) end
---@return CS.UnityEngine.Component
---@param t string
function CS.UnityEngine.Component:GetComponentInParent(t) end
---@overload fun(t:string): CS.UnityEngine.Component[]
---@return CS.UnityEngine.Component[]
---@param t string
---@param includeInactive boolean
function CS.UnityEngine.Component:GetComponentsInParent(t, includeInactive) end
---@overload fun(t:string): CS.UnityEngine.Component[]
---@return CS.UnityEngine.Component[]
---@param t string
---@param results CS.System.Collections.Generic.List_UnityEngine.Component
function CS.UnityEngine.Component:GetComponents(t, results) end
---@return boolean
---@param tag string
function CS.UnityEngine.Component:CompareTag(tag) end
---@overload fun(methodName:string): void
---@overload fun(methodName:string, value:CS.System.Object): void
---@overload fun(methodName:string, options:number): void
---@param methodName string
---@param value CS.System.Object
---@param options number
function CS.UnityEngine.Component:SendMessageUpwards(methodName, value, options) end
---@overload fun(methodName:string): void
---@overload fun(methodName:string, value:CS.System.Object): void
---@overload fun(methodName:string, options:number): void
---@param methodName string
---@param value CS.System.Object
---@param options number
function CS.UnityEngine.Component:SendMessage(methodName, value, options) end
---@overload fun(methodName:string): void
---@overload fun(methodName:string, parameter:CS.System.Object): void
---@overload fun(methodName:string, options:number): void
---@param methodName string
---@param parameter CS.System.Object
---@param options number
function CS.UnityEngine.Component:BroadcastMessage(methodName, parameter, options) end
---@return CS.UnityEngine.Component
---@param className string
function CS.UnityEngine.Component:GetComp(className) end
return CS.UnityEngine.Component
