---@class LoginMediator:UIMediator
local LoginMediator = Class("LoginMediator",ClassLibraryMap.UIMediator)
function LoginMediator:vGetBelongUIStateName()
    return 
    {
        "LoginProcedure"
    }
end

return LoginMediator