using Microsoft.VisualStudio.TestTools.UnitTesting;
using HexGameBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HexGameBoard.Tests
{
    [TestClass()]
    public class PathFinderTests
    {
        private IField[][] fields;
        private Vector2Int size = new Vector2Int(100, 100);

        [TestInitialize]
        public void Init()
        {
            fields = new IField[size.x][];

            for (int x = 0; x < size.x; x++)
            {
                fields[x] = new IField[size.y];

                for (int y = 0; y < size.y; y++)
                {
                    fields[x][y] = new TestField(x, y, 2);
                }
            }

            fields[size.x - 1][size.y - 1].AvailabilityLevel = 0;
        }

        [TestMethod()]
        public void FindTest()
        {
            var start = new Vector2Int(0, 0);
            var destination = new Vector2Int(size.x - 1, size.y - 1);

            var watch = new System.Diagnostics.Stopwatch(); watch.Start();
            var path = HexHelper.FindPath(fields, start, destination, 1);
            watch.Stop(); var time = watch.ElapsedMilliseconds;

            //var calcuated = HexHelper.GetDistance(start, destination);

            Assert.AreEqual(true, actual: path.Count == 0);
            //Assert.AreEqual(true, actual: path.Count != 0);
        }

        [TestMethod]
        public void GetDistanceTest()
        {
            Assert.AreEqual(1, HexHelper.GetDistance(new Vector2Int(0, 0), new Vector2Int(0, 1)));
            Assert.AreEqual(1, HexHelper.GetDistance(new Vector2Int(0, 0), new Vector2Int(1, 0)));

            Assert.AreEqual(2, HexHelper.GetDistance(new Vector2Int(0, 0), new Vector2Int(0, 2)));
            Assert.AreEqual(2, HexHelper.GetDistance(new Vector2Int(0, 0), new Vector2Int(2, 0)));
            Assert.AreEqual(2, HexHelper.GetDistance(new Vector2Int(0, 0), new Vector2Int(1, 1)));
            Assert.AreEqual(2, HexHelper.GetDistance(new Vector2Int(0, 0), new Vector2Int(2, 1)));

            Assert.AreEqual(4, HexHelper.GetDistance(new Vector2Int(3, 3), new Vector2Int(7, 2)));
            Assert.AreEqual(6, HexHelper.GetDistance(new Vector2Int(0, 5), new Vector2Int(6, 4)));
            Assert.AreEqual(4, HexHelper.GetDistance(new Vector2Int(4, 1), new Vector2Int(4, 5)));
            Assert.AreEqual(6, HexHelper.GetDistance(new Vector2Int(6, 3), new Vector2Int(0, 3)));
        }
    }

    public class TestField : IField
    {
        public int AvailabilityLevel { get; set; }
        public Vector2Int Position { get; set; }

        public TestField(int positionX, int positionY, int availabilityLevel)
        {
            AvailabilityLevel = availabilityLevel;
            Position = new Vector2Int(positionX, positionY);
        }
    }
}