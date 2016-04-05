namespace VanEmdeBoasTree.Tests
{
    internal class ProtoYFastTreeTests : BoundedSetTests
    {
        protected override void AssertIsValidEmptySet(IBoundedSet<object> set, int expectedUniverse)
        {
            var tree = (ProtoYFastTree<object>) set;
            Assert(tree.Min == null && tree.Max == null);
        }

        protected override IBoundedSet<object> CreateSet(int universe)
        {
            return new ProtoYFastTree<object>(universe);
        }
    }
}