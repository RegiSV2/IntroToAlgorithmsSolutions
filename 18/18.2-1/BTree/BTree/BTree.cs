using System;
using System.Collections.Generic;
using BTree;

namespace BTree
{
    internal sealed class BTree<TKey, TData>
        where TKey : IComparable<TKey>
    {
        public Node<TKey, TData> Root { get; private set; }
        private readonly int _branchingFactor;

        public BTree(int branchingFactor)
        {
            _branchingFactor = branchingFactor;
            Root = new Node<TKey, TData>(true);
        }

        public SearchResult<TData> Search(TKey key)
        {
            return InternalSearch(Root, key);
        }

        private SearchResult<TData> InternalSearch(Node<TKey, TData> node, TKey key)
        {
            var idx = node.FindProperPos(key);
            if (idx < node.Keys.Count && node.Keys[idx].CompareTo(key) == 0)
                return SearchResult<TData>.CreateFound(node.GetData(idx).Value);
            if(!node.IsLeaf)
                return InternalSearch(node.Children[idx], key);
            return SearchResult<TData>.CreateNotFound();
        }

        public void Insert(KeyValuePair<TKey, TData> item)
        {
            if (IsFull(Root))
            {
                var newRoot = new Node<TKey, TData>(false);
                newRoot.AppendChild(Root);
                SplitChild(newRoot, 0);
                Root = newRoot;
            }
            InsertNonFull(Root, item);
        }

        private void SplitChild(Node<TKey, TData> parent, int childIdx)
        {
            if(parent.Children.Count <= childIdx)
                throw new ArgumentException("Invalid child index");

            var firstChild = parent.Children[childIdx];
            var secondChild = new Node<TKey, TData>(firstChild.IsLeaf);
            
            parent.InsertChild(firstChild.GetData(_branchingFactor - 1), secondChild);
            for (var i = _branchingFactor; i < firstChild.Size; i++)
                secondChild.AppendData(firstChild.GetData(i));
            if (!firstChild.IsLeaf)
            {
                for (var i = _branchingFactor; i <= firstChild.Size; i++)
                    secondChild.AppendChild(firstChild.Children[i]);
            }
            firstChild.RemoveDataRange(_branchingFactor - 1, _branchingFactor);
        }

        private void InsertNonFull(Node<TKey, TData> node, KeyValuePair<TKey, TData> item)
        {
            if (node.IsLeaf)
                node.InsertData(item);
            else
            {
                var insertionIdx = node.FindProperPos(item.Key);
                if (IsFull(node.Children[insertionIdx]))
                {
                    SplitChild(node, insertionIdx);
                    if (insertionIdx < node.Size && item.Key.CompareTo(node.Keys[insertionIdx]) > 0)
                        insertionIdx++;
                }
                InsertNonFull(node.Children[insertionIdx], item);
            }
        }

        private bool IsFull(Node<TKey, TData> node)
        {
            return node.Size == _branchingFactor*2 - 1;
        }
    }
}