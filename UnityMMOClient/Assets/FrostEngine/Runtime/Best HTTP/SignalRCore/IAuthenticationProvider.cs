#if !BESTHTTP_DISABLE_SIGNALR_CORE

using System;
using JetBrains.Annotations;

namespace BestHTTP.SignalRCore
{
    public delegate void OnAuthenticationSucceedDelegate(IAuthenticationProvider provider);

    public delegate void OnAuthenticationFailedDelegate(IAuthenticationProvider provider, string reason);

    public interface IAuthenticationProvider
    {
        /// <summary>
        /// 必须在构建SignalR协议的任何请求之前运行身份验证
        /// </summary>
        bool IsPreAuthRequired { get; }

        /// <summary>
        /// 当预身份验证成功时，必须调用此事件。当IsPreAuthRequired为false时，没有人订阅此事件。
        /// </summary>
        [UsedImplicitly]
        event OnAuthenticationSucceedDelegate OnAuthenticationSucceed;

        /// <summary>
        /// 当预身份验证失败时，必须调用此事件。当IsPreAuthRequired为false时，没有人订阅此事件。
        /// </summary>
        [UsedImplicitly]
        event OnAuthenticationFailedDelegate OnAuthenticationFailed;

        /// <summary>
        /// 这个函数在SignalR协商开始之前调用一次。如果IsPreAuthRequired为false，则跳过此步骤。
        /// </summary>
        void StartAuthentication();

        /// <summary>
        /// 该函数将在发送每个请求之前被调用。
        /// </summary>
        void PrepareRequest(HttpRequest request);

        /// <summary>
        /// 这个函数可以自定义给定的uri。如果不打算修改uri，则该函数应返回参数。
        /// </summary>
        Uri PrepareUri(Uri uri);

        /// <summary>
        /// 取消任何正在进行的认证。
        /// </summary>
        void Cancel();
    }
}
#endif