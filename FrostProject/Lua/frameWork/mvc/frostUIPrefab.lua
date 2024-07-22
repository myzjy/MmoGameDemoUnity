---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by Administrator.
--- DateTime: 2024/7/20 上午1:41
---

---@class UIPrefab
---@field _uiAsset ULuaResource
---@field _prefab UUserWidget
local UIPrefab = Class("UIPrefab")

local UIAudioType = {
    Create = "CreateAudioEvent",      -- 创建
    Destroy = "DestroyAudioEvent",    -- 销毁时
    Visible = "VisibleAudioEvent",    -- 可见时
    Invisible = "InvisibleAudioEvent" -- 不可见时
}

---初始化成员变量。不在这里初始化的成员变量是无法赋值的。不需要调父类Constructor。
function UIPrefab:Constructor()
    self._uiAsset = false
    self._prefab = false
    self._parentTf = false

    self._mediator = false
    self._parent = false
    self._child = {}
    self._prefabClassAsyn = {}
    self._imageAsyn = {}
    self._image2AssetPath = {}
    self._audioCfg = false
    self._realLayer = false

    self._customID = GlobalEnum.EInvalidDefine.ID
    self._styleIndex = 1
    self._tagName = false
    self._isControlInputByVisible = true -- 是否通过可见性控制 vOnInputUpdateUI 处理输入事件

    self._autoPlayAnimDelay = {  -- 异步播放动画队列
        Queue = {},
        AccumulationTime = 0,
        Enable = false
    }
    ---@type {tbl:UIPrefab, func:function, param:any}[]
    self._preDestroyDelegates = {}
end

---设置是否可见
---@param inSlateVisibility ESlateVisibility 可见性类型
function UIPrefab:SetVisibility(inSlateVisibility)
    if not self._prefab then
        FrostLogE(self.__classname, "SetVisibility invalid prefab")
        return
    end
    local tOldVisibility = self._prefab:GetVisibility()
    if tOldVisibility == inSlateVisibility then return end
    local tOldIsVisible = self._prefab:IsVisible()
    self._prefab:SetVisibility(inSlateVisibility)
    local tNewIsVisible = self._prefab:IsVisible()
    if tOldIsVisible ~= tNewIsVisible then
        self:NotifyPrefabVisible(tNewIsVisible)
        -- 基于可见性，调整是否接收输入事件
        if self._isControlInputByVisible and self.vOnInputUpdateUI then
            if tNewIsVisible then
                MVCService:AddInputDelegate(self, self.OnInputHandler)
            else
                MVCService:RemoveInputDelegate(self)
            end
        end
    end

    -- 可见时，尝试播放动画
    if tNewIsVisible then
        self:_PlayAudio(UIAudioType.Visible)
        self:_PlayAutoAnim(UE4.ECMAnimationAutoPlayMode.Visible)
    else
        self:_PlayAudio(UIAudioType.Invisible)
    end

    -- todo 向下通知 _prefabClassAsyn 和 _child 隐藏事件
    self:vOnVisibilityChanged(inSlateVisibility, tOldVisibility)
end

function UIPrefab:vGetPrefabType()
    return false
end

function UIPrefab:NotifyPrefabVisible(inVisibility)
    if not self:vGetPrefabType() then return end
    local tPrefabType = self:vGetPrefabType()
    -- 缓存PrefabType的增删
    if inVisibility then
        MVCService:AddPrefabType(tPrefabType)
    else
        MVCService:RemovePrefabType(tPrefabType)
    end
    -- 通知条件系统检查条件是否完成
    EventService:SendEvent("UI.OnChangePrefab", nil, nil, tPrefabType, inVisibility, self._prefab)
end

-- 根据业务逻辑，主动添加回input监听
function UIPrefab:AddInputDelegate()
    if self.vOnInputUpdateUI then
        MVCService:AddInputDelegate(self, self.OnInputHandler)
    end
end

-- 根据业务逻辑，主动移除掉input监听
function UIPrefab:RemoveInputDelegate()
    if self.vOnInputUpdateUI then
        MVCService:RemoveInputDelegate(self)
    end
end

---获取索引对应ui样式的资源路径
---@param style number ui样式索引
---@return string ui样式的资源路径
function UIPrefab:GetResourcePath(style)
    self._styleIndex = style or 1
    local styleArray = self:vGetPath()
    if not styleArray or #styleArray == 0 then
        return nil
    end

    return styleArray[self._styleIndex]
end

