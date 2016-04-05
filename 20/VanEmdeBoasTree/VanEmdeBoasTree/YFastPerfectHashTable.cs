using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace VanEmdeBoasTree
{
    class YFastPerfectHashTable<TData> : ICollection<YFastNode<TData>>
    {
        private readonly HashTableNode[] _set;
        private readonly int _maxBitsCount;

        public YFastPerfectHashTable(int universe)
        {
            var universeBitsCount = (int)Math.Ceiling(Math.Log(universe, 2));
            var size = 0;
            for (var i = 0; i <= universeBitsCount; i++)
                size += (int) Math.Pow(2, i);
            _maxBitsCount = (int) Math.Ceiling(Math.Log(size, 2));
            _set = new HashTableNode[size];
        }

        public int Count { get; private set; }

        public bool IsReadOnly => false;

        public YFastNode<TData> this[int key]
        {
            get
            {
                var result = _set[ToIndex(key, 0)];
                if(!result.IsPresent)
                    throw new ArgumentException();
                return new YFastNode<TData>(key, 0, result.Node, result.Data);
            }
        }

        public YFastNode<TData> this[int key, byte prefixOrder]
        {
            get
            {
                var idx = ToIndex(key, prefixOrder);
                if(!_set[idx].IsPresent)
                    throw new ArgumentException();
                return new YFastNode<TData>(key, prefixOrder, _set[idx].Node, _set[idx].Data);
            }
        }

        public IEnumerator<YFastNode<TData>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(YFastNode<TData> item)
        {
            var idx = ToIndex(item);
            Debug.Assert(idx < _set.Length);
            if (!_set[idx].IsPresent)
                Count++;
            _set[idx] = ToHashTableNode(item);
        }

        public bool Contains(YFastNode<TData> item)
        {
            var idx = ToIndex(item);
            Debug.Assert(idx < _set.Length);
            return _set[idx].IsPresent;
        }

        public bool ContainsKey(int key)
        {
            return ContainsKey(key, 0);
        }

        public bool ContainsKey(int key, int prefixOrder)
        {
            if(key < 0 || prefixOrder < 0)
                throw new ArgumentOutOfRangeException();
            var idx = ToIndex(key, prefixOrder);
            return idx < _set.Length && _set[idx].IsPresent;
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void CopyTo(YFastNode<TData>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(YFastNode<TData> item)
        {
            throw new NotImplementedException();
        }

        public void Remove(int key, int prefixOrder)
        {
            var idx = ToIndex(key, prefixOrder);
            _set[idx] = new HashTableNode();
        }

        private int ToIndex(YFastNode<TData> item)
        {
            return ToIndex(item.Key, item.PrefixOrder);
        }

        private int ToIndex(int key, int prefixOrder)
        {
            var prefix = 0;
            for (var i = 0; i < prefixOrder; i++)
                prefix = prefix | (1 << (_maxBitsCount - i - 1));

            return prefix | key;
        }

        private YFastNode<TData> FromIndex(int index)
        {
            byte prefixOrder = 0;
            var key = index;
            var bitIdx = _maxBitsCount - prefixOrder - 1;
            while (key >> bitIdx != 0)
            {
                prefixOrder++;
                key = key - (1 << bitIdx);
                bitIdx = _maxBitsCount - prefixOrder - 1;
            }
            var hashTableNode = _set[index];
            return new YFastNode<TData>(key, prefixOrder, hashTableNode.Node, hashTableNode.Data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private HashTableNode ToHashTableNode(YFastNode<TData> yFastNode)
        {
            return new HashTableNode(yFastNode.Data, yFastNode.LinkedListNode);
        }

        private class Enumerator : IEnumerator<YFastNode<TData>>
        {
            private readonly YFastPerfectHashTable<TData> _hashTable;
            private int _currentIndex;

            public Enumerator(YFastPerfectHashTable<TData> hashTable)
            {
                _hashTable = hashTable;
                _currentIndex = -1;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                _currentIndex = Array.FindIndex(_hashTable._set, _currentIndex + 1, x => x.IsPresent);
                return _currentIndex != -1;
            }

            public void Reset()
            {
                _currentIndex = Array.FindIndex(_hashTable._set, x => x.IsPresent);
            }

            public YFastNode<TData> Current => _hashTable.FromIndex(_currentIndex);

            object IEnumerator.Current => Current;
        }

        private struct HashTableNode
        {
            public bool IsPresent { get; set; }
            public TData Data { get; set; }
            public LinkedListNode<int> Node { get; set; }

            public HashTableNode(TData data, LinkedListNode<int> node)
                : this()
            {
                Data = data;
                Node = node;
                IsPresent = true;
            }
        }
    }
}