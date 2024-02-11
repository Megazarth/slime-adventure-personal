using System.Collections.Generic;
using UnityEngine;

namespace Kaizen
{
    /// <summary>
    /// Object pooling utility class. Don't forget to set Script Execution Order.
    /// Go to Edit > Project Settings > Script Execution Order, then add PoolManager and set it after UnityEngine.EventSystems.EventSystem
    /// </summary>
    public class PoolManager : SingletonComponent<PoolManager>
    {
        private Dictionary<string, Pool<GameObject>> poolDictID;

        private Transform container;

        [SerializeField]
        private PoolObject[] poolObjects = new PoolObject[] {};

        protected override void Awake()
        {
            base.Awake();

            poolDictID = new Dictionary<string, Pool<GameObject>>(poolObjects.Length);

            container = transform;

            InitializePool();
        }

        private void InitializePool()
        {
            foreach (var poolObject in poolObjects)
            {
                GameObject InstantiateDelegate()
                {
                    return Instantiate(poolObject.gameObject, container);
                }
                void SpawnDelegate(GameObject item)
                {
                    if (!item.activeSelf)
                        item.SetActive(true);
                }
                void UnspawnDelegate(GameObject item)
                {
                    if (item.activeSelf)
                        item.SetActive(false);
                }
#if UNITY_EDITOR
                void EditorInfoDelegate(GameObject item)
                {
                    Debug.LogWarning($"This item of ID: {item.GetComponent<PoolObject>().id} was found inside the pool. Please check unspawn invoke.", item);
                }
                var newPool = new Pool<GameObject>(InstantiateDelegate, SpawnDelegate, UnspawnDelegate, EditorInfoDelegate);
#else
                var newPool = new Pool<GameObject>(InstantiateDelegate, SpawnDelegate, UnspawnDelegate);
#endif

                poolDictID.Add(poolObject.id, newPool);
            }
        }

        public GameObject Spawn(string id, Vector3 position, Quaternion rotation)
        {
            if (poolDictID.TryGetValue(id, out Pool<GameObject> pool))
            {
                var go = pool.Pop();
                go.transform.position = position;
                go.transform.rotation = rotation;
                return go;
            }
#if UNITY_EDITOR
            Debug.LogError($"The spawnable object with ID:{id} is not yet registered. Please register object at the PoolManager.");
#endif
            return null;
        }

        public GameObject Spawn(string id, Transform parent, bool worldPositionStays = true)
        {
            if (poolDictID.TryGetValue(id, out Pool<GameObject> pool))
            {
                var go = pool.Pop();
                go.transform.SetParent(parent, worldPositionStays);
                return go;
            }
#if UNITY_EDITOR
            Debug.LogError($"The spawnable object with ID:{id} is not yet registered. Please register object at the PoolManager.");
#endif
            return null;
        }

        public GameObject Spawn(string id, Vector3 position, Quaternion rotation, Transform parent, bool worldPositionStays = true)
        {
            var instantiated = Spawn(id, parent, worldPositionStays);
            if (instantiated == null)
                return null;

            instantiated.transform.position = position;
            instantiated.transform.rotation = rotation;
            return instantiated;
        }

        public T Spawn<T>(string id, Transform parent, bool worldPositionStays = true)
        {
            var instantiated = Spawn(id, parent, worldPositionStays);
            return instantiated.GetComponent<T>();
        }

        public T Spawn<T>(string id, Vector3 position, Quaternion rotation, Transform parent, bool worldPositionStays = true)
        {
            var instantiated = Spawn(id, position, rotation, parent, worldPositionStays);
            return instantiated.GetComponent<T>();
        }

        public void Unspawn(GameObject item)
        {
            if (poolDictID.TryGetValue(item.GetComponent<PoolObject>().id, out Pool<GameObject> pool))
            {
                pool.Push(item);
            }
        }
    }
}

