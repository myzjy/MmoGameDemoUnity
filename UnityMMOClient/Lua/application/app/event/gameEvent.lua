local GameEvent = {}
-----------------------------Login-------------------------
GameEvent.RegisterAccount = event("GameEvent.RegisterAccount")

GameEvent.LoginResonse = event("GameEvent.LoginResonse")
GameEvent.RegisterResonse = event("GameEvent.RegisterResonse")
GameEvent.LoginTapToStartResponse = event("GameEvent.LoginTapToStartResponse")


GameEvent.AcquireUserIdWeaponService = event("GameEvent.AcquireUserIdWeaponService")
GameEvent.AcquireWeaponBagServerList = event("GameEvent.AcquireUserIdWeaponService")

GameEvent.ClickBagHeaderBtnHandlerServer = event("GameEvent.ClickBagHeaderBtnHandlerServer")
--- 点击  协议号
---``` lua
--- function AllBagItemResponse:protocolId()
---  return 1008
---end
---```
GameEvent.AtBagHeaderWeaponBtnService = event("GameEvent.AtBagHeaderWeaponBtnService")
--- 点击 武器 协议号 c001的回调 c002 额外协议号
---``` lua
--- function AllBagItemResponse:protocolId()
---  return 1008
---end
---```
GameEvent.AtBagHeaderWeaponBtnServiceHandler = event("GameEvent.AtBagHeaderWeaponBtnServiceHandler")



return GameEvent
