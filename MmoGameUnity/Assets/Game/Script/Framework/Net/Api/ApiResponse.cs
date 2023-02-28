using System;
using System.Collections.Generic;
using BestHTTP;

namespace ZJYFrameWork.Net
{
    public class ApiResponse
    {
        internal HttpResponse bhResponse;

        public ApiResponse(ApiRequest request, HttpResponse bhResponse, long elapsedMilliseconds)
        {
            this.Request = request;
            this.bhResponse = bhResponse;
            this.ElapsedMilliseconds = elapsedMilliseconds;

            ValidateResponse(request._bhRequest, bhResponse);

            if (bhResponse == null)
            {
                this.StatusCode = 0;
                this.StatusMessage = "Timeout";
                this.Headers = new Dictionary<string, List<string>>();
                this.RawData = Array.Empty<byte>();
            }
            else
            {
                this.StatusCode = bhResponse.StatusCode;
                this.StatusMessage = bhResponse.Message;
                this.Headers = bhResponse.Headers;

                Debug.Log(bhResponse.Message);
                Debug.Log(bhResponse.DataAsText);
                this.RawData = bhResponse.Data;
            }
        }

        public ApiRequest Request { get; private set; }
        public int StatusCode { get; private set; }

        public string StatusMessage { get; private set; }

        public byte[] RawData { get; private set; }

        public bool IsSuccess
        {
            get { return (StatusCode >= 200 && StatusCode < 300) || StatusCode == 304; }
        }

        public bool IsTimeout
        {
            get { return StatusCode == 0; }
        }

        public long ElapsedMilliseconds { get; private set; }

        public Dictionary<string, List<string>> Headers { get; private set; }

        private void ValidateResponse(HttpRequest bhRequest, HttpResponse bhResponse)
        {
            if (bhResponse != null)
            {
                string contentType = bhResponse.GetFirstHeaderValue("content-type");
                // if (contentType != "application/msgpack")
                // {
                //     throw new Exception("Invalid Content Type: " + contentType);
                // }
            }
        }
    }
}