using System;
using System.Linq;

namespace VanEmdeBoasTree.Tests
{
    abstract class BoundedSetTests
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
            var leafset = CreateSet(2);
            AssertIsValidEmptySet(leafset, 2);
        }

        void Creation_TestCase2()
        {
            var leafset = CreateSet(1);
            AssertIsValidEmptySet(leafset, 2);
        }

        void Creation_TestCase3()
        {
            var set = CreateSet(16);
            AssertIsValidEmptySet(set, 16);
        }

        void Insert_IsMember_TestCase1()
        {
            int universe = 16;
            var set = CreateSet(universe);
            var elementsToInsert = new[] { 2, 1, 10, 12, 15 };
            foreach (var element in elementsToInsert)
                set.Insert(element, new object());
            for (var i = 0; i < universe; i++)
                Assert(elementsToInsert.Contains(i) ^ !set.Contains(i));
        }

        void GetSuccessor_TestCase1()
        {
            var set = CreateSet(16);
            var elementsToInsert = new[] { 2, 1, 10, 12, 15 };
            foreach (var element in elementsToInsert)
                set.Insert(element, new object());

            Assert(set.GetSuccessor(0) == 1);
            Assert(set.GetSuccessor(1) == 2);
            Assert(set.GetSuccessor(3) == 10);
            Assert(set.GetSuccessor(10) == 12);
            Assert(set.GetSuccessor(12) == 15);
            Assert(set.GetSuccessor(15) == null);
        }

        void GetSuccessor_TestCase2()
        {
            int universe = 32;
            var set = CreateSet(universe);

            for (int i = 0; i < 32; i++)
                Assert(set.GetSuccessor(i) == null);
        }

        void GetPredecessor_TestCase1()
        {
            var set = CreateSet(16);
            var elementsToInsert = new[] { 2, 1, 10, 12, 15 };
            foreach (var element in elementsToInsert)
                set.Insert(element, new object());

            Assert(set.GetPredecessor(0) == null);
            Assert(set.GetPredecessor(1) == null);
            Assert(set.GetPredecessor(2) == 1);
            Assert(set.GetPredecessor(3) == 2);
            Assert(set.GetPredecessor(10) == 2);
            Assert(set.GetPredecessor(12) == 10);
            Assert(set.GetPredecessor(15) == 12);
            Assert(set.GetPredecessor(16) == 15);
        }

        void GetPredecessor_TestCase2()
        {
            int universe = 32;
            var set = CreateSet(universe);

            for (int i = 0; i < 32; i++)
                Assert(set.GetPredecessor(i) == null);
        }

        void Delete_TestCase1()
        {
            int universe = 16;
            var set = CreateSet(universe);
            var elementsToInsert = new[] { 2, 1, 10, 12, 15 };
            foreach (var element in elementsToInsert)
                set.Insert(element, new object());

            foreach (var element in elementsToInsert)
            {
                set.Delete(element);
                Assert(!set.Contains(element));
            }

            set.Delete(9);
            AssertIsValidEmptySet(set, universe);
        }

        protected abstract void AssertIsValidEmptySet(IBoundedSet<object> set, int expectedUniverse);

        protected void Assert(bool condition)
        {
            if (!condition)
                throw new InvalidOperationException();
        }

        protected abstract IBoundedSet<object> CreateSet(int universe);
    }
}