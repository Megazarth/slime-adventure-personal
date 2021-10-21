using UnityEditor;
using UnityEngine;

namespace Kaizen
{
    public class WindowImportManager : EditorWindow
    {
        string[] assetTypes = { "Sprite", "Audio" };
        int toolBarSelection;

        string myString = "Hello World";
        bool groupEnabled;
        bool myBool = true;
        float myFloat = 1.23f;

        [MenuItem("Kaizen/Import Settings")]
        private static void Init()
        {
            var window = (WindowImportManager)GetWindow(typeof(WindowImportManager));
            window.titleContent = new GUIContent("Import Settings");
            window.position = new Rect(0, 0, 300, 100);
            window.ShowUtility();
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            toolBarSelection = GUILayout.Toolbar(toolBarSelection, assetTypes);
            GUILayout.EndHorizontal();

            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            myString = EditorGUILayout.TextField("Text Field", myString);

            groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
            myBool = EditorGUILayout.Toggle("Toggle", myBool);
            myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
            EditorGUILayout.EndToggleGroup();

        }
    }
}