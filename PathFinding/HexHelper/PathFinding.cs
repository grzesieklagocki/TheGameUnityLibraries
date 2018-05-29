using HexGameBoard.Nodes;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace HexGameBoard
{
    public abstract partial class HexHelper
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
        public static Stack<Vector2Int> FindPath(bool[][] fields, Vector2Int start, Vector2Int destination)
        {
            //return FindPath(fields, start, destination, new Vector2Int(0, 0), new Vector2Int(fields.Length - 1, fields[0].Length - 1));
            return PathFinding.AStarHex.FindPath(fields, start, destination, new Vector2Int(0, 0), new Vector2Int(fields.Length - 1, fields[0].Length - 1));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="start"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static Stack<Vector2Int> FindPath2(bool[][] fields, Vector2Int start, Vector2Int destination)
        {
            //return FindPath(fields, start, destination, new Vector2Int(0, 0), new Vector2Int(fields.Length - 1, fields[0].Length - 1));
            return PathFinding.AStarHex2.FindPath(fields, start, destination, new Vector2Int(0, 0), new Vector2Int(fields.Length - 1, fields[0].Length - 1));
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
        public static Stack<Vector2Int> FindPath(bool[][] fields, Vector2Int start, Vector2Int destination, Vector2Int minIndexes, Vector2Int maxIndexes)
        {
            #if DEBUG
            CheckArguments(fields, start, destination, minIndexes, maxIndexes);
            #endif

            Node[][] nodes = InitializeAstarNodes(fields);
            var openSet = new FastPriorityQueue<Node>((maxIndexes.x - minIndexes.x) * (maxIndexes.y - minIndexes.y));
            var startNode = new Node(start);

            AddToOpenSet(openSet, nodes, startNode);

            while (openSet.Count > 0)
            {
                var actualNode = openSet.Dequeue();

                // znaleziono ścieżkę
                if (actualNode.position == destination)
                    return CombinePath(actualNode);

                AddToClosedSet(nodes, actualNode);

                foreach (var neighborPosition in GetNeighbors(fields, nodes, actualNode.position, minIndexes, maxIndexes))
                {
                    if (IsInClosedSet(neighborPosition, nodes))
                        continue;

                    var actualG = actualNode.g + 1;

                    if (IsInOpenSet(neighborPosition, nodes))
                    {
                        var neighbor = nodes[neighborPosition.x][neighborPosition.y];

                        if (actualG < neighbor.g)
                        {
                            neighbor.parent = actualNode;
                            neighbor.g = actualG;

                            openSet.UpdatePriority(neighbor, neighbor.F);
                        }
                    }
                    else
                    {
                        var neighbor = new Node(neighborPosition)
                        {
                            parent = actualNode,
                            g = actualG,
                        };

                        neighbor.h = GetDistance(neighbor.position, destination);

                        AddToOpenSet(openSet, nodes, neighbor);
                    }
                }
            }

            return new Stack<Vector2Int>();
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
        private static Node[][] InitializeAstarNodes(bool[][] fields)
        {
            int sizeX = fields.Length;
            int sizeY = fields[0].Length;
            var nodes = new Node[sizeX][];

            for (int x = 0; x < sizeX; x++)
                nodes[x] = new Node[sizeY];

            //System.Threading.Tasks.Parallel.For(0, sizeX, x =>
            //{
            //    nodes[x] = new Node[sizeY];
            //});

            return nodes;
        }

        /// <summary>
        ///     Określa, czy wybrany węzeł znajduje się na liście otwartej
        /// </summary>
        /// <param name="nodePosition">Pozycja wybranego węzła</param>
        /// <param name="nodes">Tablica węzłów</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsInOpenSet(Vector2Int nodePosition, Node[][] nodes)
        {
            return nodes[nodePosition.x][nodePosition.y] != null;
            //return nodes[nodePosition.x][nodePosition.y]?.state == Node.States.InOpenSet;
        }

        /// <summary>
        ///     Określa, czy wybrany węzeł znajduje się na liście zamkniętej
        /// </summary>
        /// <param name="nodePosition">Pozycja wybranego węzła</param>
        /// <param name="nodes">Tablica węzłów</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsInClosedSet(Vector2Int nodePosition, Node[][] nodes)
        {
            return nodes[nodePosition.x][nodePosition.y].state == Node.States.InClosedSet;
        }

        /// <summary>
        ///     Zwraca listę sąsiednich węzłów.
        ///     Jeśli nie ma jej w tablicy węzłów - znajduje sąsiadów i dodaje listę do tablicy.
        /// </summary>
        /// <param name="fields">Tablica pól z zadeklarowną dostępnością</param>
        /// <param name="nodes">Tablica węzłów</param>
        /// <param name="nodePosition">Pozycja wybranego węzła</param>
        /// <param name="minIndexes">Minimalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
        /// <param name="maxIndexes">Maksymalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<Vector2Int> GetNeighbors(bool[][] fields, Node[][] nodes, Vector2Int nodePosition, Vector2Int minIndexes, Vector2Int maxIndexes)
        {
            return nodes[nodePosition.x][nodePosition.y]?.neighbors2
                    ?? (nodes[nodePosition.x][nodePosition.y].neighbors2 = FindAvailableNeighbors(fields, nodePosition.x, nodePosition.y, minIndexes, maxIndexes));
        }

        /// <summary>
        ///     Dodaje węzeł do listy otwartej.
        ///     Dodaje do tablicy węzłów.
        /// </summary>
        /// <param name="openSet">Lista otwarta</param>
        /// <param name="nodes">Tablica węzłów</param>
        /// <param name="node">Wybrany węzeł</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddToOpenSet(FastPriorityQueue<Node> openSet, Node[][] nodes, Node node)
        {
            node.state = Node.States.InOpenSet;
            openSet.Enqueue(node, node.F);
            nodes[node.position.x][node.position.y] = node;
        }

        /// <summary>
        ///     Dodaje węzeł do listy zamkniętej.
        ///     Dodaje do tablicy węzłów.
        /// </summary>
        /// <param name="nodes">Tablica węzłów</param>
        /// <param name="node">Wybrany węzeł</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddToClosedSet(Node[][] nodes, Node node)
        {
            node.state = Node.States.InClosedSet;
        }

        /// <summary>
        ///     Zwraca stos z najkrótszą ścieżką.
        /// </summary>
        /// <param name="destination">Węzeł startowy</param>
        /// <returns>Stos</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Stack<Vector2Int> CombinePath(Node destination)
        {
            var path = new Stack<Vector2Int>(2);

            for (var node = destination; node != null; node = node.parent)
                path.Push(node.position);

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
        private static IEnumerable<Vector2Int> FindAvailableNeighbors(bool[][] fields, int x, int y, Vector2Int minIndexes, Vector2Int maxIndexes)
        {
            var neightbors = new List<Vector2Int>(6);

            for (var direction = 0; direction < 6; direction++)
            {
                var indexX = Math.Abs(x % 2);
                var neighbor = new Vector2Int(x + offsets[indexX][direction][0], y + offsets[indexX][direction][1]);

                if (HasValidIndex(neighbor, minIndexes, maxIndexes) && fields[neighbor.x][neighbor.y])
                    yield return neighbor;
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
    }
}