using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace GameData.Net
{
    public class ModelDataResoucePath
    {
        private readonly Action<HttpResponse> onSuccess;
        private string path { get; set; }
        private byte[] Data { get; set; }

        /// <summary>
        /// 数据转换
        /// </summary>
        public string DataAssetText
        {
            get { return Encoding.UTF8.GetString(Data); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="_onSuccess"></param>
        public ModelDataResoucePath(string path, Action<HttpResponse> _onSuccess)
        {
            this.path = path;
            this.onSuccess = _onSuccess;
        }

        /// <summary>
        /// 初始 只带路径 根据路径去找对接json 和cs
        /// </summary>
        /// <param name="path"></param>
        public ModelDataResoucePath(string path)
        {
            this.path = path;
        }


     
        private void SetPayLoadData(string json, ServericeBase _serverice, Func<string, string> func)
        {
            switch (@json)
            {
                default:
                    UnityEngine.Debug.LogError($"not swith json path:{json} 需要SetPayLoadData内部编写");
                    break;
            }
        }

        private void ReadRequsetJson<T>(string DataPath, T _data, Func<string, string> func)
        {
            UnityEngine.Debug.Log($"{_data}");
            UnityEngine.Debug.Log($"路径{DataPath}");

            using (StreamReader file = File.OpenText(DataPath))
            {
                UnityEngine.Debug.Log("打开文件");
                if (string.IsNullOrEmpty(file.ReadToEnd()))
                {
                    UnityEngine.Debug.Log("文件为空");

                    //基础数据
                    var str = JsonConvert.SerializeObject(_data);
                    file.Close();
                    UnityEngine.Debug.Log("关闭读");
                    UnityEngine.Debug.Log($"数据{str}");

                    using (var fs = new FileStream(DataPath, FileMode.Create))
                    {
                        using (var writer = new StreamWriter(fs))
                        {
                            //写入
                            writer.Write(str);
                            // func(writer.ReadToEnd());
                            func(str);
                            writer.Close();
                        }
                    }
                }
                else
                {
                    file.Close();

                    // file.();
                    using (var fs = File.OpenText(DataPath))
                    {
                        Data = Encoding.UTF8.GetBytes(func(fs.ReadToEnd()));
                        // using (var writer = new StreamWriter(fs))
                        // {
                        //     //写入
                        //     writer.Write(fs.ReadToEnd());
                        //     func(file.ReadToEnd());
                        //
                        //     writer.Close();
                        // }
                        fs.Close();
                    }
                }

                file.Close();
            }
        }

        //写入
        private void writerUrlJson(string jsonpath, string json)
        {
            using (var fs = new FileStream(jsonpath, FileMode.Create))
            {
                using (var writer = new StreamWriter(json))
                {
                    //写入
                    writer.Write(json); // func(writer.ReadToEnd());

                    writer.Close();
                }

                fs.Close();
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="urlJson">路径</param>
        private static void CreateRequstUrlJson(string urlJson, string json)
        {
            var dir = Path.GetDirectoryName("../MaoguoDll/" + urlJson);
            if (!Directory.Exists(dir))
            {
                if (dir != null) Directory.CreateDirectory(dir);
            }

            using (var fs = new FileStream(urlJson, FileMode.Create))
            {
                using (var writer = new StreamWriter(fs))
                {
                    //写入
                    writer.Write(json);
                }
            }
        }

        public string ReadJsonText(string urlJson)
        {
            using (StreamReader file = File.OpenText(urlJson))
            {
                return file.ReadToEnd();
            }

            return "";
        }

    }
}