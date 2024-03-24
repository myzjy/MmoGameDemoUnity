local UIGameEvent = {}

--- 点击武器
UIGameEvent.BagHeaderWeaponHandler = event("UIGameEvent.BagHeaderBtnWeaponHandler")
UIGameEvent.BagHeaderWeaponBtnHandler = event("UIGameEvent.BagHeaderWeaponBtnHandler")

--- 背包界面点击 顶头的按钮
UIGameEvent.OnSelectBagHeaderBtnHandler = event("UIGameEvent.OnSelectBagHeaderBtnHandler")
-- 背包 顶上 切换页 按钮 创建时间
UIGameEvent.CreateBagHeaderBtnHandler = event("UIGameEvent.CreateBagHeaderBtnHandler")
-- 背包 顶上 切换页 按钮 移除
UIGameEvent.DeleteBagHeaderBtnHandler = event("UIGameEvent.DeleteBagHeaderBtnHandler")

return UIGameEvent
