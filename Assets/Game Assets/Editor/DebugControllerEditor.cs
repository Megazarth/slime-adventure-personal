using UnityEditor;
using UnityEngine;

namespace Slime
{
    [CustomEditor(typeof(DebugController))]
    public class DebugControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save Progress"))
            {
                //AppManager.Instance.SaveProgress();
            }
            if (GUILayout.Button("Clear Progress"))
            {
                //AppManager.Instance.ClearProgress();
            }
            GUILayout.EndHorizontal();
        }
    }
}