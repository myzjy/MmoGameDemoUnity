using BestHTTP.Extensions;

namespace BestHTTP.Forms
{
    /// <summary>
    /// A HTTP Form implementation to send textual and binary values.
    /// </summary>
    public sealed class HttpMultiPartForm : HttpFormBase
    {
        public HttpMultiPartForm()
        {
            this._boundary = "BestHTTP_HTTPMultiPartForm_" + this.GetHashCode().ToString("X");
        }

        #region Private Fields

        /// <summary>
        /// A random boundary generated in the constructor.
        /// </summary>
        private string _boundary;

        /// <summary>
        ///
        /// </summary>
        private byte[] _cachedData;

        #endregion

        #region IHTTPForm Implementation

        public override void PrepareRequest(HttpRequest request)
        {
            // Set up Content-Type header for the request
            request.SetHeader("Content-Type", "multipart/form-data; boundary=" + _boundary);
        }

        public override byte[] GetData()
        {
            if (_cachedData != null)
                return _cachedData;

            using (var ms = new BufferPoolMemoryStream())
            {
                for (int i = 0; i < Fields.Count; ++i)
                {
                    HttpFieldData field = Fields[i];

                    // Set the boundary
                    ms.WriteLine("--" + _boundary);

                    // Set up Content-Disposition header to our form with the name
                    ms.WriteLine("Content-Disposition: form-data; name=\"" + field.Name + "\"" +
                                 (!string.IsNullOrEmpty(field.FileName)
                                     ? "; filename=\"" + field.FileName + "\""
                                     : string.Empty));

                    // Set up Content-Type head for the form.
                    if (!string.IsNullOrEmpty(field.MimeType))
                        ms.WriteLine("Content-Type: " + field.MimeType);

                    ms.WriteLine();

                    // Write the actual data to the MemoryStream
                    ms.Write(field.Payload, 0, field.Payload.Length);

                    ms.Write(HttpRequest.Eol, 0, HttpRequest.Eol.Length);
                }

                // Write out the trailing boundary
                ms.WriteLine("--" + _boundary + "--");

                IsChanged = false;

                // Set the RawData of our request
                return _cachedData = ms.ToArray();
            }
        }

        #endregion
    };
}