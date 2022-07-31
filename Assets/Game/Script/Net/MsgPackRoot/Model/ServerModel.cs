using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace ZJYFrameWork.Net
{
    public abstract class SeverModel
    {
        protected abstract string ToJson();

        public override string ToString()
        {
            return ToJson();
        }

        public virtual byte[] Pack()
        {
            throw new NotImplementedException($"未为{GetType().FullName}类实现方法");
        }

        public virtual void Unpack(byte[] bytes)
        {
            // System.Text.Encoding.UTF8.GetString(bytes);
            // List<byte> readByte = new List<byte>();
            // using (MemoryStream stream = new MemoryStream(bytes))
            // {
            //     var i = stream.ReadByte();
            //     char str = Convert.ToChar(i);
            //     Debug.Log(str);
            //     switch (str)
            //     {
            //         case '{':
            //         case '}':
            //         case '"':
            //         case ':':
            //         case ',':
            //             break;
            //         default:
            //             readByte.AddRange(System.Text.Encoding.UTF8.GetBytes(str.ToString()));
            //             break;
            //     }
            //
            //     System.Text.Encoding.ASCII.GetString(a);
            // }

            throw new NotImplementedException($" 未为{GetType().FullName}类实现 Unpack(byte[] bytes)");
        }

        public virtual string BuildQuery()
        {
            throw new NotImplementedException($"未为{GetType().FullName}类实现 BuildQuery");
        }
    }
}