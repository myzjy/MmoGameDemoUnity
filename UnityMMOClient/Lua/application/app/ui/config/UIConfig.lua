local UIConfig = {
    --- UICanvasType UI层级
    UICanvasType = {
        None       = 0,
        BG         = 1,
        UI         = 2,
        LOADING    = 3,
        TOP        = 4,
        NOTICE     = 5,
        ActiviesUI = 6,
    },
    ---  排序顺序
    UISortType = {
        --- 将变换移动到本地变换列表的开头
        First = 0,
        --- 将变换移动到本地变换列表的末尾
        Last = 1,
    }
}
UIConfig.FishConfig = {
    LoginConfig = {
        name = "Login",
        scriptPath = "application.app.ui.login.LoginView"
    },
    BagUIConfig = {
        prefabName = "BagUIPanel",
        --- 当前会生成在那一层
        canvasType = UIConfig.UICanvasType.UI,
        sortType = UIConfig.UISortType.First,
        --- 当前 UI 交互事件 消息
        eventNotification = {
            --- 打开游戏主界面
            openbaguipanel = "openbaguipanel",
            --- 关闭 游戏主界面
            closebaguipanel = "closebaguipanel",
        },
        weaponIconAtlasName = "uibagweaponicon_spriteatlas",
        viewScriptPath = "application/app/ui/bag/bagUIView"
    },
}
UIConfig.BagHeaderBtnConfig = {
    Type = {
        WeaponType = 0
    },
    IconConfig = {
        Open = "headerBtnOpen",
        Hide = "headerBtnHide"
    }

}
UIConfig.BagHeaderBtnConfig.Name = {
    [UIConfig.BagHeaderBtnConfig.Type.WeaponType] = "武器"

}
return UIConfig
