using UnityEditor;
using ZJYFrameWork.AssetBundles;
using ZJYFrameWork.Base;

namespace ZJYFrameWork.Editors.Inspector
{
    [CustomEditor(typeof(AssetBundlesComponent))]
    public class AssetsBundleComponentInspector : GameFrameworkInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
        }

        private void OnEnable()
        {
        }
    }
}