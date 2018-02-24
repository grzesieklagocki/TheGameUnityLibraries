using HexGameBoard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PerformanceTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            var size = new Vector2Int(100, 100);
            IField[][] fields = InitializeFields(size.x, size.y);
            var repeats = 0;
            var start = new Vector2Int(0, 0);
            var destination = new Vector2Int(size.x - 1, size.y - 1);

            while(true)
            {
                Console.Write("Przebiegów: ");

                while (!int.TryParse(Console.ReadLine(), out repeats))
                    ;


                stopwatch.Restart();

                for (int i = 0; i < repeats; i++)
                    HexHelper.FindPath(fields, start, destination, 1);

                stopwatch.Stop();
                Console.WriteLine(stopwatch.ElapsedMilliseconds + "ms");
            }
        }


        static IField[][] InitializeFields(int sizeX, int sizeY)
        {
            var fields = new IField[sizeX][];

            for (int x = 0; x < sizeX; x++)
            {
                fields[x] = new IField[sizeY];

                for (int y = 0; y < sizeY; y++)
                {
                    fields[x][y] = new TestField(x, y, 2);
                }
            }

            fields[sizeX - 1][sizeY - 1].AvailabilityLevel = 0;

            return fields;
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
