using Microsoft.VisualStudio.TestTools.UnitTesting;
using HexGameBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HexGameBoard.HexHelper;
using UnityEngine;

namespace HexGameBoard.Tests
{
    [TestClass()]
    public class HexHelperTests
    {
        [TestMethod()]
        public void FieldsInRangeTest()
        {
            for (int i = 1; i < 20; i++)
            {
                var fields = FieldsInRange(50, 50, i, i);
                Assert.AreEqual(true, actual: fields.Count == (i * 6));
            }
        }
    }
}