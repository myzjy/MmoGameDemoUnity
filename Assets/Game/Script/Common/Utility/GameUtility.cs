using System.IO;
using System.Linq;
using UnityEngine;

namespace Common.Utility
{
    public class GameUtility
    {
        public const string AssetsFolderName = "Assets";

        public static byte[] SafeReadAllBytes(string inFile)
        {
            try
            {
                if (string.IsNullOrEmpty(inFile))
                {
                    return null;
                }

                if (!File.Exists(inFile))
                {
                    return null;
                }

                File.SetAttributes(inFile, FileAttributes.Normal);
                return File.ReadAllBytes(inFile);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"SafeReadAllBytes failed! path = {inFile} with err = {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 安全移除
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static bool LuaSafeDeleteDir(string folderPath)
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

        private static void DeleteDirectory(string dirPath)
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

        public static bool SafeClearDir(string folderPath)
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

                Directory.CreateDirectory(folderPath);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"SafeClearDir failed! path = {folderPath} with err = {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 路径替换
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string FormatToSysFilePath(string path)
        {
            return path.Replace("/", "\\");
        }

        private static string GetFileExtension(string path)
        {
            return Path.GetExtension(path).ToLower();
        }

        public static string[] GetSpecifyFilesInFolder(string path, string[] extensions = null, bool exclude = false)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            if (extensions == null)
            {
                return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            }
            else if (exclude)
            {
                return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                    .Where(f => !extensions.Contains(GetFileExtension(f))).ToArray();
            }
            else
            {
                return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                    .Where(f => extensions.Contains(GetFileExtension(f))).ToArray();
            }
        }

        public static bool LuaSafeDeleteFile(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return true;
                }

                if (!File.Exists(filePath))
                {
                    return true;
                }

                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"SafeDeleteFile failed! path = {filePath} with err: {ex.Message}");
                return false;
            }
        }

        public static bool LuaRenameFile(string sourceFileName, string destFileName)
        {
            try
            {
                if (string.IsNullOrEmpty(sourceFileName))
                {
                    return false;
                }

                if (!File.Exists(sourceFileName))
                {
                    return true;
                }

                LuaSafeDeleteFile(destFileName);
                File.SetAttributes(sourceFileName, FileAttributes.Normal);
                File.Move(sourceFileName, destFileName);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"SafeRenameFile failed! path = {sourceFileName} with err: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 检查文件和创建Dir时需要
        /// </summary>
        /// <param name="filePath">文件路径</param>
        private static void CheckFileAndCreateDirWhenNeeded(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            FileInfo file_info = new FileInfo(filePath);
            DirectoryInfo dir_info = file_info.Directory;
            if (!dir_info.Exists)
            {
                Directory.CreateDirectory(dir_info.FullName);
            }
        }

        public static bool SafeWriteAllBytes(string outFile, byte[] outBytes)
        {
            try
            {
                if (string.IsNullOrEmpty(outFile))
                {
                    return false;
                }

                //检查 之后路径不存在就会创建
                CheckFileAndCreateDirWhenNeeded(outFile);
                //路径是否存在
                if (File.Exists(outFile))
                {
                    File.SetAttributes(outFile, FileAttributes.Normal);
                }

                File.WriteAllBytes(outFile, outBytes);
                return true;
            }
            catch (System.Exception ex)
            {
                ToolsDebug.LogError($"SafeWriteAllBytes failed! path = {outFile} with err = {ex.Message}");
                return false;
            }
        }

        public static string SafeReadAllText(string inFile)
        {
            try
            {
                if (string.IsNullOrEmpty(inFile))
                {
                    return null;
                }

                if (!File.Exists(inFile))
                {
                    return null;
                }

                File.SetAttributes(inFile, FileAttributes.Normal);
                return File.ReadAllText(inFile);
            }
            catch (System.Exception ex)
            {
                ToolsDebug.LogError($"SafeReadAllText failed! path = {inFile} with err = {ex.Message}");
                return null;
            }
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
                ToolsDebug.LogError($"SafeDeleteDir failed! path = {folderPath} with err: {ex.Message}");
                return false;
            }
        }

        public static bool SafeWriteAllText(string outFile, string text)
        {
            try
            {
                if (string.IsNullOrEmpty(outFile))
                {
                    return false;
                }

                CheckFileAndCreateDirWhenNeeded(outFile);
                if (File.Exists(outFile))
                {
                    File.SetAttributes(outFile, FileAttributes.Normal);
                }

                File.WriteAllText(outFile, text);
                return true;
            }
            catch (System.Exception ex)
            {
                ToolsDebug.LogError(
                    $"SafeWriteAllText failed! path = {outFile} with err = {ex.Message}");
                return false;
            }
        }
        public static bool SafeWriteAllLines(string outFile, string[] outLines)
        {
            try
            {
                if (string.IsNullOrEmpty(outFile))
                {
                    return false;
                }

                CheckFileAndCreateDirWhenNeeded(outFile);
                if (File.Exists(outFile))
                {
                    File.SetAttributes(outFile, FileAttributes.Normal);
                }
                File.WriteAllLines(outFile, outLines);
                return true;
            }
            catch (System.Exception ex)
            {
                ToolsDebug.LogError($"SafeWriteAllLines failed! path = {outFile} with err = {ex.Message}");
                return false;
            }
        }
     
        public static string FormatToUnityPath(string path)
        {
            return path.Replace("\\", "/");
        }
        public static string FullPathToAssetPath(string full_path)
        {
            full_path = FormatToUnityPath(full_path);
            if (!full_path.StartsWith(Application.dataPath))
            {
                return null;
            }
            string ret_path = full_path.Replace(Application.dataPath, "");
            return AssetsFolderName + ret_path;
        }
        public static void CheckDirAndCreateWhenNeeded(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                return;
            }

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }
        public static bool SafeDeleteFile(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return true;
                }

                if (!File.Exists(filePath))
                {
                    return true;
                }
                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
                return true;
            }
            catch (System.Exception ex)
            {
                ToolsDebug.LogError($"SafeDeleteFile failed! path = {filePath} with err: {ex.Message}");
                return false;
            }
        }
        public static bool SafeRenameFile(string sourceFileName, string destFileName)
        {
            try
            {
                if (string.IsNullOrEmpty(sourceFileName))
                {
                    return false;
                }

                if (!File.Exists(sourceFileName))
                {
                    return true;
                }
                SafeDeleteFile(destFileName);
                File.SetAttributes(sourceFileName, FileAttributes.Normal);
                File.Move(sourceFileName, destFileName);
                return true;
            }
            catch (System.Exception ex)
            {
                ToolsDebug.LogError($"SafeRenameFile failed! path = {sourceFileName} with err: {ex.Message}");
                return false;
            }
        }
    }
}