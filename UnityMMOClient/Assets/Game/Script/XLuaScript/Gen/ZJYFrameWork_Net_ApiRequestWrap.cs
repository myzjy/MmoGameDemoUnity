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
    public class ZJYFrameWorkNetApiRequestWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(ZJYFrameWork.Net.ApiRequest);
			Utils.BeginObjectRegister(type, L, translator, 0, 4, 2, 0);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Send", _m_Send);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Abort", _m_Abort);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetHeader", _m_SetHeader);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "DumpHeaders", _m_DumpHeaders);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "Uri", _g_get_Uri);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "Method", _g_get_Method);
            
			
			
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
				if(LuaAPI.lua_gettop(L) == 8 && translator.Assignable<BestHTTP.HttpMethods>(L, 2) && translator.Assignable<System.Uri>(L, 3) && (LuaAPI.lua_isnil(L, 4) || LuaAPI.lua_type(L, 4) == LuaTypes.LUA_TSTRING) && translator.Assignable<System.Action<ZJYFrameWork.Net.ApiRequest>>(L, 5) && translator.Assignable<System.Action<ZJYFrameWork.Net.ApiResponse>>(L, 6) && translator.Assignable<System.Action<ZJYFrameWork.Net.ApiResponse>>(L, 7) && translator.Assignable<System.Action<ZJYFrameWork.Net.ApiResponse>>(L, 8))
				{
					BestHTTP.HttpMethods _method;translator.Get(L, 2, out _method);
					System.Uri _uri = (System.Uri)translator.GetObject(L, 3, typeof(System.Uri));
					byte[] _data = LuaAPI.lua_tobytes(L, 4);
					System.Action<ZJYFrameWork.Net.ApiRequest> _onBeforeSend = translator.GetDelegate<System.Action<ZJYFrameWork.Net.ApiRequest>>(L, 5);
					System.Action<ZJYFrameWork.Net.ApiResponse> _onSuccess = translator.GetDelegate<System.Action<ZJYFrameWork.Net.ApiResponse>>(L, 6);
					System.Action<ZJYFrameWork.Net.ApiResponse> _onError = translator.GetDelegate<System.Action<ZJYFrameWork.Net.ApiResponse>>(L, 7);
					System.Action<ZJYFrameWork.Net.ApiResponse> _onComplete = translator.GetDelegate<System.Action<ZJYFrameWork.Net.ApiResponse>>(L, 8);
					
					var gen_ret = new ZJYFrameWork.Net.ApiRequest(_method, _uri, _data, _onBeforeSend, _onSuccess, _onError, _onComplete);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				if(LuaAPI.lua_gettop(L) == 7 && translator.Assignable<BestHTTP.HttpMethods>(L, 2) && translator.Assignable<System.Uri>(L, 3) && (LuaAPI.lua_isnil(L, 4) || LuaAPI.lua_type(L, 4) == LuaTypes.LUA_TSTRING) && translator.Assignable<System.Action<ZJYFrameWork.Net.ApiRequest>>(L, 5) && translator.Assignable<System.Action<ZJYFrameWork.Net.ApiResponse>>(L, 6) && translator.Assignable<System.Action<ZJYFrameWork.Net.ApiResponse>>(L, 7))
				{
					BestHTTP.HttpMethods _method;translator.Get(L, 2, out _method);
					System.Uri _uri = (System.Uri)translator.GetObject(L, 3, typeof(System.Uri));
					byte[] _data = LuaAPI.lua_tobytes(L, 4);
					System.Action<ZJYFrameWork.Net.ApiRequest> _onBeforeSend = translator.GetDelegate<System.Action<ZJYFrameWork.Net.ApiRequest>>(L, 5);
					System.Action<ZJYFrameWork.Net.ApiResponse> _onSuccess = translator.GetDelegate<System.Action<ZJYFrameWork.Net.ApiResponse>>(L, 6);
					System.Action<ZJYFrameWork.Net.ApiResponse> _onError = translator.GetDelegate<System.Action<ZJYFrameWork.Net.ApiResponse>>(L, 7);
					
					var gen_ret = new ZJYFrameWork.Net.ApiRequest(_method, _uri, _data, _onBeforeSend, _onSuccess, _onError);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				if(LuaAPI.lua_gettop(L) == 6 && translator.Assignable<BestHTTP.HttpMethods>(L, 2) && translator.Assignable<System.Uri>(L, 3) && (LuaAPI.lua_isnil(L, 4) || LuaAPI.lua_type(L, 4) == LuaTypes.LUA_TSTRING) && translator.Assignable<System.Action<ZJYFrameWork.Net.ApiRequest>>(L, 5) && translator.Assignable<System.Action<ZJYFrameWork.Net.ApiResponse>>(L, 6))
				{
					BestHTTP.HttpMethods _method;translator.Get(L, 2, out _method);
					System.Uri _uri = (System.Uri)translator.GetObject(L, 3, typeof(System.Uri));
					byte[] _data = LuaAPI.lua_tobytes(L, 4);
					System.Action<ZJYFrameWork.Net.ApiRequest> _onBeforeSend = translator.GetDelegate<System.Action<ZJYFrameWork.Net.ApiRequest>>(L, 5);
					System.Action<ZJYFrameWork.Net.ApiResponse> _onSuccess = translator.GetDelegate<System.Action<ZJYFrameWork.Net.ApiResponse>>(L, 6);
					
					var gen_ret = new ZJYFrameWork.Net.ApiRequest(_method, _uri, _data, _onBeforeSend, _onSuccess);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				if(LuaAPI.lua_gettop(L) == 5 && translator.Assignable<BestHTTP.HttpMethods>(L, 2) && translator.Assignable<System.Uri>(L, 3) && (LuaAPI.lua_isnil(L, 4) || LuaAPI.lua_type(L, 4) == LuaTypes.LUA_TSTRING) && translator.Assignable<System.Action<ZJYFrameWork.Net.ApiRequest>>(L, 5))
				{
					BestHTTP.HttpMethods _method;translator.Get(L, 2, out _method);
					System.Uri _uri = (System.Uri)translator.GetObject(L, 3, typeof(System.Uri));
					byte[] _data = LuaAPI.lua_tobytes(L, 4);
					System.Action<ZJYFrameWork.Net.ApiRequest> _onBeforeSend = translator.GetDelegate<System.Action<ZJYFrameWork.Net.ApiRequest>>(L, 5);
					
					var gen_ret = new ZJYFrameWork.Net.ApiRequest(_method, _uri, _data, _onBeforeSend);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				if(LuaAPI.lua_gettop(L) == 4 && translator.Assignable<BestHTTP.HttpMethods>(L, 2) && translator.Assignable<System.Uri>(L, 3) && (LuaAPI.lua_isnil(L, 4) || LuaAPI.lua_type(L, 4) == LuaTypes.LUA_TSTRING))
				{
					BestHTTP.HttpMethods _method;translator.Get(L, 2, out _method);
					System.Uri _uri = (System.Uri)translator.GetObject(L, 3, typeof(System.Uri));
					byte[] _data = LuaAPI.lua_tobytes(L, 4);
					
					var gen_ret = new ZJYFrameWork.Net.ApiRequest(_method, _uri, _data);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				if(LuaAPI.lua_gettop(L) == 3 && translator.Assignable<BestHTTP.HttpMethods>(L, 2) && translator.Assignable<System.Uri>(L, 3))
				{
					BestHTTP.HttpMethods _method;translator.Get(L, 2, out _method);
					System.Uri _uri = (System.Uri)translator.GetObject(L, 3, typeof(System.Uri));
					
					var gen_ret = new ZJYFrameWork.Net.ApiRequest(_method, _uri);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				if(LuaAPI.lua_gettop(L) == 8 && translator.Assignable<BestHTTP.HttpMethods>(L, 2) && (LuaAPI.lua_isnil(L, 3) || LuaAPI.lua_type(L, 3) == LuaTypes.LUA_TSTRING) && (LuaAPI.lua_isnil(L, 4) || LuaAPI.lua_type(L, 4) == LuaTypes.LUA_TSTRING) && translator.Assignable<System.Action<ZJYFrameWork.Net.ApiRequest>>(L, 5) && translator.Assignable<System.Action<ZJYFrameWork.Net.ApiResponse>>(L, 6) && translator.Assignable<System.Action<ZJYFrameWork.Net.ApiResponse>>(L, 7) && translator.Assignable<System.Action<ZJYFrameWork.Net.ApiResponse>>(L, 8))
				{
					BestHTTP.HttpMethods _method;translator.Get(L, 2, out _method);
					string _uri = LuaAPI.lua_tostring(L, 3);
					byte[] _data = LuaAPI.lua_tobytes(L, 4);
					System.Action<ZJYFrameWork.Net.ApiRequest> _onBeforeSend = translator.GetDelegate<System.Action<ZJYFrameWork.Net.ApiRequest>>(L, 5);
					System.Action<ZJYFrameWork.Net.ApiResponse> _onSuccess = translator.GetDelegate<System.Action<ZJYFrameWork.Net.ApiResponse>>(L, 6);
					System.Action<ZJYFrameWork.Net.ApiResponse> _onError = translator.GetDelegate<System.Action<ZJYFrameWork.Net.ApiResponse>>(L, 7);
					System.Action<ZJYFrameWork.Net.ApiResponse> _onComplete = translator.GetDelegate<System.Action<ZJYFrameWork.Net.ApiResponse>>(L, 8);
					
					var gen_ret = new ZJYFrameWork.Net.ApiRequest(_method, _uri, _data, _onBeforeSend, _onSuccess, _onError, _onComplete);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				if(LuaAPI.lua_gettop(L) == 7 && translator.Assignable<BestHTTP.HttpMethods>(L, 2) && (LuaAPI.lua_isnil(L, 3) || LuaAPI.lua_type(L, 3) == LuaTypes.LUA_TSTRING) && (LuaAPI.lua_isnil(L, 4) || LuaAPI.lua_type(L, 4) == LuaTypes.LUA_TSTRING) && translator.Assignable<System.Action<ZJYFrameWork.Net.ApiRequest>>(L, 5) && translator.Assignable<System.Action<ZJYFrameWork.Net.ApiResponse>>(L, 6) && translator.Assignable<System.Action<ZJYFrameWork.Net.ApiResponse>>(L, 7))
				{
					BestHTTP.HttpMethods _method;translator.Get(L, 2, out _method);
					string _uri = LuaAPI.lua_tostring(L, 3);
					byte[] _data = LuaAPI.lua_tobytes(L, 4);
					System.Action<ZJYFrameWork.Net.ApiRequest> _onBeforeSend = translator.GetDelegate<System.Action<ZJYFrameWork.Net.ApiRequest>>(L, 5);
					System.Action<ZJYFrameWork.Net.ApiResponse> _onSuccess = translator.GetDelegate<System.Action<ZJYFrameWork.Net.ApiResponse>>(L, 6);
					System.Action<ZJYFrameWork.Net.ApiResponse> _onError = translator.GetDelegate<System.Action<ZJYFrameWork.Net.ApiResponse>>(L, 7);
					
					var gen_ret = new ZJYFrameWork.Net.ApiRequest(_method, _uri, _data, _onBeforeSend, _onSuccess, _onError);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				if(LuaAPI.lua_gettop(L) == 6 && translator.Assignable<BestHTTP.HttpMethods>(L, 2) && (LuaAPI.lua_isnil(L, 3) || LuaAPI.lua_type(L, 3) == LuaTypes.LUA_TSTRING) && (LuaAPI.lua_isnil(L, 4) || LuaAPI.lua_type(L, 4) == LuaTypes.LUA_TSTRING) && translator.Assignable<System.Action<ZJYFrameWork.Net.ApiRequest>>(L, 5) && translator.Assignable<System.Action<ZJYFrameWork.Net.ApiResponse>>(L, 6))
				{
					BestHTTP.HttpMethods _method;translator.Get(L, 2, out _method);
					string _uri = LuaAPI.lua_tostring(L, 3);
					byte[] _data = LuaAPI.lua_tobytes(L, 4);
					System.Action<ZJYFrameWork.Net.ApiRequest> _onBeforeSend = translator.GetDelegate<System.Action<ZJYFrameWork.Net.ApiRequest>>(L, 5);
					System.Action<ZJYFrameWork.Net.ApiResponse> _onSuccess = translator.GetDelegate<System.Action<ZJYFrameWork.Net.ApiResponse>>(L, 6);
					
					var gen_ret = new ZJYFrameWork.Net.ApiRequest(_method, _uri, _data, _onBeforeSend, _onSuccess);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				if(LuaAPI.lua_gettop(L) == 5 && translator.Assignable<BestHTTP.HttpMethods>(L, 2) && (LuaAPI.lua_isnil(L, 3) || LuaAPI.lua_type(L, 3) == LuaTypes.LUA_TSTRING) && (LuaAPI.lua_isnil(L, 4) || LuaAPI.lua_type(L, 4) == LuaTypes.LUA_TSTRING) && translator.Assignable<System.Action<ZJYFrameWork.Net.ApiRequest>>(L, 5))
				{
					BestHTTP.HttpMethods _method;translator.Get(L, 2, out _method);
					string _uri = LuaAPI.lua_tostring(L, 3);
					byte[] _data = LuaAPI.lua_tobytes(L, 4);
					System.Action<ZJYFrameWork.Net.ApiRequest> _onBeforeSend = translator.GetDelegate<System.Action<ZJYFrameWork.Net.ApiRequest>>(L, 5);
					
					var gen_ret = new ZJYFrameWork.Net.ApiRequest(_method, _uri, _data, _onBeforeSend);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				if(LuaAPI.lua_gettop(L) == 4 && translator.Assignable<BestHTTP.HttpMethods>(L, 2) && (LuaAPI.lua_isnil(L, 3) || LuaAPI.lua_type(L, 3) == LuaTypes.LUA_TSTRING) && (LuaAPI.lua_isnil(L, 4) || LuaAPI.lua_type(L, 4) == LuaTypes.LUA_TSTRING))
				{
					BestHTTP.HttpMethods _method;translator.Get(L, 2, out _method);
					string _uri = LuaAPI.lua_tostring(L, 3);
					byte[] _data = LuaAPI.lua_tobytes(L, 4);
					
					var gen_ret = new ZJYFrameWork.Net.ApiRequest(_method, _uri, _data);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				if(LuaAPI.lua_gettop(L) == 3 && translator.Assignable<BestHTTP.HttpMethods>(L, 2) && (LuaAPI.lua_isnil(L, 3) || LuaAPI.lua_type(L, 3) == LuaTypes.LUA_TSTRING))
				{
					BestHTTP.HttpMethods _method;translator.Get(L, 2, out _method);
					string _uri = LuaAPI.lua_tostring(L, 3);
					
					var gen_ret = new ZJYFrameWork.Net.ApiRequest(_method, _uri);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to ZJYFrameWork.Net.ApiRequest constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Send(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZJYFrameWork.Net.ApiRequest gen_to_be_invoked = (ZJYFrameWork.Net.ApiRequest)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& translator.Assignable<System.Action<long, long>>(L, 2)) 
                {
                    System.Action<long, long> _onProgress = translator.GetDelegate<System.Action<long, long>>(L, 2);
                    
                    gen_to_be_invoked.Send( _onProgress );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 1) 
                {
                    
                    gen_to_be_invoked.Send(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to ZJYFrameWork.Net.ApiRequest.Send!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Abort(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZJYFrameWork.Net.ApiRequest gen_to_be_invoked = (ZJYFrameWork.Net.ApiRequest)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.Abort(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetHeader(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZJYFrameWork.Net.ApiRequest gen_to_be_invoked = (ZJYFrameWork.Net.ApiRequest)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 2);
                    string _value = LuaAPI.lua_tostring(L, 3);
                    
                    gen_to_be_invoked.SetHeader( _name, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_DumpHeaders(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ZJYFrameWork.Net.ApiRequest gen_to_be_invoked = (ZJYFrameWork.Net.ApiRequest)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        var gen_ret = gen_to_be_invoked.DumpHeaders(  );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Uri(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZJYFrameWork.Net.ApiRequest gen_to_be_invoked = (ZJYFrameWork.Net.ApiRequest)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.Uri);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Method(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ZJYFrameWork.Net.ApiRequest gen_to_be_invoked = (ZJYFrameWork.Net.ApiRequest)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.Method);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
		
		
		
		
    }
}
