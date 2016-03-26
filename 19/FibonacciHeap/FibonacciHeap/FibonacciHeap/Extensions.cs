namespace FibonacciHeap
{
    static class Extensions
    {
        public static FibHeapNode<TData> AddChild<TData>(this FibHeapNode<TData> node, FibHeapNode<TData> child)
        {
            node.Children.AddLast(child);
            child.Parent = node;
            return node;
        }

        public static FibHeapNode<TData> Marked<TData>(this FibHeapNode<TData> node)
        {
            node.IsMarked = true;
            return node;
        } 
    }
}