namespace FrostEngine
{
    public class Utility
    {
        public static class Path
        {
            public static string GetExtension(string path)
            {
                var index = path.IndexOf('.');
                return index < 0 ? string.Empty : path.Substring(index + 1);
            }

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