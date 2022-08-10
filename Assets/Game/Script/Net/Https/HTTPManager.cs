using System;
using BestHTTP;
using BestHTTP.Logger;
using GameData.Net;
using UnityEngine;
using UnityEngine.Networking;
using ZJYFrameWork.Net;

namespace Net.Https
{
    public class HTTPManager : Singleton<HTTPManager>
    {
        public ServerSettings Settings { get; private set; }
        public UserAuth UserAuth;
        private ApiHandler _handler;

        protected override void Init()
        {
            base.Init();
            Settings = new ServerSettings();
            this.UserAuth = new UserAuth();
            this._handler = gameObject.AddComponent<ApiHandler>();
            this._handler.Setup(Settings.ApiHttpsBaseUrl, UserAuth.AuthToken);
            
        }

        public void Reset()
        {
            if (this._handler != null)
            {
                _handler.ResetQueue();
            }
        }
        public void ResetHost(ServerSettings.HostType hostType)
        {
            Settings.SetHost(hostType);
            UserAuth = new UserAuth();
            _handler.Setup(Settings.ApiHttpsBaseUrl, UserAuth.AuthToken);
        }

        public void Request<TRequestData, TResponseData, TError>(ApiHttp<TRequestData, TResponseData, TError> api)
            where TRequestData : Model, new()
            where TResponseData : Model, new()
            where TError : Model, IError, new()
        {
            _handler.Request(api);

        }

        private void SetAuthTokenOnce(ApiResponse response)
        {
#if DEVELOP_BUILD
            if (response.StatusCode == 401)
            {
                //请求错误
            }   
#endif
        }
    }
}