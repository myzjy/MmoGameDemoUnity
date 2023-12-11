--- 控制 网络 发送时 的 警示动画
--04

LoadingRotate = class("LoadingRotate")

function LoadingRotate:OnInit(view)
	self.gameObject = view.gameObject
	self.loadingDot = view.loadingDot
end

function LoadingRotate:OnShow()
	self.gameObject:SetActive(true)
	self.loadingDot.enabled = true
end
function LoadingRotate:OnClose()
	self.loadingDot.enabled = false
	self.gameObject:SetActive(false)
end
