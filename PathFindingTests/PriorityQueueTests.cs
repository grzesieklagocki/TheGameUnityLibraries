using Microsoft.VisualStudio.TestTools.UnitTesting;
using HexGameBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HexGameBoard.PriorityQueue;

namespace HexGameBoard.Tests
{
    [TestClass()]
    public class PriorityQueueTests
    {
        private const int count = 100;
        private int[] array;

        [TestInitialize]
        public void TestInitialize()
        {
            var random = new Random();

            array = new int[count];

            for (int i = 0; i < count; i++)
                array[i] = random.Next(int.MinValue, int.MaxValue);
        }

        [TestMethod()]
        public void EnqueueTest()
        {
            var queue = new PriorityQueue<int>();


            for (int i = 0; i < count; i++)
                queue.Enqueue(array[i]);

            //var previous = queue.Dequeue();

            //for (int i = 1; i < count; i++)
            //{
            //    Assert.AreEqual(true, previous > queue.Peek());
            //    previous = queue.Dequeue();
            //}

            for (int i = 0; i < count; i++)
                Assert.AreEqual(true, queue.Contains(array[i]));

            //var s = new SortedSet<int>();
            //for (int i = 0; i < count; i++)
            //    s.Add(random.Next(int.MinValue, int.MaxValue));

            //for (int i = 1; i < count; i++)
            //{
            //    Assert.AreEqual(true, s.ElementAt(i - 1) < s.ElementAt(i));
            //    //previous = queue.Dequeue();
            //}

            //var queue2 = new PriorityQueue2<int, object>(new MaxComparer<int>());

            //    var q = new PriorityQueue.PriorityQueue<int, int>();

            //    for (int i = 0; i < count; i++)
            //    {
            //        var value = random.Next(-50, 50);
            //        q.Add(value, value);
            //    }

            //    var previous = q.Dequeue();

            //    for (int i = 1; i < count; i++)
            //    {
            //        Assert.AreEqual(true, previous < q.Peek());
            //        previous = q.Dequeue();
            //    }                        
            //}
        }
    }
}