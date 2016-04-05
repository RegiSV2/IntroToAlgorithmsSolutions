using System;
using System.Linq;

namespace VanEmdeBoasTree.Tests
{
    class RegularVanEmdeBoasTreeTests : BoundedSetTests
    {
        protected override void AssertIsValidEmptySet(IBoundedSet<object> set, int expectedUniverse)
        {
            var tree = (VanEmdeBoasTree<object>)set;
            Assert(tree.Universe == expectedUniverse);
            Assert(!tree.Min.HasValue);
            Assert(!tree.Max.HasValue);

            if (expectedUniverse <= 2)
            {
                Assert(tree.Summary == null);
            }
            else
            {
                AssertIsValidEmptySet(tree.Summary, (int)Math.Pow(2, Math.Ceiling(Math.Log(expectedUniverse, 2) / 2)));
                Assert(tree.Clusters.All(x => x != null));
                var lowSqrt = (int)Math.Pow(2, Math.Floor(Math.Log(expectedUniverse, 2) / 2));
                foreach (var cluster in tree.Clusters)
                    AssertIsValidEmptySet(cluster, lowSqrt);
            }
        }

        protected override IBoundedSet<object> CreateSet(int universe)
        {
            return new VanEmdeBoasTree<object>(universe);
        }
    }
}