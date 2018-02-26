//using Priority_Queue;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using UnityEngine;

//namespace HexGameBoard
//{
//    public abstract partial class HexHelper
//    {
//        /// <summary>
//        ///     Wyszukuje najkrótszą ścieżkę pomiędzy dwoma polami.  
//        ///     Implementacja algorytmu A*.
//        /// </summary>
//        /// <param name="fields">Tablica pól z zadeklarowną dostępnością. Muszą dziedziczyć po klasie PathFindableField</param>
//        /// <param name="start">Pole startowe</param>
//        /// <param name="destination">Pole końcowe</param>
//        /// <seealso cref="PathFindableField"/>
//        /// <exception cref="ArgumentNullException" />
//        /// <returns>Najkrótsza ścieżka (stos)</returns>
//        public static Stack<Vector2Int> FindPath(PathFindableField[][] fields, Vector2Int start, Vector2Int destination)
//        {
//            return FindPath(fields, start, destination, new Vector2Int(0, 0), new Vector2Int(fields.Length - 1, fields[0].Length - 1));
//        }

//        /// <summary>
//        ///     Wyszukuje najkrótszą ścieżkę pomiędzy dwoma polami.  
//        ///     Implementacja algorytmu A*.
//        /// </summary>
//        /// <param name="fields">Tablica pól z zadeklarowną dostępnością. Muszą dziedziczyć po klasie PathFindableField</param>
//        /// <param name="start">Pole startowe</param>
//        /// <param name="destination">Pole końcowe</param>
//        /// <param name="minIndexes">Minimalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
//        /// <param name="maxIndexes">Maksymalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
//        /// <seealso cref="PathFindableField"/>
//        /// <exception cref="ArgumentNullException" />
//        /// <exception cref="ArgumentOutOfRangeException" />
//        /// <returns>Najkrótsza ścieżka (stos)</returns>
//        public static Stack<Vector2Int> FindPath(PathFindableField[][] fields, Vector2Int start, Vector2Int destination, Vector2Int minIndexes, Vector2Int maxIndexes)
//        {
//#if DEBUG
//            CheckArguments(fields, start, destination, minIndexes, maxIndexes);
//#endif

//            var nodes = InitializeFieldsCache(fields);
//            var openSet = new FastPriorityQueue<Node>(fields.Length * fields[0].Length); // sprawdzić
//            var startNode = new Node(start);

//            AddToOpenSet(openSet, nodes, startNode);

//            while (openSet.Count > 0)
//            {
//                var actualNode = openSet.Dequeue();

//                // znaleziono ścieżkę
//                if (actualNode.position == destination)
//                    return CombinePath(actualNode);

//                AddToClosedSet(nodes, actualNode);

//                var neighbors = GetNeighbors(fields, nodes, actualNode.position, minIndexes, maxIndexes);

//                foreach (var neighborPosition in neighbors)
//                {
//                    if (IsOnClosedSet(neighborPosition, nodes))
//                        continue;

//                    if (IsOnOpenSet(neighborPosition, nodes))
//                    {
//                        var neighbor = nodes[neighborPosition.x][neighborPosition.y];
//                        var g = actualNode.g + 1;

//                        if (g < neighbor.g)
//                        {
//                            neighbor.parent = actualNode;
//                            neighbor.g = g;

//                            openSet.UpdatePriority(neighbor, neighbor.F);
//                        }
//                    }
//                    else
//                    {
//                        var neighbor = new Node(neighborPosition)
//                        {
//                            parent = actualNode,
//                            g = actualNode.g + 1,
//                        };

//                        neighbor.h = Heuristics(neighbor.position, destination);

//                        AddToOpenSet(openSet, nodes, neighbor);
//                    }
//                }
//            }

//            return new Stack<Vector2Int>();
//        }

//        #region Convert Coordinates

//        /// <summary>
//        ///     Konwertuje współrzędne kartezjańskie 2D na współrzędne mapowania sześciennego.
//        /// </summary>
//        /// <param name="axialCoordinates">Współrzędne kartezjańskie 2D</param>
//        /// <returns>Współrzędne mapowania sześciennego</returns>
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static Vector3Int AxialToCubeCoordinates(Vector2Int axialCoordinates)
//        {
//            var x = axialCoordinates.x;
//            var z = axialCoordinates.y - (axialCoordinates.x - (axialCoordinates.x & 1)) / 2;
//            var y = -x - z;

//            return new Vector3Int(x, y, z);
//        }

//        /// <summary>
//        ///     Konwertuje współrzędne mapowania sześciennego na współrzędne kartezjańskie 2D.
//        /// </summary>
//        /// <param name="cubeCoordinates">Współrzędne mapowania sześciennego</param>
//        /// <returns>Współrzędne kartezjańskie 2D</returns>
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static Vector2Int CubeToAxialCoordinates(Vector3Int cubeCoordinates)
//        {
//            return new Vector2Int(cubeCoordinates.x, cubeCoordinates.z + (cubeCoordinates.x - (cubeCoordinates.x & 1)) / 2);
//        }

