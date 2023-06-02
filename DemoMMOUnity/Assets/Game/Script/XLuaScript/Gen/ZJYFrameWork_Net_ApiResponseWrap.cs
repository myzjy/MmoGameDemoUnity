#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class ZJYFrameWorkNetApiResponseWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(ZJYFrameWork.Net.ApiResponse);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 8, 0);
			
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "Request", _g_get_Request);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "StatusCode", _g_get_StatusCode);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "StatusMessage", _g_get_StatusMessage);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "RawData", _g_get_RawData);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "IsSuccess", _g_get_IsSuccess);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "IsTimeout", _g_get_IsTimeout);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "ElapsedMilliseconds", _g_get_ElapsedMilliseconds);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "Headers", _g_get_Headers);
            
			
			
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
				if(LuaAPI.lua_gettop(L) == 4 && translator.Assignable<ZJYFrameWork.Net.ApiRequest>(L, 2) && translator.Assignable<BestHTTP.HttpResponse>(L, 3) && (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4) || LuaAPI.lua_isint64(L, 4)))
				{
					ZJYFrameWork.Net.ApiRequest _request = (ZJYFrameWork.Net.ApiRequest)translator.GetObject(L, 2, typeof(ZJYFrameWork.Net.ApiRequest));
					BestHTTP.HttpResponse _bhResponse = (BestHTTP.HttpResponse)translator.GetObject(L, 3, typeof(BestHTTP.HttpResponse));
					long _elapsedMilliseconds = LuaAPI.lua_toint64(L, 4);
					
					var gen_ret = new ZJYFrameWork.Net.ApiResponse(_request, _bhResponse, _elapsedMilliseconds);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to ZJYFrameWork.Net.ApiResponse constructor!");
            
        }
        
		
        
		
        
        
        
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Request(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZJYFrameWork.Net.ApiResponse gen_to_be_invoked = (ZJYFrameWork.Net.ApiResponse)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.Request);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_StatusCode(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZJYFrameWork.Net.ApiResponse gen_to_be_invoked = (ZJYFrameWork.Net.ApiResponse)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.StatusCode);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_StatusMessage(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZJYFrameWork.Net.ApiResponse gen_to_be_invoked = (ZJYFrameWork.Net.ApiResponse)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushstring(L, gen_to_be_invoked.StatusMessage);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_RawData(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZJYFrameWork.Net.ApiResponse gen_to_be_invoked = (ZJYFrameWork.Net.ApiResponse)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushstring(L, gen_to_be_invoked.RawData);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_IsSuccess(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZJYFrameWork.Net.ApiResponse gen_to_be_invoked = (ZJYFrameWork.Net.ApiResponse)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.IsSuccess);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_IsTimeout(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZJYFrameWork.Net.ApiResponse gen_to_be_invoked = (ZJYFrameWork.Net.ApiResponse)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.IsTimeout);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_ElapsedMilliseconds(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZJYFrameWork.Net.ApiResponse gen_to_be_invoked = (ZJYFrameWork.Net.ApiResponse)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushint64(L, gen_to_be_invoked.ElapsedMilliseconds);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Headers(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZJYFrameWork.Net.ApiResponse gen_to_be_invoked = (ZJYFrameWork.Net.ApiResponse)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.Headers);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
		
		
		
		
    }
}
