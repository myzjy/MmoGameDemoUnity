namespace BestHTTP.Timings
{
    public static class TimingEventNames
    {
        public const string Queued = "Queued";
        public const string QueuedForRedirection = "Queued for redirection";
        public const string DnsLookup = "DNS Lookup";
        public const string TcpConnection = "TCP Connection";
        public const string ProxyNegotiation = "Proxy Negotiation";
        public const string TlsNegotiation = "TLS Negotiation";
        public const string RequestSent = "Request Sent";
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once InconsistentNaming
        public const string WaitingTTFB = "Waiting (TTFB)";
        public const string Headers = "Headers";
        public const string LoadingFromCache = "Loading from Cache";
        public const string WritingToCache = "Writing to Cache";
        public const string ResponseReceived = "Response Received";
        public const string QueuedForDispatch = "Queued for Dispatch";
        public const string Finished = "Finished in";
        public const string Callback = "Callback";
    }
}
