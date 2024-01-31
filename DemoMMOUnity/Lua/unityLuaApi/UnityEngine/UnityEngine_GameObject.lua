---@class UnityEngine.GameObject : UnityEngine.Object
---@field public transform UnityEngine.Transform
---@field public layer number
---@field public activeSelf boolean
---@field public activeInHierarchy boolean
---@field public isStatic boolean
---@field public tag string
---@field public scene UnityEngine.SceneManagement.Scene
---@field public gameObject UnityEngine.GameObject

---@type UnityEngine.GameObject
UnityEngine.GameObject = { }
---@overload fun(): UnityEngine.GameObject
---@overload fun(name:string): UnityEngine.GameObject
---@return UnityEngine.GameObject
---@param name string
---@param components System.Type[]
function UnityEngine.GameObject.New(name, components) end
---@return UnityEngine.GameObject
---@param t number
function UnityEngine.GameObject.CreatePrimitive(t) end
---@return UnityEngine.Component
---@param t string
function UnityEngine.GameObject:GetComponent(t) end
---@overload fun(t:string): UnityEngine.Component
---@return UnityEngine.Component
---@param t string
---@param includeInactive boolean
function UnityEngine.GameObject:GetComponentInChildren(t, includeInactive) end
---@return UnityEngine.Component
---@param t string
function UnityEngine.GameObject:GetComponentInParent(t) end
---@overload fun(t:string): UnityEngine.Component[]
---@return UnityEngine.Component[]
---@param t string
---@param results System.Collections.Generic.List_UnityEngine.Component
function UnityEngine.GameObject:GetComponents(t, results) end
---@overload fun(t:string): UnityEngine.Component[]
---@return UnityEngine.Component[]
---@param t string
---@param includeInactive boolean
function UnityEngine.GameObject:GetComponentsInChildren(t, includeInactive) end
---@overload fun(t:string): UnityEngine.Component[]
---@return UnityEngine.Component[]
---@param t string
---@param includeInactive boolean
function UnityEngine.GameObject:GetComponentsInParent(t, includeInactive) end
---@return UnityEngine.GameObject
---@param tag string
function UnityEngine.GameObject.FindWithTag(tag) end
---@overload fun(methodName:string): void
---@overload fun(methodName:string, options:number): void
---@overload fun(methodName:string, value:System.Object): void
---@param methodName string
---@param value System.Object
---@param options number
function UnityEngine.GameObject:SendMessageUpwards(methodName, value, options) end
---@overload fun(methodName:string): void
---@overload fun(methodName:string, options:number): void
---@overload fun(methodName:string, value:System.Object): void
---@param methodName string
---@param value System.Object
---@param options number
function UnityEngine.GameObject:SendMessage(methodName, value, options) end
---@overload fun(methodName:string): void
---@overload fun(methodName:string, options:number): void
---@overload fun(methodName:string, parameter:System.Object): void
---@param methodName string
---@param parameter System.Object
---@param options number
function UnityEngine.GameObject:BroadcastMessage(methodName, parameter, options) end
---@return UnityEngine.Component
---@param componentType string
function UnityEngine.GameObject:AddComponent(componentType) end
---@param value boolean
function UnityEngine.GameObject:SetActive(value) end
---@return boolean
---@param tag string
function UnityEngine.GameObject:CompareTag(tag) end
---@return UnityEngine.GameObject
---@param tag string
function UnityEngine.GameObject.FindGameObjectWithTag(tag) end
---@return UnityEngine.GameObject[]
---@param tag string
function UnityEngine.GameObject.FindGameObjectsWithTag(tag) end
---@return UnityEngine.GameObject
---@param name string
function UnityEngine.GameObject.Find(name) end
---@return MoonCommonLib.GameObjectCallBack
function UnityEngine.GameObject:GetCallback() end
function UnityEngine.GameObject:ResetTransform() end
---@return UnityEngine.GameObject
---@param name string
---@param recursion boolean
function UnityEngine.GameObject:FindObjectInChild(name, recursion) end
---@return UnityEngine.Component
---@param className string
function UnityEngine.GameObject:GetComp(className) end
---@param isActive boolean
function UnityEngine.GameObject:SetActiveEx(isActive) end
---@overload fun(pos:UnityEngine.Vector3): void
---@param x number
---@param y number
---@param z number
function UnityEngine.GameObject:SetPos(x, y, z) end
---@param x number
function UnityEngine.GameObject:SetPosX(x) end
---@param y number
function UnityEngine.GameObject:SetPosY(y) end
---@param z number
function UnityEngine.GameObject:SetPosZ(z) end
function UnityEngine.GameObject:SetPosZero() end
---@overload fun(other:UnityEngine.GameObject): void
---@param other UnityEngine.Transform
function UnityEngine.GameObject:SetPosToOther(other) end
---@overload fun(pos:UnityEngine.Vector3): void
---@param x number
---@param y number
---@param z number
function UnityEngine.GameObject:SetLocalPos(x, y, z) end
---@param x number
function UnityEngine.GameObject:SetLocalPosX(x) end
---@param y number
function UnityEngine.GameObject:SetLocalPosY(y) end
---@param z number
function UnityEngine.GameObject:SetLocalPosZ(z) end
function UnityEngine.GameObject:SetLocalPosZero() end
---@overload fun(other:UnityEngine.GameObject): void
---@param other UnityEngine.Transform
function UnityEngine.GameObject:SetLocalPosToOther(other) end
---@param x number
---@param y number
---@param z number
function UnityEngine.GameObject:SetLocalScale(x, y, z) end
---@param x number
function UnityEngine.GameObject:SetLocalScaleX(x) end
---@param y number
function UnityEngine.GameObject:SetLocalScaleY(y) end
---@param z number
function UnityEngine.GameObject:SetLocalScaleZ(z) end
function UnityEngine.GameObject:SetLocalScaleOne() end
---@overload fun(other:UnityEngine.GameObject): void
---@param other UnityEngine.Transform
function UnityEngine.GameObject:SetLocalScaleToOther(other) end
---@param x number
---@param y number
---@param z number
---@param w number
function UnityEngine.GameObject:SetRot(x, y, z, w) end
---@param x number
---@param y number
---@param z number
---@param w number
function UnityEngine.GameObject:SetLocalRot(x, y, z, w) end
---@param x number
---@param y number
---@param z number
function UnityEngine.GameObject:SetRotEuler(x, y, z) end
---@param x number
function UnityEngine.GameObject:SetRotEulerX(x) end
---@param y number
function UnityEngine.GameObject:SetRotEulerY(y) end
---@param z number
function UnityEngine.GameObject:SetRotEulerZ(z) end
function UnityEngine.GameObject:SetRotEulerZero() end
---@overload fun(other:UnityEngine.GameObject): void
---@param other UnityEngine.Transform
function UnityEngine.GameObject:SetRotEulerToOther(other) end
---@param x number
---@param y number
---@param z number
function UnityEngine.GameObject:SetLocalRotEuler(x, y, z) end
---@param x number
function UnityEngine.GameObject:SetLocalRotEulerX(x) end
---@param y number
function UnityEngine.GameObject:SetLocalRotEulerY(y) end
---@param z number
function UnityEngine.GameObject:SetLocalRotEulerZ(z) end
function UnityEngine.GameObject:SetLocalRotEulerZero() end
---@overload fun(other:UnityEngine.GameObject): void
---@param other UnityEngine.Transform
function UnityEngine.GameObject:SetLocalRotEulerToOther(other) end
---@param x number
---@param y number
function UnityEngine.GameObject:SetRectTransformPos(x, y) end
---@param x number
function UnityEngine.GameObject:SetRectTransformPosX(x) end
---@param width number
---@param height number
function UnityEngine.GameObject:SetRectTransformSize(width, height) end
---@param width number
function UnityEngine.GameObject:SetRectTransformWidth(width) end
---@param height number
function UnityEngine.GameObject:SetRectTransformHeight(height) end
---@param y number
function UnityEngine.GameObject:SetRectTransformPosY(y) end
---@param xMin number
---@param xMax number
---@param yMin number
---@param yMax number
function UnityEngine.GameObject:SetRectTransformOffset(xMin, xMax, yMin, yMax) end
---@param anchorMinX number
---@param anchorMinY number
---@param anchorMaxX number
---@param anchorMaxY number
function UnityEngine.GameObject:SetRectTransformAnchor(anchorMinX, anchorMinY, anchorMaxX, anchorMaxY) end
---@param pivotX number
---@param pivotY number
function UnityEngine.GameObject:SetRectTransformPivot(pivotX, pivotY) end
return UnityEngine.GameObject
