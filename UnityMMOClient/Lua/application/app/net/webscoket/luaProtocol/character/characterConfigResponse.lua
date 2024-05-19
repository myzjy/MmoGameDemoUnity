--- 角色 基础配置 response
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by Administrator.
--- DateTime: 2024/5/19 下午3:38
---@class CharacterConfigResponse
local CharacterConfigResponse = class("CharacterConfigResponse")

function CharacterConfigResponse:ctor()
    ---@type table<number,CharacterConfigData>
    self.characterConfigDataList = nil
end
--- 协议id
---@return number
function CharacterConfigResponse:protocolId()
    return 1053
end
--- 返回 角色配置 List table
---@return table<number,CharacterConfigData>
function CharacterConfigResponse:GetCharacterConfigDataList()
    return self.characterConfigDataList
end
---@param characterConfigDataList table<number,CharacterConfigData>
function CharacterConfigResponse:new(characterConfigDataList)
    self.characterConfigDataList = characterConfigDataList
    return self;
end

--- 读取相关 数据 并存放起来
---@param response CharacterConfigResponse 角色基础配置相关response
function CharacterConfigResponse:read(response)
    local responseData = self:new(response.characterConfigDataList)
    return responseData
end
return CharacterConfigResponse