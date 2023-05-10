#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using System;


namespace XLua
{
    public static partial class ObjectTranslator_Gen
    {
		public static void Init(LuaEnv luaenv, ObjectTranslator translator)
		{
		
			UnityEngineVector2_TypeID = -1;
		
			UnityEngineVector3_TypeID = -1;
		
			UnityEngineVector4_TypeID = -1;
		
			UnityEngineColor_TypeID = -1;
		
			UnityEngineQuaternion_TypeID = -1;
		
			UnityEngineRay_TypeID = -1;
		
			UnityEngineBounds_TypeID = -1;
		
			UnityEngineRay2D_TypeID = -1;
		
			UnityEngineUIInputFieldContentType_TypeID = -1;
			UnityEngineUIInputFieldContentType_EnumRef = -1;
		
			UnityEngineUIInputFieldInputType_TypeID = -1;
			UnityEngineUIInputFieldInputType_EnumRef = -1;
		
			UnityEngineUIInputFieldCharacterValidation_TypeID = -1;
			UnityEngineUIInputFieldCharacterValidation_EnumRef = -1;
		
			UnityEngineUIInputFieldLineType_TypeID = -1;
			UnityEngineUIInputFieldLineType_EnumRef = -1;
		
			UnityEngineTextAnchor_TypeID = -1;
			UnityEngineTextAnchor_EnumRef = -1;
		
			UnityEngineTouchPhase_TypeID = -1;
			UnityEngineTouchPhase_EnumRef = -1;
		
			UnityEngineRectTransformAxis_TypeID = -1;
			UnityEngineRectTransformAxis_EnumRef = -1;
		
			UnityEngineRectTransformEdge_TypeID = -1;
			UnityEngineRectTransformEdge_EnumRef = -1;
		
			UnityEngineUIScrollRectMovementType_TypeID = -1;
			UnityEngineUIScrollRectMovementType_EnumRef = -1;
		
			UnityEngineUIScrollRectScrollbarVisibility_TypeID = -1;
			UnityEngineUIScrollRectScrollbarVisibility_EnumRef = -1;
		
			UnityEngineUIImageType_TypeID = -1;
			UnityEngineUIImageType_EnumRef = -1;
		
			UnityEngineUIImageFillMethod_TypeID = -1;
			UnityEngineUIImageFillMethod_EnumRef = -1;
		
			UnityEngineUIImageOriginHorizontal_TypeID = -1;
			UnityEngineUIImageOriginHorizontal_EnumRef = -1;
		
			UnityEngineUIImageOriginVertical_TypeID = -1;
			UnityEngineUIImageOriginVertical_EnumRef = -1;
		
			UnityEngineUIImageOrigin90_TypeID = -1;
			UnityEngineUIImageOrigin90_EnumRef = -1;
		
			UnityEngineUIImageOrigin180_TypeID = -1;
			UnityEngineUIImageOrigin180_EnumRef = -1;
		
			UnityEngineUIImageOrigin360_TypeID = -1;
			UnityEngineUIImageOrigin360_EnumRef = -1;
		
			UnityEngineCameraFieldOfViewAxis_TypeID = -1;
			UnityEngineCameraFieldOfViewAxis_EnumRef = -1;
		
			UnityEngineCameraMonoOrStereoscopicEye_TypeID = -1;
			UnityEngineCameraMonoOrStereoscopicEye_EnumRef = -1;
		
			UnityEngineCameraStereoscopicEye_TypeID = -1;
			UnityEngineCameraStereoscopicEye_EnumRef = -1;
		
			UnityEngineCameraGateFitMode_TypeID = -1;
			UnityEngineCameraGateFitMode_EnumRef = -1;
		
			UnityEngineTexture2DEXRFlags_TypeID = -1;
			UnityEngineTexture2DEXRFlags_EnumRef = -1;
		
			UnityEngineKeyCode_TypeID = -1;
			UnityEngineKeyCode_EnumRef = -1;
		

		
			translator.RegisterPushAndGetAndUpdate<UnityEngine.Vector2>(translator.PushUnityEngineVector2, translator.GetUnityEngineVector2, translator.UpdateUnityEngineVector2);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.Vector3>(translator.PushUnityEngineVector3, translator.GetUnityEngineVector3, translator.UpdateUnityEngineVector3);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.Vector4>(translator.PushUnityEngineVector4, translator.GetUnityEngineVector4, translator.UpdateUnityEngineVector4);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.Color>(translator.PushUnityEngineColor, translator.GetUnityEngineColor, translator.UpdateUnityEngineColor);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.Quaternion>(translator.PushUnityEngineQuaternion, translator.GetUnityEngineQuaternion, translator.UpdateUnityEngineQuaternion);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.Ray>(translator.PushUnityEngineRay, translator.GetUnityEngineRay, translator.UpdateUnityEngineRay);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.Bounds>(translator.PushUnityEngineBounds, translator.GetUnityEngineBounds, translator.UpdateUnityEngineBounds);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.Ray2D>(translator.PushUnityEngineRay2D, translator.GetUnityEngineRay2D, translator.UpdateUnityEngineRay2D);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.UI.InputField.ContentType>(translator.PushUnityEngineUIInputFieldContentType, translator.GetUnityEngineUIInputFieldContentType, translator.UpdateUnityEngineUIInputFieldContentType);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.UI.InputField.InputType>(translator.PushUnityEngineUIInputFieldInputType, translator.GetUnityEngineUIInputFieldInputType, translator.UpdateUnityEngineUIInputFieldInputType);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.UI.InputField.CharacterValidation>(translator.PushUnityEngineUIInputFieldCharacterValidation, translator.GetUnityEngineUIInputFieldCharacterValidation, translator.UpdateUnityEngineUIInputFieldCharacterValidation);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.UI.InputField.LineType>(translator.PushUnityEngineUIInputFieldLineType, translator.GetUnityEngineUIInputFieldLineType, translator.UpdateUnityEngineUIInputFieldLineType);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.TextAnchor>(translator.PushUnityEngineTextAnchor, translator.GetUnityEngineTextAnchor, translator.UpdateUnityEngineTextAnchor);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.TouchPhase>(translator.PushUnityEngineTouchPhase, translator.GetUnityEngineTouchPhase, translator.UpdateUnityEngineTouchPhase);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.RectTransform.Axis>(translator.PushUnityEngineRectTransformAxis, translator.GetUnityEngineRectTransformAxis, translator.UpdateUnityEngineRectTransformAxis);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.RectTransform.Edge>(translator.PushUnityEngineRectTransformEdge, translator.GetUnityEngineRectTransformEdge, translator.UpdateUnityEngineRectTransformEdge);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.UI.ScrollRect.MovementType>(translator.PushUnityEngineUIScrollRectMovementType, translator.GetUnityEngineUIScrollRectMovementType, translator.UpdateUnityEngineUIScrollRectMovementType);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.UI.ScrollRect.ScrollbarVisibility>(translator.PushUnityEngineUIScrollRectScrollbarVisibility, translator.GetUnityEngineUIScrollRectScrollbarVisibility, translator.UpdateUnityEngineUIScrollRectScrollbarVisibility);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.UI.Image.Type>(translator.PushUnityEngineUIImageType, translator.GetUnityEngineUIImageType, translator.UpdateUnityEngineUIImageType);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.UI.Image.FillMethod>(translator.PushUnityEngineUIImageFillMethod, translator.GetUnityEngineUIImageFillMethod, translator.UpdateUnityEngineUIImageFillMethod);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.UI.Image.OriginHorizontal>(translator.PushUnityEngineUIImageOriginHorizontal, translator.GetUnityEngineUIImageOriginHorizontal, translator.UpdateUnityEngineUIImageOriginHorizontal);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.UI.Image.OriginVertical>(translator.PushUnityEngineUIImageOriginVertical, translator.GetUnityEngineUIImageOriginVertical, translator.UpdateUnityEngineUIImageOriginVertical);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.UI.Image.Origin90>(translator.PushUnityEngineUIImageOrigin90, translator.GetUnityEngineUIImageOrigin90, translator.UpdateUnityEngineUIImageOrigin90);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.UI.Image.Origin180>(translator.PushUnityEngineUIImageOrigin180, translator.GetUnityEngineUIImageOrigin180, translator.UpdateUnityEngineUIImageOrigin180);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.UI.Image.Origin360>(translator.PushUnityEngineUIImageOrigin360, translator.GetUnityEngineUIImageOrigin360, translator.UpdateUnityEngineUIImageOrigin360);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.Camera.FieldOfViewAxis>(translator.PushUnityEngineCameraFieldOfViewAxis, translator.GetUnityEngineCameraFieldOfViewAxis, translator.UpdateUnityEngineCameraFieldOfViewAxis);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.Camera.MonoOrStereoscopicEye>(translator.PushUnityEngineCameraMonoOrStereoscopicEye, translator.GetUnityEngineCameraMonoOrStereoscopicEye, translator.UpdateUnityEngineCameraMonoOrStereoscopicEye);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.Camera.StereoscopicEye>(translator.PushUnityEngineCameraStereoscopicEye, translator.GetUnityEngineCameraStereoscopicEye, translator.UpdateUnityEngineCameraStereoscopicEye);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.Camera.GateFitMode>(translator.PushUnityEngineCameraGateFitMode, translator.GetUnityEngineCameraGateFitMode, translator.UpdateUnityEngineCameraGateFitMode);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.Texture2D.EXRFlags>(translator.PushUnityEngineTexture2DEXRFlags, translator.GetUnityEngineTexture2DEXRFlags, translator.UpdateUnityEngineTexture2DEXRFlags);
			translator.RegisterPushAndGetAndUpdate<UnityEngine.KeyCode>(translator.PushUnityEngineKeyCode, translator.GetUnityEngineKeyCode, translator.UpdateUnityEngineKeyCode);
		
		}
        
