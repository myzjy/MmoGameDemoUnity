using UnityEditor;

namespace FrostEngine.Editor.Inspector
{
    [CustomEditor(typeof(FsmModule))]
    internal sealed class FsmComponentInspector : GameFrameworkInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox("Available during runtime only.", UnityEditor.MessageType.Info);
                return;
            }

            FsmModule t = (FsmModule)target;

            if (IsPrefabInHierarchy(t.gameObject))
            {
                EditorGUILayout.LabelField("FSM Count", t.Count.ToString());

                FsmBase[] fsms = t.GetAllFsms();
                foreach (FsmBase fsm in fsms)
                {
                    DrawFsm(fsm);
                }
            }

            Repaint();
        }

        private void DrawFsm(FsmBase fsm)
        {
            EditorGUILayout.LabelField(fsm.FullName,
                fsm.IsRunning ? StringUtils.Format("{}, {} s", fsm.CurrentStateName, fsm.CurrentStateTime.ToString(":F1")) : (fsm.IsDestroyed ? "Destroyed" : "Not Running"));
        }
    }
}