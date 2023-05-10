using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
// using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public static class Util
{
    /// <summary>
    /// button自定义设置
    /// </summary>
    /// <param name="button"></param>
    /// <param name="action"></param>
    public static void SetListener(this Button button, UnityAction action)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }

    public const string NewLineCRLF = "\r\n";
    public const string NewLineCR = "\r";
    public const string NewLineLF = "\n";

    /// <summary>
    /// 直の改行文字をNewLineにReplaceする
    /// </summary>
    public static string ReplaceNewLine(string source)
    {
        if (string.IsNullOrEmpty(source))
        {
            return "";
        }

        return source
            .Replace(NewLineCRLF, System.Environment.NewLine)
            .Replace(NewLineCR, System.Environment.NewLine)
            .Replace(NewLineLF, System.Environment.NewLine);
    }

    public static bool SafeDeleteDir(string folderPath)
    {
        try
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                return true;
            }

            if (Directory.Exists(folderPath))
            {
                DeleteDirectory(folderPath);
            }

            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"SafeDeleteDir failed! path = {folderPath} with err: {ex.Message}");
            return false;
        }
    }

    public static void DeleteDirectory(string dirPath)
    {
        string[] files = Directory.GetFiles(dirPath);
        string[] dirs = Directory.GetDirectories(dirPath);

        foreach (string file in files)
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (string dir in dirs)
        {
            DeleteDirectory(dir);
        }

        Directory.Delete(dirPath, false);
    }
}