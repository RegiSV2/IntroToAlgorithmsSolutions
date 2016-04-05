using System.Collections.Generic;

namespace VanEmdeBoasTree
{
    struct YFastNode<TData>
    {
        public int Key { get; set; }
        public byte PrefixOrder { get; set; }
        public TData Data { get; set; }
        public LinkedListNode<int> LinkedListNode { get; set; }

        public YFastNode(int key, byte prefixOrder)
            : this()
        {
            Key = key;
            PrefixOrder = prefixOrder;
        }

        public YFastNode(int key, byte prefixOrder, LinkedListNode<int> node, TData data)
            : this(key, prefixOrder)
        {
            Data = data;
            LinkedListNode = node;
        }
    }
}