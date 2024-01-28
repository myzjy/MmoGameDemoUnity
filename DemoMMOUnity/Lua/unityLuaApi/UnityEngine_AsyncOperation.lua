---@class UnityEngine.AsyncOperation : UnityEngine.YieldInstruction
---@field public isDone boolean
---@field public progress number
---@field public priority number
---@field public allowSceneActivation boolean

---@type UnityEngine.AsyncOperation
UnityEngine.AsyncOperation = { }
---@return UnityEngine.AsyncOperation
function UnityEngine.AsyncOperation.New() end
---@param value (fun(obj:UnityEngine.AsyncOperation):void)
function UnityEngine.AsyncOperation:add_completed(value) end
---@param value (fun(obj:UnityEngine.AsyncOperation):void)
function UnityEngine.AsyncOperation:remove_completed(value) end
return UnityEngine.AsyncOperation
