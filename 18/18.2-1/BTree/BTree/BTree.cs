using System;
using System.Collections.Generic;

namespace BTree
{
    internal sealed class BTree<TKey, TData>
        where TKey : IComparable<TKey>
    {
        public Node<TKey, TData> Root { get; private set; }
        private readonly int _branchingFactor;
        private readonly IBTreePersister<TKey, TData> _persister; 

        public BTree(int branchingFactor, IBTreePersister<TKey, TData> persister)
        {
            _branchingFactor = branchingFactor;
            _persister = persister;
            Root = new Node<TKey, TData>(true);
            _persister.SaveRoot(Root);
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
            if (!node.IsLeaf)
            {
                var child = _persister.Load(node.Children[idx]);
                return InternalSearch(child, key);
            }
            return SearchResult<TData>.CreateNotFound();
        }

        public void Insert(KeyValuePair<TKey, TData> item)
        {
            if (IsFull(Root))
            {
                var newRoot = new Node<TKey, TData>(false);
                newRoot.AppendChild(Root.Id);
                SplitChild(newRoot, 0);
                Root = newRoot;
                _persister.SaveRoot(Root);
            }
            InsertNonFull(Root, item);
        }

        private void SplitChild(Node<TKey, TData> parent, int childIdx)
        {
            if(parent.Children.Count <= childIdx)
                throw new ArgumentException("Invalid child index");

            var firstChild = _persister.Load(parent.Children[childIdx]);
            var secondChild = new Node<TKey, TData>(firstChild.IsLeaf);
            for (var i = _branchingFactor; i < firstChild.Size; i++)
                secondChild.AppendData(firstChild.GetData(i));
            if (!firstChild.IsLeaf)
            {
                for (var i = _branchingFactor; i <= firstChild.Size; i++)
                    secondChild.AppendChild(firstChild.Children[i]);
            }
            parent.InsertChild(firstChild.GetData(_branchingFactor - 1), secondChild.Id);
            firstChild.RemoveDataRange(_branchingFactor - 1, _branchingFactor);

            _persister.Save(parent);
            _persister.Save(firstChild);
            _persister.Save(secondChild);
        }

        private void InsertNonFull(Node<TKey, TData> node, KeyValuePair<TKey, TData> item)
        {
            while (true)
            {
                if (node.IsLeaf)
                {
                    node.InsertData(item);
                    _persister.Save(node);
                    return;
                }
                node = GetChildForInsertion(node, item);
            }
        }

        private Node<TKey, TData> GetChildForInsertion(Node<TKey, TData> node, KeyValuePair<TKey, TData> item)
        {
            var insertionIdx = node.FindProperPos(item.Key);
            var child = _persister.Load(node.Children[insertionIdx]);
            if (IsFull(child))
            {
                SplitChild(node, insertionIdx);
                if (insertionIdx < node.Size && item.Key.CompareTo(node.Keys[insertionIdx]) > 0)
                {
                    insertionIdx++;
                    child = _persister.Load(node.Children[insertionIdx]);
                }
            }
            return child;
        }

        private bool IsFull(Node<TKey, TData> node)
        {
            return node.Size == _branchingFactor * 2 - 1;
        }
    }
}