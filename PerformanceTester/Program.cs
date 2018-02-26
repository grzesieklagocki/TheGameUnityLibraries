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
            var stopwatch1 = new Stopwatch();
            var stopwatch2 = new Stopwatch();
            var size = new Vector2Int(100, 100);

            PathFindableField[][] fields = InitializeFields(size);

            var pathFinder = new PathFinder(fields);

            var repeats = 1;

            var start = new Vector2Int(0, 0);
            var destination = new Vector2Int(size.x - 1, size.y - 1);

            Stack<Vector2Int> path1 = new Stack<Vector2Int>(), path2 = new Stack<Vector2Int>();

            //for (int i = 0; i < repeats; i++)
            //{
            //    HexHelper.FindPath(fields, start, destination);
            //}

            while (true)
            {
                string parameters = string.Empty;

                do
                {
                    Console.Write("Przebiegów: ");
                }
                while (!int.TryParse(new string((parameters = Console.ReadLine()).Where(c => char.IsDigit(c)).ToArray()), out repeats));

                if (parameters.Contains("s"))
                    destination.x = size.x - 2;
                else
                    destination.x = size.x - 1;


                for (int i = 0; i < repeats; i++)
                {
                    //stopwatch1.Start();
                    ////path1 = HexHelper.FindPath(fields, start, destination);
                    //stopwatch1.Stop();

                    stopwatch2.Start();
                    //path2 = pathFinder.Find(start, destination);
                    pathFinder.Find(start, destination);
                    stopwatch2.Stop();
                }

                Console.WriteLine($"{stopwatch1.ElapsedMilliseconds} ({path1.Count}) / {stopwatch2.ElapsedMilliseconds} ({path2.Count}) [ms]");

                stopwatch1.Reset();
                stopwatch2.Reset();
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
