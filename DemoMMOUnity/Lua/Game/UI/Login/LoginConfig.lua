local LoginConfig = {
    ---预制体名字
    prefabName = "LoginPanel",
    --- 当前会生成在那一层
    canvasType = UIConfigEnum.UICanvasType.UI,
    sortType = UIConfigEnum.UISortType.First,
    --- 当前 UI 交互事件 消息
    eventNotification = {
        OPEN_LOGIN_INIT_PANEL = "OPEN_LOGIN_UI",
        CLOSE_LOGIN_INIT_PANEL = "CLOSE_LOGIN_PANEL"
    }
}

return LoginConfig
