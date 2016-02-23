using System;
using System.Collections.Generic;
using System.Data;

namespace BTree
{
    /// <summary>
    /// Base class for B-Tree nodes
    /// </summary>
    sealed class Node<TKey, TData>
        where TKey : IComparable<TKey>
    {
        public List<Node<TKey, TData>> Children { get; }
        public List<TKey> Keys { get; }
        public bool IsLeaf { get; }
        private List<TData> _data;
        public int Size => _data.Count;

        public Node(bool isLeaf)
        {
            Keys = new List<TKey>();
            _data = new List<TData>();
            IsLeaf = isLeaf;
            if(!isLeaf)
                Children = new List<Node<TKey, TData>>();
        }

        public void AppendData(KeyValuePair<TKey, TData> data)
        {
            Keys.Add(data.Key);
            _data.Add(data.Value);
        }

        public KeyValuePair<TKey, TData> GetData(int index)
        {
            if(index >= Keys.Count)
                throw new ArgumentException();

            return new KeyValuePair<TKey, TData>(Keys[index], _data[index]);
        }

        public void RemoveDataRange(int start, int count)
        {
            Keys.RemoveRange(start, count);
            _data.RemoveRange(start, count);
            if(!IsLeaf)
                Children.RemoveRange(start + 1, count);
        }

        public void InsertData(KeyValuePair<TKey, TData> data, int index)
        {
            Keys.Insert(index, data.Key);
            _data.Insert(index, data.Value);
        }

        public void InsertData(KeyValuePair<TKey, TData> newData, Node<TKey, TData> newChild)
        {
            var insertionIdx = 0;
            while (insertionIdx < Keys.Count && newData.Key.CompareTo(Keys[insertionIdx]) == 1)
                insertionIdx++;
            Keys.Insert(insertionIdx, newData.Key);
            _data.Insert(insertionIdx, newData.Value);
            Children.Insert(insertionIdx + 1, newChild);
        }
    }
}