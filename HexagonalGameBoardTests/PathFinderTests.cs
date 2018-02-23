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


        [TestInitialize]
        public void Init()
        {
            fields = new IField[10][];

            for (int i = 0; i < 10; i++)
                fields[i] = new IField[10];

            for (int x = 0; x < 10; x++)
                for (int y = 0; y < 10; y++)
                {
                    fields[x][y] = new TestField(x, y, 2);
                }
        }

        [TestMethod()]
        public void FindTest()
        {
            var start = new Vector2Int(0, 0);
            var destination = new Vector2Int(1, 1);

            var path = PathFinder.Find(fields, start, destination, 1);
            var calcuated = HexHelper.GetDistance(start, destination);

            Assert.AreEqual(true, actual: path.Count == calcuated);
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