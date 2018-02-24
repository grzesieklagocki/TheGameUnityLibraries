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
            Field[][] fields = InitializeFields(size.x, size.y);
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
                    HexHelper.FindPath(fields, start, destination);

                stopwatch.Stop();
                Console.WriteLine(stopwatch.ElapsedMilliseconds + "ms");
            }
        }


        static Field[][] InitializeFields(int sizeX, int sizeY)
        {
            var fields = new Field[sizeX][];

            for (int x = 0; x < sizeX; x++)
            {
                fields[x] = new Field[sizeY];

                for (int y = 0; y < sizeY; y++)
                {
                    fields[x][y] = new TestField(x, y);
                }
            }

            fields[sizeX - 1][sizeY - 1].isAvailable = false;

            return fields;
        }
    }


    public class TestField : Field
    {
        public bool IsAvailable { get; set; }
        public Vector2Int Position { get; set; }

        public TestField(int positionX, int positionY, bool isAvailable = true)
        {
            IsAvailable = isAvailable;
            Position = new Vector2Int(positionX, positionY);
        }
    }
}
