using System.Text;

namespace ZJYFrameWork.Net
{
    public class Serializer
    {
        private static System.DateTime UnixEpoch = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

        protected static void Indent(StringBuilder builder, int indent, bool isPretty)
        {
            if (!isPretty)
            {
                return;
            }

            for (int i = 0; i < indent; ++i)
            {
                builder.Append("   ");
            }
        }

        protected static void NewLine(StringBuilder builder, bool isPretty, int indent = 0)
        {
            if (!isPretty)
            {
                return;
            }

            builder.Append("\n");
            if (indent > 0)
            {
                Indent(builder, indent, isPretty);
            }
        }
        //
        // protected static string ToTime(long time)
        // {
        //     var timeData = GlobalDataManager.Instance.time.GetCurrEntTimeMilliseconds(time);
        //     return $"{timeData:u}";
        // }
    }
}