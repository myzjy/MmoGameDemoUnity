---@class UnityEngine.Application
---@field public isPlaying boolean
---@field public isFocused boolean
---@field public platform number
---@field public buildGUID string
---@field public isMobilePlatform boolean
---@field public isConsolePlatform boolean
---@field public runInBackground boolean
---@field public isBatchMode boolean
---@field public dataPath string
---@field public streamingAssetsPath string
---@field public persistentDataPath string
---@field public temporaryCachePath string
---@field public absoluteURL string
---@field public unityVersion string
---@field public version string
---@field public installerName string
---@field public identifier string
---@field public installMode number
---@field public sandboxType number
---@field public productName string
---@field public companyName string
---@field public cloudProjectId string
---@field public targetFrameRate number
---@field public systemLanguage number
---@field public consoleLogPath string
---@field public backgroundLoadingPriority number
---@field public internetReachability number
---@field public genuine boolean
---@field public genuineCheckAvailable boolean
---@field public isEditor boolean

---@type UnityEngine.Application
UnityEngine.Application = { }
---@return UnityEngine.Application
function UnityEngine.Application.New() end
---@overload fun(): void
---@param exitCode number
function UnityEngine.Application.Quit(exitCode) end
function UnityEngine.Application.Unload() end
---@overload fun(levelIndex:number): boolean
---@return boolean
---@param levelName string
function UnityEngine.Application.CanStreamedLevelBeLoaded(levelName) end
---@return boolean
---@param obj UnityEngine.Object
function UnityEngine.Application.IsPlaying(obj) end
---@return System.String[]
function UnityEngine.Application.GetBuildTags() end
---@param buildTags System.String[]
function UnityEngine.Application.SetBuildTags(buildTags) end
---@return boolean
function UnityEngine.Application.HasProLicense() end
---@return boolean
---@param delegateMethod (fun(advertisingId:string, trackingEnabled:boolean, errorMsg:string):void)
function UnityEngine.Application.RequestAdvertisingIdentifierAsync(delegateMethod) end
---@param url string
function UnityEngine.Application.OpenURL(url) end
---@return number
---@param logType number
function UnityEngine.Application.GetStackTraceLogType(logType) end
---@param logType number
---@param stackTraceType number
function UnityEngine.Application.SetStackTraceLogType(logType, stackTraceType) end
---@return UnityEngine.AsyncOperation
---@param mode number
function UnityEngine.Application.RequestUserAuthorization(mode) end
---@return boolean
---@param mode number
function UnityEngine.Application.HasUserAuthorization(mode) end
---@param value (fun():void)
function UnityEngine.Application.add_lowMemory(value) end
---@param value (fun():void)
function UnityEngine.Application.remove_lowMemory(value) end
---@param value (fun(condition:string, stackTrace:string, type:number):void)
function UnityEngine.Application.add_logMessageReceived(value) end
---@param value (fun(condition:string, stackTrace:string, type:number):void)
function UnityEngine.Application.remove_logMessageReceived(value) end
---@param value (fun(condition:string, stackTrace:string, type:number):void)
function UnityEngine.Application.add_logMessageReceivedThreaded(value) end
---@param value (fun(condition:string, stackTrace:string, type:number):void)
function UnityEngine.Application.remove_logMessageReceivedThreaded(value) end
---@param value (fun():void)
function UnityEngine.Application.add_onBeforeRender(value) end
---@param value (fun():void)
function UnityEngine.Application.remove_onBeforeRender(value) end
---@param value (fun(obj:boolean):void)
function UnityEngine.Application.add_focusChanged(value) end
---@param value (fun(obj:boolean):void)
function UnityEngine.Application.remove_focusChanged(value) end
---@param value (fun():boolean)
function UnityEngine.Application.add_wantsToQuit(value) end
---@param value (fun():boolean)
function UnityEngine.Application.remove_wantsToQuit(value) end
---@param value (fun():void)
function UnityEngine.Application.add_quitting(value) end
---@param value (fun():void)
function UnityEngine.Application.remove_quitting(value) end
return UnityEngine.Application
