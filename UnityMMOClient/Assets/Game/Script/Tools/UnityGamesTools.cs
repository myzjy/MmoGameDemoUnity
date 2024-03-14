using System;

using System.IO;
using UnityEngine;

public class UnityGamesTools
{
    public static byte[] MyCustomLoader(ref string filepath)
    {
        // 通过自定义filepath的解析方式来实现特殊加载功能

        // 1. 从指定的路径加载Lua文件
        if (filepath.Contains("/"))
        {
            if (File.Exists(filepath))
            {
                return File.ReadAllBytes(filepath);
                //string script = File.ReadAllText(filepath);
                //return System.Text.Encoding.UTF8.GetBytes(script);
            }
        }
        // 2. 从自定义的默认位置加载Lua文件
        else
        {
            string defaultFolder = Application.dataPath + "/Lua/";
            string file = defaultFolder + filepath + ".lua.txt";
            if (File.Exists(file))
            {
                return File.ReadAllBytes(file);
            }
        }

        // 其他加载方式：
        // 3. 加载网络文件
        // 4. 加载压缩文件并解压
        // 5. 加载加密文件并解密

        return null;
    }

    public static T InstanceTransformScript<T>(GameObject parface,Transform _tr)
    {
        var obj = UnityEngine.Object.Instantiate(parface, _tr);
        obj.name = parface.name;
        var tScript = obj.GetComponent<T>();
        return tScript;
    }

    public static GameObject InstanceTransformScript(GameObject parface,Transform _tr)
    {
        var obj = UnityEngine.Object.Instantiate(parface, _tr);
        obj.name = parface.name;
        return obj;
    }
}
public class NoticeAction
{
    public string centent;
    public Action no;
    public string title;
    public Action yes;
}

