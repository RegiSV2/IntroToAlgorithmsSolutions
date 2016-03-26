using System;
using System.Collections.Generic;
using System.Linq;

namespace FibonacciHeap
{
    internal class FibonacciHeap<TData>
    {
        private readonly TData _minDataValue;
        private readonly LinkedList<FibHeapNode<TData>> _roots;
        private LinkedListNode<FibHeapNode<TData>> _minNode;
        private readonly IComparer<TData> _comparer;
        private int _size;

        public FibonacciHeap(TData minDataValue, IComparer<TData> comparer)
            : this(minDataValue, comparer, Enumerable.Empty<FibHeapNode<TData>>())
        {
        }

        public FibonacciHeap(TData minDataValue, IComparer<TData> comparer, IEnumerable<FibHeapNode<TData>> roots)
        {
            _minDataValue = minDataValue;
            _comparer = comparer;
            _roots = new LinkedList<FibHeapNode<TData>>(roots);
            if (_roots.Any())
            {
                var node = _roots.First;
                while (node != null)
                {
                    if (_minNode == null || _comparer.Compare(node.Value.Data, _minNode.Value.Data) == -1)
                        _minNode = node;
                    node = node.Next;
                }
            }
        } 

        public IEnumerable<FibHeapNode<TData>> Roots => _roots;

        public void Insert(TData data)
        {
            _roots.AddLast(new FibHeapNode<TData>(data, null));
            if (_minNode == null || _comparer.Compare(data, _minNode.Value.Data) == -1)
                _minNode = _roots.Last;
            _size++;
        }

        /// <summary>
        /// Gets the minimum element of the heap
        /// </summary>
        public TData Min => _minNode == null ? _minDataValue : _minNode.Value.Data;

        /// <summary>
        /// Unions current heap with another heap
        /// </summary>
        public void Union(FibonacciHeap<TData> heap)
        {
            foreach (var node in heap._roots)
            {
                var linkedListNode = new LinkedListNode<FibHeapNode<TData>>(node);
                _roots.AddLast(linkedListNode);
                if (_minNode == null || _comparer.Compare(node.Data, _minNode.Value.Data) == -1)
                    _minNode = linkedListNode;
                _size += heap._size;
            }
        }

        /// <summary>
        /// Extracts minimum value from the heap
        /// </summary>
        public Tuple<bool, TData> ExtractMin()
        {
            if (_minNode == null)
                return Tuple.Create(false, _minDataValue);

            foreach (var child in _minNode.Value.Children)
            {
                _roots.AddLast(child);
                child.Parent = null;
            }
            _roots.Remove(_minNode);

            var result = _minNode.Value.Data;
            _size--;
            Consolidate();
            return Tuple.Create(true, result);
        }

        private void Consolidate()
        {
            if (_roots.Count == 0)
            {
                _minNode = null;
                return;
            }

            var maxChildren = (int)Math.Round(Math.Log(_size, (1 + Math.Sqrt(5))/2));
            var a = new FibHeapNode<TData>[maxChildren + 1];

            var node = _roots.First;
            while (node != null)
            {
                var curNode = node.Value;
                var nextNode = node.Next;
                while (a[curNode.Children.Count] != null)
                {
                    var nodeToLink = a[curNode.Children.Count];
                    a[curNode.Children.Count] = null;
                    if (_comparer.Compare(nodeToLink.Data, curNode.Data) == -1)
                    {
                        Swap(ref nodeToLink, ref curNode);
                    }
                    LinkNodes(curNode, nodeToLink);
                }
                a[curNode.Children.Count] = curNode;
                node = nextNode;
            }

            _roots.Clear();
            _minNode = null;
            foreach (var aNode in a.Where(x => x != null))
            {
                _roots.AddLast(aNode);
                if (_minNode == null || _comparer.Compare(aNode.Data, _minNode.Value.Data) == -1)
                    _minNode = _roots.Last;
            }
        }

        private static void Swap(ref FibHeapNode<TData> nodeToLink, ref FibHeapNode<TData> node)
        {
            var tmp = nodeToLink;
            nodeToLink = node;
            node = tmp;
        }

        private static void LinkNodes(FibHeapNode<TData> node, FibHeapNode<TData> nodeToLink)
        {
            node.Children.AddLast(nodeToLink);
            nodeToLink.Parent = node;
            nodeToLink.IsMarked = false;
        }

        public void DecreaseKey(FibHeapNode<TData> node, TData newKey)
        {
            if (_comparer.Compare(newKey, node.Data) == 1)
                throw new InvalidOperationException("New key is greater than current key");

            node.Data = newKey;
            var parent = node.Parent;
            if (parent != null && _comparer.Compare(node.Data, parent.Data) == -1)
            {
                Cut(node);
                CascadeCut(parent);
            }
        }

        private void Cut(FibHeapNode<TData> node)
        {
            var parent = node.Parent;
            parent.Children.Remove(node);
            node.Parent = null;
            node.IsMarked = false;
            _roots.AddLast(node);
        }

        private void CascadeCut(FibHeapNode<TData> node)
        {
            var parent = node.Parent;
            if (parent == null)
                return;

            if (!node.IsMarked)
                node.IsMarked = true;
            else
            {
                Cut(node);
                CascadeCut(parent);
            }
        }
    }
}