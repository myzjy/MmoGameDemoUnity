local UIGameEvent = {}

--- 点击武器
UIGameEvent.BagHeaderWeaponHandler = event("UIGameEvent.BagHeaderBtnWeaponHandler")
UIGameEvent.BagHeaderWeaponBtnHandler = event("UIGameEvent.BagHeaderWeaponBtnHandler")

--- 背包界面点击 顶头的按钮
UIGameEvent.OnSelectBagHeaderBtnHandler = event("UIGameEvent.OnSelectBagHeaderBtnHandler")


return UIGameEvent
