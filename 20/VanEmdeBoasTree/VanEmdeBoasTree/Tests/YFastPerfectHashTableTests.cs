using System;
using System.Collections.Generic;

namespace VanEmdeBoasTree.Tests
{
    internal static class YFastPerfectHashTableTests
    {
        public static void RunAllTests()
        {
            Insert_Contains_SyntheticTestCase();
        }

        static void Insert_Contains_SyntheticTestCase()
        {
            var universe = 1000;
            var expectedSize = 1024;
            var table = new YFastPerfectHashTable<object>(universe);
            var insertedElementsSet = FillTableWithRandomElements(table, expectedSize);

            AssertTableContainsElements(table, insertedElementsSet, expectedSize);
        }

        private static HashSet<int> FillTableWithRandomElements(YFastPerfectHashTable<object> table, int expectedSize)
        {
            var random = new Random();
            var insertedElements = new List<int>();
            for (var i = 0; i < expectedSize; i += random.Next(1, 4))
            {
                insertedElements.Add(i);
                var element = i;
                byte prefixOrder = 0;
                while (element > 0)
                {
                    table.Add(new YFastNode<object>(element, prefixOrder));
                    element = element >> 1;
                    prefixOrder++;
                }
            }
            var insertedElementsSet = new HashSet<int>(insertedElements);
            return insertedElementsSet;
        }

        private static void AssertTableContainsElements(YFastPerfectHashTable<object> table, HashSet<int> insertedElementsSet, int expectedSize)
        {
            for (var i = 0; i < expectedSize; i++)
            {
                if (insertedElementsSet.Contains(i))
                {
                    var element = i;
                    byte prefixOrder = 0;
                    while (element > 0)
                    {
                        Assert(table.Contains(new YFastNode<object>(element, prefixOrder)));
                        element = element >> 1;
                        prefixOrder++;
                    }
                }
                else
                {
                    Assert(!table.Contains(new YFastNode<object>(i, 0)));
                }
            }
        }

        static void Assert(bool condition)
        {
            if(!condition)
                throw new InvalidOperationException();
        }
    }
}