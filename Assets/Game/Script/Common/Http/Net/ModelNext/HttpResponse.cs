using System.Text;

namespace GameData.Net
{
    public class HttpResponse
    {
        private WebApiRequestHelperHander.ApiResponseData _responseData;

        public byte[] RawData
        {
            get { return _responseData.RawData; }
        }

        public int StatusCode
        {
            get { return (int) _responseData.Code; }
        }

        public string DataAssetText
        {
            get { return Encoding.UTF8.GetString(_responseData.RawData); }
        }

        public static HttpResponse Create(WebApiRequestHelperHander.ApiResponseData _api)
        {
            return new HttpResponse(_api);
        }


        public string FindFirstHeaderValue(string name)
        {
            var i = _responseData != null
                ? (_responseData.Headers != null
                   && (_responseData.Headers != null || _responseData.Headers.Count > 0)
                    ? (_responseData.Headers.ContainsKey(name) ? _responseData.Headers[name] : string.Empty)
                    : string.Empty)
                : string.Empty;

            return i;
        }

        private HttpResponse(WebApiRequestHelperHander.ApiResponseData _api)
        {
            _responseData = _api;
        }

    }
}