using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibonacciHeap
{
    class Program
    {
        static void Main(string[] args)
        {
            TestCase1();
            TestCase2();
            TestCase3();
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
    }
}
