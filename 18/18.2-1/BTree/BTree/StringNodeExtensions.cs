using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace BTree
{
    static class StringNodeExtensions
    {
        public static Node<TKey, TData> Child<TKey, TData>(this Node<TKey, TData> node, Node<TKey, TData> child)
            where TKey : IComparable<TKey>
        {
            node.AppendChild(child);
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