using System;
using Net.Https;
using ZJYFrameWork.Net;

namespace ZJYFrameWork.UISerializable.Manager
{
    public class NetworkManager : Singleton<NetworkManager>
    {
        public ServerSettings Settings { get; private set; }
        public UserAuth UserAuth;
        private ApiHandler _handler;

        protected override void Init()
        {
            base.Init();

            this.Settings = new ServerSettings();
            this.UserAuth = new UserAuth();
            this._handler = gameObject.AddComponent<ApiHandler>();
            this._handler.Setup(Settings.ApiHttpsBaseUrl, UserAuth.AuthToken);
            this._handler.onResponseGlobal += SetAuthTokenOnce;
            this._handler.onCompleteGlobal += RequestCompleteGlobal;
            this._handler.onErrorGlobal += ErrorGlobal;
        }

        public void Reset()
        {
            if (this._handler != null)
            {
                this._handler.ResetQueue();
            }
        }

        public void ResetHost(ServerSettings.HostType hostType)
        {
            Settings.SetHost(hostType);
            UserAuth = new UserAuth();
            this._handler.Setup(Settings.ApiHttpsBaseUrl, UserAuth.AuthToken);
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
            if (!string.IsNullOrEmpty(UserAuth.AuthToken))
            {
                return;
            }

            if (response.Headers.ContainsKey("authorization"))
            {
                UserAuth.AuthToken = response.Headers["authorization"][0];
                _handler.SetAuthToken(UserAuth.AuthToken);
            }
        }

        /// <summary>
        /// 设置用户Token
        /// </summary>
        /// <param name="token"></param>
        public void SetAuthToken(string token)
        {
            UserAuth.AuthToken = token;
            this._handler.Setup(Settings.ApiHttpsBaseUrl, UserAuth.AuthToken);

        }

        private void RequestCompleteGlobal(ApiResponse response)
        {
        }

        private void ErrorGlobal(ApiResponse response, IError error)
        {
            if (response.IsTimeout)
            {
                //重新发送
                _handler.Retry(response.Request);
                return;
            }
        }
    }
}