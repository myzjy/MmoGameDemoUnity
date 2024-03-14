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
	public partial class PhysicalPowerUsePropsPhysicalPowerUserPropsResponse : Model
	{
		#region Params
		/// <summary>
		/// 返回 使用体力 所扣除 当前体力
		/// </summary>
		public short NowPhysicalPower { get; set; }

		/// <summary>
		/// 一点体力增长剩余时间
		/// </summary>
		public int ResidueTime { get; set; }

		/// <summary>
		/// 当前体力实时时间 会跟着剩余时间一起变化
		/// </summary>
		public long ResidueNowTime { get; set; }

		/// <summary>
		/// 最大体力 用于限制 这个值会随着 等级增长
		/// </summary>
		public sbyte MaximumStrength { get; set; }

		/// <summary>
		/// 我恢复到最大体力的结束时间
		/// </summary>
		public sbyte MaximusResidueEndTime { get; set; }
		#endregion

		#region Serialization
		/// <summary>
		/// JSON序列化
		/// </summary>
		public override string ToJson(bool isPretty = false)
		{
			return PhysicalPowerUsePropsPhysicalPowerUserPropsResponseSerializer.ToJson(this, isPretty);
		}

		/// <summary>
		/// 将MsgPack的二进制导入变量中
		/// </summary>
		public override void Unpack(byte[] bytes)
		{
			PhysicalPowerUsePropsPhysicalPowerUserPropsResponseSerializer.Unpack(this, bytes);
		}
		#endregion
	}

	public class PhysicalPowerUsePropsPhysicalPowerUserPropsResponseSerializer : Serializer
	{
		public static PhysicalPowerUsePropsPhysicalPowerUserPropsResponse CreateAndUnpack(ByteBuffer buffer)
		{
			PhysicalPowerUsePropsPhysicalPowerUserPropsResponse model = new PhysicalPowerUsePropsPhysicalPowerUserPropsResponse();
			Unpack(model, buffer);
			return model;
		}

		public static void Unpack(PhysicalPowerUsePropsPhysicalPowerUserPropsResponse model, byte[] bytes)
		{
			var byteBuffer = ByteBuffer.ValueOf();
			byteBuffer.WriteBytes(bytes);
			Unpack(model, byteBuffer);
		}

		public static void Unpack(PhysicalPowerUsePropsPhysicalPowerUserPropsResponse model, ByteBuffer buffer)
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
					case "nowPhysicalPower":
						if (value!=null)
						{
							 var valueListString = value.ToString();
							 model.NowPhysicalPower = short.Parse(valueListString);
						 }
						break;
					case "residueTime":
						if (value!=null)
						{
							 var valueListString = value.ToString();
							 model.ResidueTime = int.Parse(valueListString);
						 }
						break;
					case "residueNowTime":
						if (value!=null)
						{
							 var valueListString = value.ToString();
							 model.ResidueNowTime = long.Parse(valueListString);
						 }
						break;
					case "maximumStrength":
						if (value!=null)
						{
							 var valueListString = value.ToString();
							 model.MaximumStrength = sbyte.Parse(valueListString);
						 }
						break;
					case "maximusResidueEndTime":
						if (value!=null)
						{
							 var valueListString = value.ToString();
							 model.MaximusResidueEndTime = sbyte.Parse(valueListString);
						 }
						break;
					default:
						break;
				}
			}
		}

		public static string ToJson(PhysicalPowerUsePropsPhysicalPowerUserPropsResponse model, bool isPretty = false)
		{
			StringBuilder builder = new StringBuilder();
			ToJson(model, builder, isPretty, 0);
			return builder.ToString();
		}

		public static void ToJson(PhysicalPowerUsePropsPhysicalPowerUserPropsResponse model, StringBuilder builder, bool isPretty = false, int indent = 0)
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
