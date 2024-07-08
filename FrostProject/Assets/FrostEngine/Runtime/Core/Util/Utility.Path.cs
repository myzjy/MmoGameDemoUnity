using System.IO;
using System.Linq;

namespace FrostEngine
{
    public class Utility
    {
        public static string[] GetSpecifyFilesInFolder(string path, bool exclude = false)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            return Directory.GetFiles(path, "._*.*", SearchOption.AllDirectories);
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
                Debug.LogError($"SafeRenameFile failed! path = {sourceFileName} with err: {ex.Message}");
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
                Debug.LogError($"SafeDeleteFile failed! path = {filePath} with err: {ex.Message}");
                return false;
            }
        }

        public static byte[] GetFileBytes(string inFile)
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

                return File.ReadAllBytes(inFile);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"SafeReadAllBytes failed! path = {inFile} with err = {ex.Message}");
                return null;
            }
        }

        public static string GetFileExtension(string path)
        {
            return System.IO.Path.GetExtension(path).ToLower();
        }

        public static class Path
        {
            // public static string GetExtension(string path)
            // {
            //     var index = path.IndexOf('.');
            //     return index < 0 ? string.Empty : path.Substring(index + 1);
            // }

            public static string GetFilePathWithoutExtension(string path)
            {
                var index = path.IndexOf('.');
                return index < 0 ? path : path.Substring(0, index);
            }

            public static string GetFileName(string path)
            {
                return System.IO.Path.GetFileName(path);
            }

            public static string GetFileNameWithoutExtension(string path)
            {
                return System.IO.Path.GetFileNameWithoutExtension(path);
            }

            /// <summary>
            /// 获取规范的路径。
            /// </summary>
            /// <param name="path">要规范的路径。</param>
            /// <returns>规范的路径。</returns>
            public static string GetRegularPath(string path)
            {
                if (path == null)
                {
                    return null;
                }

                return path.Replace('\\', '/');
            }
        }
    }
}