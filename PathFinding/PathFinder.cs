using HexGameBoard.Nodes;
using Priority_Queue;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static HexGameBoard.HexHelper;
using static HexGameBoard.Nodes.Node;

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
        public int SizeX { get { return sizeX; } }

        /// <summary>
        ///     Rozmiar Y tablicy przechowującej szablon planszy
        /// </summary>
        public int SizeY { get { return sizeY; } }


        private Node[][] nodes;
        private int sizeX, sizeY;


        #region Contructors

        /// <summary>
        ///     Inicjalizuje nową instancję podanym szablonem planszy
        /// </summary>
        /// <param name="fieldsTemplate">Szablon planszy</param>
        public PathFinder(PathFindableField[][] fieldsTemplate)
        {
            Initialize(fieldsTemplate);
        }

        #endregion

        /// <summary>
        ///     Inicjalizuje planszę polami
        /// </summary>
        /// <param name="fieldsTemplate">Szablon planszy</param>
        public void Initialize(PathFindableField[][] fieldsTemplate)
        {
            sizeX = fieldsTemplate.Length;
            sizeY = fieldsTemplate[0].Length;

            InitializeArray();
            FillArrayWithNodes(fieldsTemplate);
            FillNodesWithNeighbors();
        }
        
        #region Find

        /// <summary>
        ///     Wyszukuje najkrótszą ścieżkę pomiędzy dwoma polami.  
        ///     Implementacja algorytmu A*.
        /// </summary>
        /// <param name="start">Pole startowe</param>
        /// <param name="destination">Pole końcowe</param>
        /// <seealso cref="PathFindableField"/>
        /// <returns>Najkrótsza ścieżka (stos)</returns>
        public Stack<Vector2Int> Find(Vector2Int start, Vector2Int destination)
        {
            return Find(start, destination, new Vector2Int[0]);
        }

        /// <summary>
        ///     Wyszukuje najkrótszą ścieżkę pomiędzy dwoma polami.  
        ///     Implementacja algorytmu A*.
        /// </summary>
        /// <param name="start">Pole startowe</param>
        /// <param name="destination">Pole końcowe</param>
        /// <param name="temporaryObstacles">Pozycje tymczasowych przeszkód (niepodanych przy inicjalizacji)</param>
        /// <seealso cref="PathFindableField"/>
        /// <returns>Najkrótsza ścieżka (stos)</returns>
        public Stack<Vector2Int> Find(Vector2Int start, Vector2Int destination, IEnumerable<Vector2Int> temporaryObstacles)
        {
            var openSet = new FastPriorityQueue<Node>(sizeX * sizeY);
            var startNode = nodes[start.x][start.y];

            AddToOpenSet(openSet, startNode);
            ResetNodes(startNode);
            SetFields(temporaryObstacles, true);

            while (openSet.Count > 0)
            {
                var actualNode = openSet.Dequeue();

                // znaleziono ścieżkę
                if (actualNode.position == destination)
                {
                    SetFields(temporaryObstacles, false);
                    return CombinePath(actualNode);
                }                  

                actualNode.state = States.InClosedSet;

                foreach (var neighbor in actualNode.neighbors)
                //Parallel.ForEach(actualNode.neighbors, neighbor =>
                {
                    if (neighbor == null || neighbor.state == States.InClosedSet)
                        continue;

                    var g = actualNode.g + 1;

                    if (neighbor.state == States.Unexamined)
                    {
                        neighbor.parent = actualNode;
                        neighbor.g = g;
                        neighbor.h = GetDistance(neighbor.position, destination);

                        AddToOpenSet(openSet, neighbor);
                    }
                    else if (g < neighbor.g) // && neighbor.state == States.unexamined
                    {
                        neighbor.parent = actualNode;
                        neighbor.g = g;

                        openSet.UpdatePriority(neighbor, neighbor.F);
                    }
                }/*);*/
            }

            SetFields(temporaryObstacles, false);
            return new Stack<Vector2Int>();
        }

        #endregion

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

        private Node[] FindAvailableNeighbors(int x, int y)
        {
            var neighbors = new Node[6];

            for (var direction = 0; direction < 6; direction++)
            {
                var neighborPosition = IndexOfNeighbor(x, y, (Direction)direction);

                if (HasValidIndex(neighborPosition.x, neighborPosition.y) && nodes[neighborPosition.x][neighborPosition.y] != null)
                    neighbors[direction] = nodes[neighborPosition.x][neighborPosition.y];
            }

            return neighbors;
        }


        /// <summary>
        ///     Ustawia pole i aktualizuje relacje z sąsiednimi polami
        /// </summary>
        /// <param name="position">Pozycja pola (indeks w tablicy)</param>
        /// <param name="isAvailable"></param>
        public void SetField(Vector2Int position, bool isAvailable)
        {
            SetField(position.x, position.y, isAvailable);
        }

        /// <summary>
        ///     Aktywuje/dezaktywuje podane pole i aktualizuje relacje z jego sąsiednimi polami
        /// </summary>
        /// <param name="x">Pozycja X pola (indeks w tablicy)</param>
        /// <param name="y">Pozycja Y pola (indeks w tablicy)</param>
        /// <param name="isAvailable">Określa, czy pole ma zostać aktywowane (true) czy dezaktywowane</param>
        public void SetField(int x, int y, bool isAvailable)
        {
            var node = nodes[x][y] = isAvailable ? new Node(x, y) : null;
            var neighbors = FindAvailableNeighbors(x, y);

            if (isAvailable)
                node.neighbors = neighbors;

            for (int i = 0; i < 6; i++)
                if (neighbors[i] != null)
                    neighbors[i].neighbors[(i + 3) % 6] = node;

        }

        /// <summary>
        ///     Aktywuje/dezaktywuje wszystkie pola z dostarczonej kolekcji i aktualizuje relacje z ich sąsiednimi polami
        /// </summary>
        /// <param name="positions">Pozycje pól (indeksy tablicy) do ustawienia</param>
        /// <param name="isAvailable">Określa, czy pole ma zostać aktywowane (true) czy dezaktywowane</param>
        public void SetFields(IEnumerable<Vector2Int> positions, bool isAvailable)
        {
            foreach (var position in positions)
                SetField(position, isAvailable);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasValidIndex(int x, int y)
        {
            return x >= 0 && y >= 0 && x < sizeX && y < sizeY;
        }

        #endregion

        #region Find Helpers

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ResetNodes(Node startNode)
        {
            startNode.g = 0;

            for (int x = 0; x < sizeX; x++)
                for (int y = 0; y < sizeY; y++)
                {
                    //var node = nodes[x][y];
                    if (nodes[x][y] == null)
                        continue;

                    nodes[x][y].state = States.Unexamined;
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
            node.state = States.InOpenSet;
            openSet.Enqueue(node, node.F);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Stack<Vector2Int> CombinePath(Node destination)
        {
            var path = new Stack<Vector2Int>((int)destination.g + 1);
            //var path = new Stack<Vector2Int>(1);

            for (var node = destination; node != null; node = node.parent)
                path.Push(node.position);

            return path;
        }

        #endregion
    }
}
