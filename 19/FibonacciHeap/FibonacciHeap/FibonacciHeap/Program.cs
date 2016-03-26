using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Priority_Queue;

namespace FibonacciHeap
{
    class Program
    {
        static void Main(string[] args)
        {
            TestCase1();
            TestCase2();
            TestCase3();
            TestCase4();
            TestCase5();
        }

        static void TestCase1()
        {
            var heap = CreateHeap(5, 2, 3);
            var min = heap.Min;
            Assert(min == 2);
        }

        static void TestCase2()
        {
            var heap = CreateHeap(5, 2, 4, 6);
            var anotherHeap = CreateHeap(9, 1, 12, 15);
            heap.Union(anotherHeap);
            Assert(heap.Min == 1);
        }

        static void TestCase3()
        {
            var heap = CreateHeap(5, 2, 4, 6);
            Assert(heap.ExtractMin().Item2 == 2);
            Assert(heap.ExtractMin().Item2 == 4);
            Assert(heap.ExtractMin().Item2 == 5);
            Assert(heap.ExtractMin().Item2 == 6);
            Assert(!heap.ExtractMin().Item1);
        }

        static void TestCase4()
        {
            var random = new Random();
            var myHeap = new FibonacciHeap<double>(int.MinValue, Comparer<double>.Default);
            var thirdPartyHeap = new FastPriorityQueue<FastPriorityQueueNode>(1000);

            for (var i = 0; i < 1000; i++)
            {
                if (random.Next(3) == 0 && thirdPartyHeap.Any())
                {
                    var myResult = myHeap.ExtractMin();
                    var otherResult = thirdPartyHeap.Dequeue();
                    Assert(myResult.Item1);
                    Assert(Math.Abs(myResult.Item2 - otherResult.Priority) < double.Epsilon);
                }
                else
                {
                    var value = random.NextDouble()*10;
                    myHeap.Insert(value);
                    thirdPartyHeap.Enqueue(new FastPriorityQueueNode(), value);
                }
            }

            while (thirdPartyHeap.Any())
            {
                var myResult = myHeap.ExtractMin();
                var otherResult = thirdPartyHeap.Dequeue();
                Assert(myResult.Item1);
                Assert(Math.Abs(myResult.Item2 - otherResult.Priority) < double.Epsilon);
            }
        }

        static void TestCase5()
        {
            var root1 = Node(7)
                .AddChild(Node(24)
                    .AddChild(Node(26).Marked()
                        .AddChild(Node(35)))
                    .AddChild(Node(46)))
                .AddChild(Node(17)
                    .AddChild(Node(30)))
                .AddChild(Node(23));
            var root2 = Node(18).Marked()
                .AddChild(Node(21)
                    .AddChild(Node(52)))
                .AddChild(Node(39).Marked());
            var root3 = Node(38).AddChild(Node(41));
            var heap = new FibonacciHeap<int>(int.MinValue, Comparer<int>.Default, new[] {root1, root2, root3});

            var node1 = root1.Children.ElementAt(0).Children.ElementAt(1);
            var node2 = root1.Children.ElementAt(0).Children.ElementAt(0).Children.ElementAt(0);
            heap.DecreaseKey(node1, 15);
            heap.DecreaseKey(node2, 5);

            var expRoots = new[]
            {
                15, 5, 26, 24, 7, 18, 38
            };
            var actualRoots = heap.Roots.Select(x => x.Data).ToList();
            Assert(expRoots.Length == actualRoots.Count);
            for (var i = 0; i < expRoots.Length; i++)
                Assert(actualRoots.Contains(expRoots[i]));
        }

        private static FibonacciHeap<int> CreateHeap(params int[] values)
        {
            var heap = new FibonacciHeap<int>(int.MinValue, Comparer<int>.Default);
            foreach(var value in values)
                heap.Insert(value);
            return heap;
        }

        private static void Assert(bool condition)
        {
            if (!condition)
                throw new InvalidOperationException();
        }

        private static FibHeapNode<int> Node(int value)
        {
            return new FibHeapNode<int>(value, null);
        }
    }
}
