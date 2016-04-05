using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace VanEmdeBoasTree
{
    public class ProtoYFastTree<TData> : IBoundedSet<TData>
    {
        private readonly YFastPerfectHashTable<TData> _hashTable;
        private readonly LinkedList<int> _keysInSet = new LinkedList<int>();
        private readonly int _maxPrefixOrder;

        public ProtoYFastTree(int universe)
        {
            Universe = universe;
            _hashTable = new YFastPerfectHashTable<TData>(universe);
            _maxPrefixOrder = (int) Math.Ceiling(Math.Log(universe, 2));
        }

        public int Universe { get; }
        public int? Min => _keysInSet.First?.Value;
        public TData MinData => _keysInSet.First == null ? default(TData) : _hashTable[_keysInSet.First.Value].Data;
        public int? Max => _keysInSet.Last?.Value;
        public TData MaxData => _keysInSet.Last == null ? default(TData) : _hashTable[_keysInSet.Last.Value].Data;

        public void Insert(int key, TData data)
        {
            if(_hashTable.ContainsKey(key))
                throw new InvalidOperationException("Key is already present");
            var linkedListNode = new LinkedListNode<int>(key);
            var nextNode = GetSuccessorNode(key);
            if (nextNode == null)
                _keysInSet.AddLast(linkedListNode);
            else
                _keysInSet.AddBefore(nextNode, linkedListNode);

            _hashTable.Add(new YFastNode<TData>(key, 0, linkedListNode, data));
            byte prefixOrder = 1;
            var keyPart = key >> 1;
            while (prefixOrder <= _maxPrefixOrder)
            {
                var node = new YFastNode<TData>(keyPart, prefixOrder);
                if (!_hashTable.Contains(node))
                    _hashTable.Add(node);
                keyPart = keyPart >> 1;
                prefixOrder++;
            }
        }

        public bool Contains(int value)
        {
            return _hashTable.ContainsKey(value);
        }

        public void Delete(int key)
        {
            if (!_hashTable.ContainsKey(key))
                return;
            var node = _hashTable[key];
            _keysInSet.Remove(node.LinkedListNode);

            _hashTable.Remove(key, 0);
            var keyPart = key;
            for (var prefixOrder = 0; prefixOrder < _maxPrefixOrder; ++prefixOrder)
            {
                var lastSign = keyPart & 2;
                var siblingKey = lastSign == 1 ? keyPart - 1 : keyPart + 1;
                if (!_hashTable.ContainsKey(siblingKey, prefixOrder))
                {
                    _hashTable.Remove(keyPart >> 1, prefixOrder + 1);
                }
                keyPart = keyPart >> 1;
            }
        }

        public int? GetSuccessor(int key)
        {
            return GetSuccessorNode(key)?.Value;
        }

        private LinkedListNode<int> GetSuccessorNode(int key)
        {
            if (key < 0)
                return _keysInSet.First;
            byte prefixOrder = 0;
            var keyPart = key;
            bool containsSuccessorPrefix;
            do
            {
                var lastSign = keyPart % 2;
                keyPart = keyPart >> 1;
                prefixOrder++;
                containsSuccessorPrefix =
                    lastSign == 0
                    && _hashTable.Contains(new YFastNode<TData>(keyPart, prefixOrder))
                    && _hashTable.Contains(new YFastNode<TData>((keyPart << 1) + 1, (byte) (prefixOrder - 1)));
            } while (prefixOrder < _maxPrefixOrder && !containsSuccessorPrefix);

            if (!containsSuccessorPrefix)
                return null;

            keyPart = (keyPart << 1) + 1;
            prefixOrder--;
            while (prefixOrder > 0)
            {
                prefixOrder--;
                keyPart = keyPart << 1;
                if (!_hashTable.Contains(new YFastNode<TData>(keyPart, prefixOrder)))
                {
                    Debug.Assert(_hashTable.Contains(new YFastNode<TData>(keyPart + 1, prefixOrder)));
                    keyPart += 1;
                }
            }

            return _hashTable[keyPart, prefixOrder].LinkedListNode;
        }

        public int? GetPredecessor(int key)
        {
            return GetPredecessorNode(key)?.Value;
        }

        private LinkedListNode<int> GetPredecessorNode(int key)
        {
            if (key >= Universe)
                return _keysInSet.Last;
            byte prefixOrder = 0;
            var keyPart = key;
            bool containsPredecessorPrefix;
            do
            {
                var lastSign = keyPart % 2;
                keyPart = keyPart >> 1;
                prefixOrder++;
                containsPredecessorPrefix =
                    lastSign == 1
                    && _hashTable.Contains(new YFastNode<TData>(keyPart, prefixOrder))
                    && _hashTable.Contains(new YFastNode<TData>(keyPart << 1, (byte)(prefixOrder - 1)));
            } while (prefixOrder < _maxPrefixOrder && !containsPredecessorPrefix);

            if (!containsPredecessorPrefix)
                return null;

            keyPart = keyPart << 1;
            prefixOrder--;
            while (prefixOrder > 0)
            {
                prefixOrder--;
                keyPart = keyPart << 1;
                if (_hashTable.Contains(new YFastNode<TData>(keyPart + 1, prefixOrder)))
                    keyPart += 1;
                else
                    Debug.Assert(_hashTable.Contains(new YFastNode<TData>(keyPart, prefixOrder)));
            }

            return _hashTable[keyPart, prefixOrder].LinkedListNode;
        }
    }
}
