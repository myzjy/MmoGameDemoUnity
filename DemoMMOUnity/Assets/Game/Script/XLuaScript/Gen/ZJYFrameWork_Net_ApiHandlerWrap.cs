#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLuaBase.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class ZJYFrameWorkNetApiHandlerWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(ZJYFrameWork.Net.ApiHandler);
			Utils.BeginObjectRegister(type, L, translator, 0, 6, 5, 4);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Setup", _m_Setup);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetLogger", _m_SetLogger);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LuaRequest", _m_LuaRequest);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Retry", _m_Retry);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "ResetQueue", _m_ResetQueue);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetAuthToken", _m_SetAuthToken);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "IsAuthenticated", _g_get_IsAuthenticated);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "onBeforeSendGlobal", _g_get_onBeforeSendGlobal);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "onCompleteGlobal", _g_get_onCompleteGlobal);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "onErrorGlobal", _g_get_onErrorGlobal);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "onResponseGlobal", _g_get_onResponseGlobal);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "onBeforeSendGlobal", _s_set_onBeforeSendGlobal);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "onCompleteGlobal", _s_set_onCompleteGlobal);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "onErrorGlobal", _s_set_onErrorGlobal);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "onResponseGlobal", _s_set_onResponseGlobal);
            
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 0, 0);
			
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					var gen_ret = new ZJYFrameWork.Net.ApiHandler();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to ZJYFrameWork.Net.ApiHandler constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Setup(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZJYFrameWork.Net.ApiHandler gen_to_be_invoked = (ZJYFrameWork.Net.ApiHandler)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& (LuaAPI.lua_isnil(L, 3) || LuaAPI.lua_type(L, 3) == LuaTypes.LUA_TSTRING)) 
                {
                    string _baseUri = LuaAPI.lua_tostring(L, 2);
                    string _authToken = LuaAPI.lua_tostring(L, 3);
                    
                        var gen_ret = gen_to_be_invoked.Setup( _baseUri, _authToken );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)) 
                {
                    string _baseUri = LuaAPI.lua_tostring(L, 2);
                    
                        var gen_ret = gen_to_be_invoked.Setup( _baseUri );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to ZJYFrameWork.Net.ApiHandler.Setup!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetLogger(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZJYFrameWork.Net.ApiHandler gen_to_be_invoked = (ZJYFrameWork.Net.ApiHandler)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.ILogger _logger = (UnityEngine.ILogger)translator.GetObject(L, 2, typeof(UnityEngine.ILogger));
                    
                    gen_to_be_invoked.SetLogger( _logger );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LuaRequest(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZJYFrameWork.Net.ApiHandler gen_to_be_invoked = (ZJYFrameWork.Net.ApiHandler)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.LuaRequest(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Retry(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZJYFrameWork.Net.ApiHandler gen_to_be_invoked = (ZJYFrameWork.Net.ApiHandler)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    ZJYFrameWork.Net.ApiRequest _request = (ZJYFrameWork.Net.ApiRequest)translator.GetObject(L, 2, typeof(ZJYFrameWork.Net.ApiRequest));
                    
                    gen_to_be_invoked.Retry( _request );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ResetQueue(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZJYFrameWork.Net.ApiHandler gen_to_be_invoked = (ZJYFrameWork.Net.ApiHandler)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.ResetQueue(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetAuthToken(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZJYFrameWork.Net.ApiHandler gen_to_be_invoked = (ZJYFrameWork.Net.ApiHandler)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _authToken = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.SetAuthToken( _authToken );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_IsAuthenticated(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZJYFrameWork.Net.ApiHandler gen_to_be_invoked = (ZJYFrameWork.Net.ApiHandler)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.IsAuthenticated);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_onBeforeSendGlobal(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZJYFrameWork.Net.ApiHandler gen_to_be_invoked = (ZJYFrameWork.Net.ApiHandler)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.onBeforeSendGlobal);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_onCompleteGlobal(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZJYFrameWork.Net.ApiHandler gen_to_be_invoked = (ZJYFrameWork.Net.ApiHandler)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.onCompleteGlobal);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_onErrorGlobal(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZJYFrameWork.Net.ApiHandler gen_to_be_invoked = (ZJYFrameWork.Net.ApiHandler)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.onErrorGlobal);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_onResponseGlobal(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZJYFrameWork.Net.ApiHandler gen_to_be_invoked = (ZJYFrameWork.Net.ApiHandler)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.onResponseGlobal);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_onBeforeSendGlobal(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZJYFrameWork.Net.ApiHandler gen_to_be_invoked = (ZJYFrameWork.Net.ApiHandler)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.onBeforeSendGlobal = translator.GetDelegate<ZJYFrameWork.Net.ApiHandler.OnBeforeSendDelegate>(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_onCompleteGlobal(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZJYFrameWork.Net.ApiHandler gen_to_be_invoked = (ZJYFrameWork.Net.ApiHandler)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.onCompleteGlobal = translator.GetDelegate<ZJYFrameWork.Net.ApiHandler.OnCompleteDelegate>(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_onErrorGlobal(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZJYFrameWork.Net.ApiHandler gen_to_be_invoked = (ZJYFrameWork.Net.ApiHandler)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.onErrorGlobal = translator.GetDelegate<ZJYFrameWork.Net.ApiHandler.OnErrorDelegate>(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_onResponseGlobal(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZJYFrameWork.Net.ApiHandler gen_to_be_invoked = (ZJYFrameWork.Net.ApiHandler)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.onResponseGlobal = translator.GetDelegate<ZJYFrameWork.Net.ApiHandler.OnResponseDelegate>(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
