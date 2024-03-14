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
	public partial class UserAccountLoginResponse : Model
	{
		#region Params
		/// <summary>
		/// 用户名字
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// 金币
		/// </summary>
		public long GoldNum { get; set; }

		/// <summary>
		/// 普通钻石 由付费钻石转换成普通钻石，比例为 1:1
		/// </summary>
		public long DiamondNum { get; set; }

		/// <summary>
		/// 付费钻石 一般充值才有，付费钻石转换成普通钻石
		/// </summary>
		public long PremiumDiamondNum { get; set; }

		/// <summary>
		/// 玩家Token
		/// </summary>
		public string Token { get; set; }

		/// <summary>
		/// 玩家UID
		/// </summary>
		public long Uid { get; set; }

		/// <summary>
		/// 用户等级
		/// </summary>
		public int Lv { get; set; }

		/// <summary>
		/// 用户最大等级
		/// </summary>
		public int MaxLv { get; set; }

		/// <summary>
		/// 用户当前等级经验
		/// </summary>
		public int NowExp { get; set; }

		/// <summary>
		/// 用户当前等级最大经验
		/// </summary>
		public int MaxExp { get; set; }

		/// <summary>
		/// 当前体力
		/// </summary>
		public int NowPhysicalPowerNum { get; set; }
		#endregion

		#region Serialization
		/// <summary>
		/// JSON序列化
		/// </summary>
		public override string ToJson(bool isPretty = false)
		{
			return UserAccountLoginResponseSerializer.ToJson(this, isPretty);
		}

		/// <summary>
		/// 将MsgPack的二进制导入变量中
		/// </summary>
		public override void Unpack(byte[] bytes)
		{
			UserAccountLoginResponseSerializer.Unpack(this, bytes);
		}
		#endregion
	}

	public class UserAccountLoginResponseSerializer : Serializer
	{
		public static UserAccountLoginResponse CreateAndUnpack(ByteBuffer buffer)
		{
			UserAccountLoginResponse model = new UserAccountLoginResponse();
			Unpack(model, buffer);
			return model;
		}

		public static void Unpack(UserAccountLoginResponse model, byte[] bytes)
		{
			var byteBuffer = ByteBuffer.ValueOf();
			byteBuffer.WriteBytes(bytes);
			Unpack(model, byteBuffer);
		}

		public static void Unpack(UserAccountLoginResponse model, ByteBuffer buffer)
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
					case "userName":
						if (value!=null)
						{
							model.UserName = value.ToString();
						 }
						break;
					case "goldNum":
						if (value!=null)
						{
							 var valueListString = value.ToString();
							 model.GoldNum = long.Parse(valueListString);
						 }
						break;
					case "diamondNum":
						if (value!=null)
						{
							 var valueListString = value.ToString();
							 model.DiamondNum = long.Parse(valueListString);
						 }
						break;
					case "premiumDiamondNum":
						if (value!=null)
						{
							 var valueListString = value.ToString();
							 model.PremiumDiamondNum = long.Parse(valueListString);
						 }
						break;
					case "token":
						if (value!=null)
						{
							model.Token = value.ToString();
						 }
						break;
					case "uid":
						if (value!=null)
						{
							 var valueListString = value.ToString();
							 model.Uid = long.Parse(valueListString);
						 }
						break;
					case "lv":
						if (value!=null)
						{
							 var valueListString = value.ToString();
							 model.Lv = int.Parse(valueListString);
						 }
						break;
					case "maxLv":
						if (value!=null)
						{
							 var valueListString = value.ToString();
							 model.MaxLv = int.Parse(valueListString);
						 }
						break;
					case "nowExp":
						if (value!=null)
						{
							 var valueListString = value.ToString();
							 model.NowExp = int.Parse(valueListString);
						 }
						break;
					case "maxExp":
						if (value!=null)
						{
							 var valueListString = value.ToString();
							 model.MaxExp = int.Parse(valueListString);
						 }
						break;
					case "nowPhysicalPowerNum":
						if (value!=null)
						{
							 var valueListString = value.ToString();
							 model.NowPhysicalPowerNum = int.Parse(valueListString);
						 }
						break;
					default:
						break;
				}
			}
		}

		public static string ToJson(UserAccountLoginResponse model, bool isPretty = false)
		{
			StringBuilder builder = new StringBuilder();
			ToJson(model, builder, isPretty, 0);
			return builder.ToString();
		}

		public static void ToJson(UserAccountLoginResponse model, StringBuilder builder, bool isPretty = false, int indent = 0)
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
