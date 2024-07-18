--[[
    描述： 状态定义枚举
--]]

StateConstant =
{
    -- 切换类型
    Change = 0,     -- UI状态或Game状态有效
    Push = 1,       -- UI状态或Game状态有效
    Pop = 2,        -- UI状态或Game状态有效
    PushChild = 3,  -- UI状态有效
    PopChild = 4,   -- UI状态有效
    ChangeTop = 5,  -- UI状态有效（内部使用）
    ChangeAll = 6,  -- UI状态有效（内部使用）

    -- 操作（内部使用，外部不可见）
    Stay = 1,
    Out = 2,
    In = 3,

    -- 交互常量
    ForecastPopState = "*popstate",
    AllState = "*allstate"
}

GameState =
{
    EmptyStateID = "EmptyStateID",      -- 空state
    LoginStateID = "LoginState",        -- 登录
    LoadingStateID = "LoadingState",    -- loading
    GameStateID = "GameState",          -- 场景
    LogoutStateID = "LogoutState",      -- 登出
}
