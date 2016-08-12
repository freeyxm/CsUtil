using System;

namespace CsNet.Collections
{
    interface IBinaryTree<K, V>
    {
        bool Add(K key, V value);
        bool Remove(K key);
        bool ContainsKey(K key);
        void Clear();
        V this[K key] { get; set; }
        int Count { get; }

        void TraversePreOrder(Action<K, V> action);
        void TraverseInOrder(Action<K, V> action);
        void TraversePostOrder(Action<K, V> action);
    }
}
