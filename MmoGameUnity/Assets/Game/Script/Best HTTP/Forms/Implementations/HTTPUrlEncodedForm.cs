using System;
using System.Text;

namespace BestHTTP.Forms
{
    /// <summary>
    /// 发送文本值的HTTP表单实现。
    /// </summary>
    public sealed class HttpUrlEncodedForm : HttpFormBase
    {
        private const int EscapeTreshold = 256;

        private byte[] _cachedData;

        public override void PrepareRequest(HttpRequest request)
        {
            request.SetHeader("Content-Type", "application/x-www-form-urlencoded");
        }

        public override byte[] GetData()
        {
            if (_cachedData != null && !IsChanged)
                return _cachedData;

            var sb = new StringBuilder();

            // Create a "field1=value1&field2=value2" formatted string
            for (int i = 0; i < Fields.Count; ++i)
            {
                var field = Fields[i];

                if (i > 0)
                {
                    sb.Append("&");
                }

                sb.Append(EscapeString(field.Name));
                sb.Append("=");

                if (!string.IsNullOrEmpty(field.Text) || field.Binary == null)
                {
                    sb.Append(EscapeString(field.Text));
                }
                else
                {
                    // 如果使用二进制数据强制使用此表单类型，则将从中创建base64编码的字符串。
                    sb.Append(Convert.ToBase64String(field.Binary, 0, field.Binary.Length));
                }
            }

            IsChanged = false;
            return _cachedData = Encoding.UTF8.GetBytes(sb.ToString());
        }

        public static string EscapeString(string originalString)
        {
            if (originalString.Length < EscapeTreshold)
                return Uri.EscapeDataString(originalString);
            else
            {
                int loops = originalString.Length / EscapeTreshold;
                StringBuilder sb = new StringBuilder(loops);

                for (int i = 0; i <= loops; i++)
                    sb.Append(i < loops
                        ? Uri.EscapeDataString(originalString.Substring(EscapeTreshold * i, EscapeTreshold))
                        : Uri.EscapeDataString(originalString.Substring(EscapeTreshold * i)));
                return sb.ToString();
            }
        }
    }
}