---@class netManager
---@field SendMessageEvent function
local netManager = CS.ZJYFrameWork.Spring.Core.SpringContext.GetBean("ZJYFrameWork.Net.NetManager") or
    CS.ZJYFrameWork.Net.NetManager

---@class NetManager
local NetManager = class("NetManager")


---@param bytes string
---@param protocolId number
---@param protocolAction function
function NetManager:SendMessageEvent(bytes, protocolId, protocolAction)
    ProtocolManager:AddProtocolConfigEvent(protocolId, protocolAction)
    netManager:SendMessageEvent(bytes, protocolId, protocolAction)
end

return NetManager
