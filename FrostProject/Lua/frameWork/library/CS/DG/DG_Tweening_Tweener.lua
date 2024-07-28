---@class DG.Tweening.Tweener : DG.Tweening.Tween
DG.Tweening.Tweener = { }
---@return DG.Tweening.Tweener
---@param newStartValue CS.System.Object
---@param newDuration number
function DG.Tweening.Tweener:ChangeStartValue(newStartValue, newDuration) end
---@overload fun(newEndValue:CS.System.Object, snapStartValue:boolean): DG.Tweening.Tweener
---@return DG.Tweening.Tweener
---@param newEndValue CS.System.Object
---@param newDuration number
---@param snapStartValue boolean
function DG.Tweening.Tweener:ChangeEndValue(newEndValue, newDuration, snapStartValue) end
---@return DG.Tweening.Tweener
---@param newStartValue CS.System.Object
---@param newEndValue CS.System.Object
---@param newDuration number
function DG.Tweening.Tweener:ChangeValues(newStartValue, newEndValue, newDuration) end
return DG.Tweening.Tweener
