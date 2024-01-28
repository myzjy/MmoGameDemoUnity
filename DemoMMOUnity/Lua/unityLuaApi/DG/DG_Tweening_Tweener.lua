---@class DG.Tweening.Tweener : DG.Tweening.Tween

---@type DG.Tweening.Tweener
DG.Tweening.Tweener = { }
---@return DG.Tweening.Tweener
---@param newStartValue System.Object
---@param newDuration number
function DG.Tweening.Tweener:ChangeStartValue(newStartValue, newDuration) end
---@overload fun(newEndValue:System.Object, snapStartValue:boolean): DG.Tweening.Tweener
---@return DG.Tweening.Tweener
---@param newEndValue System.Object
---@param newDuration number
---@param snapStartValue boolean
function DG.Tweening.Tweener:ChangeEndValue(newEndValue, newDuration, snapStartValue) end
---@return DG.Tweening.Tweener
---@param newStartValue System.Object
---@param newEndValue System.Object
---@param newDuration number
function DG.Tweening.Tweener:ChangeValues(newStartValue, newEndValue, newDuration) end
return DG.Tweening.Tweener
