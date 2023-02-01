#if CSHARP_7_OR_LATER
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace BestHTTP
{
    public sealed class AsyncHTTPException : Exception
    {
        /// <summary>
        /// Content sent by the server.
        /// </summary>
        public string Content;

        /// <summary>
        /// Status code of the server's response.
        /// </summary>
        public int StatusCode;

        public AsyncHTTPException(string message)
            : base(message)
        {
        }

        public AsyncHTTPException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public AsyncHTTPException(int statusCode, string message, string content)
            : base(message)
        {
            this.StatusCode = statusCode;
            this.Content = content;
        }

        public override string ToString()
        {
            return
                $"StatusCode: {this.StatusCode}, Message: {this.Message}, Content: {this.Content}, StackTrace: {this.StackTrace}";
        }
    }

    public static class HTTPRequestAsyncExtensions
    {
        public static Task<HttpResponse> GetHTTPResponseAsync(this HttpRequest request,
            CancellationToken token = default)
        {
            return CreateTask<HttpResponse>(request, token, (req, resp, tcs) =>
            {
                switch (req.State)
                {
                    // 请求顺利完成。
                    case HttpRequestStates.Finished:
                    {
                        tcs.TrySetResult(resp);
                    }
                        break;

                    // 请求结束时出现意外错误。请求的Exception属性可能包含有关错误的更多信息。
                    case HttpRequestStates.Error:
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        StringBuilder sb = new StringBuilder(10);
                        sb.Append($"Request Finished with Error! ");
                        sb.Append((req.Exception != null
                            ? ($"{req.Exception.Message}\n{req.Exception.StackTrace}")
                            : "No Exception"));
                        Debug.Log(
                            $"[HTTPRequestAsyncExtensions] [method: GetHTTPResponseAsync] [msg|Exception] {sb.ToString()}");
#endif
                        tcs.TrySetException(CreateException("No Exception", null, req.Exception));
                    }
                        break;

                    // 由用户发起的请求中止。
                    case HttpRequestStates.Aborted:
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        Debug.Log(
                            $"[HTTPRequestAsyncExtensions] [method: GetHTTPResponseAsync] [msg|Exception] 请求被中止！");
#endif

                        tcs.TrySetCanceled();
                        break;

                    // 连接服务器超时。处理步骤
                    case HttpRequestStates.ConnectionTimedOut:
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        Debug.Log(
                            $"[HTTPRequestAsyncExtensions] [method: GetHTTPResponseAsync] [msg|Exception] 连接超时");
#endif

                        tcs.TrySetException(CreateException("连接超时!"));
                        break;

                    // 请求没有在规定的时间内完成。
                    case HttpRequestStates.TimedOut:
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        Debug.Log(
                            $"[HTTPRequestAsyncExtensions] [method: GetHTTPResponseAsync] [msg|Exception] 处理请求超时！");
#endif

                        tcs.TrySetException(CreateException("处理请求超时!"));
                        break;
                }
            });
        }

        public static Task<string> GetAsStringAsync(this HttpRequest request, CancellationToken token = default)
        {
            return CreateTask<string>(request, token, (req, resp, tcs) =>
            {
                switch (req.State)
                {
                    // 请求顺利完成。
                    case HttpRequestStates.Finished:
                        if (resp.IsSuccess)
                        {
                            tcs.TrySetResult(resp.DataAsText);
                        }
                        else
                        {
                            tcs.TrySetException(
                                CreateException("求成功完成，但是服务器发送了一个错误.", resp));
                        }

                        break;

                    // 请求结束时出现意外错误。请求的Exception属性可能包含有关错误的更多信息。
                    case HttpRequestStates.Error:
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        StringBuilder sb = new StringBuilder(10);
                        sb.Append($"Request Finished with Error! ");
                        sb.Append((req.Exception != null
                            ? ($"{req.Exception.Message}\n{req.Exception.StackTrace}")
                            : "No Exception"));
                        Debug.Log(
                            $"[HTTPRequestAsyncExtensions] [method: GetAsStringAsync] [msg|Exception] {sb.ToString()}");
#endif

                        tcs.TrySetException(CreateException("No Exception", null, req.Exception));
                    }
                        break;

                    // 由用户发起的请求中止。
                    case HttpRequestStates.Aborted:
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        Debug.Log(
                            $"[HTTPRequestAsyncExtensions] [method: GetAsStringAsync] [msg|Exception] 请求被中止！");
#endif

                        tcs.TrySetCanceled();
                        break;

                    // 连接服务器超时。处理步骤
                    case HttpRequestStates.ConnectionTimedOut:
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        Debug.Log(
                            $"[HTTPRequestAsyncExtensions] [method: GetAsStringAsync] [msg|Exception] 连接超时");
#endif
                        tcs.TrySetException(CreateException("连接超时!"));
                        break;

                    // 请求没有在规定的时间内完成。
                    case HttpRequestStates.TimedOut:
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        Debug.Log(
                            $"[HTTPRequestAsyncExtensions] [method: GetAsStringAsync] [msg|Exception] 处理请求超时！");
#endif

                        tcs.TrySetException(CreateException("处理请求超时!"));
                        break;
                }
            });
        }

        public static Task<Texture2D> GetAsTexture2DAsync(this HttpRequest request, CancellationToken token = default)
        {
            return CreateTask<Texture2D>(request, token, (req, resp, tcs) =>
            {
                switch (req.State)
                {
                    // 请求顺利完成。
                    case HttpRequestStates.Finished:
                        if (resp.IsSuccess)
                        {
                            tcs.TrySetResult(resp.DataAsTexture2D);
                        }
                        else
                        {
                            tcs.TrySetException(
                                CreateException("求成功完成，但是服务器发送了一个错误.", resp));
                        }

                        break;

                    // 请求结束时出现意外错误。请求的Exception属性可能包含有关错误的更多信息。
                    case HttpRequestStates.Error:
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        StringBuilder sb = new StringBuilder(10);
                        sb.Append($"Request Finished with Error! ");
                        sb.Append((req.Exception != null
                            ? ($"{req.Exception.Message}\n{req.Exception.StackTrace}")
                            : "No Exception"));
                        Debug.Log(
                            $"[HTTPRequestAsyncExtensions] [method: GetAsTexture2DAsync] [msg|Exception] {sb.ToString()}");
#endif

                        tcs.TrySetException(CreateException("No Exception", null, req.Exception));
                    }

                        break;

                    // 由用户发起的请求中止。
                    case HttpRequestStates.Aborted:
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        Debug.Log(
                            $"[HTTPRequestAsyncExtensions] [method: GetAsTexture2DAsync] [msg|Exception] 请求被中止！");
#endif

                        tcs.TrySetCanceled();
                        break;

                    // 连接服务器超时。处理步骤
                    case HttpRequestStates.ConnectionTimedOut:
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        Debug.Log(
                            $"[HTTPRequestAsyncExtensions] [method: GetAsTexture2DAsync] [msg|Exception] 连接超时");
#endif
                        tcs.TrySetException(CreateException("连接超时!"));
                        break;

                    // 请求没有在规定的时间内完成。
                    case HttpRequestStates.TimedOut:
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        Debug.Log(
                            $"[HTTPRequestAsyncExtensions] [method: GetAsTexture2DAsync] [msg|Exception] 处理请求超时！");
#endif

                        tcs.TrySetException(CreateException("处理请求超时!"));
                        break;
                }
            });
        }

        public static Task<byte[]> GetRawDataAsync(this HttpRequest request, CancellationToken token = default)
        {
            return CreateTask<byte[]>(request, token, (req, resp, tcs) =>
            {
                switch (req.State)
                {
                    // 请求顺利完成。
                    case HttpRequestStates.Finished:
                    {
                        if (resp.IsSuccess)
                        {
                            tcs.TrySetResult(resp.Data);
                        }
                        else
                        {
                            tcs.TrySetException(
                                CreateException("请求成功完成，但是服务器发送了一个错误.", resp));
                        }
                    }

                        break;

                    // 请求结束时出现意外错误。请求的Exception属性可能包含有关错误的更多信息。
                    case HttpRequestStates.Error:
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        StringBuilder sb = new StringBuilder(10);
                        sb.Append($"Request Finished with Error! ");
                        sb.Append((req.Exception != null
                            ? ($"{req.Exception.Message}\n{req.Exception.StackTrace}")
                            : "No Exception"));
                        Debug.Log(
                            $"[HTTPRequestAsyncExtensions] [method: GetRawDataAsync] [msg|Exception] {sb.ToString()}");
#endif
                        tcs.TrySetException(CreateException("No Exception", null, req.Exception));
                    }
                        break;

                    // 由用户发起的请求中止。
                    case HttpRequestStates.Aborted:
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        Debug.Log(
                            $"[HTTPRequestAsyncExtensions] [method: GetRawDataAsync] [msg|Exception] 请求被中止！");
#endif
                        tcs.TrySetCanceled();
                    }
                        break;

                    // 连接服务器超时。处理步骤
                    case HttpRequestStates.ConnectionTimedOut:
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        Debug.Log(
                            $"[HTTPRequestAsyncExtensions] [method: GetRawDataAsync] [msg|Exception] 连接超时");
#endif
                        tcs.TrySetException(CreateException("连接超时!"));
                    }
                        break;

                    // 请求没有在规定的时间内完成。
                    case HttpRequestStates.TimedOut:
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        Debug.Log(
                            $"[HTTPRequestAsyncExtensions] [method: GetRawDataAsync] [msg|Exception] 处理请求超时！");
#endif
                        tcs.TrySetException(CreateException("处理请求超时!"));
                        break;
                }
            });
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static Task<T> CreateTask<T>(HttpRequest request, CancellationToken token,
            Action<HttpRequest, HttpResponse, TaskCompletionSource<T>> callback)
        {
            HttpManager.Setup();

            var tcs = new TaskCompletionSource<T>();

            request.Callback = (req, resp) =>
            {
                if (token.IsCancellationRequested)
                {
                    tcs.SetCanceled();
                }
                else
                {
                    callback(req, resp, tcs);
                }
            };

            if (token.CanBeCanceled)
            {
                token.Register((state) => (state as HttpRequest)?.Abort(), request);
            }

            if (request.State == HttpRequestStates.Initial)
            {
                request.Send();
            }

            return tcs.Task;
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void VerboseLogging(HttpRequest request, string str)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log(
                $"[HTTPRequestAsyncExtensions] [method: VerboseLogging(HttpRequest request, string str)] [msg|Exception] {str}");
#endif
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static Exception CreateException(string errorMessage, HttpResponse resp = null, Exception ex = null)
        {
            if (resp != null)
            {
                return new AsyncHTTPException(resp.StatusCode, resp.Message, resp.DataAsText);
            }
            else if (ex != null)
            {
                return new AsyncHTTPException(ex.Message, ex);
            }
            else
            {
                return new AsyncHTTPException(errorMessage);
            }
        }
    }
}
#endif