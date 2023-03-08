using System;

namespace ZJYFrameWork.SpriteViewer
{
    internal class SpriteAssetViewerDefaultPackerPolicy : SpriteAssetViewerPackerPolicyBase
    {
        private Type packerPolicyType;

        protected override Type PackerPolicyType
        {
            get { return packerPolicyType ??= UnityEditorAssembly.GetType("UnityEditor.Sprites.DefaultPackerPolicy"); }
        }
    }
}