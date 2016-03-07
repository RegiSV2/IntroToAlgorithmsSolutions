using System;
using System.Collections.Generic;

namespace BTree
{
    /// <summary>
    /// Base class for B-Tree nodes
    /// </summary>
    [Serializable]
    internal class Node<TKey, TData>
        where TKey : IComparable<TKey>
    {
        public List<TData> Data { get; set; }
        public bool IsLeaf { get; private set; }
        public List<TKey> Keys { get; set; }
        public List<Guid> Children { get; set; }
        public Guid Id { get; private set; }

        private Node()
        { } 

        public Node(bool isLeaf)
        {
            Data = new List<TData>();
            Keys = new List<TKey>();
            IsLeaf = isLeaf;
            Id = Guid.NewGuid();

            if (isLeaf) return;
            Children = new List<Guid>();
        }

        public int Size => Keys.Count;

        public KeyValuePair<TKey, TData> GetData(int index)
        {
            if (index >= Size)
                throw new ArgumentException();

            return new KeyValuePair<TKey, TData>(Keys[index], Data[index]);
        }

        public int FindProperPos(TKey key)
        {
            var lower = 0;
            var upper = Keys.Count - 1;
            while (upper >= lower)
            {
                var median = (upper - lower) / 2 + lower;
                if (key.CompareTo(Keys[median]) == 1)
                    lower = median + 1;
                else
                    upper = median - 1;
            }
            return lower;
        }

        public int IndexOf(TKey key)
        {
            var result = FindProperPos(key);
            if (result < Keys.Count && Keys[result].CompareTo(key) == 0)
                return result;
            return -1;
        }
        
        public void RemoveDataRange(int start, int count)
        {
            Keys.RemoveRange(start, count);
            Data.RemoveRange(start, count);
            if(!IsLeaf)
                Children.RemoveRange(start + 1, count);
        }

        public void InsertData(KeyValuePair<TKey, TData> data)
        {
            var insertionIdx = FindProperPos(data.Key);
            Keys.Insert(insertionIdx, data.Key);
            Data.Insert(insertionIdx, data.Value);
        }

        public void InsertChild(KeyValuePair<TKey, TData> newData, Guid newChild)
        {
            var insertionIdx = FindProperPos(newData.Key);
            if (Keys.Count > insertionIdx && newData.Key.CompareTo(Keys[insertionIdx]) == 0)
                Data[insertionIdx] = newData.Value;
            else
            {
                Keys.Insert(insertionIdx, newData.Key);
                Data.Insert(insertionIdx, newData.Value);
                Children.Insert(insertionIdx + 1, newChild);
            }
        }
    }
}