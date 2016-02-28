using System;
using System.Collections.Generic;

namespace BTree
{
    static class StringNodeExtensions
    {
        public static Node<TKey, TData> Child<TKey, TData>(this Node<TKey, TData> node,
            IBTreePersister<TKey, TData> persister, Node<TKey, TData> child)
            where TKey : IComparable<TKey>
        {
            node.AppendChild(child.Id);
            persister.Save(child);
            return node;
        }  

        public static Node<string, string> Key(this Node<string, string> node, string key)
        {
            node.AppendData(NodeData(key));
            return node;
        }

        private static KeyValuePair<string, string> NodeData(string key)
        {
            return new KeyValuePair<string, string>(key, key);
        }
    }
}