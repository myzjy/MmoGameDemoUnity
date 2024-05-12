local GameEvent = {}
-----------------------------Login-------------------------
GameEvent.RegisterAccount = event("GameEvent.RegisterAccount")

GameEvent.LoginResponse = event("GameEvent.LoginResponse")
GameEvent.RegisterResponse = event("GameEvent.RegisterResponse")
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
--- GameMainUI界面 更新玩家信息
GameEvent.UpdateGameMainUserInfoMsg = event("GameEvent.UpdateGameMainUserInfoMsgServer")
-- 更新 显示金币 砖石 付费砖石 协议 更新  userMsgInfoData
GameEvent.UpDateGemsAndGoldInfo = event("GameEvent.UpDateGemsAndGoldInfo")
--- 更新体力 显示 保存得体力
GameEvent.UpdateGamePhysicalInfo = event("GameEvent.UpdateGamePhysicalInfo")






return GameEvent
