namespace BestHTTP.Forms
{
    public enum HttpFormUsage
    {
        /// <summary>
        /// The plugin will try to choose the best form sending method.
        /// </summary>
        Automatic,

        /// <summary>
        /// The plugin will use the Url-Encoded form sending.
        /// </summary>
        UrlEncoded,

        /// <summary>
        /// The plugin will use the Multipart form sending.
        /// </summary>
        Multipart
    }
}