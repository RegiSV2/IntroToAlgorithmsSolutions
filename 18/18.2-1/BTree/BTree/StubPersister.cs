using System;
using System.Collections.Generic;

namespace BTree
{
    internal class StubPersister<TKey, TData> : IBTreePersister<TKey, TData>
        where TKey : IComparable<TKey>
    {
        private readonly IDictionary<Guid, Node<TKey, TData>> _nodesPool = new Dictionary<Guid, Node<TKey, TData>>();
        private Guid _rootId = Guid.Empty;

        public void Save(Node<TKey, TData> node)
        {
            _nodesPool[node.Id] = node;
        }

        public void SaveRoot(Node<TKey, TData> root)
        {
            _rootId = root.Id;
            Save(root);
        }

        public void Remove(Guid nodeId)
        {
            _nodesPool.Remove(nodeId);
        }

        public Node<TKey, TData> Load(Guid nodeId)
        {
            return _nodesPool[nodeId];
        }
    }
}