namespace BTree
{
    internal class StringTreeBuilder : TreeBuilder<string>
    {
        public StringTreeBuilder(IBTreePersister<string, string> persister) : base(persister)
        {
        }

        public NodeBuilder Leaf(string keys)
        {
            var leaf = Leaf();
            foreach (var key in keys.Split(';'))
                leaf.Key(key);
            return leaf;
        }
    }
}