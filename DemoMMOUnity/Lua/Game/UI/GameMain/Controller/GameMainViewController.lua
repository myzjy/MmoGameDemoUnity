--- GameMainViewController gameMain 界面 UI控制器 向外发送
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by Administrator.
--- DateTime: 2023/5/15 23:41

---@class GameMainViewController
GameMainViewController = {}

local modelView = nil
function GameMainViewController:GetInstance()
    return GameMainViewController
end

function GameMainViewController:OnInit(_modelView)
    modelView = _modelView
end

function GameMainViewController:OnOpen()
    if GameMainView.reUse then
        ---直接调用
        GameMainView:OnShow()
    else
        --没有初始化 UI
        GameMainView:OnLoad()
    end
end

---@param num number 金币数量
function GameMainViewController:ShowGoldNumTextAction(num)
    GameMainView:ShowGoldNumTextAction(num)
end

function GameMainViewController:ShowTime(dataTime)
    GameMainView:ShowNowTime(dataTime)
end
