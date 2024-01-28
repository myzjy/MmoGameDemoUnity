---@class UnityEngine.Input
---@field public simulateMouseWithTouches boolean
---@field public anyKey boolean
---@field public anyKeyDown boolean
---@field public inputString string
---@field public mousePosition UnityEngine.Vector3
---@field public mouseScrollDelta UnityEngine.Vector2
---@field public imeCompositionMode number
---@field public compositionString string
---@field public imeIsSelected boolean
---@field public compositionCursorPos UnityEngine.Vector2
---@field public mousePresent boolean
---@field public touchCount number
---@field public touchPressureSupported boolean
---@field public stylusTouchSupported boolean
---@field public touchSupported boolean
---@field public multiTouchEnabled boolean
---@field public deviceOrientation number
---@field public acceleration UnityEngine.Vector3
---@field public compensateSensors boolean
---@field public accelerationEventCount number
---@field public backButtonLeavesApp boolean
---@field public location UnityEngine.LocationService
---@field public compass UnityEngine.Compass
---@field public gyro UnityEngine.Gyroscope
---@field public touches UnityEngine.Touch[]
---@field public accelerationEvents UnityEngine.AccelerationEvent[]

---@type UnityEngine.Input
UnityEngine.Input = { }
---@return UnityEngine.Input
function UnityEngine.Input.New() end
---@return number
---@param axisName string
function UnityEngine.Input.GetAxis(axisName) end
---@return number
---@param axisName string
function UnityEngine.Input.GetAxisRaw(axisName) end
---@return boolean
---@param buttonName string
function UnityEngine.Input.GetButton(buttonName) end
---@return boolean
---@param buttonName string
function UnityEngine.Input.GetButtonDown(buttonName) end
---@return boolean
---@param buttonName string
function UnityEngine.Input.GetButtonUp(buttonName) end
---@return boolean
---@param button number
function UnityEngine.Input.GetMouseButton(button) end
---@return boolean
---@param button number
function UnityEngine.Input.GetMouseButtonDown(button) end
---@return boolean
---@param button number
function UnityEngine.Input.GetMouseButtonUp(button) end
function UnityEngine.Input.ResetInputAxes() end
---@return boolean
---@param joystickName string
function UnityEngine.Input.IsJoystickPreconfigured(joystickName) end
---@return System.String[]
function UnityEngine.Input.GetJoystickNames() end
---@return UnityEngine.Touch
---@param index number
function UnityEngine.Input.GetTouch(index) end
---@return UnityEngine.AccelerationEvent
---@param index number
function UnityEngine.Input.GetAccelerationEvent(index) end
---@overload fun(key:number): boolean
---@return boolean
---@param name string
function UnityEngine.Input.GetKey(name) end
---@overload fun(key:number): boolean
---@return boolean
---@param name string
function UnityEngine.Input.GetKeyUp(name) end
---@overload fun(key:number): boolean
---@return boolean
---@param name string
function UnityEngine.Input.GetKeyDown(name) end
return UnityEngine.Input
