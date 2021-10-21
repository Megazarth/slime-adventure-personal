using System;
using System.Collections.Generic;
using UnityEngine;


namespace Slime
{
    [Serializable]
    [CreateAssetMenu(fileName = "New Levels", menuName = "Slime/LevelLibrary")]
    public class Collection : ScriptableObject
    {
        public int number;
        public new string name;
        public string description;
        public Collection next;
        public List<Level> levels;

#if UNITY_EDITOR
        private void OnValidate()
        {
            var baseNumber = (number - 1) * 10;
            for (int i = 0; i < levels.Count; i++)
            {
                var level = levels[i];
                level.number = baseNumber + i + 1;
            }
        }
#endif
    }

    [Serializable]
    public class Level
    {
        public int number;
        public SceneReference scene;
        // #if UNITY_EDITOR
        //         public SceneAsset scene;
        // #endif // UNITY_EDITOR
        // [Tooltip("Must be the same as the scene's filename without extension.")]
        // public string sceneName;
        public string description;
    }
}
