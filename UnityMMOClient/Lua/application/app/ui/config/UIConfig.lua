UIConfig = {}
--- UICanvasType UI层级
UIConfig.UICanvasType = {
    None = 0,
    BG = 1,
    UI = 2,
    LOADING = 3,
    TOP = 4,
    NOTICE = 5,
    ActiviesUI = 6,
}
---  排序顺序
UIConfig.UISortType = {
    --- 将变换移动到本地变换列表的开头
    First = 0,
    --- 将变换移动到本地变换列表的末尾
    Last = 1,
}
UIEventConfig = {
    LoginConfig = {
        name = "Login",
        scriptPath = "application.app.ui.login.LoginView",
        ---预制体名字
        prefabName = "LoginPanel",
        --- 当前会生成在那一层
        canvasType = UIConfig.UICanvasType.UI,
        sortType = UIConfig.UISortType.First,
        scriptPath = "application.app.ui.login.LoginView"
    },
    BagUIConfig = {
        canvasType = UIConfig.UICanvasType.UI,
        sortType = UIConfig.UISortType.Last,
        name = "BagUIPanel",
        prefabName = "BagUIPanel",
        scriptPath = "application/app/ui/bag/bagUIView"
    },
    GameMainConfig = {
        name = "GameMainUIPanel",
        prefabName = "GameMainUIPanel",
        --- 当前会生成在那一层
        canvasType = UIConfig.UICanvasType.UI,
        sortType = UIConfig.UISortType.First,
        scriptPath = "application/app/ui/gameMain/gameMainView"
    },
    CharacterUIConfig = {
        name = "CharacterUIPanel",
        prefabName = "CharacterUIPanel",
        canvasType = UIConfig.UICanvasType.UI,
        sortType = UIConfig.UISortType.First,
        scriptPath = "application/app/ui/character/characterUIView"
    }
}
UIConfig.BagHeaderBtnConfig = {
    Type = {
        WeaponType = 1
    },
    IconConfig = {
        Open = "headerBtnOpen",
        Hide = "headerBtnHide"
    }

}
UIConfig.BagHeaderBtnConfig.Name = {
    [UIConfig.BagHeaderBtnConfig.Type.WeaponType] = "武器"

}
