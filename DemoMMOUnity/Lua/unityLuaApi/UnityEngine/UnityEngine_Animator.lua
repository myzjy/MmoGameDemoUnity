---@class UnityEngine.Animator : UnityEngine.Behaviour
---@field public isOptimizable boolean
---@field public isHuman boolean
---@field public hasRootMotion boolean
---@field public humanScale number
---@field public isInitialized boolean
---@field public deltaPosition UnityEngine.Vector3
---@field public deltaRotation UnityEngine.Quaternion
---@field public velocity UnityEngine.Vector3
---@field public angularVelocity UnityEngine.Vector3
---@field public rootPosition UnityEngine.Vector3
---@field public rootRotation UnityEngine.Quaternion
---@field public applyRootMotion boolean
---@field public updateMode number
---@field public hasTransformHierarchy boolean
---@field public gravityWeight number
---@field public bodyPosition UnityEngine.Vector3
---@field public bodyRotation UnityEngine.Quaternion
---@field public stabilizeFeet boolean
---@field public layerCount number
---@field public parameters UnityEngine.AnimatorControllerParameter[]
---@field public parameterCount number
---@field public feetPivotActive number
---@field public pivotWeight number
---@field public pivotPosition UnityEngine.Vector3
---@field public isMatchingTarget boolean
---@field public speed number
---@field public targetPosition UnityEngine.Vector3
---@field public targetRotation UnityEngine.Quaternion
---@field public cullingMode number
---@field public playbackTime number
---@field public recorderStartTime number
---@field public recorderStopTime number
---@field public recorderMode number
---@field public runtimeAnimatorController UnityEngine.RuntimeAnimatorController
---@field public hasBoundPlayables boolean
---@field public avatar UnityEngine.Avatar
---@field public playableGraph UnityEngine.Playables.PlayableGraph
---@field public layersAffectMassCenter boolean
---@field public leftFeetBottomHeight number
---@field public rightFeetBottomHeight number
---@field public logWarnings boolean
---@field public fireEvents boolean
---@field public keepAnimatorControllerStateOnDisable boolean

