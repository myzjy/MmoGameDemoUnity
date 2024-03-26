local GameEvent = {}
-----------------------------Login-------------------------
GameEvent.RegisterAccount = event("GameEvent.RegisterAccount")

GameEvent.LoginResonse = event("GameEvent.LoginResonse")
GameEvent.RegisterResonse = event("GameEvent.RegisterResonse")
GameEvent.LoginTapToStartResponse = event("GameEvent.LoginTapToStartResponse")


GameEvent.AcquireUserIdWeaponService = event("GameEvent.AcquireUserIdWeaponService")
GameEvent.AcquireWeaponBagServerList = event("GameEvent.AcquireUserIdWeaponService")

GameEvent.ClickBagHeaderBtnHandlerServer = event("GameEvent.ClickBagHeaderBtnHandlerServer")



return GameEvent