//        #endregion

//        #region Get Distance

//        /// <summary>
//        ///     Określa dystans pomiędzy dwoma polami (zdefiniowanymi we współrzędnych kartezjańskich 2D). Ignoruje przeszkody.
//        /// </summary>
//        /// <param name="a">Pierwsze pole (współrzędne kartezjańskie 2D)</param>
//        /// <param name="b">Drugie pole (współrzędne kartezjańskie 2D)</param>
//        /// <returns></returns>
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static int GetDistance(Vector2Int a, Vector2Int b)
//        {
//            return GetDistance(AxialToCubeCoordinates(a), AxialToCubeCoordinates(b));
//        }

//        /// <summary>
//        ///     Określa dystans pomiędzy dwoma polami (zdefiniowanymi we współrzędnych mapowania sześniennego). Ignoruje przeszkody.
//        /// </summary>
//        /// <param name="a">Pierwsze pole (współrzędne mapowania sześciennego)</param>
//        /// <param name="b">Drugie pole (współrzędne mapowania sześciennego)</param>
//        /// <returns></returns>
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static int GetDistance(Vector3Int a, Vector3Int b)
//        {
//            return Math.Max(Math.Abs(a.x - b.x), Math.Max(Math.Abs(a.y - b.y), Math.Abs(a.z - b.z)));
//        }

//        #endregion

//        #region Helpers

//        /// <summary>
//        ///     Wyrzuca ewentulne wyjątki związany z niepoprawnymi argumentami funkcji FindPath
//        /// </summary>
//        /// <param name="fields">Tablica pól z zadeklarowną dostępnością. Muszą dziedziczyć po klasie PathFindableField</param>
//        /// <param name="start">Pole startowe</param>
//        /// <param name="destination">Pole końcowe</param>
//        /// <param name="minIndexes">Minimalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
//        /// <param name="maxIndexes">Maksymalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
//        /// <exception cref="ArgumentNullException" />
//        /// <exception cref="ArgumentOutOfRangeException" />
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        private static void CheckArguments(PathFindableField[][] fields, Vector2Int start, Vector2Int destination, Vector2Int minIndexes, Vector2Int maxIndexes)
//        {
//            if (fields == null)
//                throw new ArgumentNullException($"Argument {nameof(fields)} = null.");

//            if (minIndexes.x < 0 || minIndexes.y < 0)
//                throw new ArgumentOutOfRangeException($"Argument {nameof(minIndexes)} przekracza indeks tablicy {nameof(fields)}.");

//            if (maxIndexes.x >= fields.Length || maxIndexes.y >= fields[0].Length)
//                throw new ArgumentOutOfRangeException($"Argument {nameof(maxIndexes)} przekracza indeks tablicy {nameof(fields)}.");
//        }

//        /// <summary>
//        ///     Inicjalizuje tablicę do przechowywania węzłów
//        /// </summary>
//        /// <param name="fields">Tablica pól z zadeklarowną dostępnością</param>
//        /// <returns>Pomocnicza tablica</returns>
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        private static Node[][] InitializeFieldsCache(PathFindableField[][] fields)
//        {
//            var sizeX = fields.Length;
//            var sizeY = fields[0].Length;
//            var nodes = new Node[sizeX][];

//            for (int x = 0; x < sizeX; x++)
//                nodes[x] = new Node[sizeY];

//            return nodes;
//        }

//        /// <summary>
//        ///     Określa, czy wybrany węzeł znajduje się na liście otwartej
//        /// </summary>
//        /// <param name="nodePosition">Pozycja wybranego węzła</param>
//        /// <param name="nodes">Tablica węzłów</param>
//        /// <returns></returns>
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        private static bool IsOnOpenSet(Vector2Int nodePosition, Node[][] nodes)
//        {
//            return nodes[nodePosition.x][nodePosition.y]?.state == Node.States.onOpenList;
//        }

//        /// <summary>
//        ///     Określa, czy wybrany węzeł znajduje się na liście zamkniętej
//        /// </summary>
//        /// <param name="nodePosition">Pozycja wybranego węzła</param>
//        /// <param name="nodes">Tablica węzłów</param>
//        /// <returns></returns>
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        private static bool IsOnClosedSet(Vector2Int nodePosition, Node[][] nodes)
//        {
//            return nodes[nodePosition.x][nodePosition.y]?.state == Node.States.onClosedList;
//        }