---@type UnityEngine.Animator
UnityEngine.Animator = { }
---@return UnityEngine.Animator
function UnityEngine.Animator.New() end
---@overload fun(name:string): number
---@return number
---@param id number
function UnityEngine.Animator:GetFloat(id) end
---@overload fun(name:string, value:number): void
---@overload fun(id:number, value:number): void
---@overload fun(name:string, value:number, dampTime:number, deltaTime:number): void
---@param id number
---@param value number
---@param dampTime number
---@param deltaTime number
function UnityEngine.Animator:SetFloat(id, value, dampTime, deltaTime) end
---@overload fun(name:string): boolean
---@return boolean
---@param id number
function UnityEngine.Animator:GetBool(id) end
---@overload fun(name:string, value:boolean): void
---@param id number
---@param value boolean
function UnityEngine.Animator:SetBool(id, value) end
---@overload fun(name:string): number
---@return number
---@param id number
function UnityEngine.Animator:GetInteger(id) end
---@overload fun(name:string, value:number): void
---@param id number
---@param value number
function UnityEngine.Animator:SetInteger(id, value) end
---@overload fun(name:string): void
---@param id number
function UnityEngine.Animator:SetTrigger(id) end
---@overload fun(name:string): void
---@param id number
function UnityEngine.Animator:ResetTrigger(id) end
---@overload fun(name:string): boolean
---@return boolean
---@param id number
function UnityEngine.Animator:IsParameterControlledByCurve(id) end
---@return UnityEngine.Vector3
---@param goal number
function UnityEngine.Animator:GetIKPosition(goal) end
---@param goal number
---@param goalPosition UnityEngine.Vector3
function UnityEngine.Animator:SetIKPosition(goal, goalPosition) end
---@return UnityEngine.Quaternion
---@param goal number
function UnityEngine.Animator:GetIKRotation(goal) end
---@param goal number
---@param goalRotation UnityEngine.Quaternion
function UnityEngine.Animator:SetIKRotation(goal, goalRotation) end
---@return number
---@param goal number
function UnityEngine.Animator:GetIKPositionWeight(goal) end
---@param goal number
---@param value number
function UnityEngine.Animator:SetIKPositionWeight(goal, value) end
---@return number
---@param goal number
function UnityEngine.Animator:GetIKRotationWeight(goal) end
---@param goal number
---@param value number
function UnityEngine.Animator:SetIKRotationWeight(goal, value) end
---@return UnityEngine.Vector3
---@param hint number
function UnityEngine.Animator:GetIKHintPosition(hint) end
---@param hint number
---@param hintPosition UnityEngine.Vector3
function UnityEngine.Animator:SetIKHintPosition(hint, hintPosition) end
---@return number
---@param hint number
function UnityEngine.Animator:GetIKHintPositionWeight(hint) end
---@param hint number
---@param value number
function UnityEngine.Animator:SetIKHintPositionWeight(hint, value) end
---@param lookAtPosition UnityEngine.Vector3
function UnityEngine.Animator:SetLookAtPosition(lookAtPosition) end
---@overload fun(weight:number): void
---@overload fun(weight:number, bodyWeight:number): void
---@overload fun(weight:number, bodyWeight:number, headWeight:number): void
---@overload fun(weight:number, bodyWeight:number, headWeight:number, eyesWeight:number): void
---@param weight number
---@param bodyWeight number
---@param headWeight number
---@param eyesWeight number
---@param clampWeight number
function UnityEngine.Animator:SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight, clampWeight) end
---@param humanBoneId number
---@param rotation UnityEngine.Quaternion
function UnityEngine.Animator:SetBoneLocalRotation(humanBoneId, rotation) end
---@return UnityEngine.StateMachineBehaviour[]
---@param fullPathHash number
---@param layerIndex number
function UnityEngine.Animator:GetBehaviours(fullPathHash, layerIndex) end
---@return string
---@param layerIndex number
function UnityEngine.Animator:GetLayerName(layerIndex) end
---@return number
---@param layerName string
function UnityEngine.Animator:GetLayerIndex(layerName) end
---@return number
---@param layerIndex number
function UnityEngine.Animator:GetLayerWeight(layerIndex) end
---@param layerIndex number
---@param weight number
function UnityEngine.Animator:SetLayerWeight(layerIndex, weight) end
---@return UnityEngine.AnimatorStateInfo
---@param layerIndex number
function UnityEngine.Animator:GetCurrentAnimatorStateInfo(layerIndex) end
---@return UnityEngine.AnimatorStateInfo
---@param layerIndex number
function UnityEngine.Animator:GetNextAnimatorStateInfo(layerIndex) end
---@return UnityEngine.AnimatorTransitionInfo
---@param layerIndex number
function UnityEngine.Animator:GetAnimatorTransitionInfo(layerIndex) end
---@return number
---@param layerIndex number
function UnityEngine.Animator:GetCurrentAnimatorClipInfoCount(layerIndex) end
---@return number
---@param layerIndex number
function UnityEngine.Animator:GetNextAnimatorClipInfoCount(layerIndex) end
---@overload fun(layerIndex:number): UnityEngine.AnimatorClipInfo[]
---@return UnityEngine.AnimatorClipInfo[]
---@param layerIndex number
---@param clips System.Collections.Generic.List_UnityEngine.AnimatorClipInfo
function UnityEngine.Animator:GetCurrentAnimatorClipInfo(layerIndex, clips) end
---@overload fun(layerIndex:number): UnityEngine.AnimatorClipInfo[]
---@return UnityEngine.AnimatorClipInfo[]
---@param layerIndex number
---@param clips System.Collections.Generic.List_UnityEngine.AnimatorClipInfo
function UnityEngine.Animator:GetNextAnimatorClipInfo(layerIndex, clips) end
---@return boolean
---@param layerIndex number
function UnityEngine.Animator:IsInTransition(layerIndex) end
---@return UnityEngine.AnimatorControllerParameter
---@param index number
function UnityEngine.Animator:GetParameter(index) end
---@overload fun(matchPosition:UnityEngine.Vector3, matchRotation:UnityEngine.Quaternion, targetBodyPart:number, weightMask:UnityEngine.MatchTargetWeightMask, startNormalizedTime:number): void
---@param matchPosition UnityEngine.Vector3
---@param matchRotation UnityEngine.Quaternion
---@param targetBodyPart number
---@param weightMask UnityEngine.MatchTargetWeightMask
---@param startNormalizedTime number
---@param targetNormalizedTime number
function UnityEngine.Animator:MatchTarget(matchPosition, matchRotation, targetBodyPart, weightMask, startNormalizedTime, targetNormalizedTime) end
---@overload fun(): void
---@param completeMatch boolean
function UnityEngine.Animator:InterruptMatchTarget(completeMatch) end
---@overload fun(stateName:string, fixedTransitionDuration:number): void
---@overload fun(stateHashName:number, fixedTransitionDuration:number): void
---@overload fun(stateName:string, fixedTransitionDuration:number, layer:number): void
---@overload fun(stateHashName:number, fixedTransitionDuration:number, layer:number): void
---@overload fun(stateName:string, fixedTransitionDuration:number, layer:number, fixedTimeOffset:number): void
---@overload fun(stateHashName:number, fixedTransitionDuration:number, layer:number, fixedTimeOffset:number): void
---@overload fun(stateName:string, fixedTransitionDuration:number, layer:number, fixedTimeOffset:number, normalizedTransitionTime:number): void
---@param stateHashName number
---@param fixedTransitionDuration number
---@param layer number
---@param fixedTimeOffset number
---@param normalizedTransitionTime number
function UnityEngine.Animator:CrossFadeInFixedTime(stateHashName, fixedTransitionDuration, layer, fixedTimeOffset, normalizedTransitionTime) end
function UnityEngine.Animator:WriteDefaultValues() end
---@overload fun(stateName:string, normalizedTransitionDuration:number): void
---@overload fun(stateHashName:number, normalizedTransitionDuration:number): void
---@overload fun(stateName:string, normalizedTransitionDuration:number, layer:number): void
---@overload fun(stateHashName:number, normalizedTransitionDuration:number, layer:number): void
---@overload fun(stateName:string, normalizedTransitionDuration:number, layer:number, normalizedTimeOffset:number): void
---@overload fun(stateHashName:number, normalizedTransitionDuration:number, layer:number, normalizedTimeOffset:number): void
---@overload fun(stateName:string, normalizedTransitionDuration:number, layer:number, normalizedTimeOffset:number, normalizedTransitionTime:number): void
---@param stateHashName number
---@param normalizedTransitionDuration number
---@param layer number
---@param normalizedTimeOffset number
---@param normalizedTransitionTime number
function UnityEngine.Animator:CrossFade(stateHashName, normalizedTransitionDuration, layer, normalizedTimeOffset, normalizedTransitionTime) end
---@overload fun(stateName:string): void
---@overload fun(stateNameHash:number): void
---@overload fun(stateName:string, layer:number): void
---@overload fun(stateNameHash:number, layer:number): void
---@overload fun(stateName:string, layer:number, fixedTime:number): void
---@param stateNameHash number
---@param layer number
---@param fixedTime number
function UnityEngine.Animator:PlayInFixedTime(stateNameHash, layer, fixedTime) end
---@overload fun(stateName:string): void
---@overload fun(stateNameHash:number): void
---@overload fun(stateName:string, layer:number): void
---@overload fun(stateNameHash:number, layer:number): void
---@overload fun(stateName:string, layer:number, normalizedTime:number): void
---@param stateNameHash number
---@param layer number
---@param normalizedTime number
function UnityEngine.Animator:Play(stateNameHash, layer, normalizedTime) end
---@param targetIndex number
---@param targetNormalizedTime number
function UnityEngine.Animator:SetTarget(targetIndex, targetNormalizedTime) end
---@return UnityEngine.Transform
---@param humanBoneId number
function UnityEngine.Animator:GetBoneTransform(humanBoneId) end
function UnityEngine.Animator:StartPlayback() end
function UnityEngine.Animator:StopPlayback() end
---@param frameCount number
function UnityEngine.Animator:StartRecording(frameCount) end
function UnityEngine.Animator:StopRecording() end
---@return boolean
---@param layerIndex number
---@param stateID number
function UnityEngine.Animator:HasState(layerIndex, stateID) end
---@return number
---@param name string
function UnityEngine.Animator.StringToHash(name) end
---@param deltaTime number
function UnityEngine.Animator:Update(deltaTime) end
function UnityEngine.Animator:Rebind() end
function UnityEngine.Animator:ApplyBuiltinRootMotion() end
return UnityEngine.Animator
