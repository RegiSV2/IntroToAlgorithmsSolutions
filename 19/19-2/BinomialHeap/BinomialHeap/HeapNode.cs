using System.Collections.Generic;

namespace BinomialHeap
{
    internal class HeapNode<TData>
    {
        public TData Data { get; set; }

        public HeapNode<TData> Parent { get; set; }

         public LinkedList<HeapNode<TData>> Children { get; }

        public HeapNode(TData value)
        {
            Children = new LinkedList<HeapNode<TData>>();
            Data = value;
        } 
    }
}