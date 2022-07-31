using System.Text;
using Newtonsoft.Json;

namespace ZJYFrameWork.Net
{
    public  partial class MessageError:SeverModel
    {
        /// <summary>
        /// 错误Code
        /// </summary>
        public int code { get; set; }
        
        /// <summary>
        /// 时间
        /// </summary>
        public long time { get; set; }
        /// <summary>
        /// 错误的信息具体内容
        /// </summary>
        public Error date { get; set; }
        protected override string ToJson()
        {
            return MessageErrorSerializer.ToJson(this);
        }
        public override void Unpack(byte[] bytes)
        {
            MessageErrorSerializer.Unpack(this,bytes);
        }
    }

    public class MessageErrorSerializer:Serializer
    {
        public static void Unpack(MessageError model,byte[] bytes)
        {
            var json=  System.Text.Encoding.UTF8.GetString(bytes);
            model=  JsonConvert.DeserializeObject<MessageError>(json);
          
        }
        public static MessageError Unpack(byte[] bytes)
        {
            MessageError model = new MessageError();
            var json=  System.Text.Encoding.UTF8.GetString(bytes);
            model=  JsonConvert.DeserializeObject<MessageError>(json);
            return model;
        }
        
        public static string ToJson(MessageError model)
        {
            StringBuilder builder = new StringBuilder();
            ToJson(model, builder, true, 0);
            return builder.ToString();
        }
        public static void ToJson(MessageError model,StringBuilder builder, bool isPretty = false,int index=0)
        {
            builder.Append("{");
            NewLine(builder,isPretty);
            Indent(builder,index+1,isPretty);
            builder.Append($"\"code\":{model.code},");
            NewLine(builder,isPretty);
            Indent(builder,index+1,isPretty);
            builder.Append($"\"time\":{model.code},");
            NewLine(builder,isPretty);
            Indent(builder,index+1,isPretty);
            builder.Append($"\"date\":{ToTime(model.time)},");
            NewLine(builder,isPretty);
            Indent(builder,index+1,isPretty);
            if (model.date != null)
            {
                ErrorSerializer.ToJson(model.date,builder,isPretty,index+1);
            }
            else
            {
                builder.Append($"null");
            }
            NewLine(builder,isPretty);
            Indent(builder,index+1,isPretty);
            builder.Append("}");

        }
    }
}