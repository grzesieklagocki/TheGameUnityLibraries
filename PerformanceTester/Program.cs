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
            PathFindableField[][] fields = InitializeFields(size);
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


        static PathFindableField[][] InitializeFields(Vector2Int size)
        {
            var fields = new PathFindableField[size.x][];

            for (int x = 0; x < size.x; x++)
            {
                fields[x] = new PathFindableField[size.y];

                for (int y = 0; y < size.y; y++)
                {
                    fields[x][y] = new TestField(new Vector2Int(x, y));
                }
            }

            fields[size.x - 1][size.y - 1].isAvailable = false;

            return fields;
        }
    }


    public class TestField : PathFindableField
    {
        public TestField(Vector2Int position, bool isAvailable = true) : base(position, isAvailable)
        {
            this.isAvailable = isAvailable;
            this.position = position;
        }
    }
}
