---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zhangjingyi.
--- DateTime: 2023/7/6 10:40
---

---@class PhysicalPowerNetController
local PhysicalPowerNetController = class("physicalPowerNetController")

PhysicalConst = {
    Status = {

    },
    Event = {
        --- 使用体力
        PhysicalPowerSecondsProps = "Event.PhysicalConst.PhysicalPowerSeconds",
        PhysicalPowerRequest = 1023,
        PhysicalPowerResponse = 1024,
        PhysicalPowerUserPropsResponse = 1026
    }
}

function PhysicalPowerNetController:Init()
    self:InitEvent()
end

--- 初始化 事件
function PhysicalPowerNetController:InitEvent()
    GlobalEventSystem:Bind(PhysicalConst.Event.PhysicalPowerSecondsProps, function()
        PhysicalPowerService:SendPhysicalPowerSecondsRequest()
    end)
    GlobalEventSystem:Bind(PhysicalConst.Event.PhysicalPowerResponse, function(res)
        PhysicalPowerNetController:AtPhysicalPowerResponse(res)
    end)
    GlobalEventSystem:Bind(PhysicalConst.Event.PhysicalPowerUserPropsResponse, function(res)
        PhysicalPowerNetController:AtPhysicalPowerUserPropsResponse(res)
    end)
end

--- 发起体力请求 刷新
--- @param response {nowPhysicalPower:number, residueTime:number, residueNowTime:number, maximumStrength:number, maximusResidueEndTime:number}
function PhysicalPowerNetController:AtPhysicalPowerResponse(response)
    PhysicalPowerCacheData:setNowPhysicalPower(response.nowPhysicalPower)
    PhysicalPowerCacheData:setMaximumStrength(response.maximumStrength)
    PhysicalPowerCacheData:setResidueTime(response.residueTime)
    PhysicalPowerCacheData:setMaximusResidueEndTime(response.maximusResidueEndTime)
    PhysicalPowerCacheData:setResidueNowTime(response.residueNowTime)
    GameMainViewController:GetInstance():ShowPhysicalPowerPanel(response)
end

--- @param response {nowPhysicalPower:number, residueTime:number, residueNowTime:number, maximumStrength:number, maximusResidueEndTime:number,nowReturn:boolean}
function PhysicalPowerNetController:AtPhysicalPowerUserPropsResponse(response)
    PhysicalPowerCacheData:setNowPhysicalPower(response.nowPhysicalPower)
    PhysicalPowerCacheData:setMaximumStrength(response.maximumStrength)
    PhysicalPowerCacheData:setResidueTime(response.residueTime)
    PhysicalPowerCacheData:setMaximusResidueEndTime(response.maximusResidueEndTime)
    PhysicalPowerCacheData:setResidueNowTime(response.residueNowTime)
    PhysicalPowerCacheData:setReturn(response.nowReturn)
    GameMainViewController:GetInstance():ShowPhysicalPowerPanel(response)

end

return PhysicalPowerNetController