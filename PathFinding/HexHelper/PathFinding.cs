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
        /// <param name="fields">Tablica pól z zadeklarowną dostępnością. Muszą dziedziczyć po klasie HexGameBoard.PathFindableField</param>
        /// <param name="start">Pole startowe</param>
        /// <param name="destination">Pole końcowe</param>
        /// <returns>Najkrótsza ścieżka (stos)</returns>
        public static Stack<Vector2Int> FindPath(PathFindableField[][] fields, Vector2Int start, Vector2Int destination)
        {
            var startField = new Node(start);
            var openSet = new FastPriorityQueue<Node>(fields.Length * fields[0].Length); // sprawdzić
            var closedSet = new List<Node>();
            var nodes = InitializeFieldsCache(fields);

            AddToOpenSet(openSet, nodes, startField);

            while (openSet.Count > 0)
            {
                var actualField = openSet.Dequeue();

                // znaleziono ścieżkę
                if (actualField.position == destination)
                    return CombinePath(actualField, startField);

                closedSet.Add(actualField);

                var neighbors = GetNeighbors(fields, nodes, actualField.position);

                foreach (var neighborPosition in neighbors)
                {
                    if (IsOnClosedSet(neighborPosition, nodes))
                        continue;

                    if (IsOnOpenSet(neighborPosition, nodes))
                    {
                        var neighbor = new Node(neighborPosition)
                        {
                            parent = actualField,
                            g = actualField.g + 1,
                        };

                        neighbor.h = Heuristics(neighbor.position, destination);

                        AddToOpenSet(openSet, nodes, neighbor);
                    }
                    else
                    {
                        var neighbor = nodes[neighborPosition.x][neighborPosition.y];
                        var g = actualField.g + 1;

                        if (g < neighbor.g)
                        {
                            neighbor.parent = actualField;
                            neighbor.g = g;

                            openSet.UpdatePriority(neighbor, neighbor.F);
                        }
                    }
                }
            }

            return new Stack<Vector2Int>();
        }

        #region Convert Coordinates

        /// <summary>
        ///     Konwertuje współrzędne kartezjańskie 2D na współrzędne mapowania sześciennego.
        /// </summary>
        /// <param name="axialCoordinates">Współrzędne kartezjańskie 2D</param>
        /// <returns>Współrzędne mapowania sześciennego</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int AxialToCubeCoordinates(Vector2Int axialCoordinates)
        {
            var x = axialCoordinates.x;
            var z = axialCoordinates.y - (axialCoordinates.x - (axialCoordinates.x & 1)) / 2;
            var y = -x - z;

            return new Vector3Int(x, y, z);
        }

        /// <summary>
        ///     Konwertuje współrzędne mapowania sześciennego na współrzędne kartezjańskie 2D.
        /// </summary>
        /// <param name="cubeCoordinates">Współrzędne mapowania sześciennego</param>
        /// <returns>Współrzędne kartezjańskie 2D</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int CubeToAxialCoordinates(Vector3Int cubeCoordinates)
        {
            return new Vector2Int(cubeCoordinates.x, cubeCoordinates.z + (cubeCoordinates.x - (cubeCoordinates.x & 1)) / 2);
        }

        #endregion

        #region Get Distance

        /// <summary>
        ///     Określa dystans pomiędzy dwoma polami (zdefiniowanymi we współrzędnych kartezjańskich 2D). Ignoruje przeszkody.
        /// </summary>
        /// <param name="a">Pierwsze pole (współrzędne kartezjańskie 2D)</param>
        /// <param name="b">Drugie pole (współrzędne kartezjańskie 2D)</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetDistance(Vector2Int a, Vector2Int b)
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
        public static int GetDistance(Vector3Int a, Vector3Int b)
        {
            return Math.Max(Math.Abs(a.x - b.x), Math.Max(Math.Abs(a.y - b.y), Math.Abs(a.z - b.z)));
        }

        #endregion

        #region Helpers

        /// <summary>
        ///     Inicjalizuje tablicę do przechowywania węzłów
        /// </summary>
        /// <param name="fields">Tablica pól z zadeklarowną dostępnością</param>
        /// <returns>Pomocnicza tablica</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Node[][] InitializeFieldsCache(PathFindableField[][] fields)
        {
            var sizeX = fields.Length;
            var sizeY = fields[0].Length;
            var nodes = new Node[sizeX][];

            for (int x = 0; x < sizeX; x++)
                nodes[x] = new Node[sizeY];

            return nodes;
        }

        /// <summary>
        ///     Określa, czy wybrany węzeł znajduje się na liście otwartej
        /// </summary>
        /// <param name="nodePosition">Pozycja wybranego węzła</param>
        /// <param name="nodes">Tablica węzłów</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsOnOpenSet(Vector2Int nodePosition, Node[][] nodes)
        {
            return nodes[nodePosition.x][nodePosition.y] == null || !nodes[nodePosition.x][nodePosition.y].isInOpenSet;
        }

        /// <summary>
        ///     Określa, czy wybrany węzeł znajduje się na liście zamkniętej
        /// </summary>
        /// <param name="nodePosition">Pozycja wybranego węzła</param>
        /// <param name="nodes">Tablica węzłów</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsOnClosedSet(Vector2Int nodePosition, Node[][] nodes)
        {
            return nodes[nodePosition.x][nodePosition.y]?.isInClosedSet ?? false;
        }

        /// <summary>
        ///     Zwraca listę sąsiednich węzłów.
        ///     Jeśli nie ma jej w tablicy węzłów - znajduje sąsiadów i dodaje listę do tablicy.
        /// </summary>
        /// <param name="fields">Tablica pól z zadeklarowną dostępnością</param>
        /// <param name="nodes">Tablica węzłów</param>
        /// <param name="nodePosition">Pozycja wybranego węzła</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static List<Vector2Int> GetNeighbors(PathFindableField[][] fields, Node[][] nodes, Vector2Int nodePosition)
        {
            return nodes[nodePosition.x][nodePosition.y]?.neighbors
                    ?? (nodes[nodePosition.x][nodePosition.y].neighbors = FindAvailableNeighbors(fields, nodePosition.x, nodePosition.y));
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
            node.isInOpenSet = true;
            openSet.Enqueue(node, node.F);
            nodes[node.position.x][node.position.y] = node;
        }

        /// <summary>
        ///     Funkcja heurestyuczna
        /// </summary>
        /// <param name="fieldPosition">Pozycja wybranego pola</param>
        /// <param name="destinationPosition">Pozycja celu</param>
        /// <returns>Szacowana odległość</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Heuristics(Vector2Int fieldPosition, Vector2Int destinationPosition)
        {
            return GetDistance(fieldPosition, destinationPosition);
        }

        /// <summary>
        ///     Zwraca stos z najkrótszą ścieżką.
        /// </summary>
        /// <param name="destination">Węzeł startowy</param>
        /// <param name="start">Węzeł końcowy</param>
        /// <returns>Stos</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Stack<Vector2Int> CombinePath(Node destination, Node start)
        {
            var path = new Stack<Vector2Int>(1);
            
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
        /// <returns>Lista dostępnych sąsiadów</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static List<Vector2Int> FindAvailableNeighbors(PathFindableField[][] fields, int x, int y)
        {
            var neightbors = new List<Vector2Int>(2);

            for (var direction = 0; direction < 6; direction++)
            {
                var neighbor = IndexOfNeighbor(fields[x][y].position, (Direction)direction);

                if (HasValidIndex(neighbor, fields) && fields[neighbor.x][neighbor.y].isAvailable)
                    neightbors.Add(neighbor);
            }

            return neightbors;
        }

        /// <summary>
        ///     Określa, czy pole ma dozwolony index
        /// </summary>
        /// <param name="index">Pozycja pola</param>
        /// <param name="fields">Tablica pól z zadeklarowną dostępnością</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HasValidIndex(Vector2Int index, PathFindableField[][] fields)
        {
            return index.x >= 0
                && index.y >= 0
                && index.x < fields.Length
                && index.y < fields[0].Length; 
        }

        #endregion
    }
}
