UnityHelper = {}


---------------------------------------------------------------------------------------------
--- 返回ID是否合法
---@param inID number
---@return boolean 合法返回true，否则返回false
---------------------------------------------------------------------------------------------
function UnityHelper.IsValidID(inID)
    return inID and inID ~= GlobalEnum.EInvalidDefine.ID
end

function UnityHelper.GetTransformAllChild(inTransform)
    return UnityHelper.TransformChild(inTransform)
end

function UnityHelper.TransformChild(inTransform)
    local child = false
    if inTransform then
        if not child then
            child = {}
        end
        for i = 1, inTransform.transform.childCount do
            local gameObject = inTransform.transform:GetChild(i-1).gameObject
            if gameObject.transform.childCount > 0 then
                local tab = UnityHelper.TransformChild(inTransform.transform:GetChild(i-1).gameObject)
                if tab then
                    table.insert(child,tab)
                end
            else
                table.insert(child,inTransform.transform:GetChild(i-1).gameObject)
            end
        end
    end
    return child
end