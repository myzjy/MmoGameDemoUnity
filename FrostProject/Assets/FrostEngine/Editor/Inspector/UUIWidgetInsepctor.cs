using UnityEditor;

namespace FrostEngine.Editor.Inspector
{
    [CustomEditor(typeof(UUIWidget))]
    public class UUIWidgetInsepctor:GameFrameworkInspector
    {
        private SerializedProperty m_IsVariable = null;
        private SerializedProperty m_CanvasGroup = null;
 
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_IsVariable);
            EditorGUILayout.PropertyField(m_CanvasGroup);
            serializedObject.ApplyModifiedProperties();

        }

        protected override void OnCompileStart()
        {
        }

        private void OnEnable()
        {
            m_IsVariable = serializedObject.FindProperty("IsVariable");
            m_CanvasGroup = serializedObject.FindProperty("mCanvasGroup");
        }

    }
}