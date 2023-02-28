using System;
using Net.Https;
using UnityEditor;
using UnityEngine;
using ZJYFrameWork.Base;
using ZJYFrameWork.Constant;
using ZJYFrameWork.Net;
using ZJYFrameWork.Net.CsProtocol;
using ZJYFrameWork.Setting;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.WebRequest;

namespace ZJYFrameWork.WebRequest
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Game/Framework/NetworkWeb")]
    public class NetworkManager : SpringComponent
    {
        public UserAuth UserAuth;
        private ApiHandler _handler;
        [Autowired] private ISettingManager settingManager;

        protected override void OnAwake()
        {
            base.OnAwake();
        }

        public void Init()
        {
            Debug.Log("初始化NetworkManager");
            this.UserAuth = new UserAuth();
            this._handler = gameObject.AddComponent<ApiHandler>();
            this._handler.Setup(SpringContext.GetBean<ISettingManager>().GetHttpsBase(),
                settingManager.GetString(GameConstant.SETTING_LOGIN_TOKEN));
            this._handler.onResponseGlobal += SetAuthTokenOnce;
            this._handler.onCompleteGlobal += RequestCompleteGlobal;
            this._handler.onErrorGlobal += ErrorGlobal;
        }
        public UserFromAttributeData aUserFromAttr = new UserFromAttributeData();

        public void SetUserFromAttributeData(int channelCode, string platformId, string sdkToken)
        {
            aUserFromAttr.channelCode = channelCode;
            aUserFromAttr.platformId = platformId;
            aUserFromAttr.sdkToken = sdkToken;
            //Debug.Log("Set:" + channelCode + "\n" + platformId);
        }
        public void Reset()
        {
            if (this._handler != null)
            {
                this._handler.ResetQueue();
            }
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
            this._handler.Setup(SpringContext.GetBean<ISettingManager>().GetHttpsBase(), UserAuth.AuthToken);
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