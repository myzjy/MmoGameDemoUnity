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

#if UNITY_ANDROID
using System;
using System.IO;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace ZJYFrameWork.Utilities
{
    public class ZipAccessorForAndroidStreamingAssets : FileUtil.IZipAccessor
    {
        private const string ActivityJavaClass = "com.unity3d.player.UnityPlayer";

        private static AndroidJavaObject _assetManager;

        private static AndroidJavaObject AssetManager
        {
            get
            {
                if (_assetManager != null)
                    return _assetManager;

                using (AndroidJavaClass activityClass = new AndroidJavaClass(ActivityJavaClass))
                {
                    using (var context = activityClass.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        _assetManager = context.Call<AndroidJavaObject>("getAssets");
                    }
                }
                return _assetManager;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnInitialized()
        {
            FileUtil.Register(new ZipAccessorForAndroidStreamingAssets());
        }

        private string GetAssetFilePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            int start = path.LastIndexOf("!/assets/", StringComparison.Ordinal);
            if (start < 0)
                return path;

            return path.Substring(start + 9);
        }

        public int Priority => 0;

        public bool Exists(string path)
        {
            try
            {
                using AndroidJavaObject fileDescriptor = AssetManager.Call<AndroidJavaObject>("openFd", GetAssetFilePath(path));
                if (fileDescriptor != null)
                    return true;
            }
            catch (Exception)
            {
                // ignored
            }

            return false;
        }

        public Stream OpenRead(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("the filename is null or empty.");

            return new InputStreamWrapper(AssetManager.Call<AndroidJavaObject>("open", GetAssetFilePath(path)));
        }

        public bool Support(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            string fullname = path.ToLower();
            if (fullname.IndexOf(".apk", StringComparison.Ordinal) > 0 && fullname.LastIndexOf("!/assets/", StringComparison.Ordinal) > 0)
                return true;

            return false;
        }

        private class InputStreamWrapper : Stream
        {
            private readonly object _lock = new object();
            private readonly long _length;
            // ReSharper disable once InconsistentNaming
            private const long position = 0;
            private AndroidJavaObject _inputStream;
            public InputStreamWrapper(AndroidJavaObject inputStream)
            {
                this._inputStream = inputStream;
                this._length = inputStream.Call<int>("available");
            }

            public override bool CanRead => position < this._length;

            public override bool CanSeek => false;

            public override bool CanWrite => false;

            public override long Length => this._length;

            public override long Position
            {
                get => position;
                set => throw new NotSupportedException();
            }

            public override void Flush()
            {
                throw new NotSupportedException();
            }

            [Obsolete("Obsolete")]
            public override int Read(byte[] buffer, int offset, int count)
            {
                lock (_lock)
                {
                    int ret;
                    IntPtr array = IntPtr.Zero;
                    try
                    {
                        array = AndroidJNI.NewByteArray(count);
                        var method = AndroidJNIHelper.GetMethodID(_inputStream.GetRawClass(), "read", "([B)I");
                        ret = AndroidJNI.CallIntMethod(_inputStream.GetRawObject(), method, new[] { new jvalue() { l = array } });
                        byte[] data = AndroidJNI.FromByteArray(array);
                        Array.Copy(data, 0, buffer, offset, ret);
                    }
                    finally
                    {
                        if (array != IntPtr.Zero)
                            AndroidJNI.DeleteLocalRef(array);
                    }
                    return ret;
                }
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (_inputStream != null)
                {
                    if (_lock != null)
                        lock (_lock)
                        {
                            _inputStream.Call("close");
                        }

                    if (_lock != null)
                        lock (_lock)
                        {
                            _inputStream.Dispose();
                        }

                    if (_lock != null)
                        lock (_lock)
                        {
                            _inputStream = null;
                        }
                }
            }
        }
    }
}
#endif