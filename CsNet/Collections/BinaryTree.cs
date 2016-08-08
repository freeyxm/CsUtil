using System;

namespace CsNet.Collections
{
    public abstract class BinaryTree<K, V>
    {
        public delegate void TraverseActionRef(K key, ref V value);
        public delegate void TraverseAction(K key, V value);

        public abstract void Add(K key, V value);

        public abstract bool Remove(K key);

        public abstract bool ContainsKey(K key);

        public abstract V this[K key] { get; set; }

        public abstract void Clear();

        public abstract void TraversePreOrder_r(TraverseAction action);
        public abstract void TraverseInOrder_r(TraverseAction action);
        public abstract void TraversePostOrder_r(TraverseAction action);
    }
}
