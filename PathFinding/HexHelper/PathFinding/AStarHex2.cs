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

        #region A*

        /// <summary>
        ///     Wyszukuje najkrótszą ścieżkę pomiędzy dwoma polami.  
        ///     Implementacja algorytmu A*.
        /// </summary>
        /// <param name="fields">Tablica pól z zadeklarowną dostępnością. Muszą dziedziczyć po klasie PathFindableField</param>
        /// <param name="start">Pole startowe</param>
        /// <param name="destination">Pole końcowe</param>
        /// <param name="minIndexLimit">Minimalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
        /// <param name="maxIndexLimit">Maksymalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
        /// <seealso cref="PathFindableField"/>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <returns>Najkrótsza ścieżka (stos)</returns>
        internal static Stack<Vector2Int> FindPath(bool[][] fields, Vector2Int start, Vector2Int destination, Vector2Int minIndexLimit, Vector2Int maxIndexLimit)
        {
#if DEBUG
            CheckArguments(fields, start, destination, minIndexLimit, maxIndexLimit);

#endif
            var openSet = new FastPriorityQueue<AStarPosition>((maxIndexLimit.x - minIndexLimit.x) * (maxIndexLimit.y - minIndexLimit.y));
            var startNode = new AStarNode();
            InitializeAstarNodes(fields, out AStarNode[][] nodes);
            InitializeParents(fields, out Vector2Int[][] parents);

            AddToOpenSet(openSet, nodes, startNode, start);

            while (openSet.Count > 0)
            {
                var actualNodePosition = openSet.Dequeue();

                // znaleziono ścieżkę
                if (actualNodePosition.position == destination)
                    return CombinePath(actualNodePosition.position, start, nodes, parents);

                AddToClosedSet(nodes, actualNodePosition.position);

                foreach (var neighborPosition in GetNeighbors(fields, nodes, actualNodePosition.position, minIndexLimit, maxIndexLimit))
                {
                    if (IsInClosedSet(neighborPosition, nodes))
                        continue;

                    var actualG = nodes[actualNodePosition.position.x][actualNodePosition.position.y].g + 1;

                    if (IsInOpenSet(neighborPosition, nodes))
                    {
                        var neighbor = nodes[neighborPosition.x][neighborPosition.y];

                        if (actualG < neighbor.g)
                        {
                            parents[neighborPosition.x][neighborPosition.y] = actualNodePosition.position;
                            neighbor.g = actualG;

                            openSet.UpdatePriority(actualNodePosition, neighbor.F);
                        }
                    }
                    else
                    {
                        parents[neighborPosition.x][neighborPosition.y] = actualNodePosition.position;
                        nodes[neighborPosition.x][neighborPosition.y].g = actualG;
                        nodes[neighborPosition.x][neighborPosition.y].h = HexHelper.GetDistance(neighborPosition, destination);
                        nodes[neighborPosition.x][neighborPosition.y].state = AStarNode.States.InOpenSet;
                        openSet.Enqueue(new AStarPosition(neighborPosition), nodes[neighborPosition.x][neighborPosition.y].F);
                    }
                }
            }

            return new Stack<Vector2Int>();
        }

        #endregion

        #region Helpers

        /// <summary>
        ///     Wyrzuca ewentulne wyjątki związany z niepoprawnymi argumentami funkcji FindPath
        /// </summary>
        /// <param name="fields">Tablica pól z zadeklarowną dostępnością. Muszą dziedziczyć po klasie PathFindableField</param>
        /// <param name="start">Pole startowe</param>
        /// <param name="destination">Pole końcowe</param>
        /// <param name="minIndexLimit">Minimalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
        /// <param name="maxIndexLimit">Maksymalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CheckArguments(bool[][] fields, Vector2Int start, Vector2Int destination, Vector2Int minIndexLimit, Vector2Int maxIndexLimit)
        {
            if (fields == null)
                throw new ArgumentNullException($"Argument {nameof(fields)} = null.");

            if (minIndexLimit.x < 0 || minIndexLimit.y < 0)
                throw new ArgumentOutOfRangeException($"Argument {nameof(minIndexLimit)} przekracza indeks tablicy {nameof(fields)}.");

            if (maxIndexLimit.x >= fields.Length || maxIndexLimit.y >= fields[0].Length)
                throw new ArgumentOutOfRangeException($"Argument {nameof(maxIndexLimit)} przekracza indeks tablicy {nameof(fields)}.");
        }

        /// <summary>
        ///     Inicjalizuje tablicę do przechowywania węzłów
        /// </summary>
        /// <param name="fields">Tablica pól z zadeklarowną dostępnością</param>
        /// <param name="nodes">Tablica węzłów</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitializeAstarNodes(bool[][] fields, out AStarNode[][] nodes)
        {
            int sizeX = fields.Length;
            int sizeY = fields[0].Length;
            nodes = new AStarNode[sizeX][];

            for (int x = 0; x < sizeX; x++)
                nodes[x] = new AStarNode[sizeY];
        }

        /// <summary>
        ///     Inicjalizuje tablicę do przechowywania rodziców
        /// </summary>
        /// <param name="fields">Tablica pól z zadeklarowną dostępnością</param>
        /// <param name="parents">Tablica rodziców</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitializeParents(bool[][] fields, out Vector2Int[][] parents)
        {
            int sizeX = fields.Length;
            int sizeY = fields[0].Length;
             parents = new Vector2Int[sizeX][];

            for (int x = 0; x < sizeX; x++)
                parents[x] = new Vector2Int[sizeY];
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
        ///     Dodaje węzeł do listy otwartej.
        ///     Dodaje do tablicy węzłów.
        /// </summary>
        /// <param name="openSet">Lista otwarta</param>
        /// <param name="nodes">Tablica węzłów</param>
        /// <param name="node">Wybrany węzeł</param>
        /// <param name="position">Pozycja wybranego węzła</param>
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
        /// <param name="position">Pozycja wybranego węzeł</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddToClosedSet(AStarNode[][] nodes, Vector2Int position)
        {
            nodes[position.x][position.y].state = AStarNode.States.InClosedSet;
        }

        /// <summary>
        ///     Zwraca stos z najkrótszą ścieżką.
        /// </summary>
        /// <param name="destinationPosition">Pozycja węzła docelowego</param>
        /// <param name="startPosition">Pozycja węzła startowego</param>
        /// <param name="nodes">Tablica węzłów</param>
        /// <param name="parents">Tablica "rodziców" danego węzła</param>
        /// <returns>Stos</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Stack<Vector2Int> CombinePath(Vector2Int destinationPosition, Vector2Int startPosition, AStarNode[][] nodes, Vector2Int[][] parents)
        {
            var path = new Stack<Vector2Int>(2);

            for (var node = destinationPosition; node != startPosition; node = new Vector2Int(parents[node.x][node.y].x, parents[node.x][node.y].y))
                path.Push(node);

            path.Push(startPosition);

            return path;
        }

        /// <summary>
        ///     Zwraca listę sąsiednich węzłów.
        ///     Jeśli nie ma jej w tablicy węzłów - znajduje sąsiadów i dodaje listę do tablicy.
        /// </summary>
        /// <param name="fields">Tablica pól z zadeklarowną dostępnością</param>
        /// <param name="nodes">Tablica węzłów</param>
        /// <param name="nodePosition">Pozycja wybranego węzła</param>
        /// <param name="minIndexLimit">Minimalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
        /// <param name="maxIndexLimit">Maksymalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<Vector2Int> GetNeighbors(bool[][] fields, AStarNode[][] nodes, Vector2Int nodePosition, Vector2Int minIndexLimit, Vector2Int maxIndexLimit)
        {
            //return examined[nodePosition.x][nodePosition.y] == true
            //        ?? (nodes[nodePosition.x][nodePosition.y] = FindAvailableNeighbors(fields, nodePosition.x, nodePosition.y, minIndexLimit, maxIndexLimit));

            if (nodes[nodePosition.x][nodePosition.y].neighborFinded)
            {
                return nodes[nodePosition.x][nodePosition.y].neighbors;
            }
            else
            {
                nodes[nodePosition.x][nodePosition.y].neighborFinded = true;

                return nodes[nodePosition.x][nodePosition.y].neighbors = FindAvailableNeighbors(fields, nodePosition.x, nodePosition.y, minIndexLimit, maxIndexLimit);
            }
        }

        /// <summary>
        ///     Wyszukuje dostępnych sąsiadów wybranego pola
        /// </summary>
        /// <param name="fields">Tablica pól z zadeklarowną dostępnością</param>
        /// <param name="x">Pozycja X</param>
        /// <param name="y">Pozycja Y</param>
        /// <param name="minIndexLimit">Minimalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
        /// <param name="maxIndexLimit">Maksymalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
        /// <returns>Lista dostępnych sąsiadów</returns>minIndexLimit
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<Vector2Int> FindAvailableNeighbors(bool[][] fields, int x, int y, Vector2Int minIndexLimit, Vector2Int maxIndexLimit)
        {
            //var neighbors = new Vector2Int[6];

            for (var direction = 0; direction < 6; direction++)
            {
                var indexX = Math.Abs(x % 2);
                var neighbor = HexHelper.IndexOfNeighbor(x, y, (HexHelper.Direction)direction);

                if (HasValidIndex(neighbor, minIndexLimit, maxIndexLimit) && fields[neighbor.x][neighbor.y])
                    yield return neighbor;
            }
        }

        /// <summary>
        ///     Określa, czy pole ma dozwolony index
        /// </summary>
        /// <param name="index">Pozycja pola</param>
        /// <param name="minIndexLimit">Minimalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
        /// <param name="maxIndexLimit">Maksymalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HasValidIndex(Vector2Int index, Vector2Int minIndexLimit, Vector2Int maxIndexLimit)
        {
            return index.x >= minIndexLimit.x
                && index.y >= minIndexLimit.y
                && index.x <= maxIndexLimit.x
                && index.y <= maxIndexLimit.y;
        }

        #endregion
    }
}
