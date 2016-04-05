using System.Linq;

namespace VanEmdeBoasTree.Tests
{
    class RsVanEmdeBoasTreeTests : BoundedSetTests
    {
        protected override void AssertIsValidEmptySet(IBoundedSet<object> set, int expectedUniverse)
        {
            var tree = (RsVanEmdeBoasTree<object>) set;
            Assert(tree.Universe == expectedUniverse);
            Assert(!tree.Min.HasValue);
            Assert(!tree.Max.HasValue);
            Assert(tree.Summary == null);
            Assert(!tree.Clusters.Any());
        }

        protected override IBoundedSet<object> CreateSet(int universe)
        {
            return new RsVanEmdeBoasTree<object>(universe);
        }
    }
}