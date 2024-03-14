using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Sprites;

namespace ZJYFrameWork.SpriteViewer
{
    internal abstract class SpriteAssetViewerPackerPolicyBase : IPackerPolicy
    {
        private object packerPolicyInstance;

        protected object PackerPolicyInstance
        {
            get
            {
                if (packerPolicyInstance == null)
                {
                    packerPolicyInstance = Activator.CreateInstance(PackerPolicyType,
                        BindingFlags.Public | BindingFlags.Instance, null, new object[] { }, null);
                }

                return packerPolicyInstance;
            }
        }

        private Assembly unityEditorAssembly;

        protected Assembly UnityEditorAssembly
        {
            get
            {
                if (unityEditorAssembly == null)
                {
                    unityEditorAssembly = typeof(EditorWindow).Assembly;
                }

                return unityEditorAssembly;
            }
        }

        protected abstract Type PackerPolicyType { get; }

        private MethodInfo onGroupAtlasesMethodInfo;

        protected MethodInfo OnGroupAtlasesMethodInfo
        {
            get
            {
                if (onGroupAtlasesMethodInfo == null)
                {
                    onGroupAtlasesMethodInfo =
                        PackerPolicyType.GetMethod("OnGroupAtlases", BindingFlags.Public | BindingFlags.Instance);
                }

                return onGroupAtlasesMethodInfo;
            }
        }

        private MethodInfo getVersionMethodInfo;

        protected MethodInfo GetVersionMethodInfo
        {
            get
            {
                if (getVersionMethodInfo == null)
                {
                    getVersionMethodInfo =
                        PackerPolicyType.GetMethod("GetVersion", BindingFlags.Public | BindingFlags.Instance);
                }

                return getVersionMethodInfo;
            }
        }

        private MethodInfo allowSequentialPackingMethodInfo;

        protected MethodInfo AllowSequentialPackingMethodInfo
        {
            get
            {
                if (allowSequentialPackingMethodInfo == null)
                {
                    allowSequentialPackingMethodInfo = PackerPolicyType.GetMethod("get_AllowSequentialPacking",
                        BindingFlags.Public | BindingFlags.Instance);
                }

                return allowSequentialPackingMethodInfo;
            }
        }

        public virtual int GetVersion()
        {
            return (int)GetVersionMethodInfo.Invoke(PackerPolicyInstance, new object[] { });
        }

        public virtual bool AllowSequentialPacking
        {
            get { return (bool)AllowSequentialPackingMethodInfo.Invoke(PackerPolicyInstance, new object[] { }); }
        }

        public void OnGroupAtlases(BuildTarget target, PackerJob job, int[] textureImporterInstanceIDs)
        {
            textureImporterInstanceIDs = SpriteViewer.SpritePackingPreviewWindow.Instance.TextureImporterInstanceIDs;

            OnGroupAtlasesMethodInfo.Invoke(PackerPolicyInstance,
                new object[] { target, job, textureImporterInstanceIDs });
        }
    }
}