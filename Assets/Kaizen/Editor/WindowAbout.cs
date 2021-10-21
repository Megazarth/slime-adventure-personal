using UnityEditor;
using UnityEngine;

namespace Kaizen
{
    public class WindowAbout : EditorWindow
    {
        [MenuItem("Kaizen/About")]
        private static void About()
        {
            var window = CreateInstance(typeof(WindowAbout)) as WindowAbout;
            window.titleContent = new GUIContent("About");
            window.position = new Rect(0, 0, 300, 100);
            window.ShowUtility();
        }

        private void OnGUI()
        {
            GUILayout.Label("Kaizen is a framework created by Yafet Sutanto to speed up development.\nKaizen comes from the Japanese business philosophy of continuous improvement.");

        }
    }
}