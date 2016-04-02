using System;
using System.Linq;

namespace VanEmdeBoasTree.Tests
{
    class RsVanEmdeBoasTreeTests : VanEmdeBoasTreeTests
    {
        protected override void AssertIsValidEmptyTree(IVanEmdeBoasTree<object> tree, uint expectedUniverse)
        {
            Assert(tree.Universe == expectedUniverse);
            Assert(!tree.Min.HasValue);
            Assert(!tree.Max.HasValue);
            Assert(tree.Summary == null);
            Assert(!tree.Clusters.Any());
        }

        protected override IVanEmdeBoasTree<object> CreateTree(uint universe)
        {
            return new RsVanEmdeBoasTree<object>(universe);
        }
    }
}