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
	public partial class PhysicalPowerUsePropsPhysicalPowerUserPropsRequest : Model
	{
		#region Params
		/// <summary>
		/// 使用体力 体力多少会被扣除
		/// </summary>
		public short UsePropNum { get; set; }

		/// <summary>
		/// 玩家uid 
		/// </summary>
		public long Uid { get; set; }
		#endregion

		#region Serialization
		/// <summary>
		/// JSON序列化
		/// </summary>
		public override string ToJson(bool isPretty = false)
		{
			return PhysicalPowerUsePropsPhysicalPowerUserPropsRequestSerializer.ToJson(this, isPretty);
		}

		/// <summary>
		/// 变换成MsgPack格式
		/// </summary>
		public override byte[] Pack()
		{
			var byteBuffer = ByteBuffer.ValueOf();

			return PhysicalPowerUsePropsPhysicalPowerUserPropsRequestSerializer.Pack(this, byteBuffer);
		}

		/// <summary>
		/// 生成查询参数量
		/// </summary>
		public override string BuildQuery()
		{
			return PhysicalPowerUsePropsPhysicalPowerUserPropsRequestSerializer.BuildQuery(this);
		}

		#endregion
	}

	public class PhysicalPowerUsePropsPhysicalPowerUserPropsRequestSerializer : Serializer
	{
		public static byte[] Pack(PhysicalPowerUsePropsPhysicalPowerUserPropsRequest model,ByteBuffer buffer)
		{
			var json = JsonConvert.SerializeObject(model);
			buffer.WriteString(json);
			return buffer.ToBytes();
		}



		public static string BuildQuery(PhysicalPowerUsePropsPhysicalPowerUserPropsRequest model)
		{
			Dictionary<string, string> query = new Dictionary<string, string>();
			BuildQuery(model, query);
			List<string> parameters = new List<string>(query.Count);
			foreach (KeyValuePair<string, string> kv in query)
			{
				parameters.Add(string.Concat(Uri.EscapeDataString(kv.Key), '=', Uri.EscapeDataString(kv.Value)));
			}
			return parameters.Count > 0 ? "?" + string.Join('&', parameters.ToArray()) : "";
		}

		public static void BuildQuery(PhysicalPowerUsePropsPhysicalPowerUserPropsRequest model, Dictionary<string, string> query, string prefix = "", string suffix = "")
		{
			query.Add(prefix + "usePropNum" + suffix, model.UsePropNum.ToString());
			query.Add(prefix + "uid" + suffix, model.Uid.ToString());
		}


		public static string ToJson(PhysicalPowerUsePropsPhysicalPowerUserPropsRequest model, bool isPretty = false)
		{
			StringBuilder builder = new StringBuilder();
			ToJson(model, builder, isPretty, 0);
			return builder.ToString();
		}

		public static void ToJson(PhysicalPowerUsePropsPhysicalPowerUserPropsRequest model, StringBuilder builder, bool isPretty = false, int indent = 0)
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
