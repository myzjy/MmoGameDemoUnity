#if !NETFX_CORE //&& (!UNITY_WEBGL || UNITY_EDITOR)
using System;
using System.IO;

namespace BestHTTP.PlatformSupport.FileSystem
{
    public sealed class DefaultIOService : IIOService
    {
        public Stream CreateFileStream(string path, FileStreamModes mode)
        {
#if (UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG) && ENABLE_LOG_NETWORK
            Debug.Log(
                $"[DefaultIOService] [method: CreateFileStream(path:{path}, mode:{mode})] [msg|Exception] CreateFileStream path: '{path}' mode: {mode}");
#endif
            return mode switch
            {
                FileStreamModes.Create => new FileStream(path, FileMode.Create),
                FileStreamModes.OpenRead => new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read),
                FileStreamModes.OpenReadWrite => new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                    FileShare.ReadWrite),
                FileStreamModes.Append => new FileStream(path, FileMode.Append),
                _ => throw new NotImplementedException(
                    $"DefaultIOService.CreateFileStream - mode not implemented: {mode.ToString()}")
            };
        }

        public void DirectoryCreate(string path)
        {
#if (UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG) && ENABLE_LOG_NETWORK
            Debug.Log(
                $"[DefaultIOService] [method: DirectoryCreate(path:{path})] [msg|Exception] DirectoryCreate path: '{path}'");
#endif
            Directory.CreateDirectory(path);
        }

        public bool DirectoryExists(string path)
        {
            bool exists = Directory.Exists(path);
#if (UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG) && ENABLE_LOG_NETWORK
            Debug.Log(
                $"[DefaultIOService] [method: DirectoryExists(path:{path})] [msg|Exception] DirectoryExists path: '{path}' exists: {exists}");
#endif
            return exists;
        }

        public string[] GetFiles(string path)
        {
            var files = Directory.GetFiles(path);
#if (UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG) && ENABLE_LOG_NETWORK
            Debug.Log(
                $"[DefaultIOService] [method: GetFiles(path:{path})] [msg|Exception] GetFiles path: '{path}' files count: {files.Length}");
#endif
            return files;
        }

        public void FileDelete(string path)
        {
#if (UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG) && ENABLE_LOG_NETWORK
            Debug.Log(
                $"[DefaultIOService] [method: FileDelete(path:{path})] [msg|Exception] FileDelete path: '{path}'");
#endif
            File.Delete(path);
        }

        public bool FileExists(string path)
        {
            bool exists = File.Exists(path);

#if (UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG) && ENABLE_LOG_NETWORK
            Debug.Log(
                $"[DefaultIOService] [method: FileExists(path:{path})] [msg|Exception] FileExists path: '{path}' exists: {exists}");
#endif
            return exists;
        }
    }
}

#endif