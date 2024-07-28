---@class CS.UnityEngine.UI.InputField : CS.UnityEngine.UI.Selectable
---@field public shouldHideMobileInput boolean
---@field public text string
---@field public isFocused boolean
---@field public caretBlinkRate number
---@field public caretWidth number
---@field public textComponent CS.UnityEngine.UI.Text
---@field public placeholder CS.UnityEngine.UI.Graphic
---@field public caretColor CS.UnityEngine.Color
---@field public customCaretColor boolean
---@field public selectionColor CS.UnityEngine.Color
---@field public onEndEdit CS.UnityEngine.UI.InputField.SubmitEvent
---@field public onValueChanged CS.UnityEngine.UI.InputField.OnChangeEvent
---@field public onValidateInput (fun(text:string, charIndex:number, addedChar:number):number)
---@field public characterLimit number
---@field public contentType number
---@field public lineType number
---@field public inputType number
---@field public touchScreenKeyboard CS.UnityEngine.TouchScreenKeyboard
---@field public keyboardType number
---@field public characterValidation number
---@field public readOnly boolean
---@field public multiLine boolean
---@field public asteriskChar number
---@field public wasCanceled boolean
---@field public caretPosition number
---@field public selectionAnchorPosition number
---@field public selectionFocusPosition number
---@field public minWidth number
---@field public preferredWidth number
---@field public flexibleWidth number
---@field public minHeight number
---@field public preferredHeight number
---@field public flexibleHeight number
---@field public layoutPriority number
CS.UnityEngine.UI.InputField = { }
---@param shift boolean
function CS.UnityEngine.UI.InputField:MoveTextEnd(shift) end
---@param shift boolean
function CS.UnityEngine.UI.InputField:MoveTextStart(shift) end
---@param eventData CS.UnityEngine.EventSystems.PointerEventData
function CS.UnityEngine.UI.InputField:OnBeginDrag(eventData) end
---@param eventData CS.UnityEngine.EventSystems.PointerEventData
function CS.UnityEngine.UI.InputField:OnDrag(eventData) end
---@param eventData CS.UnityEngine.EventSystems.PointerEventData
function CS.UnityEngine.UI.InputField:OnEndDrag(eventData) end
---@param eventData CS.UnityEngine.EventSystems.PointerEventData
function CS.UnityEngine.UI.InputField:OnPointerDown(eventData) end
---@param e CS.UnityEngine.Event
function CS.UnityEngine.UI.InputField:ProcessEvent(e) end
---@param eventData CS.UnityEngine.EventSystems.BaseEventData
function CS.UnityEngine.UI.InputField:OnUpdateSelected(eventData) end
function CS.UnityEngine.UI.InputField:ForceLabelUpdate() end
---@param update number
function CS.UnityEngine.UI.InputField:Rebuild(update) end
function CS.UnityEngine.UI.InputField:LayoutComplete() end
function CS.UnityEngine.UI.InputField:GraphicUpdateComplete() end
function CS.UnityEngine.UI.InputField:ActivateInputField() end
---@param eventData CS.UnityEngine.EventSystems.BaseEventData
function CS.UnityEngine.UI.InputField:OnSelect(eventData) end
---@param eventData CS.UnityEngine.EventSystems.PointerEventData
function CS.UnityEngine.UI.InputField:OnPointerClick(eventData) end
function CS.UnityEngine.UI.InputField:DeactivateInputField() end
---@param eventData CS.UnityEngine.EventSystems.BaseEventData
function CS.UnityEngine.UI.InputField:OnDeselect(eventData) end
---@param eventData CS.UnityEngine.EventSystems.BaseEventData
function CS.UnityEngine.UI.InputField:OnSubmit(eventData) end
function CS.UnityEngine.UI.InputField:CalculateLayoutInputHorizontal() end
function CS.UnityEngine.UI.InputField:CalculateLayoutInputVertical() end
return CS.UnityEngine.UI.InputField
