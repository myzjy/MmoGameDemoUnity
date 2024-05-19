---@class netManager
---@field SendMessageEvent function
local netManager = CS.ZJYFrameWork.Spring.Core.SpringContext.GetBean("ZJYFrameWork.Net.NetManager") or
    CS.ZJYFrameWork.Net.NetManager

---@class NetManager
local NetManager = class("NetManager")


---@param bytes string|nil
function NetManager:SendMessageEvent(bytes)
    netManager:SendMessageEvent(bytes)
end

return NetManager
