---@class UnityEngine.UI.InputField : UnityEngine.UI.Selectable
---@field public shouldHideMobileInput boolean
---@field public text string
---@field public isFocused boolean
---@field public caretBlinkRate number
---@field public caretWidth number
---@field public textComponent UnityEngine.UI.Text
---@field public placeholder UnityEngine.UI.Graphic
---@field public caretColor UnityEngine.Color
---@field public customCaretColor boolean
---@field public selectionColor UnityEngine.Color
---@field public onEndEdit UnityEngine.UI.InputField.SubmitEvent
---@field public onValueChanged UnityEngine.UI.InputField.OnChangeEvent
---@field public onValidateInput (fun(text:string, charIndex:number, addedChar:number):number)
---@field public characterLimit number
---@field public contentType number
---@field public lineType number
---@field public inputType number
---@field public touchScreenKeyboard UnityEngine.TouchScreenKeyboard
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

---@type UnityEngine.UI.InputField
UnityEngine.UI.InputField = { }
---@param shift boolean
function UnityEngine.UI.InputField:MoveTextEnd(shift) end
---@param shift boolean
function UnityEngine.UI.InputField:MoveTextStart(shift) end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.UI.InputField:OnBeginDrag(eventData) end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.UI.InputField:OnDrag(eventData) end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.UI.InputField:OnEndDrag(eventData) end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.UI.InputField:OnPointerDown(eventData) end
---@param e UnityEngine.Event
function UnityEngine.UI.InputField:ProcessEvent(e) end
---@param eventData UnityEngine.EventSystems.BaseEventData
function UnityEngine.UI.InputField:OnUpdateSelected(eventData) end
function UnityEngine.UI.InputField:ForceLabelUpdate() end
---@param update number
function UnityEngine.UI.InputField:Rebuild(update) end
function UnityEngine.UI.InputField:LayoutComplete() end
function UnityEngine.UI.InputField:GraphicUpdateComplete() end
function UnityEngine.UI.InputField:ActivateInputField() end
---@param eventData UnityEngine.EventSystems.BaseEventData
function UnityEngine.UI.InputField:OnSelect(eventData) end
---@param eventData UnityEngine.EventSystems.PointerEventData
function UnityEngine.UI.InputField:OnPointerClick(eventData) end
function UnityEngine.UI.InputField:DeactivateInputField() end
---@param eventData UnityEngine.EventSystems.BaseEventData
function UnityEngine.UI.InputField:OnDeselect(eventData) end
---@param eventData UnityEngine.EventSystems.BaseEventData
function UnityEngine.UI.InputField:OnSubmit(eventData) end
function UnityEngine.UI.InputField:CalculateLayoutInputHorizontal() end
function UnityEngine.UI.InputField:CalculateLayoutInputVertical() end
return UnityEngine.UI.InputField
