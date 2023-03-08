using UnityEditor;
using UnityEngine;

namespace ZJYFrameWork.SpriteViewer
{
    internal partial class SpriteAssetViewerWindow
    {
        internal abstract class SpriteAssetViewerModeBase
        {
            protected SpriteAssetViewerUtility.Styles styles => SpriteAssetViewerUtility.SpriteAssetViewerStyles;

            protected SpriteAssetViewerWindow owner;

            public SpriteAssetViewerModeBase(SpriteAssetViewerWindow owner)
            {
                this.owner = owner;
            }

            public abstract EditorMode GetMode();

            public virtual void DrawExToolbar()
            {
            }

            public virtual void DrawSetting(GenericMenu menu)
            {
            }

            public abstract void DrawMode(Rect rect);

            public abstract void OnModeIn();

            public abstract void OnModeOut();

            public virtual void OnDisable()
            {
            }

            public virtual void Reload()
            {
            }
        }
    }
}