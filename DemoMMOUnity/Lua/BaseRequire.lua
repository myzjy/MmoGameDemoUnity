printDebug("require lua script start")
--下面基础组件间的require有依赖顺序相关,闲着没事也别换顺序,要加新的往文件尾加就好
require("defines.app_defines")
require("Common.util.str")
-- require("Common.util.helper")
require("Common.util.time")

Mathf = require("Common.UnityEngine.Mathf")
Vector2 = require("Common.UnityEngine.Vector2")
Vector3 = require("Common.UnityEngine.Vector3")
Vector4 = require("Common.UnityEngine.Vector4")
Quaternion = require("Common.UnityEngine.Quaternion")
Color = require("Common.UnityEngine.Color")
Ray = require("Common.UnityEngine.Ray")
Bounds = require("Common.UnityEngine.Bounds")
RaycastHit = require("Common.UnityEngine.RaycastHit")
Touch = require("Common.UnityEngine.Touch")
LayerMask = require("Common.UnityEngine.LayerMask")
Plane = require("Common.UnityEngine.Plane")
Time = require("Common.UnityEngine.Time")
Object = require("Common.UnityEngine.Object")

require("Common.System.coroutine")

require("Common.class")

require("Common.util.stringUtil")
require("Common.util.util")
require("Common.util.luaUtil")
list = require("Common.util.list")
events = require("Common.util.event")
require("Common.util.timer")
UpdateManager = require("Common.UpdateManager")
JSON = require("Common.json")
require("application.app.net.http.userAgent")
require("application.app.net.http.apiRequest")
require("application.app.net.http.apiResponse")
NetManager = require("application.app.net.webscoket.netManager")
GlobalEventSystem = require("Common.util.EventSystem")
require("application.app.cache.LoginCacheData")
require("application.app.cache.playerUserCaCheData")
ByteBuffer = require("application.app.net.webscoket.luaProtocol.buffer.byteBuffer")
Long = require("application.app.net.webscoket.luaProtocol.buffer.long")
ProtocolManager = require("application.app.net.webscoket.protocolManager")
PacketDispatcher = require("application.app.net.webscoket.packetDispatcher")
I18nManager = require("application.app.i18n.I18nManager")



UIComponentManager = require("application.app.ui.UIComponentManager")

printDebug("require lua script end")