//        /// <summary>
//        ///     Zwraca listę sąsiednich węzłów.
//        ///     Jeśli nie ma jej w tablicy węzłów - znajduje sąsiadów i dodaje listę do tablicy.
//        /// </summary>
//        /// <param name="fields">Tablica pól z zadeklarowną dostępnością</param>
//        /// <param name="nodes">Tablica węzłów</param>
//        /// <param name="nodePosition">Pozycja wybranego węzła</param>
//        /// <param name="minIndexes">Minimalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
//        /// <param name="maxIndexes">Maksymalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
//        /// <returns></returns>
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        private static IEnumerable<Vector2Int> GetNeighbors(PathFindableField[][] fields, Node[][] nodes, Vector2Int nodePosition, Vector2Int minIndexes, Vector2Int maxIndexes)
//        {
//            return nodes[nodePosition.x][nodePosition.y]?.neighbors
//                    ?? (nodes[nodePosition.x][nodePosition.y].neighbors = FindAvailableNeighbors(fields, nodePosition.x, nodePosition.y, minIndexes, maxIndexes));
//        }

//        /// <summary>
//        ///     Dodaje węzeł do listy otwartej.
//        ///     Dodaje do tablicy węzłów.
//        /// </summary>
//        /// <param name="openSet">Lista otwarta</param>
//        /// <param name="nodes">Tablica węzłów</param>
//        /// <param name="node">Wybrany węzeł</param>
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        private static void AddToOpenSet(FastPriorityQueue<Node> openSet, Node[][] nodes, Node node)
//        {
//            node.state = Node.States.onOpenList;
//            openSet.Enqueue(node, node.F);
//            nodes[node.position.x][node.position.y] = node;
//        }

//        /// <summary>
//        ///     Dodaje węzeł do listy zamkniętej.
//        ///     Dodaje do tablicy węzłów.
//        /// </summary>
//        /// <param name="nodes">Tablica węzłów</param>
//        /// <param name="node">Wybrany węzeł</param>
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        private static void AddToClosedSet(Node[][] nodes, Node node)
//        {
//            node.state = Node.States.onClosedList;
//        }

//        /// <summary>
//        ///     Funkcja heurestyuczna
//        /// </summary>
//        /// <param name="fieldPosition">Pozycja wybranego pola</param>
//        /// <param name="destinationPosition">Pozycja celu</param>
//        /// <returns>Szacowana odległość</returns>
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        private static float Heuristics(Vector2Int fieldPosition, Vector2Int destinationPosition)
//        {
//            return GetDistance(fieldPosition, destinationPosition);
//        }

//        /// <summary>
//        ///     Zwraca stos z najkrótszą ścieżką.
//        /// </summary>
//        /// <param name="destination">Węzeł startowy</param>
//        /// <param name="start">Węzeł końcowy</param>
//        /// <returns>Stos</returns>
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        private static Stack<Vector2Int> CombinePath(Node destination)
//        {
//            var path = new Stack<Vector2Int>(1);

//            for (var node = destination; node != null; node = node.parent)
//                path.Push(node.position);

//            return path;
//        }

//        /// <summary>
//        ///     Wyszukuje dostępnych sąsiadów wybranego pola
//        /// </summary>
//        /// <param name="fields">Tablica pól z zadeklarowną dostępnością</param>
//        /// <param name="x">Pozycja X</param>
//        /// <param name="y">Pozycja Y</param>
//        /// <param name="minIndexes">Minimalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
//        /// <param name="maxIndexes">Maksymalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
//        /// <returns>Lista dostępnych sąsiadów</returns>
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        private static IEnumerable<Vector2Int> FindAvailableNeighbors(PathFindableField[][] fields, int x, int y, Vector2Int minIndexes, Vector2Int maxIndexes)
//        {
//            var neightbors = new List<Vector2Int>(2);

//            for (var direction = 0; direction < 6; direction++)
//            {
//                var indexX = Math.Abs(x % 2);
//                var neighbor = new Vector2Int(fields[x][y].position.x + offsets[indexX][direction][0], fields[x][y].position.y + offsets[indexX][direction][1]);

//                if (HasValidIndex(neighbor, fields, minIndexes, maxIndexes) && fields[neighbor.x][neighbor.y].isAvailable)
//                    yield return neighbor;
//            }

//        }

//        /// <summary>
//        ///     Określa, czy pole ma dozwolony index
//        /// </summary>
//        /// <param name="index">Pozycja pola</param>
//        /// <param name="fields">Tablica pól z zadeklarowną dostępnością</param>
//        /// <param name="minIndexes">Minimalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
//        /// <param name="maxIndexes">Maksymalne indeksy pól branych pod uwagę przy wyznaczaniu ścieżki</param>
//        /// <returns></returns>
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        private static bool HasValidIndex(Vector2Int index, PathFindableField[][] fields, Vector2Int minIndexes, Vector2Int maxIndexes)
//        {
//            return index.x >= minIndexes.x
//                && index.y >= minIndexes.y
//                && index.x <= maxIndexes.x
//                && index.y <= maxIndexes.y;
//        }

//        #endregion
//    }
//}
