UIUtils = {}
function UIUtils.AddEventListener(event, func, tobj)
    if not tobj.evtListenerList then
        tobj.evtListenerList = {}
    end

    local handle = event:CreateListener(func, tobj)
    event:AddListener(handle)
    table.insert(tobj.evtListenerList, {evt = event, hanlde = handle})
end

function UIUtils.RemoveAllEventListener(tobj)
    if not tobj.evtListenerList then
        return
    end

    for __idx = 1, #tobj.evtListenerList do
        tobj.evtListenerList[__idx].evt:RemoveListener(tobj.evtListenerList[__idx].hanlde)
    end
    table.clear(tobj.evtListenerList)
    tobj.evtListenerList = nil
end

function UIUtils.OnOpenDialog(message,onClick)
    -- 错误机制
    UICommonViewController:OnOpenDialog(
        DialogConfig.ButtonType.YesNO,
        "",
        message,
        "确定",
        "取消",
        onClick
    )
end
