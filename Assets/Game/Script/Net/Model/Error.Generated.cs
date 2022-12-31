using  System.Text;
namespace ZJYFrameWork.Net
{
    public partial class Error:Model,IError
    {
        /// <summary>
        /// 出现错误的CommandID
        /// </summary>
        public int commandId { get; set; }
        /// <summary>
        /// 具体错误信息
        /// </summary>
        public string message { get; set; }
    }
    public class ErrorSerializer:Serializer
    {
        public static string ToJson(Error model)
        {
            StringBuilder builder = new StringBuilder();
            ToJson(model, builder, true, 0);
            return builder.ToString();
        }
        public static void ToJson(Error model,StringBuilder builder, bool isPretty = false,int index=0)
        {
            builder.Append("{");
            NewLine(builder,isPretty);
            Indent(builder,index+1,isPretty);
            builder.Append($"\"commandId\":{model.commandId},");
            NewLine(builder,isPretty);
            Indent(builder,index+1,isPretty);
            builder.Append($"\"message\":{model.message}");
            NewLine(builder,isPretty);
            Indent(builder,index+1,isPretty);
            builder.Append("}");

        }
    }
}