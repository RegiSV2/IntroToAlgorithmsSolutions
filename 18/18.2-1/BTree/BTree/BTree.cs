using System;
using System.Collections.Generic;
using System.Linq;

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

        public BTree(int branchingFactor, IBTreePersister<TKey, TData> persister, Node<TKey, TData> root)
        {
            //TODO: verify BTree-constraints on root
            _branchingFactor = branchingFactor;
            _persister = persister;
            Root = root;
            _persister.SaveRoot(root);
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
                newRoot.Children.Add(Root.Id);
                SplitChild(newRoot, 0);
                Root = newRoot;
                _persister.SaveRoot(Root);
            }
            InsertNonFull(Root, item);
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

        public bool Remove(TKey key)
        {
            var result = RemoveInternal(Root, key);
            if (Root.Size == 0 && !Root.IsLeaf && Root.Children.Any())
            {
                _persister.Remove(Root.Id);
                Root = _persister.Load(Root.Children.First());
                _persister.SaveRoot(Root);
            }
            return result;
        }

        private bool RemoveInternal(Node<TKey, TData> node, TKey key)
        {
            if (node.IsLeaf)
            {
                return TryRemoveKey(node, key);
            }
            else
            {
                var idx = node.FindProperPos(key);
                if (idx >= node.Size || node.Keys[idx].CompareTo(key) != 0)
                {
                    var child = _persister.Load(node.Children[idx]);
                    var nodeToRecurse = child.Size >= _branchingFactor
                        ? child
                        : PrepareNodesForRecursion(node, child, idx);
                    return RemoveInternal(nodeToRecurse, key);
                }
                else
                {
                    var leftChild = _persister.Load(node.Children[idx]);
                    if (leftChild.Size >= _branchingFactor)
                    {
                        var predecessor = RemoveEdgeKey(leftChild, EdgeKeyType.Max);
                        node.Keys[idx] = predecessor.Key;
                        node.Data[idx] = predecessor.Value;
                        _persister.Save(node);
                    }
                    else
                    {
                        var rightChild = _persister.Load(node.Children[idx + 1]);
                        if (rightChild.Size >= _branchingFactor)
                        {
                            var successor = RemoveEdgeKey(rightChild, EdgeKeyType.Min);
                            node.Keys[idx] = successor.Key;
                            node.Data[idx] = successor.Value;
                            _persister.Save(node);
                        }
                        else
                        {
                            var nodeToRecurse = MergeNodes(node, idx, leftChild, rightChild);
                            RemoveInternal(nodeToRecurse, key);
                        }
                    }
                    return true;
                }
            }
        }

        private bool TryRemoveKey(Node<TKey, TData> node, TKey key)
        {
            var keyPos = node.IndexOf(key);
            if (keyPos == -1)
                return false;
            node.RemoveDataRange(keyPos, 1);
            _persister.Save(node);
            return true;
        }

        private Node<TKey, TData> PrepareNodesForRecursion(Node<TKey, TData> node, Node<TKey, TData> child, int childIdx)
        {
            Node<TKey, TData> leftSibling = null, rightSibling = null;
            if (childIdx > 0)
            {
                leftSibling = _persister.Load(node.Children[childIdx - 1]);
                if (leftSibling.Size >= _branchingFactor)
                {
                    child.Keys.Insert(0, node.Keys[childIdx - 1]);
                    child.Data.Insert(0, node.Data[childIdx - 1]);
                    if (!child.IsLeaf)
                        child.Children.Insert(0, leftSibling.Children.Last());
                    node.Keys[childIdx - 1] = leftSibling.Keys.Last();
                    node.Data[childIdx - 1] = leftSibling.Data.Last();
                    leftSibling.RemoveDataRange(leftSibling.Size - 1, 1);
                    _persister.Save(leftSibling);
                    return child;
                }
            }
            if (childIdx < node.Size)
            {
                rightSibling = _persister.Load(node.Children[childIdx + 1]);
                if (rightSibling.Size >= _branchingFactor)
                {
                    child.Keys.Add(node.Keys[childIdx]);
                    child.Data.Add(node.Data[childIdx]);
                    if(!child.IsLeaf)
                        child.Children.Add(rightSibling.Children.First());
                    node.Keys[childIdx] = rightSibling.Keys.First();
                    node.Data[childIdx] = rightSibling.Data.First();
                    rightSibling.RemoveDataRange(0, 1);
                    _persister.Save(rightSibling);
                    return child;
                }
            }
            return leftSibling != null
                ? MergeNodes(node, childIdx - 1, leftSibling, child)
                : MergeNodes(node, childIdx, child, rightSibling);
        }

        private Node<TKey, TData> MergeNodes(Node<TKey, TData> node, int keyIdx, Node<TKey, TData> left, Node<TKey, TData> right)
        {
            var mergedNode = new Node<TKey, TData>(left.IsLeaf);
            var nodeSize = left.Size + right.Size + 1;
            mergedNode.Keys = new List<TKey>(nodeSize);
            mergedNode.Keys.AddRange(left.Keys);
            mergedNode.Keys.Add(node.Keys[keyIdx]);
            mergedNode.Keys.AddRange(right.Keys);
            mergedNode.Data = new List<TData>(nodeSize);
            mergedNode.Data.AddRange(left.Data);
            mergedNode.Data.Add(node.Data[keyIdx]);
            mergedNode.Data.AddRange(right.Data);
            if (!mergedNode.IsLeaf)
            {
                mergedNode.Children = new List<Guid>(nodeSize + 1);
                mergedNode.Children.AddRange(left.Children);
                mergedNode.Children.AddRange(right.Children);
            }

            node.Children[keyIdx] = mergedNode.Id;
            node.Children.RemoveAt(keyIdx + 1);
            node.Keys.RemoveAt(keyIdx);
            node.Data.RemoveAt(keyIdx);

            _persister.Save(node);
            _persister.Save(mergedNode);
            _persister.Remove(left.Id);
            _persister.Remove(right.Id);

            return mergedNode;
        }

        private KeyValuePair<TKey, TData> RemoveEdgeKey(Node<TKey, TData> node, EdgeKeyType edgeKeyType)
        {
            if (node.IsLeaf)
            {
                var keyIdx = edgeKeyType == EdgeKeyType.Max ? node.Keys.Count - 1 : 0;
                var result = new KeyValuePair<TKey, TData>(node.Keys[keyIdx], node.Data[keyIdx]);
                node.RemoveDataRange(keyIdx, 1);
                _persister.Save(node);
                return result;
            }
            else
            {
                var childIdx = edgeKeyType == EdgeKeyType.Max ? node.Children.Count - 1 : 0;
                var lastChild = _persister.Load(node.Children[childIdx]);
                var nodeToRecurse = lastChild.Size >= _branchingFactor
                    ? PrepareNodesForRecursion(node, lastChild, childIdx)
                    : lastChild;
                return RemoveEdgeKey(nodeToRecurse, edgeKeyType);
            }
        }

        private void SplitChild(Node<TKey, TData> parent, int childIdx)
        {
            if (parent.Children.Count <= childIdx)
                throw new ArgumentException("Invalid child index");

            var firstChild = _persister.Load(parent.Children[childIdx]);
            var secondChild = new Node<TKey, TData>(firstChild.IsLeaf);
            for (var i = _branchingFactor; i < firstChild.Size; i++)
            {
                secondChild.Keys.Add(firstChild.Keys[i]);
                secondChild.Data.Add(firstChild.Data[i]);
            }
            if (!firstChild.IsLeaf)
            {
                for (var i = _branchingFactor; i <= firstChild.Size; i++)
                    secondChild.Children.Add(firstChild.Children[i]);
            }
            parent.InsertChild(firstChild.GetData(_branchingFactor - 1), secondChild.Id);
            firstChild.RemoveDataRange(_branchingFactor - 1, _branchingFactor);

            _persister.Save(parent);
            _persister.Save(firstChild);
            _persister.Save(secondChild);
        }

        private enum EdgeKeyType
        {
            Min = 0,
            Max = 1
        }
    }
}