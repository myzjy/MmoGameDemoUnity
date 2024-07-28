---@class CS.UnityEngine.SceneManagement.Scene
---@field public name string
---@field public isLoaded boolean
---@field public buildIndex number
---@field public isDirty boolean
---@field public dirtyID number
---@field public rootCount number
---@field public isSubScene boolean
---@field public handle number
---@field public path string
CS.UnityEngine.SceneManagement.Scene ={}

---@return boolean
function CS.UnityEngine.SceneManagement.Scene:IsValid()
    return false
end

---@overload fun(rootGameObjects:table<number,CS.UnityEngine.GameObject>):void
---@return table<number,CS.UnityEngine.GameObject>
function CS.UnityEngine.SceneManagement.Scene:GetRootGameObjects()
    return {}
end

---@return number
function  CS.UnityEngine.SceneManagement.Scene:GetHashCode()
    return 0
end
