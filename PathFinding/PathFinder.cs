using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using static HexGameBoard.HexHelper;
using static HexGameBoard.Node;

namespace HexGameBoard
{
    /// <summary>
    /// 
    /// </summary>
    public class PathFinder
    {
        /// <summary>
        ///     Rozmiar Y tablicy przechowującej szablon planszy
        /// </summary>
        public int SizeX
        {
            get { return sizeX; }
        }

        /// <summary>
        ///     Rozmiar Y tablicy przechowującej szablon planszy
        /// </summary>
        public int SizeY
        {
            get { return sizeY; }
        }

        private Node[][] nodes;
        private int sizeX, sizeY;


        #region Contructors
        
        /// <summary>
        ///     
        /// </summary>
        /// <param name="fieldsTemplate"></param>
        public PathFinder(PathFindableField[][] fieldsTemplate)
        {
            Initialize(fieldsTemplate);
        }

        #endregion

        /// <summary>
        ///     
        /// </summary>
        /// <param name="fieldsTemplate"></param>
        public void Initialize(PathFindableField[][] fieldsTemplate)
        {
            sizeX = fieldsTemplate.Length;
            sizeY = fieldsTemplate[0].Length;

            InitializeArray();
            FillArrayWithNodes(fieldsTemplate);
            FillNodesWithNeighbors();
        }

        public Stack<Vector2Int> Find(Vector2Int start, Vector2Int destination)
        {
            var openSet = new FastPriorityQueue<Node>(sizeX * sizeY);
            var startNode = nodes[start.x][start.y];
            Node actualNode;
            AddToOpenSet(openSet, startNode);

            ResetNodes();

            while (openSet.Count > 0)
            {
                actualNode = openSet.Dequeue();

                // znaleziono ścieżkę
                if (actualNode.position == destination)
                    return CombinePath(actualNode);

                actualNode.state = Node.States.onClosedList;

                foreach (var neighborPosition in actualNode.neighbors)
                {
                    var neighbor = nodes[neighborPosition.x][neighborPosition.y];

                    if (neighbor.state == States.onClosedList)
                        continue;

                    if (neighbor.state == States.onOpenList)
                    {
                        var g = actualNode.g + 1;

                        if (g < neighbor.g)
                        {
                            neighbor.parent = actualNode;
                            neighbor.g = g;

                            openSet.UpdatePriority(neighbor, neighbor.F);
                        }
                    }
                    else
                    {
                        neighbor.parent = actualNode;
                        neighbor.g = actualNode.g + 1;
                        neighbor.h = GetDistance(neighbor.position, destination);

                        AddToOpenSet(openSet, neighbor);
                    }
                }
            }

            return new Stack<Vector2Int>();
        }

        #region Initialization Helpers

        private void InitializeArray()
        {
            nodes = new Node[sizeX][];

            for (int x = 0; x < sizeX; x++)
                nodes[x] = new Node[sizeY];
        }

        private void FillArrayWithNodes(PathFindableField[][] fieldsTemplate)
        {
            for (int y = 0; y < sizeY; y++)
                for (int x = 0; x < sizeX; x++)
                    if (fieldsTemplate[x][y].isAvailable)
                        nodes[x][y] = new Node(fieldsTemplate[x][y].position);
        }

        private void FillNodesWithNeighbors()
        {
            for (int y = 0; y < sizeY; y++)
                for (int x = 0; x < sizeX; x++)
                    if (nodes[x][y] != null)
                    nodes[x][y].neighbors = FindAvailableNeighbors(x, y);
        }

        private List<Vector2Int> FindAvailableNeighbors(int x, int y)
        {
            var neighbors = new List<Vector2Int>(2);

            //for (var direction = 0; direction < 6; direction++)
            //{
            //    var neighbor = HexHelper.IndexOfNeighbor(x, y, (HexHelper.Direction)direction);

            //    if (HasValidIndex(neighbor.x, neighbor.y) && nodes[neighbor.x][neighbor.y] != null)
            //        neightbors.Add(neighbor);
            //    //yield return neighbor;
            //}
            Parallel.For(0, 6, direction =>
            {
                var neighbor = IndexOfNeighbor(x, y, (Direction)direction);

                if (HasValidIndex(neighbor.x, neighbor.y) && nodes[neighbor.x][neighbor.y] != null)
                    lock(neighbors)
                        neighbors.Add(neighbor);
                //yield return neighbor;
            });

            return neighbors;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasValidIndex(int x, int y)
        {
            return x >= 0 && y >= 0 && x < sizeX && y < sizeY;
        }

        #endregion

        #region Find Helpers

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ResetNodes()
        {
            for (int x = 0; x < sizeX; x++)
                for (int y = 0; y < sizeY; y++)
                {
                    //var node = nodes[x][y];
                    if (nodes[x][y] == null)
                        continue;

                    //nodes[x][y].isInClosedSet = false;
                    //nodes[x][y].isInOpenSet = false;
                    nodes[x][y].state = States.unexamined;
                }

            //Parallel.For(0, sizeX, x =>
            //{
            //    Parallel.For(0, sizeY, y=>
            //    {
            //        //var node = nodes[x][y];
            //        if (nodes[x][y] != null)
            //        {
            //            nodes[x][y].isInClosedSet = false;
            //            nodes[x][y].isInOpenSet = false;
            //        }
            //    });
            //});
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddToOpenSet(FastPriorityQueue<Node> openSet, Node node)
        {
            node.state = States.onOpenList;
            openSet.Enqueue(node, node.F);
        }

        /// <summary>
        ///     Określa dystans pomiędzy dwoma polami (zdefiniowanymi we współrzędnych kartezjańskich 2D). Ignoruje przeszkody.
        /// </summary>
        /// <param name="a">Pierwsze pole (współrzędne kartezjańskie 2D)</param>
        /// <param name="b">Drugie pole (współrzędne kartezjańskie 2D)</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetDistance(Vector2Int a, Vector2Int b)
        {
            return GetDistance(AxialToCubeCoordinates(a), AxialToCubeCoordinates(b));
        }

        /// <summary>
        ///     Określa dystans pomiędzy dwoma polami (zdefiniowanymi we współrzędnych mapowania sześniennego). Ignoruje przeszkody.
        /// </summary>
        /// <param name="a">Pierwsze pole (współrzędne mapowania sześciennego)</param>
        /// <param name="b">Drugie pole (współrzędne mapowania sześciennego)</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetDistance(Vector3Int a, Vector3Int b)
        {
            return Math.Max(Math.Abs(a.x - b.x), Math.Max(Math.Abs(a.y - b.y), Math.Abs(a.z - b.z)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector3Int AxialToCubeCoordinates(Vector2Int axialCoordinates)
        {
            var x = axialCoordinates.x;
            var z = axialCoordinates.y - (axialCoordinates.x - (axialCoordinates.x & 1)) / 2;
            var y = -x - z;

            return new Vector3Int(x, y, z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Stack<Vector2Int> CombinePath(Node destination)
        {
            var path = new Stack<Vector2Int>(1);

            for (var node = destination; node != null; node = node.parent)
                path.Push(node.position);

            return path;
        }

        #endregion
    }
}