---创建ui
---@param mediator UIMediator 可以通过SendAction传递到信息的对应中介
---@param parentPrefabClass UIPrefab 父ui
---@param parentTf UPanelWidget UE中的父控件（即创建出的控件所要挂载的地方）
---@param argument table 自定义参数，用于vInitialize时读取
---@param customID number 自定义索引，给父UI查找用（即父UI可以通过这个ID找到这个UI）
---@param style number ui样式索引，对应vGetPath里的资源路径
---@param layer number parentTf为空时挂载的对应层级索引
---@param asyncLoadID number 异步加载任务ID，用来获取资源
---@param bNotAddLayer boolean 为true则不直接挂载到对应层级，由自己后续控制
---@param insertFirst boolean 是否从头部插入节点
function UIPrefab:Create(mediator, parentPrefabClass, parentTf, argument, customID, style, layer, asyncLoadID, bNotAddLayer, insertFirst)
    local resourcePath = self:GetResourcePath(style)
    if not resourcePath then
        FrostLogE(self.__classname, "UIPrefab.Create : resource path error")
        return
    end

    self._uiAsset = AssetService:LoadUIAsset(resourcePath, LifeType.UIState, asyncLoadID)
    if not self._uiAsset then
        FrostLogE(self.__classname, "UIPrefab.Create : load resource failed - ", resourcePath)
        return
    end

    self._mediator = mediator or false
    self._parent = parentPrefabClass or false
    self._prefab = self._uiAsset:GetWidget()
    self._parentTf = parentTf or false

    self._customID = customID or GlobalEnum.EInvalidDefine.ID

    -- 避免点击穿透（仅对没有挂接点直接创建在viewport上的界面生效）
    if not self._parentTf then
        self._prefab.IsBlockInput = true
    end

    if not bNotAddLayer then
        self._realLayer = MVCService:AddLayerPrefab(self, self._parent, parentTf, layer, insertFirst) or false
    end

    if self.vOnInputUpdateUI then
        MVCService:AddInputDelegate(self, self.OnInputHandler)
    end

    CNSService:BeginFlow(FlowId.All, FlowId.ResourceInitialize, self.__classname)
    self:vOnResourceLoaded()
    CNSService:EndFlow(FlowId.All, FlowId.ResourceInitialize)

    CNSService:BeginFlow(FlowId.All, FlowId.LogicInitialize, self.__classname)
    self:vOnInitialize(argument)
    self:NotifyPrefabVisible(true)
    CNSService:EndFlow(FlowId.All, FlowId.LogicInitialize)

    -- 初始化audioCfg
    if self._prefab.AudioEventCfg then
        self._audioCfg = self._prefab.AudioEventCfg:ToTable() or false
        if self._audioCfg and next(self._audioCfg) == nil then
            self._audioCfg = false
        end
        self:_PlayAudio(UIAudioType.Create)
    end

    if CheatService and CheatService.IsHidenUI then
        if self._prefab then
            self._prefab:SetVisibility(UE4.ESlateVisibility.Collapsed)
            self._prefab.WidgetTree.RootWidget:SetRenderOpacity(0)
        end
    else
        self:_PlayAutoAnim(UE4.ECMAnimationAutoPlayMode.Create)
    end

end

---销毁。会调vOnUninitialize。
function UIPrefab:Destroy()
    self:NotifyPreDestroyDelegate()
    table.clear(self._preDestroyDelegates)

    if self.vOnInputUpdateUI then
        MVCService:RemoveInputDelegate(self)
    end
    CNSService:BeginFlow(FlowId.All, FlowId.LogicUninitialize, self.__classname)
    self:NotifyPrefabVisible(false)

    -- 销毁自动动效相关的处理
    for i = #self._autoPlayAnimDelay.Queue, 1, -1 do
        Pool:DestroyTable(self._autoPlayAnimDelay.Queue[i])
    end
    table.clear(self._autoPlayAnimDelay.Queue)
    self._autoPlayAnimDelay.Enable = false
    self._autoPlayAnimDelay.AccumulationTime = 0
    ScheduleService:RemoveUpdater(self, self._DelayPlayAnimationTick)

    self:vOnUninitialize()
    if self._prefab then
        UIUtility:StopUIAnimation(self._prefab, false, true)
    end
    CNSService:EndFlow(FlowId.All, FlowId.LogicUninitialize)

    if #self._child > 0 then
        for i = 1, #self._child do
            if self._child[i] ~= null then
                FrostLogE(self.__classname, "UIPrefab.Destroy : prefabClass ", self._child[i].__classname, " of ", self.__classname, " is living")
            end
        end
    end

    if #self._prefabClassAsyn > 0 then
        FrostLogE(self.__classname, "UIPrefab.Destroy : prefabClassAsyn is not empty" .. self.__classname)

        for i = 1, #self._prefabClassAsyn do
            self._prefabClassAsyn[i]:Destroy()
        end
    end

    for k,v in pairs(self._imageAsyn) do
        AssetService:CancelAsyn(self, k)
    end
    table.clear(self._imageAsyn)
    table.clear(self._image2AssetPath)

    CNSService:BeginFlow(FlowId.All, FlowId.ResourceUninitialize, self.__classname)
    self:vOnResourceUnLoaded()
    CNSService:EndFlow(FlowId.All, FlowId.ResourceUninitialize)

    EventService:RemoveUIEventByTable(self) -- 出现偶现事件错乱问题，应该有业务没有正常移除自己的事件，这里强行保护一下
    MVCService:RemovePrefab(self, self._parentTf)
    AssetService:Unload(self._uiAsset)

    self:_PlayAudio(UIAudioType.Destroy)

    self._audioCfg = false

    self._uiAsset = false
    self._prefab = false

    self._mediator = false
    self._parent = false
    self._parentTf = false

    self._customID = GlobalEnum.EInvalidDefine.ID
    self._styleIndex = 1
    self._tagName = false
    AssetService:ClearReference()
end

---异步加载ui成功。会调vOnCreateAsynPrefabClassCompleted。
---@param async UIPrefabClassAsyn 异步ui处理
---@param cls UIPrefab UI界面类定义
---@param parentTf UPanelWidget UE中的父控件（即创建出的控件所要挂载的地方）
---@param argument table 自定义参数，用于vInitialize时读取
---@param customID string|number 自定义索引，给父UI查找用（即父UI可以通过这个ID找到这个UI）
---@param style number ui样式索引，对应vGetPath里的资源路径
---@param asyncLoadID number 异步加载任务ID
---@param insertFirst boolean 是否从头部插入节点
function UIPrefab:CreateChildPrefabClassAsynCompleted(async, cls, parentTf, argument, customID, style, asyncLoadID, insertFirst)
    local prefabClass = self:CreateChildPrefabClass(cls, parentTf, argument, customID, style, asyncLoadID, insertFirst)
    async:ApplyUpdateUI(prefabClass)

    for i = 1, #self._prefabClassAsyn do
        if self._prefabClassAsyn[i] == async then
            table.remove(self._prefabClassAsyn, i)
            break
        end
    end
    self:vOnCreateAsynPrefabClassCompleted(prefabClass)
