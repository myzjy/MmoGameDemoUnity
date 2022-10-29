using Newtonsoft.Json;
using ZJYFrameWork.Net.CsProtocol.Buffer;

namespace ZJYFrameWork.Net
{
    public partial class RegisterServerCodeData:Model
    {
        public int code { get; set; }
        public string msg { get; set; }
        // public RegisterServerDataModel data { get; set; }
        public override string ToJson(bool isPretty = false)
        {
#if DEVELOP_BUILD&&ENABLE_LOG
            var str = JsonConvert.SerializeObject(this);
            JsonSerializer serializer = new JsonSerializer();
            TextReader tr = new StringReader(str);
            JsonTextReader jtr = new JsonTextReader(tr);
            var obj = serializer.Deserialize(jtr);
            if (obj != null)
            {
                StringWriter sw = new StringWriter();
                JsonTextWriter jsonTextWriter = new JsonTextWriter(sw)
                {
                    Formatting = Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                serializer.Serialize(jsonTextWriter, obj);
                return sw.ToString();
            } 
#endif
            return "";
        }

        public override void Unpack(byte[] bytes)
        {
            RegisterServerCodeDataResponseSerializer.Unpack(this,bytes);
        }
    }

    public class RegisterServerCodeDataResponseSerializer
    {
        public static void Unpack(RegisterServerCodeData registerServerCodeData,byte[] bytes)
        {
            var byteBuff = ByteBuffer.ValueOf();
            byteBuff.WriteBytes(bytes);
            registerServerCodeData= JsonConvert.DeserializeObject<RegisterServerCodeData>(byteBuff.ReadString());
        }
    }
}