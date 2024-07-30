---@class LoginMediator:UIMediator
local LoginMediator = Class("LoginMediator",ClassLibraryMap.UIMediator)
function LoginMediator:vGetBelongUIStateName()
    return 
    {
        "LoginProcedure"
    }
end

function LoginMediator:SwitchUIStateIn(switchType, userData)
    self:CreateAsyncPrefabClass(ClassLibraryMap.LoginPanel, nil, nil,nil, 1)
end

return LoginMediator