--- 通知界面的 控制器
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zhangjingyi.
--- DateTime: 2023/11/27 15:08
---
---@class UICommonViewController
UICommonViewController = class("UICommonViewController")
local instance = nil
function UICommonViewController:ctor()
    self.viewPanel = nil
    self.LoadingRotate = nil
    self.dataloading = nil
end

function UICommonViewController:GetInstance()
    if not instance then
        instance = UICommonViewController()
    end
    return instance
end

function UICommonViewController:OnInit()
    self.viewPanel = UICommonView
    self.LoadingRotate = LoadingRotate
    self.dataloading = CS.ZJYFrameWork.UISerializable.Common.CommonController.Instance.snackbar.UIDataLoading
    self.viewPanel:OnInit(CS.ZJYFrameWork.UISerializable.Common.CommonController.Instance.snackbar.DialogPanelObject)
    self.LoadingRotate:OnInit(CS.ZJYFrameWork.UISerializable.Common.CommonController.Instance.loadingRotate)
end

function UICommonViewController:OnOpenDialog(buttonType, title, body, yesButtonText, noButtonText, onClick)
    self.viewPanel:OnOpen(buttonType, title, body, yesButtonText, noButtonText, onClick)
end

--- 转换场景的时候调用显示面板
---@param nowDownNums number 当前进度
---@param maxDownNums number 最大进度
function UICommonViewController:OpenUIDataScenePanel(nowDownNums, maxDownNums)
    if self.dataloading.GetSelfObjCanvasGroup.alpha < 1 then
        self.dataloading.OnOpen()
    end
    self.dataloading:SetSceneProgress(nowDownNums, maxDownNums)
end
