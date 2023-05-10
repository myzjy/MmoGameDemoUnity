---@class Enum
Enum=BaseClass()


---@param enum table
---@param enumVal number
function Enum.ToString(enum,enumVal)
    for k, v in pairs(enum) do
        if v == enumVal then
            return tostring(k)
        end
    end

    return tostring(enumVal)
end

---Equals
---@param enum table
---@param enumVal string
function Enum.ToInit(enum, enumVal)
    for k, v in pairs(enum) do
        if tostring(k) == enumVal then
            return v
        end
    end

    return 0
end

return Enum;