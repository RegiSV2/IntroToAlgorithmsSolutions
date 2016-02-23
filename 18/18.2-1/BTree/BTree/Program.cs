using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTree
{
    class Program
    {
        static void Main(string[] args)
        {
            InsertionTestCase1();
            SearchTestCase1();
        }

        static void InsertionTestCase1()
        {
            var keys = "F;S;Q;K;C;L;H;T;V;W;M;R;N;P;A;B;X;Y;D;Z;E".Split(';');
            var tree = new BTree<string, string>(2);
            foreach(var key in keys)
                tree.Insert(new KeyValuePair<string, string>(key, key));

            var expectedTree = InnerNode()
                .Child(InnerNode()
                    .Child(Leaf().Key("A"))
                    .Key("B")
                    .Child(Leaf().Key("C").Key("D").Key("E"))
                    .Key("F")
                    .Child(Leaf().Key("H")))
                .Key("K")
                .Child(InnerNode()
                    .Child(InnerNode().Key("L"))
                    .Key("M")
                    .Child(InnerNode().Key("N").Key("P")))
                .Key("Q")
                .Child(InnerNode()
                    .Child(Leaf().Key("R").Key("S"))
                    .Key("T")
                    .Child(Leaf().Key("V"))
                    .Key("W")
                    .Child(Leaf().Key("X").Key("Y").Key("Z")));
            AssertEqual(tree.Root, expectedTree);
        }

        static void SearchTestCase1()
        {
            var keys = "F;S;Q;K;C;L;H;T;V;W;M;R;N;P;A;B;X;Y;D;Z;E".Split(';');
            var tree = new BTree<string, string>(2);
            foreach (var key in keys)
                tree.Insert(new KeyValuePair<string, string>(key, key));
            
            AssertFound(tree.Search("K"), "K");
            AssertFound(tree.Search("X"), "X");
            AssertFound(tree.Search("L"), "L");
            AssertNotFound(tree.Search("I"));
            AssertNotFound(tree.Search("0"));
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

        static void AssertEqual<TKey, TData>(Node<TKey, TData> node1, Node<TKey, TData> node2)
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

            if (node1.IsLeaf)
            {
                if(node1.Children.Count != node2.Children.Count)
                    throw new InvalidOperationException("Children count not equal");
                for(var i = 0; i < node1.Children.Count; i++)
                    AssertEqual(node1.Children[i], node2.Children[i]);
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
