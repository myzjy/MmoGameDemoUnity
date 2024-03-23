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
    }
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
