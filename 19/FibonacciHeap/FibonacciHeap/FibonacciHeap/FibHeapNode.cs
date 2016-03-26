using System.Collections.Generic;

namespace FibonacciHeap
{
    internal class FibHeapNode<TData>
    {
        public FibHeapNode(TData data, FibHeapNode<TData> parent)
        {
            Children = new LinkedList<FibHeapNode<TData>>();
            Data = data;
            Parent = parent;
        }

        public LinkedList<FibHeapNode<TData>> Children { get; }

        public bool IsMarked { get; internal set; }

        public TData Data { get; internal set; } 

        public FibHeapNode<TData> Parent { get; internal set; }   
    }
}