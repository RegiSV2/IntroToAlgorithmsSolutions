using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BTree
{
    class Program
    {
        private static readonly string CommonBTreeFolder = Path.Combine(Environment.CurrentDirectory, "BTreeContent");
        private const string BTreeKeysSequence = "F;S;Q;K;C;L;H;T;V;W;M;R;N;P;A;B;X;Y;D;Z;E";
        static void Main(string[] args)
        {
            InsertionTestCase1();
            SearchTestCase1();
            DeletionTestCase1();
        }

        static void InsertionTestCase1()
        {
            var treeTuple = BuildBTree();

            var expectedPersister = new StubPersister<string, string>();
            var tb = new StringTreeBuilder(expectedPersister);
            var expectedTree = tb.InnerNode()
                .Child(tb.InnerNode()
                    .Child(tb.Leaf("A"))
                    .Key("B")
                    .Child(tb.Leaf("C;D;E"))
                    .Key("F")
                    .Child(tb.Leaf("H")))
                .Key("K")
                .Child(tb.InnerNode()
                    .Child(tb.Leaf("L"))
                    .Key("M")
                    .Child(tb.Leaf("N;P")))
                .Key("Q")
                .Child(tb.InnerNode()
                    .Child(tb.Leaf("R;S"))
                    .Key("T")
                    .Child(tb.Leaf("V"))
                    .Key("W")
                    .Child(tb.Leaf("X;Y;Z")))
                .Build();

            AssertEqual(treeTuple.Item1.Root, treeTuple.Item2, expectedTree, expectedPersister);

            Directory.Delete(CommonBTreeFolder, true);
        }

        static void SearchTestCase1()
        {
            var treeTuple = BuildBTree();

            AssertFound(treeTuple.Item1.Search("K"), "K");
            AssertFound(treeTuple.Item1.Search("X"), "X");
            AssertFound(treeTuple.Item1.Search("L"), "L");
            AssertNotFound(treeTuple.Item1.Search("I"));
            AssertNotFound(treeTuple.Item1.Search("0"));

            Directory.Delete(CommonBTreeFolder, true);
        }

        private static void DeletionTestCase1()
        {
            var persister = new FilePerNodePersister<string, string>(CommonBTreeFolder);
            var tb = new StringTreeBuilder(persister);
            var tree = new BTree<string, string>(3, persister,
                tb.InnerNode()
                    .Child(tb.InnerNode()
                        .Child(tb.Leaf("A;B"))
                        .Key("C")
                        .Child(tb.Leaf("D;E;F"))
                        .Key("G")
                        .Child(tb.Leaf("J;K;L"))
                        .Key("M")
                        .Child(tb.Leaf("N;O")))
                    .Key("P")
                    .Child(tb.InnerNode()
                        .Child(tb.Leaf("Q;R;S"))
                        .Key("T")
                        .Child(tb.Leaf("U;V"))
                        .Key("X")
                        .Child(tb.Leaf("Y;Z")))
                    .Build());


            tree.Remove("F");
            tree.Remove("M");
            tree.Remove("G");
            tree.Remove("D");
            tree.Remove("B");

            var expectedPersister = new StubPersister<string, string>();
            var tbExp = new StringTreeBuilder(expectedPersister);
            var expectedResult = tbExp.InnerNode()
                .Child(tbExp.Leaf("A;C"))
                .Key("E")
                .Child(tbExp.Leaf("J;K"))
                .Key("L")
                .Child(tbExp.Leaf("N;O"))
                .Key("P")
                .Child(tbExp.Leaf("Q;R;S"))
                .Key("T")
                .Child(tbExp.Leaf("U;V"))
                .Key("X")
                .Child(tbExp.Leaf("Y;Z"))
                .Build();

            AssertEqual(tree.Root, persister, expectedResult, expectedPersister);

            if(tree.Remove("fdsfdsfd"))
                throw new InvalidOperationException("False should be returned on unsuccessful deletion");

            Directory.Delete(CommonBTreeFolder, true);
        }

        private static Tuple<BTree<string, string>, IBTreePersister<string, string>> BuildBTree()
        {
            IBTreePersister<string, string> persister = new FilePerNodePersister<string, string>(CommonBTreeFolder);
            var keys = BTreeKeysSequence.Split(';');
            var tree = new BTree<string, string>(2, persister);
            foreach (var key in keys)
                tree.Insert(new KeyValuePair<string, string>(key, key));
            return Tuple.Create(tree, persister);
        }

        static void AssertFound<TData>(SearchResult<TData> result, TData expected)
            where TData : IComparable<TData>
        {
            if(!result.IsFound || result.Data.CompareTo(expected) != 0)
                throw new InvalidOperationException("Invalid search result");
        }

        static void AssertNotFound<TData>(SearchResult<TData> result)
        {
            if(result.IsFound)
                throw new InvalidOperationException("Invalid search result");
        }

        static void AssertEqual<TKey, TData>(Node<TKey, TData> node1, IBTreePersister<TKey, TData> persister1, 
            Node<TKey, TData> node2, IBTreePersister<TKey, TData> persister2)
            where TKey : IComparable<TKey>
        {
            var typesEqual = node1.IsLeaf == node2.IsLeaf;
            if(!typesEqual)
                throw new InvalidOperationException("Node types not equal");
            var keysEqual = node1.Keys.Count == node2.Keys.Count
                            && node1.Keys.Select((key, i) => key.CompareTo(node2.Keys[i]) == 0)
                                .Aggregate((a, b) => a && b);
            if(!keysEqual)
                throw new InvalidOperationException("Keys of nodes not equal");

            if (!node1.IsLeaf)
            {
                if(node1.Children.Count != node2.Children.Count)
                    throw new InvalidOperationException("Children count not equal");
                for (var i = 0; i < node1.Children.Count; i++)
                {
                    var child1 = persister1.Load(node1.Children[i]);
                    var child2 = persister2.Load(node2.Children[i]);
                    AssertEqual(child1, persister1, child2, persister2);
                }
            }
        }
    }
}
