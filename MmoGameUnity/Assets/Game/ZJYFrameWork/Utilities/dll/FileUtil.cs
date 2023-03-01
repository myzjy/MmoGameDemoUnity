/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NON INFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
namespace ZJYFrameWork.Utilities
{
    public static class FileUtil
    {
        private static readonly List<IZipAccessor> List = new List<IZipAccessor>();

        public static void Register(IZipAccessor zipAccessor)
        {
            if (List.Contains(zipAccessor))
                return;
            List.Add(zipAccessor);
            List.Sort((x, y) => y.Priority.CompareTo(x.Priority));
        }

        public static void Unregister(IZipAccessor zipAccessor)
        {
            if (!List.Contains(zipAccessor))
                return;
            List.Remove(zipAccessor);
        }

        public static string[] ReadAllLines(string path)
        {
            return ReadAllLines(path, Encoding.UTF8);
        }

        private static string[] ReadAllLines(string path, Encoding encoding)
        {
            if (!IsZipArchive(path))
                return File.ReadAllLines(path, encoding);

            List<string> lines = new List<string>();
            using (var stream = OpenReadInZip(path))
            {
                using (StreamReader sr = new StreamReader(stream, encoding, true))
                {
                    while (sr.ReadLine() is { } line)
                        lines.Add(line);
                }
            }

            return lines.ToArray();
        }

        public static string ReadAllText(string path)
        {
            return ReadAllText(path, Encoding.UTF8);
        }

        private static string ReadAllText(string path, Encoding encoding)
        {
            if (!IsZipArchive(path))
                return File.ReadAllText(path, encoding);

            using var stream = OpenReadInZip(path);
            using StreamReader sr = new StreamReader(stream, encoding, true);
            return sr.ReadToEnd();
        }

        public static byte[] ReadAllBytes(string path)
        {
            if (!IsZipArchive(path))
                return File.ReadAllBytes(path);

            using var stream = OpenReadInZip(path);
            byte[] buffer = new byte[stream.Length];
            // ReSharper disable once MustUseReturnValue
            stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        public static Stream OpenRead(string path)
        {
            if (!IsZipArchive(path))
                return File.OpenRead(path);

            return OpenReadInZip(path);
        }

        public static bool Exists(string path)
        {
            if (!IsZipArchive(path))
                return File.Exists(path);

            return ExistsInZip(path);
        }

        private static Stream OpenReadInZip(string path)
        {
            foreach (var zipAccessor in List.Where(zipAccessor => zipAccessor.Support(path)))
            {
                return zipAccessor.OpenRead(path);
            }

#if UNITY_ANDROID
            if (path.IndexOf(".obb", StringComparison.OrdinalIgnoreCase) > 0)
            {
                Debug.Log(
                    "无法读取\"中的内容。obb\"文件，请点击链接获取帮助，并启用对obb文件的访问。https://github.com/cocowolf/loxodon-framework/blob/master/docs/faq.md");
            }

#endif

            throw new NotSupportedException(path);
        }

        private static bool ExistsInZip(string path)
        {
            foreach (var zipAccessor in List.Where(zipAccessor => zipAccessor.Support(path)))
            {
                return zipAccessor.Exists(path);
            }

            throw new NotSupportedException(path);
        }

        public static bool IsZipArchive(string path)
        {
            if (Regex.IsMatch(path, @"(jar:file:///)|(\.jar)|(\.apk)|(\.obb)|(\.zip)", RegexOptions.IgnoreCase))
                return true;
            return false;
        }

        public interface IZipAccessor
        {
            int Priority { get; }

            bool Support(string path);

            Stream OpenRead(string path);

            bool Exists(string path);
        }
    }
}