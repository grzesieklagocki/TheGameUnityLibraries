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
            Assert.AreEqual(true, PathFinder.Find(fields, Vector2Int.zero, Vector2Int.one * 2, 1).Count > 0);
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