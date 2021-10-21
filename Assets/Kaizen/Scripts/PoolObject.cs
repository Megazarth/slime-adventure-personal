using System.Collections;
using UnityEngine;

namespace Kaizen
{
    public class PoolObject : MonoBehaviour
    {
        public string id;
        [Tooltip("Automatically unspawn this GameObject after the given duration.")]
        public bool automaticUnspawn;
        public float duration;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(id))
                id = gameObject.name;
        }

        private void OnEnable()
        {
            if (automaticUnspawn)
                StartCoroutine(UnspawnCoroutine());
        }

        private IEnumerator UnspawnCoroutine()
        {
#if UNITY_EDITOR
            if (duration <= 0 && automaticUnspawn)
                Debug.LogWarning($"The duration for unspawning {id} is 0!", gameObject);
#endif
            var t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                yield return null;
            }
            PoolManager.Instance.Unspawn(gameObject);
        }

        public void Destroy()
        {
            PoolManager.Instance.Unspawn(gameObject);
        }
    }
}