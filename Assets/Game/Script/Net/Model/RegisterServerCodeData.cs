using System.IO;
using Newtonsoft.Json;
using ZJYFrameWork.Net.CsProtocol.Buffer;

namespace ZJYFrameWork.Net
{
    public partial class RegisterServerCodeData : Model
    {
        public int code { get; set; }

        public string msg { get; set; }

        // public RegisterServerDataModel data { get; set; }


        public override void Unpack(byte[] bytes)
        {
            RegisterServerCodeDataResponseSerializer.Unpack(this, bytes);
        }
    }

    public class RegisterServerCodeDataResponseSerializer
    {
        public static void Unpack(RegisterServerCodeData registerServerCodeData, byte[] bytes)
        {
            var byteBuff = ByteBuffer.ValueOf();
            byteBuff.WriteBytes(bytes);
            registerServerCodeData = JsonConvert.DeserializeObject<RegisterServerCodeData>(byteBuff.ReadString());
        }
    }
}