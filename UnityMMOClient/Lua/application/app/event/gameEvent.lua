local GameEvent = {}
-----------------------------Login-------------------------
GameEvent.RegisterAccount = event("GameEvent.RegisterAccount")

GameEvent.LoginResonse = event("GameEvent.LoginResonse")
GameEvent.RegisterResonse = event("GameEvent.RegisterResonse")
GameEvent.LoginTapToStartResponse = event("GameEvent.LoginTapToStartResponse")


GameEvent.AcquireUserIdWeaponService = event("GameEvent.AcquireUserIdWeaponService")
GameEvent.AcquireWeaponBagServerList = event("GameEvent.AcquireUserIdWeaponService")
--- 点击  协议号
---``` lua
--- function AllBagItemResponse:protocolId()
---  return 1007
---end
---```
GameEvent.ClickBagHeaderBtnHandlerServer = event("GameEvent.ClickBagHeaderBtnHandlerServer")
--- 点击 头部按钮 协议号 
---``` lua
--- function AllBagItemResponse:protocolId()
---  return 1008
---end
---```
GameEvent.AtBagHeaderBtnService = event("GameEvent.AtBagHeaderBtnService")





return GameEvent
