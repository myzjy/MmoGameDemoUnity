#if !BESTHTTP_DISABLE_SIGNALR_CORE
using System;
using System.Collections.Generic;
using System.Text;

namespace BestHTTP.SignalRCore.Messages
{
    public sealed class SupportedTransport
    {
        internal SupportedTransport(string transportName, List<string> transferFormats)
        {
            this.Name = transportName;
            this.SupportedFormats = transferFormats;
        }

        /// <summary>
        /// Name of the transport.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Supported transfer formats of the transport.
        /// </summary>
        public List<string> SupportedFormats { get; private set; }
    }

    /// <summary>
    /// Negotiation result of the /negotiation request.
    /// <see cref="https://github.com/dotnet/aspnetcore/blob/master/src/SignalR/docs/specs/TransportProtocols.md#post-endpoint-basenegotiate-request"/>
    /// </summary>
    public sealed class NegotiationResult
    {
        public int NegotiateVersion { get; private set; }

        /// <summary>
        /// The connectionToken which is required by the Long Polling and Server-Sent Events transports (in order to correlate sends and receives).
        /// </summary>
        public string ConnectionToken { get; private set; }

        /// <summary>
        /// The connectionId which is required by the Long Polling and Server-Sent Events transports (in order to correlate sends and receives).
        /// </summary>
        public string ConnectionId { get; private set; }

        /// <summary>
        /// The availableTransports list which describes the transports the server supports. For each transport, the name of the transport (transport) is listed, as is a list of "transfer formats" supported by the transport (transferFormats)
        /// </summary>
        public List<SupportedTransport> SupportedTransports { get; private set; }

        /// <summary>
        /// The url which is the URL the client should connect to.
        /// </summary>
        public Uri Url { get; private set; }


        /// <summary>
        /// The accessToken which is an optional bearer token for accessing the specified url.
        /// </summary>
        public string AccessToken { get; private set; }

        public HttpResponse NegotiationResponse { get; internal set; }

        internal static NegotiationResult Parse(HttpResponse resp, out string error, HubConnection hub)
        {
            error = null;

            Dictionary<string, object> response =
                BestHTTP.JSON.Json.Decode(resp.DataAsText) as Dictionary<string, object>;
            if (response == null)
            {
                error = "Json decoding failed!";
                return null;
            }

            try
            {
                NegotiationResult result = new NegotiationResult();
                result.NegotiationResponse = resp;

                object value;

                if (response.TryGetValue("negotiateVersion", out value))
                {
                    int version;
                    if (int.TryParse(value.ToString(), out version))
                        result.NegotiateVersion = version;
                }

                if (response.TryGetValue("connectionId", out value))
                    result.ConnectionId = value.ToString();

                if (response.TryGetValue("connectionToken", out value))
                    result.ConnectionToken = value.ToString();

                if (response.TryGetValue("availableTransports", out value))
                {
                    List<object> transports = value as List<object>;
                    if (transports != null)
                    {
                        List<SupportedTransport> supportedTransports = new List<SupportedTransport>(transports.Count);

                        foreach (Dictionary<string, object> transport in transports)
                        {
                            string transportName = string.Empty;
                            List<string> transferModes = null;

                            if (transport.TryGetValue("transport", out value))
                                transportName = value.ToString();

                            if (transport.TryGetValue("transferFormats", out value))
                            {
                                List<object> transferFormats = value as List<object>;

                                if (transferFormats != null)
                                {
                                    transferModes = new List<string>(transferFormats.Count);
                                    foreach (var mode in transferFormats)
                                        transferModes.Add(mode.ToString());
                                }
                            }

                            supportedTransports.Add(new SupportedTransport(transportName, transferModes));
                        }

                        result.SupportedTransports = supportedTransports;
                    }
                }

                if (response.TryGetValue("url", out value))
                {
                    string uriStr = value.ToString();

                    Uri redirectUri;

                    // Here we will try to parse the received url. If TryCreate fails, we will throw an exception
                    //  as it should be able to successfully parse whole (absolute) urls (like "https://server:url/path")
                    //  and relative ones (like "/path").
                    if (!Uri.TryCreate(uriStr, UriKind.RelativeOrAbsolute, out redirectUri))
                    {
                        throw new Exception($"Couldn't parse url: '{uriStr}'");
                    }
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    StringBuilder sb = new StringBuilder();
                    sb.Append($"Parsed url({uriStr}) ");
                    sb.Append($"into uri({redirectUri}). ");
                    sb.Append($"uri.IsAbsoluteUri: {redirectUri.IsAbsoluteUri}, ");
                    sb.Append($"IsAbsolute: {IsAbsolute(uriStr)}");
                    Debug.Log(
                        $"[NegotiationResult] [method: NegotiationResult.Parse] [msg|Exception] {sb.ToString()}");
#endif

                    // 如果收到一个相对url，我们将使用中心的当前url附加到它的新路径。
                    if (!IsAbsolute(uriStr))
                    {
                        Uri oldUri = hub.Uri;
                        var builder = new UriBuilder(oldUri);

                        // ?, /
                        var pathAndQuery = uriStr.Split(new string[] { "?", "%3F", "%3f", "/", "%2F", "%2f" },
                            StringSplitOptions.RemoveEmptyEntries);

                        builder.Query = pathAndQuery.Length > 1 ? pathAndQuery[1] : string.Empty;

                        builder.Path = pathAndQuery[0];

                        redirectUri = builder.Uri;
                    }

                    result.Url = redirectUri;
                }

                if (response.TryGetValue("accessToken", out value))
                {
                    result.AccessToken = value.ToString();
                }
                else if (hub.NegotiationResult != null)
                {
                    result.AccessToken = hub.NegotiationResult.AccessToken;
                }

                return result;
            }
            catch (Exception ex)
            {
                error = $"Error while parsing result: {ex.Message} StackTrace: {ex.StackTrace} ";
                return null;
            }
        }

        private static bool IsAbsolute(string url)
        {
            // an url is absolute if contains a scheme, an authority, and a path.
            return url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                   url.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
        }
    }
}
#endif