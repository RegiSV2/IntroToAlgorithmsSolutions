using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTree
{
    class Program
    {
        private static readonly string CommonBTreeFolder = Path.Combine(Environment.CurrentDirectory, "BTreeContent");
        static void Main(string[] args)
        {
            InsertionTestCase1();
            SearchTestCase1();
        }

        static void InsertionTestCase1()
        {
            IBTreePersister<string, string> persister = new FilePerNodePersister<string, string>(CommonBTreeFolder);
            var keys = "F;S;Q;K;C;L;H;T;V;W;M;R;N;P;A;B;X;Y;D;Z;E".Split(';');
            var tree = new BTree<string, string>(2, persister);
            foreach(var key in keys)
                tree.Insert(new KeyValuePair<string, string>(key, key));

            IBTreePersister<string, string> expectedPersister = new StubPersister<string, string>();
            var expectedTree = InnerNode()
                .Child(expectedPersister, InnerNode()
                    .Child(expectedPersister, Leaf().Key("A"))
                    .Key("B")
                    .Child(expectedPersister, Leaf().Key("C").Key("D").Key("E"))
                    .Key("F")
                    .Child(expectedPersister, Leaf().Key("H")))
                .Key("K")
                .Child(expectedPersister, InnerNode()
                    .Child(expectedPersister, Leaf().Key("L"))
                    .Key("M")
                    .Child(expectedPersister, Leaf().Key("N").Key("P")))
                .Key("Q")
                .Child(expectedPersister, InnerNode()
                    .Child(expectedPersister, Leaf().Key("R").Key("S"))
                    .Key("T")
                    .Child(expectedPersister, Leaf().Key("V"))
                    .Key("W")
                    .Child(expectedPersister, Leaf().Key("X").Key("Y").Key("Z")));
            AssertEqual(tree.Root, persister, expectedTree, expectedPersister);

            Directory.Delete(CommonBTreeFolder, true);
        }

        static void SearchTestCase1()
        {
            IBTreePersister<string, string> persister = new FilePerNodePersister<string, string>(CommonBTreeFolder);
            var keys = "F;S;Q;K;C;L;H;T;V;W;M;R;N;P;A;B;X;Y;D;Z;E".Split(';');
            var tree = new BTree<string, string>(2, persister);
            foreach (var key in keys)
                tree.Insert(new KeyValuePair<string, string>(key, key));
            
            AssertFound(tree.Search("K"), "K");
            AssertFound(tree.Search("X"), "X");
            AssertFound(tree.Search("L"), "L");
            AssertNotFound(tree.Search("I"));
            AssertNotFound(tree.Search("0"));

            Directory.Delete(CommonBTreeFolder, true);
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

        static Node<string, string> InnerNode()
        {
            return new Node<string, string>(false);
        }

        static Node<string, string> Leaf()
        {
            return new Node<string, string>(true);
        } 
    }
}
