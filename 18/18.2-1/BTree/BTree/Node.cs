using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BTree
{
    /// <summary>
    /// Base class for B-Tree nodes
    /// </summary>
    internal sealed class Node<TKey, TData>
        where TKey : IComparable<TKey>
    {
        private readonly List<Node<TKey, TData>> _children;
        private readonly List<TKey> _keys;
        private readonly List<TData> _data;
        public bool IsLeaf { get; }
        public IReadOnlyList<TKey> Keys { get; }
        public IReadOnlyList<Node<TKey, TData>> Children { get; }

        public Node(bool isLeaf)
        {
            _data = new List<TData>();
            _keys = new List<TKey>();
            Keys = new ReadOnlyCollection<TKey>(_keys);
            IsLeaf = isLeaf;

            if (isLeaf) return;
            _children = new List<Node<TKey, TData>>();
            Children = new ReadOnlyCollection<Node<TKey, TData>>(_children);
        }

        public int Size => _keys.Count;

        public void AppendData(KeyValuePair<TKey, TData> data)
        {
            _keys.Add(data.Key);
            _data.Add(data.Value);
        }

        public void AppendChild(Node<TKey, TData> child)
        {
            _children.Add(child);
        }

        public KeyValuePair<TKey, TData> GetData(int index)
        {
            if(index >= _keys.Count)
                throw new ArgumentException();

            return new KeyValuePair<TKey, TData>(_keys[index], _data[index]);
        }

        public void RemoveDataRange(int start, int count)
        {
            _keys.RemoveRange(start, count);
            _data.RemoveRange(start, count);
            if(!IsLeaf)
                _children.RemoveRange(start + 1, count);
        }

        public void InsertData(KeyValuePair<TKey, TData> data)
        {
            var insertionIdx = FindProperPos(data.Key);
            _keys.Insert(insertionIdx, data.Key);
            _data.Insert(insertionIdx, data.Value);
        }

        public void InsertChild(KeyValuePair<TKey, TData> newData, Node<TKey, TData> newChild)
        {
            var insertionIdx = FindProperPos(newData.Key);
            if (_keys.Count > insertionIdx && newData.Key.CompareTo(_keys[insertionIdx]) == 0)
                _data[insertionIdx] = newData.Value;
            else
            {
                _keys.Insert(insertionIdx, newData.Key);
                _data.Insert(insertionIdx, newData.Value);
                _children.Insert(insertionIdx + 1, newChild);
            }
        }

        public int FindProperPos(TKey key)
        {
            var lower = 0;
            var upper = Keys.Count - 1;
            while (upper >= lower)
            {
                var median = (upper - lower)/2 + lower;
                if (key.CompareTo(_keys[median]) == 1)
                    lower = median + 1;
                else
                    upper = median - 1;
            }
            return lower;
        }
    }
}