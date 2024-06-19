using System;
using System.IO;
using FrostEngine;
using Newtonsoft.Json;
using ZJYFrameWork.Net.CsProtocol.Buffer;

namespace ZJYFrameWork.Net
{
    public abstract class Model
    {
        /// <summary>
        /// 将MsgPack的二进制注入变量
        /// </summary>
        public virtual void Unpack(byte[] bytes)
        {
            throw new NotImplementedException("Unpack(byte[] bytes) is not Implemented for class " +
                                              GetType().FullName);
        }

        /// <summary>
        /// 转换成MsgPack格式
        /// </summary>
        public virtual byte[] Pack()
        {
            var str = JsonConvert.SerializeObject(this);
            Debug.Log(str);
            var byteBuff = ByteBuffer.ValueOf();
            byteBuff.WriteString(str);
            return byteBuff.ToBytes();
        }

        /// <summary>
        /// 生成查询参数
        /// </summary>
        public virtual string BuildQuery()
        {
            throw new NotImplementedException("BuildQuery is not Implemented for class " + GetType().FullName);
        }

        public virtual void Update<T>(T model) where T : Model
        {
            throw new NotImplementedException("Update(T model) is not implemented for class " + GetType().FullName);
        }
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
        /// <summary>
        /// JSON串行化
        /// </summary>
        public virtual string ToJson(bool isPretty = false)
        {
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
            else
            {
                throw new NotImplementedException("ToJson() is not Implemented for class " +
                                                  GetType().FullName);
            }
        }

        /// <summary>
        /// 文字列に変換
        /// </summary>
        public override string ToString()
        {
            return ToJson();
        }
#endif
    }
}