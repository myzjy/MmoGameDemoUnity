using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

namespace GameData.Net
{
    /// <summary>
    /// 
    /// </summary>
    public class ServericeBase
    {
        private HttpMethods _methods;

        public HttpMethods Methods
        {
            get { return _methods; }
        }

        private string m_payloadStr;

        // 我要修改 的字段名 修改类型数据格式
        public Dictionary<string, object> _playdict = new Dictionary<string, object>();
        /// <summary>
        /// 传递出去
        /// </summary>
        public Dictionary<string, string> _querydict = new Dictionary<string, string>();
        
        public bool NeedsRetry{ get{ return Methods != HttpMethods.Get; } }


        /// <summary>
        /// 初始化base
        /// </summary>
        /// <param name="methods"></param>
        public ServericeBase(HttpMethods methods)
        {
            _methods = methods;
            if (_methods == HttpMethods.Get)
            {
                _playdict = new Dictionary<string, object>();
            }

            if (_methods == HttpMethods.Post)
            {
                _querydict = new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// 设置传递时的Get
        /// </summary>
        /// <returns></returns>
        public static ServericeBase ServericeBaseGet()
        {
            return new ServericeBase(HttpMethods.Get);
        }

        /// <summary>
        /// 设置 传递时的post
        /// </summary>
        /// <returns></returns>
        public static ServericeBase ServericeBasePost()
        {
            return new ServericeBase(HttpMethods.Post);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="strV"></param>
        /// <param name="value"></param>
        public void AddPayload(string strV, object value)
        {
            if (_methods == HttpMethods.Get)
            {
                AddQuery(strV, (string) value);
                return;
            }

            _playdict.Add(strV, value);
        }

        private void AddQuery(string key, string value)
        {
            if (_querydict == null)
            {
                _querydict = new Dictionary<string, string>();
            }

            _querydict.Add(key, value);
        }

        private void SetPayLoad<T>(T obj)
        {
            if (_methods != HttpMethods.Get)
            {
                Debug.Assert(_playdict.Count==0,"not dictionary");
                m_payloadStr = JsonConvert.SerializeObject(obj);
            }
            else
            {
                var dict = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)
                    .ToDictionary(file => file.Name, file => file.GetValue(obj));
                SetPayload(dict);
            }
        }
        public void SetPayload(Dictionary<string,object> v ){
            if( _methods != HttpMethods.Get ){
                _playdict = v;
                return;
            }

            foreach( var pair in v ){
                AddQuery( pair.Key , (string)pair.Value );
            }
        }
        public void SetupRequestOperation(RequestOperationBase request)
        {
            request.PayloadStr = m_payloadStr;

            request.QueryParam = _querydict;
            request.Payload = _playdict;
            request.NeedsToMiniJsonParser = false;
        }
    }
}