        static int UnityEngineVector2_TypeID = -1;
        public static void PushUnityEngineVector2(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.Vector2 val)
        {
            if (UnityEngineVector2_TypeID == -1)
            {
			    bool is_first;
                UnityEngineVector2_TypeID = thiz.getTypeId(L, typeof(UnityEngine.Vector2), out is_first);
				
            }
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 8, UnityEngineVector2_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, val))
            {
                throw new Exception("pack fail fail for UnityEngine.Vector2 ,value="+val);
            }
			
        }
		
        public static void GetUnityEngineVector2(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.Vector2 val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineVector2_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Vector2");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);if (!CopyByValue_Gen.UnPack(buff, 0, out val))
                {
                    throw new Exception("unpack fail for UnityEngine.Vector2");
                }
            }
			else if (type ==LuaTypes.LUA_TTABLE)
			{
			    CopyByValue_Gen.UnPack(thiz, L, index, out val);
			}
            else
            {
                val = (UnityEngine.Vector2)thiz.objectCasters.GetCaster(typeof(UnityEngine.Vector2))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineVector2(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.Vector2 val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineVector2_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Vector2");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  val))
                {
                    throw new Exception("pack fail for UnityEngine.Vector2 ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineVector3_TypeID = -1;
        public static void PushUnityEngineVector3(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.Vector3 val)
        {
            if (UnityEngineVector3_TypeID == -1)
            {
			    bool is_first;
                UnityEngineVector3_TypeID = thiz.getTypeId(L, typeof(UnityEngine.Vector3), out is_first);
				
            }
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 12, UnityEngineVector3_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, val))
            {
                throw new Exception("pack fail fail for UnityEngine.Vector3 ,value="+val);
            }
			
        }
		
        public static void GetUnityEngineVector3(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.Vector3 val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineVector3_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Vector3");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);if (!CopyByValue_Gen.UnPack(buff, 0, out val))
                {
                    throw new Exception("unpack fail for UnityEngine.Vector3");
                }
            }
			else if (type ==LuaTypes.LUA_TTABLE)
			{
			    CopyByValue_Gen.UnPack(thiz, L, index, out val);
			}
            else
            {
                val = (UnityEngine.Vector3)thiz.objectCasters.GetCaster(typeof(UnityEngine.Vector3))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineVector3(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.Vector3 val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineVector3_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Vector3");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  val))
                {
                    throw new Exception("pack fail for UnityEngine.Vector3 ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineVector4_TypeID = -1;
        public static void PushUnityEngineVector4(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.Vector4 val)
        {
            if (UnityEngineVector4_TypeID == -1)
            {
			    bool is_first;
                UnityEngineVector4_TypeID = thiz.getTypeId(L, typeof(UnityEngine.Vector4), out is_first);
				
            }
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 16, UnityEngineVector4_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, val))
            {
                throw new Exception("pack fail fail for UnityEngine.Vector4 ,value="+val);
            }
			
        }
		
        public static void GetUnityEngineVector4(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.Vector4 val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineVector4_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Vector4");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);if (!CopyByValue_Gen.UnPack(buff, 0, out val))
                {
                    throw new Exception("unpack fail for UnityEngine.Vector4");
                }
            }
			else if (type ==LuaTypes.LUA_TTABLE)
			{
			    CopyByValue_Gen.UnPack(thiz, L, index, out val);
			}
            else
            {
                val = (UnityEngine.Vector4)thiz.objectCasters.GetCaster(typeof(UnityEngine.Vector4))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineVector4(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.Vector4 val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineVector4_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Vector4");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  val))
                {
                    throw new Exception("pack fail for UnityEngine.Vector4 ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineColor_TypeID = -1;
        public static void PushUnityEngineColor(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.Color val)
        {
            if (UnityEngineColor_TypeID == -1)
            {
			    bool is_first;
                UnityEngineColor_TypeID = thiz.getTypeId(L, typeof(UnityEngine.Color), out is_first);
				
            }
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 16, UnityEngineColor_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, val))
            {
                throw new Exception("pack fail fail for UnityEngine.Color ,value="+val);
            }
			
        }
		
        public static void GetUnityEngineColor(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.Color val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineColor_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Color");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);if (!CopyByValue_Gen.UnPack(buff, 0, out val))
                {
                    throw new Exception("unpack fail for UnityEngine.Color");
                }
            }
			else if (type ==LuaTypes.LUA_TTABLE)
			{
			    CopyByValue_Gen.UnPack(thiz, L, index, out val);
			}
            else
            {
                val = (UnityEngine.Color)thiz.objectCasters.GetCaster(typeof(UnityEngine.Color))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineColor(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.Color val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineColor_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Color");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  val))
                {
                    throw new Exception("pack fail for UnityEngine.Color ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineQuaternion_TypeID = -1;
        public static void PushUnityEngineQuaternion(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.Quaternion val)
        {
            if (UnityEngineQuaternion_TypeID == -1)
            {
			    bool is_first;
                UnityEngineQuaternion_TypeID = thiz.getTypeId(L, typeof(UnityEngine.Quaternion), out is_first);
				
            }
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 16, UnityEngineQuaternion_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, val))
            {
                throw new Exception("pack fail fail for UnityEngine.Quaternion ,value="+val);
            }
			
        }
		
        public static void GetUnityEngineQuaternion(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.Quaternion val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineQuaternion_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Quaternion");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);if (!CopyByValue_Gen.UnPack(buff, 0, out val))
                {
                    throw new Exception("unpack fail for UnityEngine.Quaternion");
                }
            }
			else if (type ==LuaTypes.LUA_TTABLE)
			{
			    CopyByValue_Gen.UnPack(thiz, L, index, out val);
			}
            else
            {
                val = (UnityEngine.Quaternion)thiz.objectCasters.GetCaster(typeof(UnityEngine.Quaternion))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineQuaternion(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.Quaternion val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineQuaternion_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Quaternion");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  val))
                {
                    throw new Exception("pack fail for UnityEngine.Quaternion ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineRay_TypeID = -1;
        public static void PushUnityEngineRay(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.Ray val)
        {
            if (UnityEngineRay_TypeID == -1)
            {
			    bool is_first;
                UnityEngineRay_TypeID = thiz.getTypeId(L, typeof(UnityEngine.Ray), out is_first);
				
            }
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 24, UnityEngineRay_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, val))
            {
                throw new Exception("pack fail fail for UnityEngine.Ray ,value="+val);
            }
			
        }
		
        public static void GetUnityEngineRay(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.Ray val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineRay_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Ray");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);if (!CopyByValue_Gen.UnPack(buff, 0, out val))
                {
                    throw new Exception("unpack fail for UnityEngine.Ray");
                }
            }
			else if (type ==LuaTypes.LUA_TTABLE)
			{
			    CopyByValue_Gen.UnPack(thiz, L, index, out val);
			}
            else
            {
                val = (UnityEngine.Ray)thiz.objectCasters.GetCaster(typeof(UnityEngine.Ray))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineRay(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.Ray val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineRay_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Ray");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  val))
                {
                    throw new Exception("pack fail for UnityEngine.Ray ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineBounds_TypeID = -1;
        public static void PushUnityEngineBounds(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.Bounds val)
        {
            if (UnityEngineBounds_TypeID == -1)
            {
			    bool is_first;
                UnityEngineBounds_TypeID = thiz.getTypeId(L, typeof(UnityEngine.Bounds), out is_first);
				
            }
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 24, UnityEngineBounds_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, val))
            {
                throw new Exception("pack fail fail for UnityEngine.Bounds ,value="+val);
            }
			
        }
		
        public static void GetUnityEngineBounds(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.Bounds val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineBounds_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Bounds");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);if (!CopyByValue_Gen.UnPack(buff, 0, out val))
                {
                    throw new Exception("unpack fail for UnityEngine.Bounds");
                }
            }
			else if (type ==LuaTypes.LUA_TTABLE)
			{
			    CopyByValue_Gen.UnPack(thiz, L, index, out val);
			}
            else
            {
                val = (UnityEngine.Bounds)thiz.objectCasters.GetCaster(typeof(UnityEngine.Bounds))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineBounds(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.Bounds val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineBounds_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Bounds");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  val))
                {
                    throw new Exception("pack fail for UnityEngine.Bounds ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineRay2D_TypeID = -1;
        public static void PushUnityEngineRay2D(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.Ray2D val)
        {
            if (UnityEngineRay2D_TypeID == -1)
            {
			    bool is_first;
                UnityEngineRay2D_TypeID = thiz.getTypeId(L, typeof(UnityEngine.Ray2D), out is_first);
				
            }
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 16, UnityEngineRay2D_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, val))
            {
                throw new Exception("pack fail fail for UnityEngine.Ray2D ,value="+val);
            }
			
        }
		
        public static void GetUnityEngineRay2D(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.Ray2D val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineRay2D_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Ray2D");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);if (!CopyByValue_Gen.UnPack(buff, 0, out val))
                {
                    throw new Exception("unpack fail for UnityEngine.Ray2D");
                }
            }
			else if (type ==LuaTypes.LUA_TTABLE)
			{
			    CopyByValue_Gen.UnPack(thiz, L, index, out val);
			}
            else
            {
                val = (UnityEngine.Ray2D)thiz.objectCasters.GetCaster(typeof(UnityEngine.Ray2D))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineRay2D(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.Ray2D val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineRay2D_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Ray2D");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  val))
                {
                    throw new Exception("pack fail for UnityEngine.Ray2D ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineUIInputFieldContentType_TypeID = -1;
		static int UnityEngineUIInputFieldContentType_EnumRef = -1;
        
        public static void PushUnityEngineUIInputFieldContentType(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.UI.InputField.ContentType val)
        {
            if (UnityEngineUIInputFieldContentType_TypeID == -1)
            {
			    bool is_first;
                UnityEngineUIInputFieldContentType_TypeID = thiz.getTypeId(L, typeof(UnityEngine.UI.InputField.ContentType), out is_first);
				
				if (UnityEngineUIInputFieldContentType_EnumRef == -1)
				{
				    Utils.LoadCSTable(L, typeof(UnityEngine.UI.InputField.ContentType));
				    UnityEngineUIInputFieldContentType_EnumRef = LuaAPI.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
				}
				
            }
			
			if (LuaAPI.xlua_tryget_cachedud(L, (int)val, UnityEngineUIInputFieldContentType_EnumRef) == 1)
            {
			    return;
			}
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 4, UnityEngineUIInputFieldContentType_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, (int)val))
            {
                throw new Exception("pack fail fail for UnityEngine.UI.InputField.ContentType ,value="+val);
            }
			
			LuaAPI.lua_getref(L, UnityEngineUIInputFieldContentType_EnumRef);
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.xlua_rawseti(L, -2, (int)val);
			LuaAPI.lua_pop(L, 1);
			
        }
		
        public static void GetUnityEngineUIInputFieldContentType(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.UI.InputField.ContentType val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineUIInputFieldContentType_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.UI.InputField.ContentType");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
				int e;
                if (!CopyByValue_Gen.UnPack(buff, 0, out e))
                {
                    throw new Exception("unpack fail for UnityEngine.UI.InputField.ContentType");
                }
				val = (UnityEngine.UI.InputField.ContentType)e;
                
            }
            else
            {
                val = (UnityEngine.UI.InputField.ContentType)thiz.objectCasters.GetCaster(typeof(UnityEngine.UI.InputField.ContentType))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineUIInputFieldContentType(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.UI.InputField.ContentType val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineUIInputFieldContentType_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.UI.InputField.ContentType");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  (int)val))
                {
                    throw new Exception("pack fail for UnityEngine.UI.InputField.ContentType ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineUIInputFieldInputType_TypeID = -1;
		static int UnityEngineUIInputFieldInputType_EnumRef = -1;
        
        public static void PushUnityEngineUIInputFieldInputType(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.UI.InputField.InputType val)
        {
            if (UnityEngineUIInputFieldInputType_TypeID == -1)
            {
			    bool is_first;
                UnityEngineUIInputFieldInputType_TypeID = thiz.getTypeId(L, typeof(UnityEngine.UI.InputField.InputType), out is_first);
				
				if (UnityEngineUIInputFieldInputType_EnumRef == -1)
				{
				    Utils.LoadCSTable(L, typeof(UnityEngine.UI.InputField.InputType));
				    UnityEngineUIInputFieldInputType_EnumRef = LuaAPI.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
				}
				
            }
			
			if (LuaAPI.xlua_tryget_cachedud(L, (int)val, UnityEngineUIInputFieldInputType_EnumRef) == 1)
            {
			    return;
			}
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 4, UnityEngineUIInputFieldInputType_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, (int)val))
            {
                throw new Exception("pack fail fail for UnityEngine.UI.InputField.InputType ,value="+val);
            }
			
			LuaAPI.lua_getref(L, UnityEngineUIInputFieldInputType_EnumRef);
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.xlua_rawseti(L, -2, (int)val);
			LuaAPI.lua_pop(L, 1);
			
        }
		
        public static void GetUnityEngineUIInputFieldInputType(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.UI.InputField.InputType val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineUIInputFieldInputType_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.UI.InputField.InputType");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
				int e;
                if (!CopyByValue_Gen.UnPack(buff, 0, out e))
                {
                    throw new Exception("unpack fail for UnityEngine.UI.InputField.InputType");
                }
				val = (UnityEngine.UI.InputField.InputType)e;
                
            }
            else
            {
                val = (UnityEngine.UI.InputField.InputType)thiz.objectCasters.GetCaster(typeof(UnityEngine.UI.InputField.InputType))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineUIInputFieldInputType(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.UI.InputField.InputType val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineUIInputFieldInputType_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.UI.InputField.InputType");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  (int)val))
                {
                    throw new Exception("pack fail for UnityEngine.UI.InputField.InputType ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineUIInputFieldCharacterValidation_TypeID = -1;
		static int UnityEngineUIInputFieldCharacterValidation_EnumRef = -1;
        
        public static void PushUnityEngineUIInputFieldCharacterValidation(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.UI.InputField.CharacterValidation val)
        {
            if (UnityEngineUIInputFieldCharacterValidation_TypeID == -1)
            {
			    bool is_first;
                UnityEngineUIInputFieldCharacterValidation_TypeID = thiz.getTypeId(L, typeof(UnityEngine.UI.InputField.CharacterValidation), out is_first);
				
				if (UnityEngineUIInputFieldCharacterValidation_EnumRef == -1)
				{
				    Utils.LoadCSTable(L, typeof(UnityEngine.UI.InputField.CharacterValidation));
				    UnityEngineUIInputFieldCharacterValidation_EnumRef = LuaAPI.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
				}
				
            }
			
			if (LuaAPI.xlua_tryget_cachedud(L, (int)val, UnityEngineUIInputFieldCharacterValidation_EnumRef) == 1)
            {
			    return;
			}
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 4, UnityEngineUIInputFieldCharacterValidation_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, (int)val))
            {
                throw new Exception("pack fail fail for UnityEngine.UI.InputField.CharacterValidation ,value="+val);
            }
			
			LuaAPI.lua_getref(L, UnityEngineUIInputFieldCharacterValidation_EnumRef);
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.xlua_rawseti(L, -2, (int)val);
			LuaAPI.lua_pop(L, 1);
			
        }
		
        public static void GetUnityEngineUIInputFieldCharacterValidation(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.UI.InputField.CharacterValidation val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineUIInputFieldCharacterValidation_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.UI.InputField.CharacterValidation");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
				int e;
                if (!CopyByValue_Gen.UnPack(buff, 0, out e))
                {
                    throw new Exception("unpack fail for UnityEngine.UI.InputField.CharacterValidation");
                }
				val = (UnityEngine.UI.InputField.CharacterValidation)e;
                
            }
            else
            {
                val = (UnityEngine.UI.InputField.CharacterValidation)thiz.objectCasters.GetCaster(typeof(UnityEngine.UI.InputField.CharacterValidation))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineUIInputFieldCharacterValidation(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.UI.InputField.CharacterValidation val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineUIInputFieldCharacterValidation_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.UI.InputField.CharacterValidation");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  (int)val))
                {
                    throw new Exception("pack fail for UnityEngine.UI.InputField.CharacterValidation ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineUIInputFieldLineType_TypeID = -1;
		static int UnityEngineUIInputFieldLineType_EnumRef = -1;
        
        public static void PushUnityEngineUIInputFieldLineType(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.UI.InputField.LineType val)
        {
            if (UnityEngineUIInputFieldLineType_TypeID == -1)
            {
			    bool is_first;
                UnityEngineUIInputFieldLineType_TypeID = thiz.getTypeId(L, typeof(UnityEngine.UI.InputField.LineType), out is_first);
				
				if (UnityEngineUIInputFieldLineType_EnumRef == -1)
				{
				    Utils.LoadCSTable(L, typeof(UnityEngine.UI.InputField.LineType));
				    UnityEngineUIInputFieldLineType_EnumRef = LuaAPI.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
				}
				
            }
			
			if (LuaAPI.xlua_tryget_cachedud(L, (int)val, UnityEngineUIInputFieldLineType_EnumRef) == 1)
            {
			    return;
			}
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 4, UnityEngineUIInputFieldLineType_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, (int)val))
            {
                throw new Exception("pack fail fail for UnityEngine.UI.InputField.LineType ,value="+val);
            }
			
			LuaAPI.lua_getref(L, UnityEngineUIInputFieldLineType_EnumRef);
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.xlua_rawseti(L, -2, (int)val);
			LuaAPI.lua_pop(L, 1);
			
        }
		
        public static void GetUnityEngineUIInputFieldLineType(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.UI.InputField.LineType val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineUIInputFieldLineType_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.UI.InputField.LineType");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
				int e;
                if (!CopyByValue_Gen.UnPack(buff, 0, out e))
                {
                    throw new Exception("unpack fail for UnityEngine.UI.InputField.LineType");
                }
				val = (UnityEngine.UI.InputField.LineType)e;
                
            }
            else
            {
                val = (UnityEngine.UI.InputField.LineType)thiz.objectCasters.GetCaster(typeof(UnityEngine.UI.InputField.LineType))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineUIInputFieldLineType(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.UI.InputField.LineType val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineUIInputFieldLineType_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.UI.InputField.LineType");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  (int)val))
                {
                    throw new Exception("pack fail for UnityEngine.UI.InputField.LineType ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineTextAnchor_TypeID = -1;
		static int UnityEngineTextAnchor_EnumRef = -1;
        
        public static void PushUnityEngineTextAnchor(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.TextAnchor val)
        {
            if (UnityEngineTextAnchor_TypeID == -1)
            {
			    bool is_first;
                UnityEngineTextAnchor_TypeID = thiz.getTypeId(L, typeof(UnityEngine.TextAnchor), out is_first);
				
				if (UnityEngineTextAnchor_EnumRef == -1)
				{
				    Utils.LoadCSTable(L, typeof(UnityEngine.TextAnchor));
				    UnityEngineTextAnchor_EnumRef = LuaAPI.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
				}
				
            }
			
			if (LuaAPI.xlua_tryget_cachedud(L, (int)val, UnityEngineTextAnchor_EnumRef) == 1)
            {
			    return;
			}
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 4, UnityEngineTextAnchor_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, (int)val))
            {
                throw new Exception("pack fail fail for UnityEngine.TextAnchor ,value="+val);
            }
			
			LuaAPI.lua_getref(L, UnityEngineTextAnchor_EnumRef);
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.xlua_rawseti(L, -2, (int)val);
			LuaAPI.lua_pop(L, 1);
			
        }
		
        public static void GetUnityEngineTextAnchor(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.TextAnchor val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineTextAnchor_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.TextAnchor");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
				int e;
                if (!CopyByValue_Gen.UnPack(buff, 0, out e))
                {
                    throw new Exception("unpack fail for UnityEngine.TextAnchor");
                }
				val = (UnityEngine.TextAnchor)e;
                
            }
            else
            {
                val = (UnityEngine.TextAnchor)thiz.objectCasters.GetCaster(typeof(UnityEngine.TextAnchor))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineTextAnchor(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.TextAnchor val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineTextAnchor_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.TextAnchor");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  (int)val))
                {
                    throw new Exception("pack fail for UnityEngine.TextAnchor ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineTouchPhase_TypeID = -1;
		static int UnityEngineTouchPhase_EnumRef = -1;
        
        public static void PushUnityEngineTouchPhase(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.TouchPhase val)
        {
            if (UnityEngineTouchPhase_TypeID == -1)
            {
			    bool is_first;
                UnityEngineTouchPhase_TypeID = thiz.getTypeId(L, typeof(UnityEngine.TouchPhase), out is_first);
				
				if (UnityEngineTouchPhase_EnumRef == -1)
				{
				    Utils.LoadCSTable(L, typeof(UnityEngine.TouchPhase));
				    UnityEngineTouchPhase_EnumRef = LuaAPI.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
				}
				
            }
			
			if (LuaAPI.xlua_tryget_cachedud(L, (int)val, UnityEngineTouchPhase_EnumRef) == 1)
            {
			    return;
			}
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 4, UnityEngineTouchPhase_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, (int)val))
            {
                throw new Exception("pack fail fail for UnityEngine.TouchPhase ,value="+val);
            }
			
			LuaAPI.lua_getref(L, UnityEngineTouchPhase_EnumRef);
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.xlua_rawseti(L, -2, (int)val);
			LuaAPI.lua_pop(L, 1);
			
        }
		
        public static void GetUnityEngineTouchPhase(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.TouchPhase val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineTouchPhase_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.TouchPhase");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
				int e;
                if (!CopyByValue_Gen.UnPack(buff, 0, out e))
                {
                    throw new Exception("unpack fail for UnityEngine.TouchPhase");
                }
				val = (UnityEngine.TouchPhase)e;
                
            }
            else
            {
                val = (UnityEngine.TouchPhase)thiz.objectCasters.GetCaster(typeof(UnityEngine.TouchPhase))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineTouchPhase(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.TouchPhase val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineTouchPhase_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.TouchPhase");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  (int)val))
                {
                    throw new Exception("pack fail for UnityEngine.TouchPhase ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineRectTransformAxis_TypeID = -1;
		static int UnityEngineRectTransformAxis_EnumRef = -1;
        
        public static void PushUnityEngineRectTransformAxis(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.RectTransform.Axis val)
        {
            if (UnityEngineRectTransformAxis_TypeID == -1)
            {
			    bool is_first;
                UnityEngineRectTransformAxis_TypeID = thiz.getTypeId(L, typeof(UnityEngine.RectTransform.Axis), out is_first);
				
				if (UnityEngineRectTransformAxis_EnumRef == -1)
				{
				    Utils.LoadCSTable(L, typeof(UnityEngine.RectTransform.Axis));
				    UnityEngineRectTransformAxis_EnumRef = LuaAPI.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
				}
				
            }
			
			if (LuaAPI.xlua_tryget_cachedud(L, (int)val, UnityEngineRectTransformAxis_EnumRef) == 1)
            {
			    return;
			}
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 4, UnityEngineRectTransformAxis_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, (int)val))
            {
                throw new Exception("pack fail fail for UnityEngine.RectTransform.Axis ,value="+val);
            }
			
			LuaAPI.lua_getref(L, UnityEngineRectTransformAxis_EnumRef);
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.xlua_rawseti(L, -2, (int)val);
			LuaAPI.lua_pop(L, 1);
			
        }
		
        public static void GetUnityEngineRectTransformAxis(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.RectTransform.Axis val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineRectTransformAxis_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.RectTransform.Axis");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
				int e;
                if (!CopyByValue_Gen.UnPack(buff, 0, out e))
                {
                    throw new Exception("unpack fail for UnityEngine.RectTransform.Axis");
                }
				val = (UnityEngine.RectTransform.Axis)e;
                
            }
            else
            {
                val = (UnityEngine.RectTransform.Axis)thiz.objectCasters.GetCaster(typeof(UnityEngine.RectTransform.Axis))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineRectTransformAxis(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.RectTransform.Axis val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineRectTransformAxis_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.RectTransform.Axis");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  (int)val))
                {
                    throw new Exception("pack fail for UnityEngine.RectTransform.Axis ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineRectTransformEdge_TypeID = -1;
		static int UnityEngineRectTransformEdge_EnumRef = -1;
        
        public static void PushUnityEngineRectTransformEdge(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.RectTransform.Edge val)
        {
            if (UnityEngineRectTransformEdge_TypeID == -1)
            {
			    bool is_first;
                UnityEngineRectTransformEdge_TypeID = thiz.getTypeId(L, typeof(UnityEngine.RectTransform.Edge), out is_first);
				
				if (UnityEngineRectTransformEdge_EnumRef == -1)
				{
				    Utils.LoadCSTable(L, typeof(UnityEngine.RectTransform.Edge));
				    UnityEngineRectTransformEdge_EnumRef = LuaAPI.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
				}
				
            }
			
			if (LuaAPI.xlua_tryget_cachedud(L, (int)val, UnityEngineRectTransformEdge_EnumRef) == 1)
            {
			    return;
			}
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 4, UnityEngineRectTransformEdge_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, (int)val))
            {
                throw new Exception("pack fail fail for UnityEngine.RectTransform.Edge ,value="+val);
            }
			
			LuaAPI.lua_getref(L, UnityEngineRectTransformEdge_EnumRef);
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.xlua_rawseti(L, -2, (int)val);
			LuaAPI.lua_pop(L, 1);
			
        }
		
        public static void GetUnityEngineRectTransformEdge(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.RectTransform.Edge val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineRectTransformEdge_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.RectTransform.Edge");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
				int e;
                if (!CopyByValue_Gen.UnPack(buff, 0, out e))
                {
                    throw new Exception("unpack fail for UnityEngine.RectTransform.Edge");
                }
				val = (UnityEngine.RectTransform.Edge)e;
                
            }
            else
            {
                val = (UnityEngine.RectTransform.Edge)thiz.objectCasters.GetCaster(typeof(UnityEngine.RectTransform.Edge))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineRectTransformEdge(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.RectTransform.Edge val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineRectTransformEdge_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.RectTransform.Edge");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  (int)val))
                {
                    throw new Exception("pack fail for UnityEngine.RectTransform.Edge ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineUIScrollRectMovementType_TypeID = -1;
		static int UnityEngineUIScrollRectMovementType_EnumRef = -1;
        
        public static void PushUnityEngineUIScrollRectMovementType(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.UI.ScrollRect.MovementType val)
        {
            if (UnityEngineUIScrollRectMovementType_TypeID == -1)
            {
			    bool is_first;
                UnityEngineUIScrollRectMovementType_TypeID = thiz.getTypeId(L, typeof(UnityEngine.UI.ScrollRect.MovementType), out is_first);
				
				if (UnityEngineUIScrollRectMovementType_EnumRef == -1)
				{
				    Utils.LoadCSTable(L, typeof(UnityEngine.UI.ScrollRect.MovementType));
				    UnityEngineUIScrollRectMovementType_EnumRef = LuaAPI.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
				}
				
            }
			
			if (LuaAPI.xlua_tryget_cachedud(L, (int)val, UnityEngineUIScrollRectMovementType_EnumRef) == 1)
            {
			    return;
			}
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 4, UnityEngineUIScrollRectMovementType_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, (int)val))
            {
                throw new Exception("pack fail fail for UnityEngine.UI.ScrollRect.MovementType ,value="+val);
            }
			
			LuaAPI.lua_getref(L, UnityEngineUIScrollRectMovementType_EnumRef);
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.xlua_rawseti(L, -2, (int)val);
			LuaAPI.lua_pop(L, 1);
			
        }
		
        public static void GetUnityEngineUIScrollRectMovementType(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.UI.ScrollRect.MovementType val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineUIScrollRectMovementType_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.UI.ScrollRect.MovementType");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
				int e;
                if (!CopyByValue_Gen.UnPack(buff, 0, out e))
                {
                    throw new Exception("unpack fail for UnityEngine.UI.ScrollRect.MovementType");
                }
				val = (UnityEngine.UI.ScrollRect.MovementType)e;
                
            }
            else
            {
                val = (UnityEngine.UI.ScrollRect.MovementType)thiz.objectCasters.GetCaster(typeof(UnityEngine.UI.ScrollRect.MovementType))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineUIScrollRectMovementType(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.UI.ScrollRect.MovementType val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineUIScrollRectMovementType_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.UI.ScrollRect.MovementType");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  (int)val))
                {
                    throw new Exception("pack fail for UnityEngine.UI.ScrollRect.MovementType ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineUIScrollRectScrollbarVisibility_TypeID = -1;
		static int UnityEngineUIScrollRectScrollbarVisibility_EnumRef = -1;
        
        public static void PushUnityEngineUIScrollRectScrollbarVisibility(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.UI.ScrollRect.ScrollbarVisibility val)
        {
            if (UnityEngineUIScrollRectScrollbarVisibility_TypeID == -1)
            {
			    bool is_first;
                UnityEngineUIScrollRectScrollbarVisibility_TypeID = thiz.getTypeId(L, typeof(UnityEngine.UI.ScrollRect.ScrollbarVisibility), out is_first);
				
				if (UnityEngineUIScrollRectScrollbarVisibility_EnumRef == -1)
				{
				    Utils.LoadCSTable(L, typeof(UnityEngine.UI.ScrollRect.ScrollbarVisibility));
				    UnityEngineUIScrollRectScrollbarVisibility_EnumRef = LuaAPI.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
				}
				
            }
			
			if (LuaAPI.xlua_tryget_cachedud(L, (int)val, UnityEngineUIScrollRectScrollbarVisibility_EnumRef) == 1)
            {
			    return;
			}
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 4, UnityEngineUIScrollRectScrollbarVisibility_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, (int)val))
            {
                throw new Exception("pack fail fail for UnityEngine.UI.ScrollRect.ScrollbarVisibility ,value="+val);
            }
			
			LuaAPI.lua_getref(L, UnityEngineUIScrollRectScrollbarVisibility_EnumRef);
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.xlua_rawseti(L, -2, (int)val);
			LuaAPI.lua_pop(L, 1);
			
        }
		
        public static void GetUnityEngineUIScrollRectScrollbarVisibility(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.UI.ScrollRect.ScrollbarVisibility val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineUIScrollRectScrollbarVisibility_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.UI.ScrollRect.ScrollbarVisibility");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
				int e;
                if (!CopyByValue_Gen.UnPack(buff, 0, out e))
                {
                    throw new Exception("unpack fail for UnityEngine.UI.ScrollRect.ScrollbarVisibility");
                }
				val = (UnityEngine.UI.ScrollRect.ScrollbarVisibility)e;
                
            }
            else
            {
                val = (UnityEngine.UI.ScrollRect.ScrollbarVisibility)thiz.objectCasters.GetCaster(typeof(UnityEngine.UI.ScrollRect.ScrollbarVisibility))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineUIScrollRectScrollbarVisibility(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.UI.ScrollRect.ScrollbarVisibility val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineUIScrollRectScrollbarVisibility_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.UI.ScrollRect.ScrollbarVisibility");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  (int)val))
                {
                    throw new Exception("pack fail for UnityEngine.UI.ScrollRect.ScrollbarVisibility ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineUIImageType_TypeID = -1;
		static int UnityEngineUIImageType_EnumRef = -1;
        
        public static void PushUnityEngineUIImageType(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.UI.Image.Type val)
        {
            if (UnityEngineUIImageType_TypeID == -1)
            {
			    bool is_first;
                UnityEngineUIImageType_TypeID = thiz.getTypeId(L, typeof(UnityEngine.UI.Image.Type), out is_first);
				
				if (UnityEngineUIImageType_EnumRef == -1)
				{
				    Utils.LoadCSTable(L, typeof(UnityEngine.UI.Image.Type));
				    UnityEngineUIImageType_EnumRef = LuaAPI.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
				}
				
            }
			
			if (LuaAPI.xlua_tryget_cachedud(L, (int)val, UnityEngineUIImageType_EnumRef) == 1)
            {
			    return;
			}
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 4, UnityEngineUIImageType_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, (int)val))
            {
                throw new Exception("pack fail fail for UnityEngine.UI.Image.Type ,value="+val);
            }
			
			LuaAPI.lua_getref(L, UnityEngineUIImageType_EnumRef);
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.xlua_rawseti(L, -2, (int)val);
			LuaAPI.lua_pop(L, 1);
			
        }
		
        public static void GetUnityEngineUIImageType(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.UI.Image.Type val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineUIImageType_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.UI.Image.Type");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
				int e;
                if (!CopyByValue_Gen.UnPack(buff, 0, out e))
                {
                    throw new Exception("unpack fail for UnityEngine.UI.Image.Type");
                }
				val = (UnityEngine.UI.Image.Type)e;
                
            }
            else
            {
                val = (UnityEngine.UI.Image.Type)thiz.objectCasters.GetCaster(typeof(UnityEngine.UI.Image.Type))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineUIImageType(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.UI.Image.Type val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineUIImageType_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.UI.Image.Type");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  (int)val))
                {
                    throw new Exception("pack fail for UnityEngine.UI.Image.Type ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineUIImageFillMethod_TypeID = -1;
		static int UnityEngineUIImageFillMethod_EnumRef = -1;
        
        public static void PushUnityEngineUIImageFillMethod(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.UI.Image.FillMethod val)
        {
            if (UnityEngineUIImageFillMethod_TypeID == -1)
            {
			    bool is_first;
                UnityEngineUIImageFillMethod_TypeID = thiz.getTypeId(L, typeof(UnityEngine.UI.Image.FillMethod), out is_first);
				
				if (UnityEngineUIImageFillMethod_EnumRef == -1)
				{
				    Utils.LoadCSTable(L, typeof(UnityEngine.UI.Image.FillMethod));
				    UnityEngineUIImageFillMethod_EnumRef = LuaAPI.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
				}
				
            }
			
			if (LuaAPI.xlua_tryget_cachedud(L, (int)val, UnityEngineUIImageFillMethod_EnumRef) == 1)
            {
			    return;
			}
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 4, UnityEngineUIImageFillMethod_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, (int)val))
            {
                throw new Exception("pack fail fail for UnityEngine.UI.Image.FillMethod ,value="+val);
            }
			
			LuaAPI.lua_getref(L, UnityEngineUIImageFillMethod_EnumRef);
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.xlua_rawseti(L, -2, (int)val);
			LuaAPI.lua_pop(L, 1);
			
        }
		
        public static void GetUnityEngineUIImageFillMethod(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.UI.Image.FillMethod val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineUIImageFillMethod_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.UI.Image.FillMethod");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
				int e;
                if (!CopyByValue_Gen.UnPack(buff, 0, out e))
                {
                    throw new Exception("unpack fail for UnityEngine.UI.Image.FillMethod");
                }
				val = (UnityEngine.UI.Image.FillMethod)e;
                
            }
            else
            {
                val = (UnityEngine.UI.Image.FillMethod)thiz.objectCasters.GetCaster(typeof(UnityEngine.UI.Image.FillMethod))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineUIImageFillMethod(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.UI.Image.FillMethod val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineUIImageFillMethod_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.UI.Image.FillMethod");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  (int)val))
                {
                    throw new Exception("pack fail for UnityEngine.UI.Image.FillMethod ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineUIImageOriginHorizontal_TypeID = -1;
		static int UnityEngineUIImageOriginHorizontal_EnumRef = -1;
        
        public static void PushUnityEngineUIImageOriginHorizontal(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.UI.Image.OriginHorizontal val)
        {
            if (UnityEngineUIImageOriginHorizontal_TypeID == -1)
            {
			    bool is_first;
                UnityEngineUIImageOriginHorizontal_TypeID = thiz.getTypeId(L, typeof(UnityEngine.UI.Image.OriginHorizontal), out is_first);
				
				if (UnityEngineUIImageOriginHorizontal_EnumRef == -1)
				{
				    Utils.LoadCSTable(L, typeof(UnityEngine.UI.Image.OriginHorizontal));
				    UnityEngineUIImageOriginHorizontal_EnumRef = LuaAPI.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
				}
				
            }
			
			if (LuaAPI.xlua_tryget_cachedud(L, (int)val, UnityEngineUIImageOriginHorizontal_EnumRef) == 1)
            {
			    return;
			}
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 4, UnityEngineUIImageOriginHorizontal_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, (int)val))
            {
                throw new Exception("pack fail fail for UnityEngine.UI.Image.OriginHorizontal ,value="+val);
            }
			
			LuaAPI.lua_getref(L, UnityEngineUIImageOriginHorizontal_EnumRef);
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.xlua_rawseti(L, -2, (int)val);
			LuaAPI.lua_pop(L, 1);
			
        }
		
        public static void GetUnityEngineUIImageOriginHorizontal(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.UI.Image.OriginHorizontal val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineUIImageOriginHorizontal_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.UI.Image.OriginHorizontal");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
				int e;
                if (!CopyByValue_Gen.UnPack(buff, 0, out e))
                {
                    throw new Exception("unpack fail for UnityEngine.UI.Image.OriginHorizontal");
                }
				val = (UnityEngine.UI.Image.OriginHorizontal)e;
                
            }
            else
            {
                val = (UnityEngine.UI.Image.OriginHorizontal)thiz.objectCasters.GetCaster(typeof(UnityEngine.UI.Image.OriginHorizontal))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineUIImageOriginHorizontal(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.UI.Image.OriginHorizontal val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineUIImageOriginHorizontal_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.UI.Image.OriginHorizontal");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  (int)val))
                {
                    throw new Exception("pack fail for UnityEngine.UI.Image.OriginHorizontal ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineUIImageOriginVertical_TypeID = -1;
		static int UnityEngineUIImageOriginVertical_EnumRef = -1;
        
        public static void PushUnityEngineUIImageOriginVertical(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.UI.Image.OriginVertical val)
        {
            if (UnityEngineUIImageOriginVertical_TypeID == -1)
            {
			    bool is_first;
                UnityEngineUIImageOriginVertical_TypeID = thiz.getTypeId(L, typeof(UnityEngine.UI.Image.OriginVertical), out is_first);
				
				if (UnityEngineUIImageOriginVertical_EnumRef == -1)
				{
				    Utils.LoadCSTable(L, typeof(UnityEngine.UI.Image.OriginVertical));
				    UnityEngineUIImageOriginVertical_EnumRef = LuaAPI.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
				}
				
            }
			
			if (LuaAPI.xlua_tryget_cachedud(L, (int)val, UnityEngineUIImageOriginVertical_EnumRef) == 1)
            {
			    return;
			}
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 4, UnityEngineUIImageOriginVertical_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, (int)val))
            {
                throw new Exception("pack fail fail for UnityEngine.UI.Image.OriginVertical ,value="+val);
            }
			
			LuaAPI.lua_getref(L, UnityEngineUIImageOriginVertical_EnumRef);
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.xlua_rawseti(L, -2, (int)val);
			LuaAPI.lua_pop(L, 1);
			
        }
		
        public static void GetUnityEngineUIImageOriginVertical(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.UI.Image.OriginVertical val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineUIImageOriginVertical_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.UI.Image.OriginVertical");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
				int e;
                if (!CopyByValue_Gen.UnPack(buff, 0, out e))
                {
                    throw new Exception("unpack fail for UnityEngine.UI.Image.OriginVertical");
                }
				val = (UnityEngine.UI.Image.OriginVertical)e;
                
            }
            else
            {
                val = (UnityEngine.UI.Image.OriginVertical)thiz.objectCasters.GetCaster(typeof(UnityEngine.UI.Image.OriginVertical))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineUIImageOriginVertical(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.UI.Image.OriginVertical val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineUIImageOriginVertical_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.UI.Image.OriginVertical");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  (int)val))
                {
                    throw new Exception("pack fail for UnityEngine.UI.Image.OriginVertical ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineUIImageOrigin90_TypeID = -1;
		static int UnityEngineUIImageOrigin90_EnumRef = -1;
        
        public static void PushUnityEngineUIImageOrigin90(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.UI.Image.Origin90 val)
        {
            if (UnityEngineUIImageOrigin90_TypeID == -1)
            {
			    bool is_first;
                UnityEngineUIImageOrigin90_TypeID = thiz.getTypeId(L, typeof(UnityEngine.UI.Image.Origin90), out is_first);
				
				if (UnityEngineUIImageOrigin90_EnumRef == -1)
				{
				    Utils.LoadCSTable(L, typeof(UnityEngine.UI.Image.Origin90));
				    UnityEngineUIImageOrigin90_EnumRef = LuaAPI.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
				}
				
            }
			
			if (LuaAPI.xlua_tryget_cachedud(L, (int)val, UnityEngineUIImageOrigin90_EnumRef) == 1)
            {
			    return;
			}
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 4, UnityEngineUIImageOrigin90_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, (int)val))
            {
                throw new Exception("pack fail fail for UnityEngine.UI.Image.Origin90 ,value="+val);
            }
			
			LuaAPI.lua_getref(L, UnityEngineUIImageOrigin90_EnumRef);
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.xlua_rawseti(L, -2, (int)val);
			LuaAPI.lua_pop(L, 1);
			
        }
		
        public static void GetUnityEngineUIImageOrigin90(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.UI.Image.Origin90 val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineUIImageOrigin90_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.UI.Image.Origin90");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
				int e;
                if (!CopyByValue_Gen.UnPack(buff, 0, out e))
                {
                    throw new Exception("unpack fail for UnityEngine.UI.Image.Origin90");
                }
				val = (UnityEngine.UI.Image.Origin90)e;
                
            }
            else
            {
                val = (UnityEngine.UI.Image.Origin90)thiz.objectCasters.GetCaster(typeof(UnityEngine.UI.Image.Origin90))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineUIImageOrigin90(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.UI.Image.Origin90 val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineUIImageOrigin90_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.UI.Image.Origin90");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  (int)val))
                {
                    throw new Exception("pack fail for UnityEngine.UI.Image.Origin90 ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineUIImageOrigin180_TypeID = -1;
		static int UnityEngineUIImageOrigin180_EnumRef = -1;
        
        public static void PushUnityEngineUIImageOrigin180(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.UI.Image.Origin180 val)
        {
            if (UnityEngineUIImageOrigin180_TypeID == -1)
            {
			    bool is_first;
                UnityEngineUIImageOrigin180_TypeID = thiz.getTypeId(L, typeof(UnityEngine.UI.Image.Origin180), out is_first);
				
				if (UnityEngineUIImageOrigin180_EnumRef == -1)
				{
				    Utils.LoadCSTable(L, typeof(UnityEngine.UI.Image.Origin180));
				    UnityEngineUIImageOrigin180_EnumRef = LuaAPI.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
				}
				
            }
			
			if (LuaAPI.xlua_tryget_cachedud(L, (int)val, UnityEngineUIImageOrigin180_EnumRef) == 1)
            {
			    return;
			}
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 4, UnityEngineUIImageOrigin180_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, (int)val))
            {
                throw new Exception("pack fail fail for UnityEngine.UI.Image.Origin180 ,value="+val);
            }
			
			LuaAPI.lua_getref(L, UnityEngineUIImageOrigin180_EnumRef);
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.xlua_rawseti(L, -2, (int)val);
			LuaAPI.lua_pop(L, 1);
			
        }
		
        public static void GetUnityEngineUIImageOrigin180(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.UI.Image.Origin180 val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineUIImageOrigin180_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.UI.Image.Origin180");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
				int e;
                if (!CopyByValue_Gen.UnPack(buff, 0, out e))
                {
                    throw new Exception("unpack fail for UnityEngine.UI.Image.Origin180");
                }
				val = (UnityEngine.UI.Image.Origin180)e;
                
            }
            else
            {
                val = (UnityEngine.UI.Image.Origin180)thiz.objectCasters.GetCaster(typeof(UnityEngine.UI.Image.Origin180))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineUIImageOrigin180(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.UI.Image.Origin180 val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineUIImageOrigin180_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.UI.Image.Origin180");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  (int)val))
                {
                    throw new Exception("pack fail for UnityEngine.UI.Image.Origin180 ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineUIImageOrigin360_TypeID = -1;
		static int UnityEngineUIImageOrigin360_EnumRef = -1;
        
        public static void PushUnityEngineUIImageOrigin360(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.UI.Image.Origin360 val)
        {
            if (UnityEngineUIImageOrigin360_TypeID == -1)
            {
			    bool is_first;
                UnityEngineUIImageOrigin360_TypeID = thiz.getTypeId(L, typeof(UnityEngine.UI.Image.Origin360), out is_first);
				
				if (UnityEngineUIImageOrigin360_EnumRef == -1)
				{
				    Utils.LoadCSTable(L, typeof(UnityEngine.UI.Image.Origin360));
				    UnityEngineUIImageOrigin360_EnumRef = LuaAPI.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
				}
				
            }
			
			if (LuaAPI.xlua_tryget_cachedud(L, (int)val, UnityEngineUIImageOrigin360_EnumRef) == 1)
            {
			    return;
			}
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 4, UnityEngineUIImageOrigin360_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, (int)val))
            {
                throw new Exception("pack fail fail for UnityEngine.UI.Image.Origin360 ,value="+val);
            }
			
			LuaAPI.lua_getref(L, UnityEngineUIImageOrigin360_EnumRef);
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.xlua_rawseti(L, -2, (int)val);
			LuaAPI.lua_pop(L, 1);
			
        }
		
        public static void GetUnityEngineUIImageOrigin360(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.UI.Image.Origin360 val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineUIImageOrigin360_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.UI.Image.Origin360");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
				int e;
                if (!CopyByValue_Gen.UnPack(buff, 0, out e))
                {
                    throw new Exception("unpack fail for UnityEngine.UI.Image.Origin360");
                }
				val = (UnityEngine.UI.Image.Origin360)e;
                
            }
            else
            {
                val = (UnityEngine.UI.Image.Origin360)thiz.objectCasters.GetCaster(typeof(UnityEngine.UI.Image.Origin360))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineUIImageOrigin360(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.UI.Image.Origin360 val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineUIImageOrigin360_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.UI.Image.Origin360");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  (int)val))
                {
                    throw new Exception("pack fail for UnityEngine.UI.Image.Origin360 ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineCameraFieldOfViewAxis_TypeID = -1;
		static int UnityEngineCameraFieldOfViewAxis_EnumRef = -1;
        
        public static void PushUnityEngineCameraFieldOfViewAxis(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.Camera.FieldOfViewAxis val)
        {
            if (UnityEngineCameraFieldOfViewAxis_TypeID == -1)
            {
			    bool is_first;
                UnityEngineCameraFieldOfViewAxis_TypeID = thiz.getTypeId(L, typeof(UnityEngine.Camera.FieldOfViewAxis), out is_first);
				
				if (UnityEngineCameraFieldOfViewAxis_EnumRef == -1)
				{
				    Utils.LoadCSTable(L, typeof(UnityEngine.Camera.FieldOfViewAxis));
				    UnityEngineCameraFieldOfViewAxis_EnumRef = LuaAPI.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
				}
				
            }
			
			if (LuaAPI.xlua_tryget_cachedud(L, (int)val, UnityEngineCameraFieldOfViewAxis_EnumRef) == 1)
            {
			    return;
			}
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 4, UnityEngineCameraFieldOfViewAxis_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, (int)val))
            {
                throw new Exception("pack fail fail for UnityEngine.Camera.FieldOfViewAxis ,value="+val);
            }
			
			LuaAPI.lua_getref(L, UnityEngineCameraFieldOfViewAxis_EnumRef);
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.xlua_rawseti(L, -2, (int)val);
			LuaAPI.lua_pop(L, 1);
			
        }
		
        public static void GetUnityEngineCameraFieldOfViewAxis(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.Camera.FieldOfViewAxis val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineCameraFieldOfViewAxis_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Camera.FieldOfViewAxis");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
				int e;
                if (!CopyByValue_Gen.UnPack(buff, 0, out e))
                {
                    throw new Exception("unpack fail for UnityEngine.Camera.FieldOfViewAxis");
                }
				val = (UnityEngine.Camera.FieldOfViewAxis)e;
                
            }
            else
            {
                val = (UnityEngine.Camera.FieldOfViewAxis)thiz.objectCasters.GetCaster(typeof(UnityEngine.Camera.FieldOfViewAxis))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineCameraFieldOfViewAxis(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.Camera.FieldOfViewAxis val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineCameraFieldOfViewAxis_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Camera.FieldOfViewAxis");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  (int)val))
                {
                    throw new Exception("pack fail for UnityEngine.Camera.FieldOfViewAxis ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineCameraMonoOrStereoscopicEye_TypeID = -1;
		static int UnityEngineCameraMonoOrStereoscopicEye_EnumRef = -1;
        
        public static void PushUnityEngineCameraMonoOrStereoscopicEye(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.Camera.MonoOrStereoscopicEye val)
        {
            if (UnityEngineCameraMonoOrStereoscopicEye_TypeID == -1)
            {
			    bool is_first;
                UnityEngineCameraMonoOrStereoscopicEye_TypeID = thiz.getTypeId(L, typeof(UnityEngine.Camera.MonoOrStereoscopicEye), out is_first);
				
				if (UnityEngineCameraMonoOrStereoscopicEye_EnumRef == -1)
				{
				    Utils.LoadCSTable(L, typeof(UnityEngine.Camera.MonoOrStereoscopicEye));
				    UnityEngineCameraMonoOrStereoscopicEye_EnumRef = LuaAPI.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
				}
				
            }
			
			if (LuaAPI.xlua_tryget_cachedud(L, (int)val, UnityEngineCameraMonoOrStereoscopicEye_EnumRef) == 1)
            {
			    return;
			}
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 4, UnityEngineCameraMonoOrStereoscopicEye_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, (int)val))
            {
                throw new Exception("pack fail fail for UnityEngine.Camera.MonoOrStereoscopicEye ,value="+val);
            }
			
			LuaAPI.lua_getref(L, UnityEngineCameraMonoOrStereoscopicEye_EnumRef);
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.xlua_rawseti(L, -2, (int)val);
			LuaAPI.lua_pop(L, 1);
			
        }
		
        public static void GetUnityEngineCameraMonoOrStereoscopicEye(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.Camera.MonoOrStereoscopicEye val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineCameraMonoOrStereoscopicEye_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Camera.MonoOrStereoscopicEye");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
				int e;
                if (!CopyByValue_Gen.UnPack(buff, 0, out e))
                {
                    throw new Exception("unpack fail for UnityEngine.Camera.MonoOrStereoscopicEye");
                }
				val = (UnityEngine.Camera.MonoOrStereoscopicEye)e;
                
            }
            else
            {
                val = (UnityEngine.Camera.MonoOrStereoscopicEye)thiz.objectCasters.GetCaster(typeof(UnityEngine.Camera.MonoOrStereoscopicEye))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineCameraMonoOrStereoscopicEye(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.Camera.MonoOrStereoscopicEye val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineCameraMonoOrStereoscopicEye_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Camera.MonoOrStereoscopicEye");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  (int)val))
                {
                    throw new Exception("pack fail for UnityEngine.Camera.MonoOrStereoscopicEye ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineCameraStereoscopicEye_TypeID = -1;
		static int UnityEngineCameraStereoscopicEye_EnumRef = -1;
        
        public static void PushUnityEngineCameraStereoscopicEye(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.Camera.StereoscopicEye val)
        {
            if (UnityEngineCameraStereoscopicEye_TypeID == -1)
            {
			    bool is_first;
                UnityEngineCameraStereoscopicEye_TypeID = thiz.getTypeId(L, typeof(UnityEngine.Camera.StereoscopicEye), out is_first);
				
				if (UnityEngineCameraStereoscopicEye_EnumRef == -1)
				{
				    Utils.LoadCSTable(L, typeof(UnityEngine.Camera.StereoscopicEye));
				    UnityEngineCameraStereoscopicEye_EnumRef = LuaAPI.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
				}
				
            }
			
			if (LuaAPI.xlua_tryget_cachedud(L, (int)val, UnityEngineCameraStereoscopicEye_EnumRef) == 1)
            {
			    return;
			}
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 4, UnityEngineCameraStereoscopicEye_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, (int)val))
            {
                throw new Exception("pack fail fail for UnityEngine.Camera.StereoscopicEye ,value="+val);
            }
			
			LuaAPI.lua_getref(L, UnityEngineCameraStereoscopicEye_EnumRef);
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.xlua_rawseti(L, -2, (int)val);
			LuaAPI.lua_pop(L, 1);
			
        }
		
        public static void GetUnityEngineCameraStereoscopicEye(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.Camera.StereoscopicEye val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineCameraStereoscopicEye_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Camera.StereoscopicEye");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
				int e;
                if (!CopyByValue_Gen.UnPack(buff, 0, out e))
                {
                    throw new Exception("unpack fail for UnityEngine.Camera.StereoscopicEye");
                }
				val = (UnityEngine.Camera.StereoscopicEye)e;
                
            }
            else
            {
                val = (UnityEngine.Camera.StereoscopicEye)thiz.objectCasters.GetCaster(typeof(UnityEngine.Camera.StereoscopicEye))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineCameraStereoscopicEye(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.Camera.StereoscopicEye val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineCameraStereoscopicEye_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Camera.StereoscopicEye");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  (int)val))
                {
                    throw new Exception("pack fail for UnityEngine.Camera.StereoscopicEye ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineCameraGateFitMode_TypeID = -1;
		static int UnityEngineCameraGateFitMode_EnumRef = -1;
        
        public static void PushUnityEngineCameraGateFitMode(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.Camera.GateFitMode val)
        {
            if (UnityEngineCameraGateFitMode_TypeID == -1)
            {
			    bool is_first;
                UnityEngineCameraGateFitMode_TypeID = thiz.getTypeId(L, typeof(UnityEngine.Camera.GateFitMode), out is_first);
				
				if (UnityEngineCameraGateFitMode_EnumRef == -1)
				{
				    Utils.LoadCSTable(L, typeof(UnityEngine.Camera.GateFitMode));
				    UnityEngineCameraGateFitMode_EnumRef = LuaAPI.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
				}
				
            }
			
			if (LuaAPI.xlua_tryget_cachedud(L, (int)val, UnityEngineCameraGateFitMode_EnumRef) == 1)
            {
			    return;
			}
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 4, UnityEngineCameraGateFitMode_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, (int)val))
            {
                throw new Exception("pack fail fail for UnityEngine.Camera.GateFitMode ,value="+val);
            }
			
			LuaAPI.lua_getref(L, UnityEngineCameraGateFitMode_EnumRef);
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.xlua_rawseti(L, -2, (int)val);
			LuaAPI.lua_pop(L, 1);
			
        }
		
        public static void GetUnityEngineCameraGateFitMode(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.Camera.GateFitMode val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineCameraGateFitMode_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Camera.GateFitMode");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
				int e;
                if (!CopyByValue_Gen.UnPack(buff, 0, out e))
                {
                    throw new Exception("unpack fail for UnityEngine.Camera.GateFitMode");
                }
				val = (UnityEngine.Camera.GateFitMode)e;
                
            }
            else
            {
                val = (UnityEngine.Camera.GateFitMode)thiz.objectCasters.GetCaster(typeof(UnityEngine.Camera.GateFitMode))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineCameraGateFitMode(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.Camera.GateFitMode val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineCameraGateFitMode_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Camera.GateFitMode");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  (int)val))
                {
                    throw new Exception("pack fail for UnityEngine.Camera.GateFitMode ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineTexture2DEXRFlags_TypeID = -1;
		static int UnityEngineTexture2DEXRFlags_EnumRef = -1;
        
        public static void PushUnityEngineTexture2DEXRFlags(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.Texture2D.EXRFlags val)
        {
            if (UnityEngineTexture2DEXRFlags_TypeID == -1)
            {
			    bool is_first;
                UnityEngineTexture2DEXRFlags_TypeID = thiz.getTypeId(L, typeof(UnityEngine.Texture2D.EXRFlags), out is_first);
				
				if (UnityEngineTexture2DEXRFlags_EnumRef == -1)
				{
				    Utils.LoadCSTable(L, typeof(UnityEngine.Texture2D.EXRFlags));
				    UnityEngineTexture2DEXRFlags_EnumRef = LuaAPI.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
				}
				
            }
			
			if (LuaAPI.xlua_tryget_cachedud(L, (int)val, UnityEngineTexture2DEXRFlags_EnumRef) == 1)
            {
			    return;
			}
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 4, UnityEngineTexture2DEXRFlags_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, (int)val))
            {
                throw new Exception("pack fail fail for UnityEngine.Texture2D.EXRFlags ,value="+val);
            }
			
			LuaAPI.lua_getref(L, UnityEngineTexture2DEXRFlags_EnumRef);
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.xlua_rawseti(L, -2, (int)val);
			LuaAPI.lua_pop(L, 1);
			
        }
		
        public static void GetUnityEngineTexture2DEXRFlags(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.Texture2D.EXRFlags val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineTexture2DEXRFlags_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Texture2D.EXRFlags");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
				int e;
                if (!CopyByValue_Gen.UnPack(buff, 0, out e))
                {
                    throw new Exception("unpack fail for UnityEngine.Texture2D.EXRFlags");
                }
				val = (UnityEngine.Texture2D.EXRFlags)e;
                
            }
            else
            {
                val = (UnityEngine.Texture2D.EXRFlags)thiz.objectCasters.GetCaster(typeof(UnityEngine.Texture2D.EXRFlags))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineTexture2DEXRFlags(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.Texture2D.EXRFlags val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineTexture2DEXRFlags_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.Texture2D.EXRFlags");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  (int)val))
                {
                    throw new Exception("pack fail for UnityEngine.Texture2D.EXRFlags ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        static int UnityEngineKeyCode_TypeID = -1;
		static int UnityEngineKeyCode_EnumRef = -1;
        
        public static void PushUnityEngineKeyCode(this ObjectTranslator thiz, RealStatePtr L, UnityEngine.KeyCode val)
        {
            if (UnityEngineKeyCode_TypeID == -1)
            {
			    bool is_first;
                UnityEngineKeyCode_TypeID = thiz.getTypeId(L, typeof(UnityEngine.KeyCode), out is_first);
				
				if (UnityEngineKeyCode_EnumRef == -1)
				{
				    Utils.LoadCSTable(L, typeof(UnityEngine.KeyCode));
				    UnityEngineKeyCode_EnumRef = LuaAPI.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
				}
				
            }
			
			if (LuaAPI.xlua_tryget_cachedud(L, (int)val, UnityEngineKeyCode_EnumRef) == 1)
            {
			    return;
			}
			
            IntPtr buff = LuaAPI.xlua_pushstruct(L, 4, UnityEngineKeyCode_TypeID);
            if (!CopyByValue_Gen.Pack(buff, 0, (int)val))
            {
                throw new Exception("pack fail fail for UnityEngine.KeyCode ,value="+val);
            }
			
			LuaAPI.lua_getref(L, UnityEngineKeyCode_EnumRef);
			LuaAPI.lua_pushvalue(L, -2);
			LuaAPI.xlua_rawseti(L, -2, (int)val);
			LuaAPI.lua_pop(L, 1);
			
        }
		
        public static void GetUnityEngineKeyCode(this ObjectTranslator thiz, RealStatePtr L, int index, out UnityEngine.KeyCode val)
        {
		    LuaTypes type = LuaAPI.lua_type(L, index);
            if (type == LuaTypes.LUA_TUSERDATA )
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineKeyCode_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.KeyCode");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
				int e;
                if (!CopyByValue_Gen.UnPack(buff, 0, out e))
                {
                    throw new Exception("unpack fail for UnityEngine.KeyCode");
                }
				val = (UnityEngine.KeyCode)e;
                
            }
            else
            {
                val = (UnityEngine.KeyCode)thiz.objectCasters.GetCaster(typeof(UnityEngine.KeyCode))(L, index, null);
            }
        }
		
        public static void UpdateUnityEngineKeyCode(this ObjectTranslator thiz, RealStatePtr L, int index, UnityEngine.KeyCode val)
        {
		    
            if (LuaAPI.lua_type(L, index) == LuaTypes.LUA_TUSERDATA)
            {
			    if (LuaAPI.xlua_gettypeid(L, index) != UnityEngineKeyCode_TypeID)
				{
				    throw new Exception("invalid userdata for UnityEngine.KeyCode");
				}
				
                IntPtr buff = LuaAPI.lua_touserdata(L, index);
                if (!CopyByValue_Gen.Pack(buff, 0,  (int)val))
                {
                    throw new Exception("pack fail for UnityEngine.KeyCode ,value="+val);
                }
            }
			
            else
            {
                throw new Exception("try to update a data with lua type:" + LuaAPI.lua_type(L, index));
            }
        }
        
        
		// table cast optimze
		
        
    }
	
	public partial class StaticLuaCallbacks_Wrap
    {
	    internal static bool __tryArrayGet(Type type, RealStatePtr L, ObjectTranslator translator, object obj, int index)
		{
		
			if (type == typeof(UnityEngine.Vector2[]))
			{
			    UnityEngine.Vector2[] array = obj as UnityEngine.Vector2[];
				translator.PushUnityEngineVector2(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.Vector3[]))
			{
			    UnityEngine.Vector3[] array = obj as UnityEngine.Vector3[];
				translator.PushUnityEngineVector3(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.Vector4[]))
			{
			    UnityEngine.Vector4[] array = obj as UnityEngine.Vector4[];
				translator.PushUnityEngineVector4(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.Color[]))
			{
			    UnityEngine.Color[] array = obj as UnityEngine.Color[];
				translator.PushUnityEngineColor(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.Quaternion[]))
			{
			    UnityEngine.Quaternion[] array = obj as UnityEngine.Quaternion[];
				translator.PushUnityEngineQuaternion(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.Ray[]))
			{
			    UnityEngine.Ray[] array = obj as UnityEngine.Ray[];
				translator.PushUnityEngineRay(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.Bounds[]))
			{
			    UnityEngine.Bounds[] array = obj as UnityEngine.Bounds[];
				translator.PushUnityEngineBounds(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.Ray2D[]))
			{
			    UnityEngine.Ray2D[] array = obj as UnityEngine.Ray2D[];
				translator.PushUnityEngineRay2D(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.UI.InputField.ContentType[]))
			{
			    UnityEngine.UI.InputField.ContentType[] array = obj as UnityEngine.UI.InputField.ContentType[];
				translator.PushUnityEngineUIInputFieldContentType(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.UI.InputField.InputType[]))
			{
			    UnityEngine.UI.InputField.InputType[] array = obj as UnityEngine.UI.InputField.InputType[];
				translator.PushUnityEngineUIInputFieldInputType(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.UI.InputField.CharacterValidation[]))
			{
			    UnityEngine.UI.InputField.CharacterValidation[] array = obj as UnityEngine.UI.InputField.CharacterValidation[];
				translator.PushUnityEngineUIInputFieldCharacterValidation(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.UI.InputField.LineType[]))
			{
			    UnityEngine.UI.InputField.LineType[] array = obj as UnityEngine.UI.InputField.LineType[];
				translator.PushUnityEngineUIInputFieldLineType(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.TextAnchor[]))
			{
			    UnityEngine.TextAnchor[] array = obj as UnityEngine.TextAnchor[];
				translator.PushUnityEngineTextAnchor(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.TouchPhase[]))
			{
			    UnityEngine.TouchPhase[] array = obj as UnityEngine.TouchPhase[];
				translator.PushUnityEngineTouchPhase(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.RectTransform.Axis[]))
			{
			    UnityEngine.RectTransform.Axis[] array = obj as UnityEngine.RectTransform.Axis[];
				translator.PushUnityEngineRectTransformAxis(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.RectTransform.Edge[]))
			{
			    UnityEngine.RectTransform.Edge[] array = obj as UnityEngine.RectTransform.Edge[];
				translator.PushUnityEngineRectTransformEdge(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.UI.ScrollRect.MovementType[]))
			{
			    UnityEngine.UI.ScrollRect.MovementType[] array = obj as UnityEngine.UI.ScrollRect.MovementType[];
				translator.PushUnityEngineUIScrollRectMovementType(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.UI.ScrollRect.ScrollbarVisibility[]))
			{
			    UnityEngine.UI.ScrollRect.ScrollbarVisibility[] array = obj as UnityEngine.UI.ScrollRect.ScrollbarVisibility[];
				translator.PushUnityEngineUIScrollRectScrollbarVisibility(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.UI.Image.Type[]))
			{
			    UnityEngine.UI.Image.Type[] array = obj as UnityEngine.UI.Image.Type[];
				translator.PushUnityEngineUIImageType(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.UI.Image.FillMethod[]))
			{
			    UnityEngine.UI.Image.FillMethod[] array = obj as UnityEngine.UI.Image.FillMethod[];
				translator.PushUnityEngineUIImageFillMethod(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.UI.Image.OriginHorizontal[]))
			{
			    UnityEngine.UI.Image.OriginHorizontal[] array = obj as UnityEngine.UI.Image.OriginHorizontal[];
				translator.PushUnityEngineUIImageOriginHorizontal(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.UI.Image.OriginVertical[]))
			{
			    UnityEngine.UI.Image.OriginVertical[] array = obj as UnityEngine.UI.Image.OriginVertical[];
				translator.PushUnityEngineUIImageOriginVertical(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.UI.Image.Origin90[]))
			{
			    UnityEngine.UI.Image.Origin90[] array = obj as UnityEngine.UI.Image.Origin90[];
				translator.PushUnityEngineUIImageOrigin90(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.UI.Image.Origin180[]))
			{
			    UnityEngine.UI.Image.Origin180[] array = obj as UnityEngine.UI.Image.Origin180[];
				translator.PushUnityEngineUIImageOrigin180(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.UI.Image.Origin360[]))
			{
			    UnityEngine.UI.Image.Origin360[] array = obj as UnityEngine.UI.Image.Origin360[];
				translator.PushUnityEngineUIImageOrigin360(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.Camera.FieldOfViewAxis[]))
			{
			    UnityEngine.Camera.FieldOfViewAxis[] array = obj as UnityEngine.Camera.FieldOfViewAxis[];
				translator.PushUnityEngineCameraFieldOfViewAxis(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.Camera.MonoOrStereoscopicEye[]))
			{
			    UnityEngine.Camera.MonoOrStereoscopicEye[] array = obj as UnityEngine.Camera.MonoOrStereoscopicEye[];
				translator.PushUnityEngineCameraMonoOrStereoscopicEye(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.Camera.StereoscopicEye[]))
			{
			    UnityEngine.Camera.StereoscopicEye[] array = obj as UnityEngine.Camera.StereoscopicEye[];
				translator.PushUnityEngineCameraStereoscopicEye(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.Camera.GateFitMode[]))
			{
			    UnityEngine.Camera.GateFitMode[] array = obj as UnityEngine.Camera.GateFitMode[];
				translator.PushUnityEngineCameraGateFitMode(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.Texture2D.EXRFlags[]))
			{
			    UnityEngine.Texture2D.EXRFlags[] array = obj as UnityEngine.Texture2D.EXRFlags[];
				translator.PushUnityEngineTexture2DEXRFlags(L, array[index]);
				return true;
			}
			else if (type == typeof(UnityEngine.KeyCode[]))
			{
			    UnityEngine.KeyCode[] array = obj as UnityEngine.KeyCode[];
				translator.PushUnityEngineKeyCode(L, array[index]);
				return true;
			}
            return false;
		}
		
		internal static bool __tryArraySet(Type type, RealStatePtr L, ObjectTranslator translator, object obj, int array_idx, int obj_idx)
		{
		
			if (type == typeof(UnityEngine.Vector2[]))
			{
			    UnityEngine.Vector2[] array = obj as UnityEngine.Vector2[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.Vector3[]))
			{
			    UnityEngine.Vector3[] array = obj as UnityEngine.Vector3[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.Vector4[]))
			{
			    UnityEngine.Vector4[] array = obj as UnityEngine.Vector4[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.Color[]))
			{
			    UnityEngine.Color[] array = obj as UnityEngine.Color[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.Quaternion[]))
			{
			    UnityEngine.Quaternion[] array = obj as UnityEngine.Quaternion[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.Ray[]))
			{
			    UnityEngine.Ray[] array = obj as UnityEngine.Ray[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.Bounds[]))
			{
			    UnityEngine.Bounds[] array = obj as UnityEngine.Bounds[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.Ray2D[]))
			{
			    UnityEngine.Ray2D[] array = obj as UnityEngine.Ray2D[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.UI.InputField.ContentType[]))
			{
			    UnityEngine.UI.InputField.ContentType[] array = obj as UnityEngine.UI.InputField.ContentType[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.UI.InputField.InputType[]))
			{
			    UnityEngine.UI.InputField.InputType[] array = obj as UnityEngine.UI.InputField.InputType[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.UI.InputField.CharacterValidation[]))
			{
			    UnityEngine.UI.InputField.CharacterValidation[] array = obj as UnityEngine.UI.InputField.CharacterValidation[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.UI.InputField.LineType[]))
			{
			    UnityEngine.UI.InputField.LineType[] array = obj as UnityEngine.UI.InputField.LineType[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.TextAnchor[]))
			{
			    UnityEngine.TextAnchor[] array = obj as UnityEngine.TextAnchor[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.TouchPhase[]))
			{
			    UnityEngine.TouchPhase[] array = obj as UnityEngine.TouchPhase[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.RectTransform.Axis[]))
			{
			    UnityEngine.RectTransform.Axis[] array = obj as UnityEngine.RectTransform.Axis[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.RectTransform.Edge[]))
			{
			    UnityEngine.RectTransform.Edge[] array = obj as UnityEngine.RectTransform.Edge[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.UI.ScrollRect.MovementType[]))
			{
			    UnityEngine.UI.ScrollRect.MovementType[] array = obj as UnityEngine.UI.ScrollRect.MovementType[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.UI.ScrollRect.ScrollbarVisibility[]))
			{
			    UnityEngine.UI.ScrollRect.ScrollbarVisibility[] array = obj as UnityEngine.UI.ScrollRect.ScrollbarVisibility[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.UI.Image.Type[]))
			{
			    UnityEngine.UI.Image.Type[] array = obj as UnityEngine.UI.Image.Type[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.UI.Image.FillMethod[]))
			{
			    UnityEngine.UI.Image.FillMethod[] array = obj as UnityEngine.UI.Image.FillMethod[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.UI.Image.OriginHorizontal[]))
			{
			    UnityEngine.UI.Image.OriginHorizontal[] array = obj as UnityEngine.UI.Image.OriginHorizontal[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.UI.Image.OriginVertical[]))
			{
			    UnityEngine.UI.Image.OriginVertical[] array = obj as UnityEngine.UI.Image.OriginVertical[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.UI.Image.Origin90[]))
			{
			    UnityEngine.UI.Image.Origin90[] array = obj as UnityEngine.UI.Image.Origin90[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.UI.Image.Origin180[]))
			{
			    UnityEngine.UI.Image.Origin180[] array = obj as UnityEngine.UI.Image.Origin180[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.UI.Image.Origin360[]))
			{
			    UnityEngine.UI.Image.Origin360[] array = obj as UnityEngine.UI.Image.Origin360[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.Camera.FieldOfViewAxis[]))
			{
			    UnityEngine.Camera.FieldOfViewAxis[] array = obj as UnityEngine.Camera.FieldOfViewAxis[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.Camera.MonoOrStereoscopicEye[]))
			{
			    UnityEngine.Camera.MonoOrStereoscopicEye[] array = obj as UnityEngine.Camera.MonoOrStereoscopicEye[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.Camera.StereoscopicEye[]))
			{
			    UnityEngine.Camera.StereoscopicEye[] array = obj as UnityEngine.Camera.StereoscopicEye[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.Camera.GateFitMode[]))
			{
			    UnityEngine.Camera.GateFitMode[] array = obj as UnityEngine.Camera.GateFitMode[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.Texture2D.EXRFlags[]))
			{
			    UnityEngine.Texture2D.EXRFlags[] array = obj as UnityEngine.Texture2D.EXRFlags[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
			else if (type == typeof(UnityEngine.KeyCode[]))
			{
			    UnityEngine.KeyCode[] array = obj as UnityEngine.KeyCode[];
				translator.Get(L, obj_idx, out array[array_idx]);
				return true;
			}
            return false;
		}
	}
}