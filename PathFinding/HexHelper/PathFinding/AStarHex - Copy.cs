using HexGameBoard.Nodes;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace HexGameBoard.PathFinding
{
    internal class AStarHex2
    {
        /// <summary>
        ///     Wyszukuje najkrótszą ścieżkę pomiędzy dwoma polami.  
        ///     Implementacja algorytmu A*.
        /// </summary>
        /// <param name="fields">Tablica pól z zadeklarowną dostępnością. Muszą dziedziczyć po klasie PathFindableField</param>
        /// <param name="start">Pole startowe</param>
        /// <param name="destination">Pole końcowe</param>
        /// <seealso cref="PathFindableField"/>
        /// <exception cref="ArgumentNullException" />
        /// <returns>Najkrótsza ścieżka (stos)</returns>
        internal static Stack<Vector2Int> FindPath(bool[][] fields, Vector2Int start, Vector2Int destination)
        {
            return FindPath(fields, start, destination, new Vector2Int(0, 0), new Vector2Int(fields.Length - 1, fields[0].Length - 1));
        }

        /// <summary>
        ///     Wyszukuje najkrótszą ścieżkę pomiędzy dwoma polami.  
        ///     Implementacja algorytmu A*.
        /// </summary>
        /// <param name="fields">Tablica pól z zadeklarowną dostępnością. Muszą dziedziczyć po klasie PathFindableField</param>
        /// <param name="start">Pole startowe</param>
        /// <param name="destination">Pole końcowe</param>
        /// <param name="minIndexes">Minimalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
        /// <param name="maxIndexes">Maksymalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
        /// <seealso cref="PathFindableField"/>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <returns>Najkrótsza ścieżka (stos)</returns>
        internal static Stack<Vector2Int> FindPath(bool[][] fields, Vector2Int start, Vector2Int destination, Vector2Int minIndexes, Vector2Int maxIndexes)
        {
#if DEBUG
            CheckArguments(fields, start, destination, minIndexes, maxIndexes);
#endif

            AStarNode[][] nodes = InitializeAstarNodes(fields);
            Vector2Int[][][] neighbors = InitializeParents(fields);
            var openSet = new FastPriorityQueue<AStarPosition>((maxIndexes.x - minIndexes.x) * (maxIndexes.y - minIndexes.y));
            var startNode = new AStarNode();

            AddToOpenSet(openSet, nodes, startNode, start);

            while (openSet.Count > 0)
            {
                var actualNodePosition = openSet.Dequeue();

                // znaleziono ścieżkę
                if (actualNodePosition.position == destination)
                    return CombinePath(actualNodePosition.position, start, nodes);

                AddToClosedSet(nodes, actualNodePosition.position);

                //foreach (var neighborPosition in GetNeighbors(fields, neighbors, nodes, actualNodePosition.position, minIndexes, maxIndexes))
                var actualNeighbors = GetNeighbors(fields, neighbors, nodes, actualNodePosition.position, minIndexes, maxIndexes);
                for (int i = 0; i < 6; i++)
                {
                    var neighborPosition = actualNeighbors[i];
                    if (IsInClosedSet(neighborPosition, nodes))
                        continue;

                    var actualG = nodes[actualNodePosition.position.x][actualNodePosition.position.y].g + 1;

                    if (IsInOpenSet(neighborPosition, nodes))
                    {
                        var neighbor = nodes[neighborPosition.x][neighborPosition.y];

                        if (actualG < neighbor.g)
                        {
                            neighbor.parent = actualNodePosition.position;
                            neighbor.g = actualG;

                            openSet.UpdatePriority(actualNodePosition, neighbor.F);
                        }
                    }
                    else
                    {
                        //var neighbor = new AStarNode()
                        //{
                        //    parent = actualNodePosition.position,
                        //    g = actualG,
                        //};

                        //neighbor.h = HexHelper.GetDistance(neighborPosition, destination);

                        //AddToOpenSet(openSet, nodes, neighbor, neighborPosition);

                        nodes[neighborPosition.x][neighborPosition.y].parent = actualNodePosition.position;
                        nodes[neighborPosition.x][neighborPosition.y].g = actualG;
                        nodes[neighborPosition.x][neighborPosition.y].h = HexHelper.GetDistance(neighborPosition, destination);
                        nodes[neighborPosition.x][neighborPosition.y].state = AStarNode.States.InOpenSet;
                        openSet.Enqueue(new AStarPosition(neighborPosition), nodes[neighborPosition.x][neighborPosition.y].F);
                        //nodes[position.x][position.y] = node;
                    }
                }
            }

            return new Stack<Vector2Int>();
        }

        private static bool[][] InitializeneighborsExamined(bool[][] fields)
        {
            int sizeX = fields.Length;
            int sizeY = fields[0].Length;
            var neighbors = new bool[sizeX][];

            for (int i = 0; i < sizeX; i++)
                neighbors[i] = new bool[sizeY];
            
            return neighbors;
        }

        #region Helpers

        /// <summary>
        ///     Wyrzuca ewentulne wyjątki związany z niepoprawnymi argumentami funkcji FindPath
        /// </summary>
        /// <param name="fields">Tablica pól z zadeklarowną dostępnością. Muszą dziedziczyć po klasie PathFindableField</param>
        /// <param name="start">Pole startowe</param>
        /// <param name="destination">Pole końcowe</param>
        /// <param name="minIndexes">Minimalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
        /// <param name="maxIndexes">Maksymalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CheckArguments(bool[][] fields, Vector2Int start, Vector2Int destination, Vector2Int minIndexes, Vector2Int maxIndexes)
        {
            if (fields == null)
                throw new ArgumentNullException($"Argument {nameof(fields)} = null.");

            if (minIndexes.x < 0 || minIndexes.y < 0)
                throw new ArgumentOutOfRangeException($"Argument {nameof(minIndexes)} przekracza indeks tablicy {nameof(fields)}.");

            if (maxIndexes.x >= fields.Length || maxIndexes.y >= fields[0].Length)
                throw new ArgumentOutOfRangeException($"Argument {nameof(maxIndexes)} przekracza indeks tablicy {nameof(fields)}.");
        }

        /// <summary>
        ///     Inicjalizuje tablicę do przechowywania węzłów
        /// </summary>
        /// <param name="fields">Tablica pól z zadeklarowną dostępnością</param>
        /// <returns>Pomocnicza tablica</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static AStarNode[][] InitializeAstarNodes(bool[][] fields)
        {
            int sizeX = fields.Length;
            int sizeY = fields[0].Length;
            var nodes = new AStarNode[sizeX][];

            for (int x = 0; x < sizeX; x++)
                nodes[x] = new AStarNode[sizeY];

            //System.Threading.Tasks.Parallel.For(0, sizeX, x =>
            //{
            //    nodes[x] = new AStarNode[sizeY];
            //});

            return nodes;
        }

        /// <summary>
        ///     Inicjalizuje tablicę do przechowywania węzłów
        /// </summary>
        /// <param name="fields">Tablica pól z zadeklarowną dostępnością</param>
        /// <returns>Pomocnicza tablica</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector2Int[][][] InitializeParents(bool[][] fields)
        {
            int sizeX = fields.Length;
            int sizeY = fields[0].Length;
            var nodes = new Vector2Int[sizeX][][];

            for (int x = 0; x < sizeX; x++)
            {
                nodes[x] = new Vector2Int[sizeY][];
            }

            return nodes;
        }

        /// <summary>
        ///     Określa, czy wybrany węzeł znajduje się na liście otwartej
        /// </summary>
        /// <param name="nodePosition">Pozycja wybranego węzła</param>
        /// <param name="nodes">Tablica węzłów</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsInOpenSet(Vector2Int nodePosition, AStarNode[][] nodes)
        {
            return nodes[nodePosition.x][nodePosition.y].state == AStarNode.States.InOpenSet;
            //return nodes[nodePosition.x][nodePosition.y]?.state == Node.States.InOpenSet;
        }

        /// <summary>
        ///     Określa, czy wybrany węzeł znajduje się na liście zamkniętej
        /// </summary>
        /// <param name="nodePosition">Pozycja wybranego węzła</param>
        /// <param name="nodes">Tablica węzłów</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsInClosedSet(Vector2Int nodePosition, AStarNode[][] nodes)
        {
            return nodes[nodePosition.x][nodePosition.y].state == AStarNode.States.InClosedSet;
        }

        /// <summary>
        ///     Zwraca listę sąsiednich węzłów.
        ///     Jeśli nie ma jej w tablicy węzłów - znajduje sąsiadów i dodaje listę do tablicy.
        /// </summary>
        /// <param name="fields">Tablica pól z zadeklarowną dostępnością</param>
        /// <param name="neighbors">Tablica węzłów</param>
        /// <param name="nodePosition">Pozycja wybranego węzła</param>
        /// <param name="minIndexes">Minimalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
        /// <param name="maxIndexes">Maksymalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector2Int[] GetNeighbors(bool[][] fields, Vector2Int[][][] neighbors, AStarNode[][] nodes, Vector2Int nodePosition, Vector2Int minIndexes, Vector2Int maxIndexes)
        {
            //return examined[nodePosition.x][nodePosition.y] == true
            //        ?? (nodes[nodePosition.x][nodePosition.y] = FindAvailableNeighbors(fields, nodePosition.x, nodePosition.y, minIndexes, maxIndexes));

            if (nodes[nodePosition.x][nodePosition.y].neighborFinded)
            {
                return neighbors[nodePosition.x][nodePosition.y];
            }
            else
            {
                nodes[nodePosition.x][nodePosition.y].neighborFinded = true;
                return neighbors[nodePosition.x][nodePosition.y] = FindAvailableNeighbors(fields, nodePosition.x, nodePosition.y, minIndexes, maxIndexes);
                //SetAvailableNeighbors(fields, ref neighbors, nodePosition, minIndexes, maxIndexes);
                //return neighbors[nodePosition.x][nodePosition.y];
            }
        }

        /// <summary>
        ///     Dodaje węzeł do listy otwartej.
        ///     Dodaje do tablicy węzłów.
        /// </summary>
        /// <param name="openSet">Lista otwarta</param>
        /// <param name="nodes">Tablica węzłów</param>
        /// <param name="node">Wybrany węzeł</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddToOpenSet(FastPriorityQueue<AStarPosition> openSet, AStarNode[][] nodes, AStarNode node, Vector2Int position)
        {
            openSet.Enqueue(new AStarPosition(position), node.F);
            node.state = AStarNode.States.InOpenSet;
            nodes[position.x][position.y] = node;
        }

        /// <summary>
        ///     Dodaje węzeł do listy zamkniętej.
        ///     Dodaje do tablicy węzłów.
        /// </summary>
        /// <param name="nodes">Tablica węzłów</param>
        /// <param name="node">Wybrany węzeł</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddToClosedSet(AStarNode[][] nodes, Vector2Int position)
        {
            nodes[position.x][position.y].state = AStarNode.States.InClosedSet;
        }

        /// <summary>
        ///     Zwraca stos z najkrótszą ścieżką.
        /// </summary>
        /// <param name="destination">Węzeł startowy</param>
        /// <returns>Stos</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Stack<Vector2Int> CombinePath(Vector2Int destinationPosition, Vector2Int startPosition, AStarNode[][] nodes)
        {
            var path = new Stack<Vector2Int>(2);

            for (var node = destinationPosition; node != startPosition; node = new Vector2Int(nodes[node.x][node.y].parent.x, nodes[node.x][node.y].parent.y))
                path.Push(node);

            path.Push(startPosition);

            return path;
        }

        /// <summary>
        ///     Wyszukuje dostępnych sąsiadów wybranego pola
        /// </summary>
        /// <param name="fields">Tablica pól z zadeklarowną dostępnością</param>
        /// <param name="x">Pozycja X</param>
        /// <param name="y">Pozycja Y</param>
        /// <param name="minIndexes">Minimalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
        /// <param name="maxIndexes">Maksymalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
        /// <returns>Lista dostępnych sąsiadów</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector2Int[] FindAvailableNeighbors(bool[][] fields, int x, int y, Vector2Int minIndexes, Vector2Int maxIndexes)
        {
            var neighbors = new Vector2Int[6];

            for (var direction = 0; direction < 6; direction++)
            {
                var indexX = Math.Abs(x % 2);
                var neighbor = HexHelper.IndexOfNeighbor(x, y, (HexHelper.Direction)direction);

                if (HasValidIndex(neighbor, minIndexes, maxIndexes) && fields[neighbor.x][neighbor.y])
                    neighbors[direction] = neighbor;
            }

            //System.Threading.Tasks.Parallel.For(0, 6, direction =>
            //{
            //    var indexX = Math.Abs(x % 2);
            //    var neighbor = HexHelper.IndexOfNeighbor(x, y, (HexHelper.Direction)direction);

            //    if (HasValidIndex(neighbor, minIndexes, maxIndexes) && fields[neighbor.x][neighbor.y])
            //        neighbors[direction] = neighbor;
            //});


            return neighbors;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetAvailableNeighbors(bool[][] fields, ref Vector2Int[][][] neighbors, Vector2Int position, Vector2Int minIndexes, Vector2Int maxIndexes)
        {
            for (var direction = 0; direction < 6; direction++)
            {
                var indexX = Math.Abs(position.x % 2);
                var neighbor = HexHelper.IndexOfNeighbor(position.x, position.y, (HexHelper.Direction)direction);

                if (HasValidIndex(neighbor, minIndexes, maxIndexes) && fields[neighbor.x][neighbor.y])
                    neighbors[position.x][position.y][direction] = neighbor;
            }
        }

        /// <summary>
        ///     Określa, czy pole ma dozwolony index
        /// </summary>
        /// <param name="index">Pozycja pola</param>
        /// <param name="minIndexes">Minimalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
        /// <param name="maxIndexes">Maksymalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HasValidIndex(Vector2Int index, Vector2Int minIndexes, Vector2Int maxIndexes)
        {
            return index.x >= minIndexes.x
                && index.y >= minIndexes.y
                && index.x <= maxIndexes.x
                && index.y <= maxIndexes.y;
        }

        #endregion

        private struct AStarNode
        {
            public enum States
            {
                /// <summary>Niesprawdzony</summary>
                Unexamined,
                /// <summary>Na liście otwartej</summary>
                InOpenSet,
                /// <summary>Na liście zamkniętej</summary>
                InClosedSet
            }

            public Vector2Int parent;
            public float h;
            public float g;
            public float F { get { return g + h; } }
            public States state;
            public bool neighborFinded;
        }

        private class AStarPosition : FastPriorityQueueNode
        {
            public Vector2Int position;

            public AStarPosition(Vector2Int position)
            {
                this.position = position;
            }
        }
    }
}
