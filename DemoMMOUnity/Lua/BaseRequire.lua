--下面基础组件间的require有依赖顺序相关,闲着没事也别换顺序,要加新的往文件尾加就好

Mathf       = require "Common.UnityEngine.Mathf"
Vector2     = require "Common.UnityEngine.Vector2"
Vector3     = require "Common.UnityEngine.Vector3"
Vector4		= require "Common.UnityEngine.Vector4"
Quaternion	= require "Common.UnityEngine.Quaternion"
Color		= require "Common.UnityEngine.Color"
Ray			= require "Common.UnityEngine.Ray"
Bounds		= require "Common.UnityEngine.Bounds"
RaycastHit	= require "Common.UnityEngine.RaycastHit"
Touch		= require "Common.UnityEngine.Touch"
LayerMask	= require "Common.UnityEngine.LayerMask"
Plane		= require "Common.UnityEngine.Plane"
Time		= require "Common.UnityEngine.Time"
Object		= require "Common.UnityEngine.Object"

require("Common/BaseClass")

