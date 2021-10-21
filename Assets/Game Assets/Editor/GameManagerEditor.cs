using UnityEditor;

namespace Slime
{
    [CustomEditor(typeof(GameManager))]
    public class GameManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var gameManager = target as GameManager;
            var collection = gameManager.CurrentCollection != null ? gameManager.CurrentCollection.name : "";
            var level = gameManager.CurrentLevel != null ? gameManager.CurrentLevel.number : 0;

            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Collection : {collection}");
            EditorGUILayout.LabelField($"Level : {level}");
            EditorGUILayout.LabelField($"State : {gameManager.State}");

            EditorUtility.SetDirty(target);
        }
    }
}