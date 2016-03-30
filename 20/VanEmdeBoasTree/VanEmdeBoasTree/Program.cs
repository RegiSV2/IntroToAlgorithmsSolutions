using System;
using System.Linq;

namespace VanEmdeBoasTree
{
    class Program
    {
        static void Main(string[] args)
        {
            Creation_TestCase1();
            Creation_TestCase2();
            Creation_TestCase3();
            Insert_IsMember_TestCase1();
            GetSuccessor_TestCase1();
            GetSuccessor_TestCase2();
            GetPredecessor_TestCase1();
            GetPredecessor_TestCase2();
            Delete_TestCase1();
        }

        static void Creation_TestCase1()
        {
            var leafTree = new VanEmdeBoasTree<object>(2);
            AssertIsValidEmptyTree(leafTree, 2);
        }

        static void Creation_TestCase2()
        {
            var leafTree = new VanEmdeBoasTree<object>(1);
            AssertIsValidEmptyTree(leafTree, 2);
        }

        static void Creation_TestCase3()
        {
            var tree = new VanEmdeBoasTree<object>(16);
            AssertIsValidEmptyTree(tree, 16);
        }

        static void Insert_IsMember_TestCase1()
        {
            uint universe = 16;
            var tree = new VanEmdeBoasTree<object>(universe);
            var elementsToInsert = new uint[] {2, 1, 10, 12, 15};
            foreach(var element in elementsToInsert)
                tree.Insert(element, new object());
            for(uint i = 0; i < universe; i++)
                    Assert(elementsToInsert.Contains(i) ^ !tree.Contains(i));
        }

        private static void GetSuccessor_TestCase1()
        {
            var tree = new VanEmdeBoasTree<object>(16);
            var elementsToInsert = new uint[] { 2, 1, 10, 12, 15 };
            foreach (var element in elementsToInsert)
                tree.Insert(element, new object());
            
            Assert(tree.GetSuccessor(0) == 1);
            Assert(tree.GetSuccessor(1) == 2);
            Assert(tree.GetSuccessor(3) == 10);
            Assert(tree.GetSuccessor(10) == 12);
            Assert(tree.GetSuccessor(12) == 15);
            Assert(tree.GetSuccessor(15) == null);
        }

        private static void GetSuccessor_TestCase2()
        {
            uint universe = 32;
            var tree = new VanEmdeBoasTree<object>(universe);

            for(uint i = 0; i < 32; i++)
                Assert(tree.GetSuccessor(i) == null);
        }

        private static void GetPredecessor_TestCase1()
        {
            var tree = new VanEmdeBoasTree<object>(16);
            var elementsToInsert = new uint[] { 2, 1, 10, 12, 15 };
            foreach (var element in elementsToInsert)
                tree.Insert(element, new object());

            Assert(tree.GetPredecessor(0) == null);
            Assert(tree.GetPredecessor(1) == null);
            Assert(tree.GetPredecessor(2) == 1);
            Assert(tree.GetPredecessor(3) == 2);
            Assert(tree.GetPredecessor(10) == 2);
            Assert(tree.GetPredecessor(12) == 10);
            Assert(tree.GetPredecessor(15) == 12);
            Assert(tree.GetPredecessor(16) == 15);
        }

        private static void GetPredecessor_TestCase2()
        {
            uint universe = 32;
            var tree = new VanEmdeBoasTree<object>(universe);

            for (uint i = 0; i < 32; i++)
                Assert(tree.GetPredecessor(i) == null);
        }

        private static void Delete_TestCase1()
        {
            uint universe = 16;
            var tree = new VanEmdeBoasTree<object>(universe);
            var elementsToInsert = new uint[] { 2, 1, 10, 12, 15 };
            foreach (var element in elementsToInsert)
                tree.Insert(element, new object());

            foreach (var element in elementsToInsert)
            {
                tree.Delete(element);
                Assert(!tree.Contains(element));
            }

            tree.Delete(9);
            AssertIsValidEmptyTree(tree, universe);
        }

        private static void AssertIsValidEmptyTree(VanEmdeBoasTree<object> tree, uint expectedUniverse)
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

        static void Assert(bool condition)
        {
            if(!condition)
                throw new InvalidOperationException();
        }
    }
}
