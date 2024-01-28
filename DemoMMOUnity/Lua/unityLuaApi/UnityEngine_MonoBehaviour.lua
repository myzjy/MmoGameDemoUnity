---@class UnityEngine.MonoBehaviour : UnityEngine.Behaviour
---@field public useGUILayout boolean
---@field public runInEditMode boolean

---@type UnityEngine.MonoBehaviour
UnityEngine.MonoBehaviour = { }
---@return UnityEngine.MonoBehaviour
function UnityEngine.MonoBehaviour.New() end
---@overload fun(): boolean
---@return boolean
---@param methodName string
function UnityEngine.MonoBehaviour:IsInvoking(methodName) end
---@overload fun(): void
---@param methodName string
function UnityEngine.MonoBehaviour:CancelInvoke(methodName) end
---@param methodName string
---@param time number
function UnityEngine.MonoBehaviour:Invoke(methodName, time) end
---@param methodName string
---@param time number
---@param repeatRate number
function UnityEngine.MonoBehaviour:InvokeRepeating(methodName, time, repeatRate) end
---@overload fun(methodName:string): UnityEngine.Coroutine
---@overload fun(routine:System.Collections.IEnumerator): UnityEngine.Coroutine
---@return UnityEngine.Coroutine
---@param methodName string
---@param value System.Object
function UnityEngine.MonoBehaviour:StartCoroutine(methodName, value) end
---@overload fun(routine:System.Collections.IEnumerator): void
---@overload fun(routine:UnityEngine.Coroutine): void
---@param methodName string
function UnityEngine.MonoBehaviour:StopCoroutine(methodName) end
function UnityEngine.MonoBehaviour:StopAllCoroutines() end
---@param message System.Object
function UnityEngine.MonoBehaviour.print(message) end
return UnityEngine.MonoBehaviour
