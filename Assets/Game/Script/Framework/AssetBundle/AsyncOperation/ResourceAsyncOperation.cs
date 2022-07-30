using System;
using System.Collections;

namespace Framework.AssetBundle.AsyncOperation
{
    public abstract class ResourceAsyncOperation : IEnumerator, IDisposable
    {
        public object Current
        {
            get
            {
                return null;
            }
        }

        public bool isDone
        {
            get
            {
                return IsDone();
            }
        }

        public float progress
        {
            get
            {
                return Progress();
            }
        }

        public abstract void Update();

        public bool MoveNext()
        {
            return !IsDone();
        }

        public void Reset()
        {
        }

        public abstract bool IsDone();

        public abstract float Progress();

        public virtual void Dispose()
        {
        }
    }

    public abstract class BaseAssetBundleAsyncLoader : ResourceAsyncOperation
    {
        public string assetbundleName
        {
            get;
            set;
        }

        public UnityEngine.AssetBundle assetbundle
        {
            get;
            set;
        }

        public override void Dispose()
        {
            assetbundleName = null;
            assetbundle = null;
        }
    }

    public abstract class BaseAssetAsyncLoader : ResourceAsyncOperation
    {
        public UnityEngine.Object asset
        {
            get;
            set;
        }

        public override void Dispose()
        {
            asset = null;
        }
    }
}