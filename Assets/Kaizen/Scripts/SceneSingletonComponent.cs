using UnityEngine;

namespace Kaizen
{
    public class SceneSingletonComponent<T> : MonoBehaviour where T : Component
    {
        public static T Instance;

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                return;
            }
            Destroy(gameObject);
        }
    }
}