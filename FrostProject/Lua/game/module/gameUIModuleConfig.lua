local GameUIModuleConfigList = {
    -- 配置模板
    -- templateModuleClassName = {
    --     moduleClassName = "TemplateModuleClassName",
    --     mediatorClassName = {
    --         "TemplateMediatorClassName1",
    --         "TemplateMediatorClassName2",
    --     },
    --     filesPath = {
    --         ""
    --     }
    -- }
    
    -- 登录 模块 包括登录、注册
    loginModule = {
        moduleClassName = "LoginModule",
        mediatorClassName = {
            "LoginMediator", --登录
            "RegisterMediator", -- 注册
        },
        filesPath = {
        }
    }
}

return GameUIModuleConfigList