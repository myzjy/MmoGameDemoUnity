local LoginConfig = {
    ---预制体名字
    prefabName = "LoginPanel",
    --- 当前会生成在那一层
    canvasType = UIConfigEnum.UICanvasType.UI,
    sortType = UIConfigEnum.UISortType.First,
    --- 当前 UI 交互事件 消息
    eventNotification = {
        --- 打开登录 注册界面
        OPEN_LOGIN_INIT_PANEL = "OPEN_LOGIN_UI",
        --- 关闭登录 注册界面
        CLOSE_LOGIN_INIT_PANEL = "CLOSE_LOGIN_PANEL",
        --- 打开 登录界面的开始游戏按钮
        OpenLoginTapToStartUI = "OPEN_LOGIN_TAP_TO_START_UI",
        --- 当我们登录成功之后，闪过登录账号
        ShowLoginAccountUI = "Show_Login_Account_UI"
    },
    scriptPath="application.app.ui.login.LoginView"
}

return LoginConfig
