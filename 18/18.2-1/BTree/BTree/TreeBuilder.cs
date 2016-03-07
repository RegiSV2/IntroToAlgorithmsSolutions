using System;
using System.Collections.Generic;

namespace BTree
{
    internal class TreeBuilder<TKey>
        where TKey : IComparable<TKey>
    {
        private readonly IBTreePersister<TKey, TKey> _persister;

        public TreeBuilder(IBTreePersister<TKey, TKey> persister)
        {
            _persister = persister;
        }

        public NodeBuilder InnerNode()
        {
            return new NodeBuilder(_persister, new Node<TKey, TKey>(false));
        }

        public NodeBuilder Leaf()
        {
            return new NodeBuilder(_persister, new Node<TKey, TKey>(true));
        }

        public class NodeBuilder
        {
            private readonly IBTreePersister<TKey, TKey> _persister;
            private readonly Node<TKey, TKey> _node;

            public NodeBuilder(IBTreePersister<TKey, TKey> persister, Node<TKey, TKey> node)
            {
                _persister = persister;
                _node = node;
            }

            public Node<TKey, TKey> Build()
            {
                _persister.Save(_node);
                return _node;
            } 

            public NodeBuilder Child(NodeBuilder child)
            {
                var childNode = child.Build();
                _node.Children.Add(childNode.Id);
                return this;
            }

            public NodeBuilder Key(TKey key)
            {
                _node.Keys.Add(key);
                _node.Data.Add(key);
                return this;
            }

            private static KeyValuePair<TKey, TKey> NodeData(TKey key)
            {
                return new KeyValuePair<TKey, TKey>(key, key);
            }
        }
    }
}