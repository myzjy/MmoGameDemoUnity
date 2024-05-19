PrintDebug("require lua script start")
--下面基础组件间的require有依赖顺序相关,闲着没事也别换顺序,要加新的往文件尾加就好
require("defines.app_defines")

PrintDebug("start load common\\unityEngine 文件夹下 得所有 lua scripts")
Mathf = require("common.unityEngine.Mathf")
Vector2 = require("common.unityEngine.Vector2")
Vector3 = require("common.unityEngine.Vector3")
Vector4 = require("common.unityEngine.Vector4")
Quaternion = require("common.unityEngine.Quaternion")
Color = require("common.unityEngine.Color")
Ray = require("common.unityEngine.Ray")
Bounds = require("common.unityEngine.Bounds")
RaycastHit = require("common.unityEngine.RaycastHit")
Touch = require("common.unityEngine.Touch")
LayerMask = require("common.unityEngine.LayerMask")
Plane = require("common.unityEngine.Plane")
Time = require("common.unityEngine.Time")
Object = require("common.unityEngine.Object")
PrintDebug("end load common\\unityEngine 文件夹下 得所有 lua scripts")

require("common.system.coroutine")

require("common.class")

PrintDebug("start load common\\util 文件夹下 得所有 lua scripts")
require("common.util.str")
require("common.util.time")
GlobalEventSystem = require("common.util.event_System")
require("common.util.string_util")
require("common.util.table_util")
require("common.util.util")
require("common.util.lua_util")
list = require("common.util.list")
events = require("common.util.event")
require("common.util.timer")
require("common.util.UI_utils")
PrintDebug("end load common\\util 文件夹下 得所有 lua scripts")

UpdateManager = require("common.updateManager")
JSON = require("common.json")

require("application.app.net.http.userAgent")
require("application.app.net.http.apiRequest")
require("application.app.net.http.apiResponse")

require("application.app.common.LuaUtils")

require("application.app.cache.LoginCacheData")
require("application.app.cache.playerUserCaCheData")

NetManager = require("application.app.net.webSocket.netManager")
ByteBuffer = require("application.app.net.webSocket.luaProtocol.buffer.byteBuffer")
Long = require("application.app.net.webSocket.luaProtocol.buffer.long")
ProtocolManager = require("application.app.net.webSocket.protocolManager")
PacketDispatcher = require("application.app.net.webSocket.packetDispatcher")

I18nManager = require("application.app.i18n.I18nManager")

GameEvent = require("application.app.event.gameEvent")
GameMainEvent = require("application.app.event.gameMainEvent")
UIGameEvent = require("application.app.event.uiGameEvent")

ProcedureGame = require("application.app.procedure.procedureFsmManager")

UIComponentManager = require("application.app.ui.UIComponentManager")

PrintDebug("require lua script end")
