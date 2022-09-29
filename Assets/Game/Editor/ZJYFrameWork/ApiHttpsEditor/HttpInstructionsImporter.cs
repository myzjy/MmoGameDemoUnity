using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var namespaceName = "ZJYFrameWork.Net";
            var managerString = "using System.Text;\nusing Newtonsoft.Json;\n\n";

            InstructionType nowInstructionType = InstructionType.None;
            List<IntInstruction> list = pack.InstructionsList.Select(i => i as IntInstruction).ToList();

            List<IntInstruction> ApiBoolList = list.Where(a => a.ApiBool).ToList();
            List<IntInstruction> ResponseBoolList = list.Where(a => a.ResponseBool).ToList();
            List<IntInstruction> RequestBoolList = list.Where(a => a.RequestBool).ToList();
            List<IntInstruction> modelBoolList = list.Where(a => a.modelBool).ToList();


            foreach (var nameBea in ApiBoolList.Select(item =>
                         Path.Combine(Application.dataPath, $"Game/Script/Net/Api/Data")))
            {
                if (!Directory.Exists(nameBea))
                {
                    Directory.CreateDirectory(nameBea);
                }

                var paths = $"{nameBea}/{baseName}.cs";
                var stringBuilder = new StringBuilder(10);
                stringBuilder.Append(managerString);
                stringBuilder.Append($"namespace {namespaceName}\n{{\n");
                stringBuilder.Append(
                    $"\tpublic  class {baseName}:ApiHttp<{baseName}Request,{baseName}Response,Error>\n");
                stringBuilder.Append($"\t{{\n");
                stringBuilder.Append($"\t\tpublic {baseName}()\n");
                stringBuilder.Append($"\t\t{{\n");

                stringBuilder.Append($"\t\t}}\n");
                stringBuilder.Append($"\t}}\n");
                stringBuilder.Append($"}}");
                File.WriteAllText(paths, stringBuilder.ToString());
            }

            #region Response

            var ResponsePah = Path.Combine(Application.dataPath, $"Game/Script/Net/Api/Response");
            var pathsResponse = $"{ResponsePah}/{baseName}{Respone}";
            if (!Directory.Exists(ResponsePah))
            {
                Directory.CreateDirectory(ResponsePah);
            }

            var stringResponse = new StringBuilder(10);
            stringResponse.Append(managerString);
            stringResponse.Append($"namespace {namespaceName}\n{{\n");
            stringResponse.Append(
                $"\tpublic partial  class {baseName}Response:Model\n");
            stringResponse.Append($"\t{{\n");
            foreach (var item in ResponseBoolList)
            {
            }

            stringResponse.Append("\t\tpublic override string ToJson(bool isPretty = false)\n");
            stringResponse.Append($"\t\t{{\n");

            stringResponse.Append($"\t\t\treturn  string.Empty;\n");
            stringResponse.Append($"\t\t}}\n");
            stringResponse.Append($"\t}}\n");
            stringResponse.Append($"}}");
            File.WriteAllText(pathsResponse, stringResponse.ToString());

            #endregion

            #region Request

            var requestPah = Path.Combine(Application.dataPath, $"Game/Script/Net/Api/Request");
            var pathsRequest = $"{requestPah}/{baseName}{Request}";
            if (!Directory.Exists(requestPah))
            {
                Directory.CreateDirectory(requestPah);
            }

            var stringRequest = new StringBuilder(10);
            stringRequest.Append(managerString);
            stringRequest.Append($"namespace {namespaceName}\n{{\n");
            stringRequest.Append(
                $"\tpublic partial  class {baseName}Request:Model\n");
            stringRequest.Append($"\t{{\n");
            foreach (var item in ResponseBoolList)
            {
            }

            stringRequest.Append("\t\tpublic override string ToJson(bool isPretty = false)\n");
            stringRequest.Append($"\t\t{{\n");

            stringRequest.Append($"\t\t\treturn  string.Empty;\n");
            stringRequest.Append($"\t\t}}\n");

            stringRequest.Append($"\t}}\n");
            stringRequest.Append($"}}");
            File.WriteAllText(pathsRequest, stringRequest.ToString());

            #endregion
        }
    }
}