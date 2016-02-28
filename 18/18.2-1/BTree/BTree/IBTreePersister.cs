using System;

namespace BTree
{
    internal interface IBTreePersister<TKey, TData>
        where TKey : IComparable<TKey>
    {
        void Save(Node<TKey, TData> node);

        void SaveRoot(Node<TKey, TData> root);

        Node<TKey, TData> Load(Guid nodeId);
    }
}