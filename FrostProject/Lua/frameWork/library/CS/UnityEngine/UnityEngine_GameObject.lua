---@class CS.UnityEngine.GameObject : CS.UnityEngine.Object
---@field public transform CS.UnityEngine.Transform
---@field public layer number
---@field public activeSelf boolean
---@field public activeInHierarchy boolean
---@field public isStatic boolean
---@field public tag string
---@field public scene CS.UnityEngine.SceneManagement.Scene
---@field public gameObject CS.UnityEngine.GameObject
CS.UnityEngine.GameObject = { }
---@overload fun(): CS.UnityEngine.GameObject
---@overload fun(name:string): CS.UnityEngine.GameObject
---@return CS.UnityEngine.GameObject
---@param name string
---@param components CS.System.Type[]
function CS.UnityEngine.GameObject.New(name, components) end
---@return CS.UnityEngine.GameObject
---@param t number
function CS.UnityEngine.GameObject.CreatePrimitive(t) end
---@return CS.UnityEngine.Component
---@param t string
function CS.UnityEngine.GameObject:GetComponent(t) end
---@overload fun(t:string): CS.UnityEngine.Component
---@return CS.UnityEngine.Component
---@param t string
---@param includeInactive boolean
function CS.UnityEngine.GameObject:GetComponentInChildren(t, includeInactive) end
---@return CS.UnityEngine.Component
---@param t string
function CS.UnityEngine.GameObject:GetComponentInParent(t) end
---@overload fun(t:string): CS.UnityEngine.Component[]
---@return CS.UnityEngine.Component[]
---@param t string
---@param results CS.System.Collections.Generic.List_UnityEngine.Component
function CS.UnityEngine.GameObject:GetComponents(t, results) end
---@overload fun(t:string): CS.UnityEngine.Component[]
---@return CS.UnityEngine.Component[]
---@param t string
---@param includeInactive boolean
function CS.UnityEngine.GameObject:GetComponentsInChildren(t, includeInactive) end
---@overload fun(t:string): CS.UnityEngine.Component[]
---@return CS.UnityEngine.Component[]
---@param t string
---@param includeInactive boolean
function CS.UnityEngine.GameObject:GetComponentsInParent(t, includeInactive) end
---@return CS.UnityEngine.GameObject
---@param tag string
function CS.UnityEngine.GameObject.FindWithTag(tag) end
---@overload fun(methodName:string): void
---@overload fun(methodName:string, options:number): void
---@overload fun(methodName:string, value:CS.System.Object): void
---@param methodName string
---@param value CS.System.Object
---@param options number
function CS.UnityEngine.GameObject:SendMessageUpwards(methodName, value, options) end
---@overload fun(methodName:string): void
---@overload fun(methodName:string, options:number): void
---@overload fun(methodName:string, value:CS.System.Object): void
---@param methodName string
---@param value CS.System.Object
---@param options number
function CS.UnityEngine.GameObject:SendMessage(methodName, value, options) end
---@overload fun(methodName:string): void
---@overload fun(methodName:string, options:number): void
---@overload fun(methodName:string, parameter:CS.System.Object): void
---@param methodName string
---@param parameter CS.System.Object
---@param options number
function CS.UnityEngine.GameObject:BroadcastMessage(methodName, parameter, options) end
---@return CS.UnityEngine.Component
---@param componentType string
function CS.UnityEngine.GameObject:AddComponent(componentType) end
---@param value boolean
function CS.UnityEngine.GameObject:SetActive(value) end
---@return boolean
---@param tag string
function CS.UnityEngine.GameObject:CompareTag(tag) end
---@return CS.UnityEngine.GameObject
---@param tag string
function CS.UnityEngine.GameObject.FindGameObjectWithTag(tag) end
---@return CS.UnityEngine.GameObject[]
---@param tag string
function CS.UnityEngine.GameObject.FindGameObjectsWithTag(tag) end
---@return CS.UnityEngine.GameObject
---@param name string
function CS.UnityEngine.GameObject.Find(name) end
---@return MoonCommonLib.GameObjectCallBack
function CS.UnityEngine.GameObject:GetCallback() end
function CS.UnityEngine.GameObject:ResetTransform() end
---@return CS.UnityEngine.GameObject
---@param name string
---@param recursion boolean
function CS.UnityEngine.GameObject:FindObjectInChild(name, recursion) end
---@return CS.UnityEngine.Component
---@param className string
function CS.UnityEngine.GameObject:GetComp(className) end
---@param isActive boolean
function CS.UnityEngine.GameObject:SetActiveEx(isActive) end
---@overload fun(pos:CS.UnityEngine.Vector3): void
---@param x number
---@param y number
---@param z number
function CS.UnityEngine.GameObject:SetPos(x, y, z) end
---@param x number
function CS.UnityEngine.GameObject:SetPosX(x) end
---@param y number
function CS.UnityEngine.GameObject:SetPosY(y) end
---@param z number
function CS.UnityEngine.GameObject:SetPosZ(z) end
function CS.UnityEngine.GameObject:SetPosZero() end
---@overload fun(other:CS.UnityEngine.GameObject): void
---@param other CS.UnityEngine.Transform
function CS.UnityEngine.GameObject:SetPosToOther(other) end
---@overload fun(pos:CS.UnityEngine.Vector3): void
---@param x number
---@param y number
---@param z number
function CS.UnityEngine.GameObject:SetLocalPos(x, y, z) end
---@param x number
function CS.UnityEngine.GameObject:SetLocalPosX(x) end
---@param y number
function CS.UnityEngine.GameObject:SetLocalPosY(y) end
---@param z number
function CS.UnityEngine.GameObject:SetLocalPosZ(z) end
function CS.UnityEngine.GameObject:SetLocalPosZero() end
---@overload fun(other:CS.UnityEngine.GameObject): void
---@param other CS.UnityEngine.Transform
function CS.UnityEngine.GameObject:SetLocalPosToOther(other) end
---@param x number
---@param y number
---@param z number
function CS.UnityEngine.GameObject:SetLocalScale(x, y, z) end
---@param x number
function CS.UnityEngine.GameObject:SetLocalScaleX(x) end
---@param y number
function CS.UnityEngine.GameObject:SetLocalScaleY(y) end
---@param z number
function CS.UnityEngine.GameObject:SetLocalScaleZ(z) end
function CS.UnityEngine.GameObject:SetLocalScaleOne() end
---@overload fun(other:CS.UnityEngine.GameObject): void
---@param other CS.UnityEngine.Transform
function CS.UnityEngine.GameObject:SetLocalScaleToOther(other) end
---@param x number
---@param y number
---@param z number
---@param w number
function CS.UnityEngine.GameObject:SetRot(x, y, z, w) end
---@param x number
---@param y number
---@param z number
---@param w number
function CS.UnityEngine.GameObject:SetLocalRot(x, y, z, w) end
---@param x number
---@param y number
---@param z number
function CS.UnityEngine.GameObject:SetRotEuler(x, y, z) end
---@param x number
function CS.UnityEngine.GameObject:SetRotEulerX(x) end
---@param y number
function CS.UnityEngine.GameObject:SetRotEulerY(y) end
---@param z number
function CS.UnityEngine.GameObject:SetRotEulerZ(z) end
function CS.UnityEngine.GameObject:SetRotEulerZero() end
---@overload fun(other:CS.UnityEngine.GameObject): void
---@param other CS.UnityEngine.Transform
function CS.UnityEngine.GameObject:SetRotEulerToOther(other) end
---@param x number
---@param y number
---@param z number
function CS.UnityEngine.GameObject:SetLocalRotEuler(x, y, z) end
---@param x number
function CS.UnityEngine.GameObject:SetLocalRotEulerX(x) end
---@param y number
function CS.UnityEngine.GameObject:SetLocalRotEulerY(y) end
---@param z number
function CS.UnityEngine.GameObject:SetLocalRotEulerZ(z) end
function CS.UnityEngine.GameObject:SetLocalRotEulerZero() end
---@overload fun(other:CS.UnityEngine.GameObject): void
---@param other CS.UnityEngine.Transform
function CS.UnityEngine.GameObject:SetLocalRotEulerToOther(other) end
---@param x number
---@param y number
function CS.UnityEngine.GameObject:SetRectTransformPos(x, y) end
---@param x number
function CS.UnityEngine.GameObject:SetRectTransformPosX(x) end
---@param width number
---@param height number
function CS.UnityEngine.GameObject:SetRectTransformSize(width, height) end
---@param width number
function CS.UnityEngine.GameObject:SetRectTransformWidth(width) end
---@param height number
function CS.UnityEngine.GameObject:SetRectTransformHeight(height) end
---@param y number
function CS.UnityEngine.GameObject:SetRectTransformPosY(y) end
---@param xMin number
---@param xMax number
---@param yMin number
---@param yMax number
function CS.UnityEngine.GameObject:SetRectTransformOffset(xMin, xMax, yMin, yMax) end
---@param anchorMinX number
---@param anchorMinY number
---@param anchorMaxX number
---@param anchorMaxY number
function CS.UnityEngine.GameObject:SetRectTransformAnchor(anchorMinX, anchorMinY, anchorMaxX, anchorMaxY) end
---@param pivotX number
---@param pivotY number
function CS.UnityEngine.GameObject:SetRectTransformPivot(pivotX, pivotY) end
return CS.UnityEngine.GameObject
