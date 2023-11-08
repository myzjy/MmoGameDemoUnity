/*
 * 【注意事项】本文档，是由服务器的http:generate自动生成的代码，因此请勿在此追加hash任何逻辑。
 */
using ZJYFrameWork.Net;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net
{
	public partial class UserAccountRegisterResponse : Model
	{
		#region Params
		/// <summary>
		/// 是否注册成功
		/// </summary>
		public bool MRegister { get; set; }

		/// <summary>
		/// 注册时，没有注册成功时，错误消息
		/// </summary>
		public string Error { get; set; }
		#endregion

		#region Serialization
		/// <summary>
		/// JSON序列化
		/// </summary>
		public override string ToJson(bool isPretty = false)
		{
			return UserAccountRegisterResponseSerializer.ToJson(this, isPretty);
		}

		/// <summary>
		/// 将MsgPack的二进制导入变量中
		/// </summary>
		public override void Unpack(byte[] bytes)
		{
			UserAccountRegisterResponseSerializer.Unpack(this, bytes);
		}
		#endregion
	}

	public class UserAccountRegisterResponseSerializer : Serializer
	{
		public static UserAccountRegisterResponse CreateAndUnpack(ByteBuffer buffer)
		{
			UserAccountRegisterResponse model = new UserAccountRegisterResponse();
			Unpack(model, buffer);
			return model;
		}

		public static void Unpack(UserAccountRegisterResponse model, byte[] bytes)
		{
			var byteBuffer = ByteBuffer.ValueOf();
			byteBuffer.WriteBytes(bytes);
			Unpack(model, byteBuffer);
		}

		public static void Unpack(UserAccountRegisterResponse model, ByteBuffer buffer)
		{
			var dataBuffer = buffer.ToBytes();
			if (dataBuffer.Length<1)
			{
				return;
			}
			var json = StringUtils.BytesToString(dataBuffer);
			var dict = JsonConvert.DeserializeObject<Dictionary<object, object>>(json);
			int length = dict.Count;
			foreach (var (key, value) in dict)
			{
				string keyString =  key.ToString();
				switch (keyString)
				{
					case "mRegister":
						if (value!=null)
						{
							 var valueListString = value.ToString();
							 model.MRegister = bool.Parse(valueListString);
						 }
						break;
					case "error":
						if (value!=null)
						{
							model.Error = value.ToString();
						 }
						break;
					default:
						break;
				}
			}
		}

		public static string ToJson(UserAccountRegisterResponse model, bool isPretty = false)
		{
			StringBuilder builder = new StringBuilder();
			ToJson(model, builder, isPretty, 0);
			return builder.ToString();
		}

		public static void ToJson(UserAccountRegisterResponse model, StringBuilder builder, bool isPretty = false, int indent = 0)
		{
			var str = JsonConvert.SerializeObject(model);
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
			}
			else
			{
				//throw new NotImplementedException("ToJson() is not Implemented for class " + GetType().FullName);
			}
			builder.Append(serializer.ToString());

		}
	}
}
