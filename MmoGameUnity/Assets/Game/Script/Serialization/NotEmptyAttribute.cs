using System;
using System.Collections;
using System.Diagnostics.Contracts;

namespace Serialize
{
    //不允许空数据(空字符串，空数组，0)的属性。
    public class NotEmptyAttribute : Attribute
    {
        public void Validate(object obj)
        {
            if(obj.GetType().IsArray || obj is IList)
            {
                Contract.Assert(((IList)obj).Count > 0, "NotEmpty");
            }
            else switch (obj)
            {
                case string s:
                    Contract.Assert(s.Length > 0, "NotEmpty");
                    break;
                case int i:
                    Contract.Assert(i != 0, "NotEmpty");
                    break;
            }
        }
    }
}