end

--- 获取默认图标配置路径
--- 使用说明：获取默认图标配置路径，有需要自定义默认图标的Prefab可override本接口进行实现
--- @return IconPath string 默认的图片资产路径
function UIPrefab:GetDefaultIconPath()
    local Row = DatabinService:GetSingleRow(DataTableConfig.GlobalIconData, "IconForAsync")
    return Row and Row.ImageSoftPath.AssetPathName or ""
end

--- 异步加载设置图标
--- 使用说明：1、使用异步的方式加载资源，并回调赋值 2、生命周期与Prefab本身保持一致，退出时会自动取消异步加载 3、支持重入
--- @param image UImage UE图标控件
--- @param assetName FSoftObjectPath|FSoftObjectPtr|string 资源路径，兼容 FSoftObjectPath、FSoftObjectPtr 和 DT_GlobalImage 中的行名
--- @param bIsSetDefault boolean 是否设置默认图片（透明图），true则未加载完成时对应图标控件显示透明
function UIPrefab:SetAsynIcon(image, assetName, bIsSetDefault)
    if not image or (not image:GetClass():IsChildOf(UE4.UImage) and not image:GetClass():IsChildOf(UE4.UCMProgressBar)) then
        FrostLogE(self.__classname, "UIPrefab.SetAsynIcon : image param error ")
        return
    end
    local assetPath = GlobalEnum.EInvalidDefine.SoftObjectPath
    if assetName then
        if assetName.__name == "FSoftObjectPath" then
            assetPath = assetName.AssetPathName
        elseif type(assetName) == "string" and not string.find(assetName, "/") then
            local tDTRow = DatabinService:GetSingleRow(DataTableConfig.GlobalIconData, assetName)
            assetPath = tDTRow and tDTRow.ImageSoftPath.AssetPathName or false
        else
            -- 兼容 FSoftObjectPtr
            assetPath = tostring(assetName)
        end
    end

    if not UE4Helper.IsValidPath(assetPath) then
        FrostLogE(self.__classname, "UIPrefab.SetAsynIcon : assetName param error ", assetPath)
        return
    end

    if self._image2AssetPath[image] then
        local oldAssetPath = self._image2AssetPath[image]
        if assetPath == oldAssetPath then
            return
        end
        local isCancel = true
        if table.length(self._image2AssetPath) > 1 then
            local _assetPathNum = 0
            for _, value in pairs(self._image2AssetPath) do
                if value == oldAssetPath then
                    _assetPathNum = _assetPathNum + 1
                end
            end

            if _assetPathNum > 1 then
                isCancel = false
            end
        end

        if isCancel then
            AssetService:CancelAsyn(self, oldAssetPath)
        end

        if self._imageAsyn[oldAssetPath] then
            for i = 1, #self._imageAsyn[oldAssetPath] do
                if self._imageAsyn[oldAssetPath][i] == image then
                    table.remove(self._imageAsyn[oldAssetPath], i)
                    break
                end
            end
            if #self._imageAsyn[oldAssetPath] == 0 then
                self._imageAsyn[oldAssetPath] = nil
            end
        end
    end

    self._image2AssetPath[image] = assetPath

    if bIsSetDefault then
        local PathName = self:GetDefaultIconPath()
        UIUtility:SetImageWithPath(image, PathName)
    end

    if not self._imageAsyn[assetPath] then
        self._imageAsyn[assetPath] = {image}
        AssetService:LoadAssetAsyn(LoadPriority.ImmediateShow, self, AssetType.Texture, assetPath, LifeType.Immediate)
    else
        self._imageAsyn[assetPath][#self._imageAsyn[assetPath] + 1] = image
    end
end

--- 在不同时机播放不同音效
--- @param inUIAudioType UIAudioType  音效类型
function UIPrefab:_PlayAudio(inUIAudioType)
    if self._prefab and self._audioCfg and self._audioCfg[inUIAudioType] and self._audioCfg[inUIAudioType] ~= "" and (not self._parentTf or self._prefab.bAudioEventForcePlay) then
        UIUtility:PlayUIAudioEventByName(self._audioCfg[inUIAudioType])
        --UE4.UKismetSystemLibrary.PrintString(GetCurrentWorld(), self._audioCfg[inUIAudioType])
    end
end

---异步加载资源完成。会调vOnLoadAssetCompleted。
---@param asyncLoadID number 异步加载任务ID，用来获取资源
---@param assetType number AssetType，在AssetDeclare.lua里定义
---@param assetName string 资源路径
---@param obj UObject 加载好的资源object
function UIPrefab:OnLoadAssetCompleted(asyncLoadID, assetType, assetName, obj)
    if assetType == AssetType.Texture then
        if self._imageAsyn[assetName] then
            for i = 1, #self._imageAsyn[assetName] do
                if self._imageAsyn[assetName][i]:GetClass():IsChildOf(UE4.UCMImage) then
                    self._imageAsyn[assetName][i]:SetResourceObject(obj)
                else
                    self._imageAsyn[assetName][i]:SetBrushResourceObject(obj)
                end
            end
            self._imageAsyn[assetName] = nil
            return
        end
    end
    self:vOnLoadAssetCompleted(asyncLoadID, assetType, assetName, obj)
end
function UIPrefab:GetPrefabIsVisible()
    return IsValidObject(self._prefab) and self._prefab.IsVisible and self._prefab:IsVisible() or false
end
function UIPrefab:GetPrefabClassByTag(tagName, param)
    if not param then
        FrostLogE(self.__classname, "GetPrefabClassByTag param is error, Prohibit modifying param in the process!", debug.traceback())
    end
    local outPrefabInstance = self:vGetPrefabClassByTag(tagName, param)
    if outPrefabInstance then
        if not outPrefabInstance:GetPrefabIsVisible() then
            return false
        end
        return outPrefabInstance
    end
    return self:_DoEachChildPrefab(function(inPrefabInstance)
        if inPrefabInstance and inPrefabInstance ~= null then
            -- FrostLogE("NewbieGuide UIPrefab ", self.__classname, inPrefabInstance.__classname, inPrefabInstance:GetCustomID(), tagName, tostring(param.CustomID))
            if not inPrefabInstance:GetPrefabIsVisible() then
                -- 简单拦截，因为也有可能在更上层进行的隐藏，
                if inPrefabInstance.__classname == ClassLib.CMGridClass.__classname then
                    if IsValidObject(inPrefabInstance._uiGridObj) and inPrefabInstance._uiGridObj.IsVisible and inPrefabInstance._uiGridObj:IsVisible() then
                    else
                        return nil
                    end
                elseif inPrefabInstance.__classname == ClassLib.CMListViewClass.__classname then
                    if IsValidObject(inPrefabInstance._listViewObj) and inPrefabInstance._listViewObj.IsVisible and inPrefabInstance._listViewObj:IsVisible() then
                    else
                        return nil
                    end
                elseif inPrefabInstance.__classname == ClassLib.CMCurveListViewClass.__classname then
                    if IsValidObject(inPrefabInstance._mListViewObj) and inPrefabInstance._mListViewObj.IsVisible and inPrefabInstance._mListViewObj:IsVisible() then
                    else
                        return nil
                    end
                else
                    return nil
                end
            end
            if (inPrefabInstance.__classname == tagName and string.isEmpty(param.CustomID))
                    or (inPrefabInstance.__classname == tagName and not string.isEmpty(param.CustomID) and tostring(inPrefabInstance:GetCustomID()) == param.CustomID)
                    or inPrefabInstance:GetTagName() == tagName then
                if not inPrefabInstance:GetPrefabIsVisible() then
                    return false
                end
                return inPrefabInstance
            end
            return inPrefabInstance:GetPrefabClassByTag(tagName, param)
        end
    end)
end
function UIPrefab:GetPrefabNodeByTag(nodeTagName, param)
    if self._prefab and self._prefab[nodeTagName] then
        local prefabWidget = self._prefab
        local prefabNode = self._prefab[nodeTagName]
        local btnList = {}
        local replaceNodeParam = param and param.__internalData and param.__internalData.ReplaceNodeParam
        if replaceNodeParam and replaceNodeParam.AutoWidgetNames:Length() > 0 then
            for i = 1, replaceNodeParam.AutoWidgetNames:Length() do
                local autoWidgetName = replaceNodeParam.AutoWidgetNames:Get(i)
                if not string.isEmpty(autoWidgetName) then
                    prefabWidget = prefabWidget:GetAutoLoadWidget(autoWidgetName, true)
                end
            end
        end
        if prefabWidget and not string.isEmpty(replaceNodeParam.NodeName) then
            prefabNode = prefabWidget[replaceNodeParam.NodeName] or false
        end
        if prefabNode and prefabWidget then
            if not string.isEmpty(replaceNodeParam.ButtonName) then
                table.addUnique(btnList, prefabWidget[replaceNodeParam.ButtonName])
            else
                table.addUnique(btnList, prefabNode)
            end
            return prefabNode:GetParent(), btnList
        end
        return nil, btnList
    end
    local node, btnList = self:vGetPrefabNodeByTag(nodeTagName, param)
    return node or false, btnList or false
end

function UIPrefab:GetOutsideWidgetByTag(tagName, param)
    local reWidgets = {}
    if UIUtility:HasOnClickOutside(self._prefab) then
        table.insert(reWidgets, self._prefab)
    end

    local widgetOutsides = self:vGetOutsideWidgetByTag(tagName, param)
    if not table.isEmpty(widgetOutsides) then
        for _, widget in ipairs(widgetOutsides) do
            if UIUtility:HasOnClickOutside(widget) then
                table.insert(reWidgets, widget)
            end
        end
    end

    local parentWidgetOutSides = self._parent and self._parent:GetOutsideWidgetByTag(tagName, param) or false
    if not table.isEmpty(parentWidgetOutSides) then
        for _, parentWidget in ipairs(parentWidgetOutSides) do
            if UIUtility:HasOnClickOutside(parentWidget) then
                table.insert(reWidgets, parentWidget)
            end
        end
    end
    return reWidgets
end

--- 播放预设动画
--- @param autoPlayMode ECMAnimationAutoPlayMode
function UIPrefab:_PlayAutoAnim(autoPlayMode)
    if self._prefab and self._prefab.AnimAutoPlaySettings and self._prefab.AnimAutoPlaySettings:Length() > 0 then
        for i = 1, self._prefab.AnimAutoPlaySettings:Length() do
            local setting = self._prefab.AnimAutoPlaySettings:Get(i)
            if setting.AutoPlayMode == autoPlayMode and self._prefab[setting.AnimName] then
                local animParam = Pool:CreateTable()
                animParam.startAtTime = setting.StartAtTime
                animParam.endAtTime = setting.EndAtTime
                animParam.times = setting.NumLoopsToPlay
                animParam.playMode = setting.AnimPlayMode
                animParam.playbackSpeed = setting.PlaybackSpeed
                animParam.bRestoreState = setting.bRestoreState

                if setting.DelayPlayTime > 0 then
                    self._autoPlayAnimDelay.Queue[#self._autoPlayAnimDelay.Queue + 1] = {
                        setting.DelayPlayTime,
                        setting.AnimName,
                        animParam
                    }
                    if not self._autoPlayAnimDelay.Enable then
                        self._autoPlayAnimDelay.AccumulationTime = 0
                        self._autoPlayAnimDelay.Enable = true
                        ScheduleService:AddUpdater(self, self._DelayPlayAnimationTick)
                    end
                else
                    UIUtility:PlayUIAnimation(self._prefab, setting.AnimName, animParam)
                    Pool:DestroyTable(animParam)
                end
            end
        end
    end
end

--- 延迟播放动画Tick
function UIPrefab:_DelayPlayAnimationTick(deltaTime)
    self._autoPlayAnimDelay.AccumulationTime = self._autoPlayAnimDelay.AccumulationTime + deltaTime
    for i = #self._autoPlayAnimDelay.Queue, 1, -1 do
        local data = self._autoPlayAnimDelay.Queue[i]
        if self._autoPlayAnimDelay.AccumulationTime >= data[1] then
            UIUtility:PlayUIAnimation(self._prefab, data[2], data[3])
            table.remove(self._autoPlayAnimDelay.Queue, i)
            Pool:DestroyTable(data)
        end
    end
    if #self._autoPlayAnimDelay.Queue == 0 then
        ScheduleService:RemoveUpdater(self, self._DelayPlayAnimationTick)
        self._autoPlayAnimDelay.Enable = false
    end
end

--- 重载
---获取资源路径
--- @return string[] 资源路径
function UIPrefab:vGetPath()
end

--- 重载
--- 资源装载后回调，必定在vOnInitialize之前。
function UIPrefab:vOnResourceLoaded()
end

--- 重载
--- 资源卸载前回调
function UIPrefab:vOnResourceUnLoaded()
end

--- 重载
--- 逻辑初始化，必定在vOnResourceLoaded之后。
--- @param argument table 自定义参数
function UIPrefab:vOnInitialize(argument)
end

--- 重载
--- 逻辑反初始化，销毁时调
function UIPrefab:vOnUninitialize()
end

--- 异步创建子UIPrefabClass（CreateAsynChildPrefabClass）完成后回调
---@param prefabClass UIPrefab 异步创建的子prefabClass
---@return void
function UIPrefab:vOnCreateAsynPrefabClassCompleted(prefabClass)
end

--- 异步加载资源完成后回调
---@param asyncLoadID number 异步加载任务ID，用来获取资源
---@param assetType number AssetType，在AssetDeclare.lua里定义
---@param assetName string 资源路径
---@param obj UObject 加载好的资源object
function UIPrefab:vOnLoadAssetCompleted(asyncLoadID, assetType, assetName, obj)
end

--- 重载
--- 更新UI回调，以 module -> mediator -> parent UI prefab -> child UI prefab 顺序向下传递（其中任何一个都可以自主发起下发传递，不必然从module开始），与SendUpdateUI配合使用
--- @param id string|number 更新标识，推荐使用字符串和数字
--- @param argument table 自定义更新参数
--- @return boolean 是否阻断向下传递刷新，true为阻断
function UIPrefab:vOnUpdateUI(id, argument)
end

--- 重载
--- 操作回调，以 child UI prefab -> parent UI prefab -> mediator -> module顺序向上传递（其中任何一个都可以自主发起下发传递，不必然从child UI prefab开始）,与SendAction配合使用
--- @param id string|number 操作标识，推荐使用字符串和数字
--- @param argument table 自定义操作参数
--- @return boolean 是否阻断向上传递操作，true为阻断
function UIPrefab:vOnAction(id, argument)
end

--- 重载
--- 操作input响应。由mediator下发传递。由于mediator是在SwitchUIStateIn的时候绑定input回调，如果mediator没有state或者不处于state in的状态，则不会下发传递input。如果依然需要传递input，目前的做法是由需要的UIPrefabClass自行通过MVCService:AddInputDelegate(self, self.OnInputHandler)绑定input回调。
--- @param inputName string 实际操作的inputName
--- @param inputActionValue FVector input参数
--- @return boolean 是否阻断向下传递操作，true为阻断
--function UIPrefab:vOnInputUpdateUI(inputName, inputActionValue)
--end

-------------------------------------------------------------------------------------------
--- 重载，响应可见性发生改变
--- @param inNewVisibility ESlateVisibility 更改后的可见性类型
--- @param inOldVisibility ESlateVisibility 更改前的可见性类型
-------------------------------------------------------------------------------------------
function UIPrefab:vOnVisibilityChanged(inNewVisibility, inOldVisibility)
end

--- 重载
--- 操作回调
--- @return boolean 是否已经处理返回键, 如果返回false, 返回键默认走到处理退出游戏的逻辑
function UIPrefab:vOnAndroidBackButtonClick()
end

--- 重载
--- 获取UIPrefabClass
--- 说明1：新手引导在使用该函数，比如在list列表中得到指定item项的UIPrefabClass
--- 说明2：搜索时会匹配TagName和ClassName
--- @param tagName string 类的自定义标签
--- @param param table 自定义参数
--- @return UIPrefab 获取到的UIPrefabClass
function UIPrefab:vGetPrefabClassByTag(tagName, param)
    return false
end
--- 重载
--- 获取节点
--- 说明1：新手引导在使用该函数，比如主菜单中，需要MainHUD返回相应的按钮节点和其父节点
--- 说明2：[TODO]: 返回值的按钮列表是临时需求，等enterli加上Button的穿透点击后会去掉
--- @param nodeTagName string 节点的名字或者标签
--- @param param table 自定义参数
--- @return UPanelWidget|boolean, UWidget[]|void 找到则返回对应的父节点parent和object列表，没找到返回false
function UIPrefab:vGetPrefabNodeByTag(nodeTagName, param)
    return false, false
end
--- 重载
--- 获取widget
--- 说明1：新手引导在使用该函数，需要返回OnClickOutside的节点
--- @param tagName string 标签类型
--- @param param table 自定义参数
--- @return UWdiget[]
function UIPrefab:vGetOutsideWidgetByTag(tagName, param)
    return false
end
-------------------------------------------------------------------------------------------
--- 获得指定变量名称的UWidget
--- @param inWidgetName(string) UWidget变量的名称
--- @return(UWidget) 如果找不到，返回nil
-------------------------------------------------------------------------------------------
function UIPrefab:vGetPrefabWidget(inWidgetName)
    return self._prefab and self._prefab[inWidgetName] or nil
end

--- 获取UI样式索引（style）
--- @return number
function UIPrefab:GetStyleIndex()
    return self._styleIndex
end

--- 获取自定义索引（customID）
--- @return number
function UIPrefab:GetCustomID()
    return self._customID
end

-------------------------------------------------------------------------------------------
--- 设置自定义索引（customID）
--- @param inCustomID number 自定义索引
-------------------------------------------------------------------------------------------
function UIPrefab:SetCustomID(inCustomID)
    self._customID = inCustomID or GlobalEnum.EInvalidDefine.ID
end

--- 获取自定义的标签名
--- 新手引导使用TagName来标识UIPrefab，方便用vGetPrefabClassByTag、vGetPrefabNodeByTag查找
--- @return string|boolean
function UIPrefab:GetTagName()
    return self._tagName
end

-------------------------------------------------------------------------------------------
--- 设置自定义的标签名
--- @param inTagName string 自定义的标签名（tagName）
-------------------------------------------------------------------------------------------
function UIPrefab:SetTagName(inTagName)
    self._tagName = inTagName or false
end

-------------------------------------------------------------------------------------------
--- 判断指定子界面是否已创建，或者正在创建
--- @param inCustomID string|number 自定义索引
--- @return boolean, UIPrefab 如果已创建或正在创建，返回true。如果已创建，也会返回创建的实例
-------------------------------------------------------------------------------------------
function UIPrefab:HasChildPrefab(inCustomID)
    local tChildPrefab = self:GetPrefabClassByCustomID(inCustomID)
    if not tChildPrefab then
        return self:GetCreateAsynChildPrefabClassStateByCustomID(inCustomID)
    end
    return true, tChildPrefab
end

-------------------------------------------------------------------------------------------
--- 获得当前正在进行异步创建的对象数量
--- @return number 正在进行异步创建的对象数量
-------------------------------------------------------------------------------------------
function UIPrefab:GetAsynChildPrefabCount()
    return #self._prefabClassAsyn
end

--- 同步创建子UIPrefabClass（创建UI请使用CreateAsynChildPrefabClass，不要使用这个）
--- @param cls UIPrefab 要创建的子UIPrefabClass类定义
--- @param parentTf UPanelWidget UE中的父控件（即创建出的控件所要挂载的地方）
--- @param argument table 自定义参数
--- @param customID string|number 自定义索引
--- @param style number 资源样式索引
--- @param asyncLoadID number 异步加载任务ID，用来获取资源
--- @return UIPrefab 创建的子UIPrefabClass实例（=nil创建失败）
--- @param insertFirst boolean 是否从头部插入节点
function UIPrefab:CreateChildPrefabClass(cls, parentTf, argument, customID, style, asyncLoadID, insertFirst)
    if not cls then
        FrostLogE(self.__classname, "CreateChildPrefabClass failed, Can not found UIPrefab nil")
        return nil
    end

    if not parentTf then
        FrostLogE(self.__classname, "CreateChildPrefabClass failed, Can not found parent nil", cls.__classname)
        return nil
    end

    local obj = cls:New()
    self._child[#self._child+1] = obj
    obj:Create(self._mediator, self, parentTf, argument, customID, style, false, asyncLoadID, false, insertFirst)

    return obj
end

--- 异步创建子UIPrefabClass（创建UI请使用这个）
--- @param cls UIPrefab 创建的子UIPrefabClass类定义
--- @param parentTf UPanelWidget UE中的父控件（即创建出的控件所要挂载的地方）
--- @param argument table 自定义参数
--- @param loadPriority number 装载类型（默认为LoadPriority.SpareShow）
--- @param life number 生命周期（默认为：LifeType.Immediate）
--- @param customID string|number 自定义索引
--- @param style number 资源样式索引
--- @param insertFirst boolean 是否从头部插入节点
--- @return void
function UIPrefab:CreateAsynChildPrefabClass(cls, parentTf, argument, loadPriority, life, customID, style, insertFirst)
    if not cls or not parentTf then
        FrostLogE(self.__classname, "UIPrefab.CreateAsynChildPrefabClass : invalid parameter")
        return
    end
    if customID then
        if self:GetPrefabClassByCustomID(customID) then
            FrostLogE(self.__classname, "UIPrefab.CreateAsynChildPrefabClass : already exist prefab", customID)
            return
        end
        if self:GetCreateAsynChildPrefabClassStateByCustomID(customID) then
            FrostLogE(self.__classname, "UIPrefab.CreateAsynChildPrefabClass : prefab is createing", customID)
            return
        end
    end
    local obj = ClassLib.UIPrefabClassAsyn:New()
    self._prefabClassAsyn[#self._prefabClassAsyn+1] = obj
    obj:Create(nil, self, parentTf, cls, argument, loadPriority or LoadPriority.SpareShow, life or LifeType.Immediate, customID, style, false, insertFirst)
end

--- 销毁子UIPrefabClass
--- @param obj UIPrefab 子UIPrefabClass
function UIPrefab:DestroyChildPrefabClass(obj)
    if not obj then
        return
    end

    for i = 1, #self._child do
        if self._child[i] == obj then
            self._child[i] = null
            break
        end
    end

    obj:Destroy()
end

--- 销毁子UIPrefabClass
--- @param name string 类名
--- @param onlyAsyn boolean 只销毁符合条件的异步子UIPrefabClass，默认销毁所有符合条件的
--- @return void
function UIPrefab:DestroyChildPrefabClassByName(name, onlyAsyn)
    if not name then
        FrostLogE(self.__classname, "UIPrefab.DestroyChildPrefabClassByName : invalid parameter")
        return
    end

    if not onlyAsyn then
        for i = #self._child, 1, -1 do
            local pc = self._child[i]
            if pc ~= null and pc.__classname == name then
                self._child[i] = null
                pc:Destroy()
            end
        end
    end

    for i = #self._prefabClassAsyn, 1, -1 do
        local pca = self._prefabClassAsyn[i]
        if pca:GetPrefabClassName() == name then
            table.remove(self._prefabClassAsyn, i)
            pca:Destroy()
        end
    end
end

--- 销毁子UIPrefabClass
--- @param customID string|number 自定义ID
--- @param onlyAsyn boolean 只销毁符合条件的异步子UIPrefabClass，默认销毁所有符合条件的
--- @return void
function UIPrefab:DestroyChildPrefabClassByCustomID(customID, onlyAsyn)
    if not customID then
        FrostLogE(self.__classname, "UIPrefab.DestroyChildPrefabClassByCustomID : invalid parameter")
        return
    end

    if not onlyAsyn then
        for i = #self._child, 1, -1 do
            local pc = self._child[i]
            if pc ~= null and pc._customID == customID then
                self._child[i] = null
                pc:Destroy()
            end
        end
    end

    for i = #self._prefabClassAsyn, 1, -1 do
        local pca = self._prefabClassAsyn[i]
        if pca._customID == customID then
            table.remove(self._prefabClassAsyn, i)
            pca:Destroy()
        end
    end
end

--- 销毁所有子UIPrefabClass
--- @param onlyAsyn boolean 只销毁异步子UIPrefabClass，默认销毁所有
function UIPrefab:DestroyAllChildPrefabClass(onlyAsyn)
    if not onlyAsyn then
        for i = 1, #self._child do
            local obj = self._child[i]
            if obj ~= null then
                self._child[i] = null
                obj:Destroy()
            end
        end
    end

    local asyn = table.remove(self._prefabClassAsyn)
    while asyn do
        asyn:Destroy()
        asyn = table.remove(self._prefabClassAsyn)
    end
end

--- 获取子UIPrefabClass
--- @param name string 类名
--- @return UIPrefab 获取到的子UIPrefabClass
function UIPrefab:GetPrefabClassByName(name)
    if not name then
        FrostLogE(self.__classname, "UIPrefab.GetPrefabClassByName : invalid parameter")
        return nil
    end

    for i = 1, #self._child do
        local pc = self._child[i]
        if pc ~= null and pc.__classname == name then
            return pc
        end
    end

    return nil
end

--- 获取子UIPrefabClass
--- @param customID string 自定义ID
--- @return UIPrefab 获取到的UIPrefabClass
function UIPrefab:GetPrefabClassByCustomID(customID)
    if not customID then
        FrostLogE(self.__classname, "UIPrefab.GetPrefabClassByCustomID : invalid parameter")
        return nil
    end

    for i = 1, #self._child do
        local pc = self._child[i]
        if pc ~= null and pc._customID == customID then
            return pc
        end
    end

    return nil
end

--- 获取子UIPrefabClass异步状态状态
--- @param name string UIPrefabClass类名
--- @return boolean true：异步创建中  false: 不在异步创建中
function UIPrefab:GetCreateAsynChildPrefabClassStateByName(name)
    if not name then
        FrostLogE(self.__classname, "UIPrefab.GetCreateAsynChildPrefabClassStateByName : invalid parameter")
        return
    end

    for i = #self._prefabClassAsyn, 1, -1 do
        local pca = self._prefabClassAsyn[i]
        if pca:GetPrefabClassName() == name then
            return true
        end
    end
    return false
end

--- 获取子UIPrefabClass异步状态状态
--- @param customID string|number 自定义ID
--- @return boolean true：异步创建中  false: 不在异步创建中
function UIPrefab:GetCreateAsynChildPrefabClassStateByCustomID(customID)
    if not customID then
        FrostLogE(self.__classname, "UIPrefab.GetCreateAsynChildPrefabClassStateByCustomID : invalid parameter")
        return
    end
    for i = #self._prefabClassAsyn, 1, -1 do
        local pca = self._prefabClassAsyn[i]
        if pca._customID == customID then
            return true
        end
    end
    return false
end

--- 获取自动加载的Widget
--- @param name string Widget标识
--- @param inIgnoreError(boolean) 为 true 时忽略报错信息，用于外部尝试获取的情况
--- @return UUserWidget 获取到的Widget, 失败返回nil
function UIPrefab:GetAutoLoadWidget(name, inIgnoreError)
    local prefab = self._prefab
    if not prefab then
        return nil
    end
    local widget = prefab:GetAutoLoadWidget(name)
    if not widget and not inIgnoreError then
        NGRFormatLogE(self.__classname, "GetAutoLoadWidget %s is not auto load!", name)
    end
    return widget
end

--- 向下发送更新UI（只能在实例内部调用该函数，如：self:SendUpdateUI("zvlc")，与vOnUpdateUI配合使用。自己不会调vOnUpdateUI。
--- @param id string 刷新ID标识
--- @param argument table 参数
--- @param dummy boolean 不要管这个参数
function UIPrefab:SendUpdateUI(id, argument, dummy)
    if not id then
        return
    end

    if dummy then
        CNSService:BeginFlow(FlowId.All, FlowId.SendUpdateUI, string.format("%s %s", self.__classname, id))
        if self:vOnUpdateUI(id, argument) then
            CNSService:EndFlow(FlowId.All, FlowId.SendUpdateUI)
            return
        end
        CNSService:EndFlow(FlowId.All, FlowId.SendUpdateUI)
    end

    for i = 1, #self._prefabClassAsyn do
        local asyn = self._prefabClassAsyn[i]
        asyn:SendUpdateUI(id, argument)
    end
    -- 晚于_prefabClassAsyn执行，避免为新创建的prefab添加事件
    for i = #self._child, 1, -1 do
        local obj = self._child[i]
        if obj ~= null then
            obj:SendUpdateUI(id, argument, true)
        end
    end
end

--- 向上发送操作（只能在实例内部调用该函数，如：self:SendAction("zvlc")，与vOnAction配合使用。自己不会调vOnAction。
--- @param id string 操作ID标识
--- @param argument table 参数
--- @param dummy boolean 不要管这个参数
function UIPrefab:SendAction(id, argument, dummy)
    if not id then
        return
    end

    if dummy then
        CNSService:BeginFlow(FlowId.All, FlowId.SendAction, string.format("%s %s", self.__classname, id))
        if self:vOnAction(id, argument) then
            CNSService:EndFlow(FlowId.All, FlowId.SendAction)
            return
        end
        CNSService:EndFlow(FlowId.All, FlowId.SendAction)
    end

    local target = self._parent or self._mediator
    if target then
        target:SendAction(id, argument, true)
    end
end

--- 发送输入（只能在实例内部调用该函数，如：self:SendInputUpdateUI("zvlc")。自己会先调vOnInputUpdateUI，不返回true的话会调子UI的vOnInputUpdateUI。
--- @param inputName string 键盘等外设输入
--- @param inputActionValue FVector input参数
function UIPrefab:OnInputHandler(inputName, inputActionValue)
    return self:vOnInputUpdateUI(inputName, inputActionValue)
end

-- 查找UIPrefab对应的PrefabClass
-- @param uiPrefab UIPrefab对象
-- @return 获取到的UIPrefabClass
function UIPrefab:GetPrefabClassByUIPrefab(uiPrefab)
    if self._prefab == uiPrefab then
        return self, nil
    end

    if IsValidObject(uiPrefab:GetParent()) then
        if self:GetAutoLoadWidget(uiPrefab:GetParent():GetName()) == uiPrefab then
            return self, uiPrefab:GetParent():GetName()
        end
    end

    for i, v in ipairs(self._child) do
        if v and v.GetPrefabClassByUIPrefab then
            local prefabClass, autoLoadNode = v:GetPrefabClassByUIPrefab(uiPrefab)
            if prefabClass then
                return prefabClass, autoLoadNode
            end
        end
    end

    return nil, nil
end

-- 获取所属的父Prefab
function UIPrefab:GetParent()
    return self._parent
end

---获取数据中心
---@return DataCentreClass
function UIPrefab:GetDataCentre()
    if self._mediator then
        return self._mediator:GetDataCentre()
    end
    return DataCentre
end


--- 封装，保持和ModuleBase接口一致性。
--- 获取本地玩家控制的Character。
---@return ACharacter 获取不到返回nil
function UIPrefab:GetOwnerActor()
    return UE4Helper.GetMainPlayerCharacter(true)
end

--- 封装，保持和ModuleBase接口一致性
--- 获取本地玩家的Controller
---@return APlayerController 获取不到返回nil
function UIPrefab:GetOwnerController()
    return UE4Helper.GetMainPlayerController(true)
end

-- 获取prefab所属的OwnerEntity
function UIPrefab:GetOwner()
    return SceneEntityService:GetPlayerEntity()
end

--- 调每个子UI的对应函数，直到返回一个不为nil的值
---@param inFunc fun(...) 要调的函数
---@return any 子UI的返回值
function UIPrefab:_DoEachChildPrefab(inFunc, ...)
    local tReturnValue = nil
    for tIndex = 1, #self._child do
        tReturnValue = inFunc(self._child[tIndex], ...)
        if nil ~= tReturnValue then
            return tReturnValue
        end
    end
end

function UIPrefab:AddPreDestroyDelegate(tbl, func, ...)
    if self._preDestroyDelegates then
        for i = 1, #self._preDestroyDelegates do
            local preDestroyDelegate = self._preDestroyDelegates[i]
            if preDestroyDelegate.tbl == tbl and preDestroyDelegate.func == func then
                return
            end
        end
        table.insert(self._preDestroyDelegates, {
            tbl = tbl,
            func = func,
            param = table.pack(...)
        })
    end
end
function UIPrefab:RemovePreDestroyDelegate(tbl, func)
    if self._preDestroyDelegates then
        for i = 1, #self._preDestroyDelegates do
            local preDestroyDelegate = self._preDestroyDelegates[i]
            if preDestroyDelegate.tbl == tbl and preDestroyDelegate.func == func then
                table.remove(self._preDestroyDelegates, i)
                return
            end
        end
    end
end

function UIPrefab:NotifyPreDestroyDelegate()
    if self._preDestroyDelegates and #self._preDestroyDelegates > 0 then
        for i = #self._preDestroyDelegates, 1, -1 do --存在调用中移除
            local preDestroyDelegate = self._preDestroyDelegates[i]
            if preDestroyDelegate.tbl and preDestroyDelegate.func then
                preDestroyDelegate.func(preDestroyDelegate.tbl, table.unpack(preDestroyDelegate.param))
            end
        end
    end
end

---@return CMWidget
function UIPrefab:GetParentTf()
    return self._parentTf
end

return UIPrefab