using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Priority_Queue;

namespace BinomialHeap
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
            var heap = new BinomialHeap<int>(int.MinValue, Comparer<int>.Default);
            heap.Insert(4);
            heap.Insert(2);
            heap.Insert(7);
            heap.Insert(1);
            heap.Insert(9);
            Assert(heap.ExtractMin().Item2 == 1);
            Assert(heap.ExtractMin().Item2 == 2);
            Assert(heap.ExtractMin().Item2 == 4);
            Assert(heap.ExtractMin().Item2 == 7);
            Assert(heap.ExtractMin().Item2 == 9);
            Assert(!heap.ExtractMin().Item1);
        }

        static void TestCase2()
        {
            var random = new Random();
            var myHeap = new BinomialHeap<double>(double.MinValue, Comparer<double>.Default);
            var otherQueue = new FastPriorityQueue<FastPriorityQueueNode>(10000);
            for (var i = 0; i < 10000; i++)
            {
                if (otherQueue.Any() && random.Next(3) == 0)
                {
                    Assert(Math.Abs(myHeap.ExtractMin().Item2 - otherQueue.Dequeue().Priority) < double.Epsilon);
                }
                else
                {
                    var newValue = random.NextDouble()*10;
                    myHeap.Insert(newValue);
                    otherQueue.Enqueue(new FastPriorityQueueNode(), newValue);
                }
            }

            while(otherQueue.Any())
                Assert(Math.Abs(myHeap.ExtractMin().Item2 - otherQueue.Dequeue().Priority) < double.Epsilon);
        }

        static void TestCase3()
        {
            var heap1Data = new[] {4, 2, 7, 1, 9};
            var heap1 = BuildHeap(heap1Data);
            var heap2Data = new[] {5, 6, 1, 5, 2};
            var heap2 = BuildHeap(heap2Data);
            heap1.Merge(heap2);
            var expectedSequence = heap1Data.Concat(heap2Data).OrderBy(x => x).ToList();
            foreach(var item in expectedSequence)
                Assert(heap1.ExtractMin().Item2 == item);
        }

        static BinomialHeap<int> BuildHeap(params int[] data)
        {
            var heap = new BinomialHeap<int>(int.MinValue, Comparer<int>.Default);
            foreach(var t in data)
                heap.Insert(t);
            return heap;
        } 

        static void Assert(bool condition)
        {
            if(!condition)
                throw new InvalidOperationException();
        }
    }
}
