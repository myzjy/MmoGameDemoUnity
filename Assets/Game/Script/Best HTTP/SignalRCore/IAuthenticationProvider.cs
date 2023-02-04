#if !BESTHTTP_DISABLE_SIGNALR_CORE

using System;

namespace BestHTTP.SignalRCore
{
    public delegate void OnAuthenticationSuccededDelegate(IAuthenticationProvider provider);

    public delegate void OnAuthenticationFailedDelegate(IAuthenticationProvider provider, string reason);

    public interface IAuthenticationProvider
    {
        /// <summary>
        /// The authentication must be run before any request made to build up the SignalR protocol
        /// </summary>
        bool IsPreAuthRequired { get; }

        /// <summary>
        /// 当预身份验证成功时，必须调用此事件。当IsPreAuthRequired为false时，没有人订阅此事件。
        /// </summary>
        event OnAuthenticationSuccededDelegate OnAuthenticationSucceded;

        /// <summary>
        /// 当预身份验证失败时，必须调用此事件。当IsPreAuthRequired为false时，没有人订阅此事件。
        /// </summary>
        event OnAuthenticationFailedDelegate OnAuthenticationFailed;

        /// <summary>
        /// This function called once, when the before the SignalR negotiation begins. If IsPreAuthRequired is false, then this step will be skipped.
        /// </summary>
        void StartAuthentication();

        /// <summary>
        /// This function will be called for every request before sending it.
        /// </summary>
        void PrepareRequest(HttpRequest request);

        /// <summary>
        /// This function can customize the given uri. If there's no intention to modify the uri, this function should return with the parameter.
        /// </summary>
        Uri PrepareUri(Uri uri);

        /// <summary>
        /// Cancel any ongoing authentication.
        /// </summary>
        void Cancel();
    }
}
#endif