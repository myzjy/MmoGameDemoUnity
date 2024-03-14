using System;

namespace ZJYFrameWork.SpriteViewer
{
    internal class SpriteAssetViewerTightPackerPolicy : SpriteAssetViewerDefaultPackerPolicy
    {
        private Type packerPolicyType;
        protected override Type PackerPolicyType
        {
            get
            {
                if (packerPolicyType == null)
                {
                    packerPolicyType = UnityEditorAssembly.GetType("UnityEditor.Sprites.TightPackerPolicy");
                }
                return packerPolicyType;
            }
        }
    }
}