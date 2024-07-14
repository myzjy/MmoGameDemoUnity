
--[[---------------------------------------------------------------------------------------
    Copyright 2024 - 2026 Tencent. All Rights Reserved
    Author : zhangjingyi
    brief : 用于声明全局枚举, 后续的面向仅客户端的枚举类全部加入此类中
--]]---------------------------------------------------------------------------------------

GlobalEnum = {
    EState = {
        None = 0,
        
        -- [Clinet]
        -- Stage切换顺序 Init -> Login -> Rookie -> Lobby -> Preload -> GameWorld <-> Exit
        Init = 1,
        Preload = 2,
        Login = 3,
        Lobby = 4,
        Rookie = 5,
        GameWorld = 6,
        Exit = 7, -- 主要用于退出清理
    }
}