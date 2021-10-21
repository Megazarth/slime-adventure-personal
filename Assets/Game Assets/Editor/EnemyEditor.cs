using UnityEditor;
using UnityEngine;

namespace Slime
{
    [CustomEditor(typeof(Enemy)), CanEditMultipleObjects]
    public class EnemyEditor : Editor
    {
        private SerializedObject so;

        private void OnSceneGUI()
        {
            so = new SerializedObject(target);
            var serializedEnemyPatrolData = so.FindProperty("patrolDatas");

            for (int i = 0; i < serializedEnemyPatrolData.arraySize; i++)
            {
                var element = serializedEnemyPatrolData.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("position").vector2Value = Handles.PositionHandle(element.FindPropertyRelative("position").vector2Value, Quaternion.identity);
            }
            so.ApplyModifiedProperties();
        }
    }
}