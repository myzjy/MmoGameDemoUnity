using System;

namespace ZJYFrameWork.SpriteViewer
{
    internal class SpriteAssetViewerTightRotateEnabledSpritePackerPolicy : SpriteAssetViewerDefaultPackerPolicy
    {
        private Type packerPolicyType;
        protected override Type PackerPolicyType
        {
            get
            {
                if (packerPolicyType == null)
                {
                    packerPolicyType = UnityEditorAssembly.GetType("UnityEditor.Sprites.TightRotateEnabledSpritePackerPolicy");
                }
                return packerPolicyType;
            }
        }
    }
}