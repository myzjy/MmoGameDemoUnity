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
    public class FrostEngineGameEventWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(FrostEngine.GameEvent);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 5, 1, 0);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "AddEventListener", _m_AddEventListener_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RemoveEventListener", _m_RemoveEventListener_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Send", _m_Send_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Shutdown", _m_Shutdown_xlua_st_);
            
			
            
			Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "EventMgr", _g_get_EventMgr);
            
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "FrostEngine.GameEvent does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddEventListener_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& translator.Assignable<object>(L, 2)&& translator.Assignable<System.Action>(L, 3)) 
                {
                    int _eventType = LuaAPI.xlua_tointeger(L, 1);
                    object _funObj = translator.GetObject(L, 2, typeof(object));
                    System.Action _handler = translator.GetDelegate<System.Action>(L, 3);
                    
                        var gen_ret = FrostEngine.GameEvent.AddEventListener( _eventType, _funObj, _handler );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<object>(L, 2)&& translator.Assignable<System.Action>(L, 3)) 
                {
                    string _eventType = LuaAPI.lua_tostring(L, 1);
                    object _funObj = translator.GetObject(L, 2, typeof(object));
                    System.Action _handler = translator.GetDelegate<System.Action>(L, 3);
                    
                        var gen_ret = FrostEngine.GameEvent.AddEventListener( _eventType, _funObj, _handler );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to FrostEngine.GameEvent.AddEventListener!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RemoveEventListener_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& translator.Assignable<System.Action>(L, 2)) 
                {
                    int _eventType = LuaAPI.xlua_tointeger(L, 1);
                    System.Action _handler = translator.GetDelegate<System.Action>(L, 2);
                    
                    FrostEngine.GameEvent.RemoveEventListener( _eventType, _handler );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& translator.Assignable<System.Delegate>(L, 2)) 
                {
                    int _eventType = LuaAPI.xlua_tointeger(L, 1);
                    System.Delegate _handler = translator.GetDelegate<System.Delegate>(L, 2);
                    
                    FrostEngine.GameEvent.RemoveEventListener( _eventType, _handler );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Action>(L, 2)) 
                {
                    string _eventType = LuaAPI.lua_tostring(L, 1);
                    System.Action _handler = translator.GetDelegate<System.Action>(L, 2);
                    
                    FrostEngine.GameEvent.RemoveEventListener( _eventType, _handler );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Delegate>(L, 2)) 
                {
                    string _eventType = LuaAPI.lua_tostring(L, 1);
                    System.Delegate _handler = translator.GetDelegate<System.Delegate>(L, 2);
                    
                    FrostEngine.GameEvent.RemoveEventListener( _eventType, _handler );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to FrostEngine.GameEvent.RemoveEventListener!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Send_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 1&& translator.Assignable<object>(L, 1)) 
                {
                    object _eventType = translator.GetObject(L, 1, typeof(object));
                    
                    FrostEngine.GameEvent.Send( _eventType );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string _eventType = LuaAPI.lua_tostring(L, 1);
                    
                    FrostEngine.GameEvent.Send( _eventType );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& translator.Assignable<object>(L, 1)&& translator.Assignable<System.Delegate>(L, 2)) 
                {
                    object _eventType = translator.GetObject(L, 1, typeof(object));
                    System.Delegate _handler = translator.GetDelegate<System.Delegate>(L, 2);
                    
                    FrostEngine.GameEvent.Send( _eventType, _handler );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Delegate>(L, 2)) 
                {
                    string _eventType = LuaAPI.lua_tostring(L, 1);
                    System.Delegate _handler = translator.GetDelegate<System.Delegate>(L, 2);
                    
                    FrostEngine.GameEvent.Send( _eventType, _handler );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to FrostEngine.GameEvent.Send!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Shutdown_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    FrostEngine.GameEvent.Shutdown(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_EventMgr(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    translator.Push(L, FrostEngine.GameEvent.EventMgr);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
		
		
		
		
    }
}
