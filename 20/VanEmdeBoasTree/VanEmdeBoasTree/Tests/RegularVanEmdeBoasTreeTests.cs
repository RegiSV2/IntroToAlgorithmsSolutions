using System;
using System.Linq;

namespace VanEmdeBoasTree.Tests
{
    class RegularVanEmdeBoasTreeTests : VanEmdeBoasTreeTests
    {
        protected override void AssertIsValidEmptyTree(IVanEmdeBoasTree<object> tree, uint expectedUniverse)
        {
            Assert(tree.Universe == expectedUniverse);
            Assert(!tree.Min.HasValue);
            Assert(!tree.Max.HasValue);

            if (expectedUniverse <= 2)
            {
                Assert(tree.Summary == null);
            }
            else
            {
                AssertIsValidEmptyTree(tree.Summary, (uint)Math.Pow(2, Math.Ceiling(Math.Log(expectedUniverse, 2) / 2)));
                Assert(tree.Clusters.All(x => x != null));
                var lowSqrt = (uint)Math.Pow(2, Math.Floor(Math.Log(expectedUniverse, 2) / 2));
                foreach (var cluster in tree.Clusters)
                    AssertIsValidEmptyTree(cluster, lowSqrt);
            }
        }

        protected override IVanEmdeBoasTree<object> CreateTree(uint universe)
        {
            return new VanEmdeBoasTree<object>(universe);
        }
    }
}