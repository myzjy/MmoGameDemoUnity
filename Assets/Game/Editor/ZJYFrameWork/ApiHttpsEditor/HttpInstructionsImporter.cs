using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameEditor.ApiHttpsEditor;
using Newtonsoft.Json;
using Serialization;
using UnityEngine;

namespace ZJYFrameWork
{
    public class HttpInstructionsImporter
    {
        private const string FullPathEndsWith = ".json.txt";

        /// <summary>
        /// 刷新文件
        /// </summary>
        /// <param name="fullPath">路径</param>
        /// <param name="assetRefresh">是否刷新资源 默认必须刷</param>
        public static void ProcessFile(string fullPath, bool assetRefresh = true)
        {
            if (!fullPath.EndsWith(FullPathEndsWith))
            {
                //传递路径不对
                Debug.Log($"路径错误：{fullPath},解析出错了！！");
                return;
            }

            //后缀名
            var VScript = ".cs";

            //将文本完全读到
            var text = File.ReadAllText(fullPath);
            var pack = Serializer.jsonSerializer.Deserialize<Instructions>(text);
            //两次去掉后缀
            var baseName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(fullPath));
            //将字符串数组组合成一个路径
            var basePath = Path.Combine(Application.dataPath,
                "Game/Editor/ZJYFrameWork/ApiHttpsEditor/Json/" + baseName);

            //通用后缀名
            var modelVScript = ".Generated.cs";
            //Request
            var Request = "Request.Generated.cs";
            var RequestVScript = "Request.cs";
            //Response
            var Respone = "Response.Generated.cs";
            var responseVScript = "Response.cs";

            //命名空间
            var namespaceName = "FZYXBuildfreely.Net";
            var managerString = "using System.Text;\nusing Newtonsoft.Json;\n";

            List<IntInstruction> list = new List<IntInstruction>();
            InstructionType nowInstructionType = InstructionType.None;
            foreach (var item in pack.InstructionsList)
            {
                var i = item;
                var intInstruction = i as IntInstruction;
                list.Add(intInstruction);
                nowInstructionType = i switch
                {
                    ModelBoolInstruction { modelBool: true } modelInstruction => InstructionType.Model,
                    ApiBoolInstruction { ApiBool: true } apiInstruction => InstructionType.Api,
                    ResponseBoolInstruction { ResponseBool: true } responseInstruction => InstructionType.Response,
                    RequestBoolInstruction { RequestBool: true } requestInstruction => InstructionType.Request,
                    _ => nowInstructionType
                };
            }

            StringBuilder stringBuilder = new StringBuilder(10);
            stringBuilder.Append(managerString);
            stringBuilder.Append($"namespace {namespaceName}\n{{");
            switch (nowInstructionType)
            {
                case InstructionType.Model:
                {
                    stringBuilder.Append($"\t\tpublic partial class {baseName}\n");
                    stringBuilder.Append("\t\t{{\n");
                    stringBuilder.Append("\t\t}}\n");
                    var nameBea = Path.Combine(Application.dataPath, $"Game/Script/Net/Model");
                    if (!Directory.Exists(nameBea))
                    {
                        Directory.CreateDirectory(nameBea);
                    }

                    var paths = $"{nameBea}/{baseName}{modelVScript}";
                    stringBuilder.Append($"}}");
                    File.WriteAllText(paths, stringBuilder.ToString());
                }

                    break;
                case InstructionType.Api:
                    break;
                case InstructionType.Request:
                    break;
                case InstructionType.Response:
                    break;
                case InstructionType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        
        }
    }
}