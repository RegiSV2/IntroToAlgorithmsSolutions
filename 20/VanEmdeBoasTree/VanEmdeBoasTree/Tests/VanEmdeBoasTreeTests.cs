using System;
using System.Linq;

namespace VanEmdeBoasTree.Tests
{
    abstract class VanEmdeBoasTreeTests
    {
        public void RunAllTests()
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

        void Creation_TestCase1()
        {
            var leafTree = CreateTree(2);
            AssertIsValidEmptyTree(leafTree, 2);
        }

        void Creation_TestCase2()
        {
            var leafTree = CreateTree(1);
            AssertIsValidEmptyTree(leafTree, 2);
        }

        void Creation_TestCase3()
        {
            var tree = CreateTree(16);
            AssertIsValidEmptyTree(tree, 16);
        }

        void Insert_IsMember_TestCase1()
        {
            uint universe = 16;
            var tree = CreateTree(universe);
            var elementsToInsert = new uint[] { 2, 1, 10, 12, 15 };
            foreach (var element in elementsToInsert)
                tree.Insert(element, new object());
            for (uint i = 0; i < universe; i++)
                Assert(elementsToInsert.Contains(i) ^ !tree.Contains(i));
        }

        void GetSuccessor_TestCase1()
        {
            var tree = CreateTree(16);
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

        void GetSuccessor_TestCase2()
        {
            uint universe = 32;
            var tree = CreateTree(universe);

            for (uint i = 0; i < 32; i++)
                Assert(tree.GetSuccessor(i) == null);
        }

        void GetPredecessor_TestCase1()
        {
            var tree = CreateTree(16);
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

        void GetPredecessor_TestCase2()
        {
            uint universe = 32;
            var tree = CreateTree(universe);

            for (uint i = 0; i < 32; i++)
                Assert(tree.GetPredecessor(i) == null);
        }

        void Delete_TestCase1()
        {
            uint universe = 16;
            var tree = CreateTree(universe);
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

        protected abstract void AssertIsValidEmptyTree(IVanEmdeBoasTree<object> tree, uint expectedUniverse);

        protected void Assert(bool condition)
        {
            if (!condition)
                throw new InvalidOperationException();
        }

        protected abstract IVanEmdeBoasTree<object> CreateTree(uint universe);
    }
}