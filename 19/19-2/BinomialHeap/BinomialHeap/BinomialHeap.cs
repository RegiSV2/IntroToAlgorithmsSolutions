using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BinomialHeap
{
    internal class BinomialHeap<TData>
    {
        private readonly TData _minValue;

        private readonly IComparer<TData> _comparer;

        private readonly LinkedList<HeapNode<TData>> _roots;

        private LinkedListNode<HeapNode<TData>> _minNode;

        public BinomialHeap(TData minValue, IComparer<TData> comparer)
        {
            _minValue = minValue;
            _comparer = comparer;
            _roots = new LinkedList<HeapNode<TData>>();
        }

        public TData Min => _minNode == null ? _minValue : _minNode.Value.Data;

        public void Merge(BinomialHeap<TData> anotherHeap)
        {
            foreach(var root in anotherHeap._roots)
                InsertHeapNode(root);
        }

        public void Insert(TData value)
        {
            InsertHeapNode(new HeapNode<TData>(value));
        }

        public Tuple<bool, TData> ExtractMin()
        {
            if (_minNode == null)
                return Tuple.Create(false, _minValue);

            var result = _minNode.Value;
            _roots.Remove(_minNode);
            FindMinNode();
            foreach (var newRoot in result.Children)
                InsertHeapNode(newRoot);
            return Tuple.Create(true, result.Data);
        }

        private void FindMinNode()
        {
            _minNode = _roots.First;
            var node = _minNode;
            while (node != null)
            {
                if (_comparer.Compare(_minNode.Value.Data, node.Value.Data) == 1)
                    _minNode = node;
                node = node.Next;
            }
        }

        private void InsertHeapNode(HeapNode<TData> newNode)
        {
            var insertedNode = InsertHeapNodeIntoRoots(newNode);
            ConsolidateRootsAfter(insertedNode);
        }

        private LinkedListNode<HeapNode<TData>> InsertHeapNodeIntoRoots(HeapNode<TData> newNode)
        {
            var insertBefore = _roots.First;
            while (insertBefore != null && insertBefore.Value.Children.Count < newNode.Children.Count)
                insertBefore = insertBefore.Next;
            return insertBefore == null
                ? _roots.AddLast(newNode)
                : _roots.AddBefore(insertBefore, newNode);
        }

        private void ConsolidateRootsAfter(LinkedListNode<HeapNode<TData>> insertedNode)
        {
            while (insertedNode.Next != null && insertedNode.Value.Children.Count == insertedNode.Next.Value.Children.Count)
            {
                if (_minNode == insertedNode || _minNode == insertedNode.Next)
                    _minNode = null;
                var merged = Merge(insertedNode.Value, insertedNode.Next.Value);
                var newInsertedNode = _roots.AddAfter(insertedNode.Next, merged);
                _roots.Remove(insertedNode.Next);
                _roots.Remove(insertedNode);
                insertedNode = newInsertedNode;
            }
            if (_minNode == null || _comparer.Compare(_minNode.Value.Data, insertedNode.Value.Data) == 1)
                _minNode = insertedNode;
        }

        private HeapNode<TData> Merge(HeapNode<TData> a, HeapNode<TData> b)
        {
            Debug.Assert(a.Children.Count == b.Children.Count);
            if (_comparer.Compare(a.Data, b.Data) == 1)
                return Merge(b, a);
            a.Children.AddLast(b);
            b.Parent = a;
            return a;
        }
    }
}