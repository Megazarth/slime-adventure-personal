using System.Collections.Generic;

namespace Kaizen
{
    public class Pool<T> : Stack<T>
    {
        public delegate T InstantiateDelegate();
        public delegate void SpawnDelegate(T item);
        public delegate void UnspawnDelegate(T item);
        public delegate void EditorInfoDelegate(T item);

        private InstantiateDelegate onInstantiate;
        private SpawnDelegate onSpawn;
        private UnspawnDelegate onUnspawn;
        private EditorInfoDelegate editorInfo;

        public Pool(InstantiateDelegate instantiateDelegate, SpawnDelegate spawnDelegate, UnspawnDelegate unspawnDelegate)
        {
            onInstantiate = instantiateDelegate;
            onSpawn = spawnDelegate;
            onUnspawn = unspawnDelegate;
        }

#if UNITY_EDITOR
        public Pool(InstantiateDelegate instantiateDelegate, SpawnDelegate spawnDelegate, UnspawnDelegate unspawnDelegate, EditorInfoDelegate editorInfoDelegate)
        {
            onInstantiate = instantiateDelegate;
            onSpawn = spawnDelegate;
            onUnspawn = unspawnDelegate;
            editorInfo = editorInfoDelegate;
        }
#endif

        public new void Push(T item)
        {
            onUnspawn(item);

            if (Contains(item))
            {
#if UNITY_EDITOR
                editorInfo(item);
#endif
                return;
            }

            base.Push(item);
        }

        public new T Pop()
        {
            T item = default;
            if (Count == 0)
            {
                item = onInstantiate();
            }
            else
            {
                item = base.Pop();
            }
            onSpawn(item);
            return item;
        }
    }
}