using Microsoft.VisualStudio.TestTools.UnitTesting;
using HexGameBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexGameBoard.Tests
{
    [TestClass()]
    public class PriorityQueueTests
    {
        [TestMethod()]
        public void EnqueueTest()
        {
            var q = new PriorityQueue<int>();
            int count = 100;

            for (int i = 0; i < 100; i++)
                q.Enqueue(i);

            for (int i = 0; i < count / -1; i++)
                Assert.AreEqual(true, q[i] >= q[2*i + 1] && q[i] >= q[2 * i + 2]);


        }
    }
}