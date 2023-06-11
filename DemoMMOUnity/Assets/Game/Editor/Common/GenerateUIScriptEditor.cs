using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
// using UnityEditor.AddressableAssets;
// using UnityEditor.AddressableAssets.Build;
// using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace ZJYFrameWork.UISerializable.UIViewEditor
{
    public class GenerateUIScriptEditor
    {
        // [MenuItem("Tools/Build/BuildAllCont")]
        // public static void BuilContent()
        // {
        //     AddressableAssetSettings.BuildPlayerContent();
        // }
        // [MenuItem("Tools/Build/BuildUpdate")]
        // public static void BuildUpdate()
        // {
        //     var pat = ContentUpdateScript.GetContentStateDataPath(false);
        //     var m_setting = AddressableAssetSettingsDefaultObject.Settings;
        //     var res = ContentUpdateScript.BuildContentUpdate(m_setting, pat);
        // }


        private static string TemplateCS = $"using ZJYFrameWork.UISerializable.UIInitView;\n" +
                                           $"using UnityEngine;\n" +
                                           $"using UnityEngine.UI;\n" +
                                           $"\n" +
                                           $"namespace ZJYFrameWork.UISerializable\n" +
                                           $"{{\n" +
                                           $"    public class ${{ClassName}}:UIViewInterface\n" +
                                           $"    {{\n" +
                                           $"        ${{MemberVariables}}\n" +
                                           $"\n" +
                                           $"\n" +
                                           $"        public void Init(UIView _view)\n" +
                                           $"        {{\n" +
                                           $"            ${{Init}}\n" +
                                           $"        }}\n" +
                                           $"    }}\n" +
                                           $"}}";


        [MenuItem("Tools/UI/GenerateUIScript")]
        private static void GenerateUI()
        {
            var target = $"{Application.dataPath}/Game/AssetBundles/UI/Prefabs/";
            var template = $"{Application.dataPath}/../NewPrefab/";
            var outputPath = $"{Application.dataPath}/Game/Hoftix/Script/GenerateScripts/UIModules/";
            var outputLuaPath = $"{Application.dataPath}/../Lua/Game/GenerateScripts/UIModules/";
            var outputProCsPath = $"{Application.dataPath}/../Assembly-CSharp.csproj";
            var UISObjet = new DirectoryInfo(target);
            var outputPathDirectoryInfo = new DirectoryInfo(outputPath);
            if (!outputPathDirectoryInfo.Exists)
            {
                Directory.CreateDirectory(outputPath);
            }

            var UIInfos = UISObjet.GetFiles("*.*", SearchOption.AllDirectories);

            //输出
            var csproStr = "";
            if (File.Exists(outputProCsPath))
            {
                csproStr = File.ReadAllText(outputProCsPath);
            }
            else
            {
                UnityEngine.Debug.LogError("当前项目找不到csproj文件,生成引用失败");
            }

            foreach (var item in UIInfos)
            {
                if (item.Extension.Contains("meta"))
                {
                    continue;
                }

                var classStr = TemplateCS;

                var path = item.FullName.Replace("\\", "/");
                path = path.Replace(Application.dataPath, "Assets");
                if (item.Extension.Contains("prefab"))
                {
                    var UISelfObj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    var UIView = UISelfObj.GetComponent<UIView>();
                    if (UIView == null)
                    {
                        UnityEngine.Debug.LogError($"{path} 预制体 没有UIView组件");
                        continue;
                    }

                    Debug.Log($"{item.Name}");
                    //保存的UI组件
                    var UIViewDataList = UIView.dataList;
                    var className = $"{Path.GetFileNameWithoutExtension(item.Name)}View";
                    //成员变量
                    var memberStr = "";
                    //初始化
                    var initStr = "";
                    UIViewDataList.ForEach(a =>
                    {
                        var memberName = a.UI_Serializable_Key;
                        var typeString = a.UI_Serializable_Obj.GetType();
                        memberStr += $"public {typeString} {memberName}=null;\r\n\t\t";
                        initStr += $"{memberName}=_view.GetObjType<{typeString}>(\"{memberName}\");\r\n\t\t\t";
                    });
                    //类名替换
                    classStr = classStr.Replace("${ClassName}", className);
                    //属性构造替换
                    classStr = classStr.Replace("${MemberVariables}", memberStr);
                    //方法体 替换
                    classStr = classStr.Replace("${Init}", initStr);
                    var OutPutFileFUllPath = $"{outputPath}{className}.cs";
                    UnityEngine.Debug.Log($"创建脚本目录:{OutPutFileFUllPath}");

                    var stream = new FileStream(OutPutFileFUllPath, FileMode.Create, FileAccess.Write);
                    var fileWrite = new StreamWriter(stream, System.Text.Encoding.UTF8);
                    fileWrite.Write(classStr);
                    fileWrite.Flush();
                    fileWrite.Close();
                    stream.Close();
                    UnityEngine.Debug.LogError($"创建脚本{OutPutFileFUllPath}成功!!!!");
                    UnityEngine.Debug.LogError(classStr);
                    if (!string.IsNullOrEmpty(csproStr))
                    {
                        if (!Regex.IsMatch(csproStr, $"{className}.cs"))
                        {
                            var compileStr =
                                $"<!--UIvew-->\r\n\t<Compile Include=\"Asset\\Game\\Scripts\\GenerateScripts\\UIModules\\{className}.cs\" />";
                            stream = new FileStream(outputProCsPath, FileMode.Open, FileAccess.Write);
                            csproStr = csproStr.Replace("<!--UIView-->", compileStr);
                            fileWrite = new StreamWriter(stream);
                            fileWrite.Write(csproStr);
                            fileWrite.Flush();
                            fileWrite.Close();
                            stream.Close();
                            UnityEngine.Debug.Log("生成Csproj配置文件成功!!!");
                        }
                    }
                }
            }
        }

        [MenuItem("Tools/UI/GenerateLuaUI")]
        private static void GenerateLuaUI()
        {
            var outputLuaPath = $"{Application.dataPath}/../Lua/Game/GenerateScripts/UIModules/";
            var outputPathDirectoryInfo = new DirectoryInfo(outputLuaPath);
            if (!outputPathDirectoryInfo.Exists)
            {
                Directory.CreateDirectory(outputLuaPath);
            }

            outputLuaPath = outputPathDirectoryInfo.FullName;
            Debug.Log($"outputLuaPath:{outputLuaPath}");
            var target = $"{Application.dataPath}/Game/AssetBundles/UI/Prefabs/";
            var UISObjet = new DirectoryInfo(target);
            var UIInfos = UISObjet.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (var item in UIInfos)
            {
                if (item.Extension.Contains("meta"))
                {
                    continue;
                }

                var path = item.FullName.Replace("\\", "/");
                path = path.Replace(Application.dataPath, "Assets");
                var UISelfObj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                var UIView = UISelfObj.GetComponent<UIView>();
                if (UIView == null)
                {
                    UnityEngine.Debug.LogError($"{path} 预制体 没有UIView组件");
                    continue;
                }

                var UIViewDataList = UIView.dataList;
                var className = $"{Path.GetFileNameWithoutExtension(item.Name)}View";

                //初始化
                var initStr = "";
                UIViewDataList.ForEach(a =>
                {
                    var memberName = a.UI_Serializable_Key;
                    var typeString = a.UI_Serializable_Obj.GetType();
                    initStr += $"\tself.{memberName} = _UIView:GetObjType(\"{memberName}\") or CS.{typeString}\n";
                });
                string TemplateLuaCS = $"---@{className}\n" +
                                       $"local {className} = BaseClass()\n" +
                                       "local _UIView = {}\n" +
                                       $"function {className}:Init(view)\n" +
                                       $"\t_UIView = view:GetComponent(\"UIView\")\n" +
                                       $"{initStr}" +
                                       $"end\n\n" +
                                       $"return {className}";
                var OutPutFileFUllPath = $"{outputLuaPath}{className}.lua";
                Debug.Log($"创建脚本目录:{OutPutFileFUllPath}，\n文件：\n {TemplateLuaCS}");
                var stream = new FileStream(OutPutFileFUllPath, FileMode.Create, FileAccess.Write);
                Encoding end = new UTF8Encoding(false);
                var fileWrite = new StreamWriter(stream,end);
                fileWrite.Write(TemplateLuaCS);
                fileWrite.Flush();
                fileWrite.Close();
                stream.Close();
            }
        }
    }
}