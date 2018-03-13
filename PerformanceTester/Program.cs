using HexGameBoard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            const string unityProjectPluginsPath = @"C:\Users\grzes\Dysk Google\Unity Projects\The Game\Assets\Plugins";
            const string releaseFolder = @"D:\Users\grzes\OneDrive\Dokumenty\visual studio 2017\Projects\PathFinding\PathFinding\bin\Release";

            var stopwatch1 = new Stopwatch();
            var stopwatch2 = new Stopwatch();
            var stopwatch3 = new Stopwatch();
            var size = new Vector2Int(100, 100);

            PathFindableField[][] fields = InitializeFields(size);

            var pathFinder = new PathFinder(fields);

            var repeats = 1;

            var start = new Vector2Int(0, 0);
            var destination = new Vector2Int(size.x - 1, size.y - 1);

            var fields2 = fields.Select(f => f.Select(f2 => f2.isAvailable).ToArray()).ToArray();

            Stack<Vector2Int> path1 = new Stack<Vector2Int>(), path2 = new Stack<Vector2Int>(), path3 = new Stack<Vector2Int>();

            //for (int i = 0; i < repeats; i++)
            //{
            //    HexHelper.FindPath(fields, start, destination);
            //}

            Console.Write("Wgrać do projektu unity? [y - potwierdzenie]: ");
            string answer = Console.ReadLine();

            if (answer.Equals("y", StringComparison.CurrentCultureIgnoreCase))
            {
                foreach (var file in Directory.GetFiles(releaseFolder))
                {
                    if (file.Contains("UnityEngine"))
                        continue;

                    File.Copy(file, Path.Combine(unityProjectPluginsPath, Path.GetFileName(file)));
                }
            }

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

                //stopwatch1.Start();
                //Parallel.For(0, repeats, i =>
                //{
                //    path1 = HexHelper.FindPath(fields.Select(f => f.Select(f2 => f2.isAvailable).ToArray()).ToArray(), start, destination);
                //    //System.Diagnostics.Debug.WriteLine(i);
                //});
                //stopwatch1.Stop();

                for (int i = 0; i < repeats; i++)
                {
                    stopwatch1.Start();
                    path1 = HexHelper.FindPath(fields.Select(r => r.Select(f => f.isAvailable).ToArray()).ToArray(), start, destination);
                    //path1 = HexHelper.FindPath(fields2, start, destination);
                    stopwatch1.Stop();

                    stopwatch2.Start();
                    path2 = HexHelper.FindPath2(fields.Select(r => r.Select(f => f.isAvailable).ToArray()).ToArray(), start, destination);
                    stopwatch2.Stop();

                    stopwatch3.Start();
                    path3 = pathFinder.Find(start, destination);
                    stopwatch3.Stop();
                }

                Console.WriteLine($"static: {stopwatch1.ElapsedMilliseconds} ({path1.Count}) / static2: {stopwatch2.ElapsedMilliseconds} ({path2.Count}) / {stopwatch3.ElapsedMilliseconds} ({path3.Count}) [ms]");

                stopwatch1.Reset();

                stopwatch1.Restart();
                for (int i = 0; i < repeats; i++)
                    fields.Select(r => r.Select(f => f.isAvailable).ToArray()).ToArray();
                stopwatch1.Stop();

                Console.WriteLine($"Przekształcanie tablicy: {stopwatch1.ElapsedMilliseconds}ms");

                stopwatch1.Reset();
                stopwatch2.Reset();
                stopwatch3.Reset();
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
