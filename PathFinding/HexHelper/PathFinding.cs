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
        /// <param name="fields">Tablica pól z zadeklarowną dostępnością</param>
        /// <param name="start">Pole startowe</param>
        /// <param name="destination">Pole końcowe</param>
        /// <param name="minAvailabilityLevel">Minimalny poziom dostępności</param>
        /// <returns>Najkrótsza ścieżka (stos)</returns>
        public static Stack<Vector2Int> FindPath(IField[][] fields, Vector2Int start, Vector2Int destination, int minAvailabilityLevel)
        {
            var startField = new Field(start);
            var openSet = new FastPriorityQueue<Field>(fields.Length * fields[0].Length); // sprawdzić
            var closedSet = new List<Field>();
            var fieldsCache = InitializeFieldsCache(fields);

            AddToOpenSet(openSet, fieldsCache, startField);

            while (openSet.Count > 0)
            {
                var actualField = openSet.Dequeue();

                // znaleziono ścieżkę
                if (actualField.position == destination)
                    return CombinePath(actualField, startField);

                closedSet.Add(actualField);

                var neighbors = GetNeighbors(fields, fieldsCache, actualField.position, minAvailabilityLevel);

                foreach (var neighborPosition in neighbors)
                {
                    if (IsOnClosedSet(neighborPosition, fieldsCache))
                        continue;

                    if (IsOnOpenSet(neighborPosition, fieldsCache))
                    {
                        var neighbor = new Field(neighborPosition)
                        {
                            parent = actualField,
                            g = actualField.g + 1,
                        };

                        neighbor.h = Heuristics(neighbor.position, destination);

                        AddToOpenSet(openSet, fieldsCache, neighbor);
                    }
                    else
                    {
                        var neighbor = fieldsCache[neighborPosition.x][neighborPosition.y];
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


        #region Helpers

        /// <summary>
        ///     Inicjalizuje pomocniczą tablicę do przechowywania węzłów
        /// </summary>
        /// <param name="fields">Tablica pól z zadeklarowną dostępnością</param>
        /// <returns>Pomocnicza tablica</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Field[][] InitializeFieldsCache(IField[][] fields)
        {
            var sizeX = fields.Length;
            var sizeY = fields[0].Length;
            var tempFields = new Field[sizeX][];

            for (int x = 0; x < sizeX; x++)
                tempFields[x] = new Field[sizeY];

            return tempFields;
        }

        /// <summary>
        ///     Określa, czy wybrane pole znajduje się na liście otwartej
        /// </summary>
        /// <param name="field">Wybrane pole</param>
        /// <param name="fields">Tymczasowa tablica</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsOnOpenSet(Vector2Int field, Field[][] fields)
        {
            return fields[field.x][field.y] == null || !fields[field.x][field.y].isInOpenSet;
        }

        /// <summary>
        ///     Określa, czy wybrane pole znajduje się na liście zamkniętej
        /// </summary>
        /// <param name="field">Wybrane pole</param>
        /// <param name="fields">Tymczasowa tablica</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsOnClosedSet(Vector2Int field, Field[][] fields)
        {
            return fields[field.x][field.y]?.isInClosedSet ?? false;
        }

        /// <summary>
        ///     Zwraca listę dostępnych sąsiadów.
        ///     Jeśli nie ma ich w fieldsCache - znajduje i dodaje.
        /// </summary>
        /// <param name="fields">Tablica pól z zadeklarowną dostępnością</param>
        /// <param name="fieldsCache">Tymczasowa tablica</param>
        /// <param name="field">Wybrane pole</param>
        /// <param name="minAvailabilityLevel">Minimalny poziom dostępności</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static List<Vector2Int> GetNeighbors(IField[][] fields, Field[][] fieldsCache, Vector2Int field, int minAvailabilityLevel)
        {
            return fieldsCache[field.x][field.y]?.neighbors
                    ?? (fieldsCache[field.x][field.y].neighbors = FindAvailableNeighbors(fields, field.x, field.y, minAvailabilityLevel));
        }

        /// <summary>
        ///     Dodaje pole do listy otwartej.
        ///     Dodaje do fieldsCache.
        /// </summary>
        /// <param name="openSet">Lista otwarta</param>
        /// <param name="fields">Tablica pól z zadeklarowną dostępnością</param>
        /// <param name="field">Wybrane pole</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddToOpenSet(FastPriorityQueue<Field> openSet, Field[][] fields, Field field)
        {
            field.isInOpenSet = true;
            openSet.Enqueue(field, field.F);
            fields[field.position.x][field.position.y] = field;
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
        /// <param name="destination">Pole startowe</param>
        /// <param name="start">Pole końcowe</param>
        /// <returns>Stos</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Stack<Vector2Int> CombinePath(Field destination, Field start)
        {
            var path = new Stack<Vector2Int>(1);

            for (var field = destination; field != null; field = field.parent)
                path.Push(field.position);

            return path;
        }

        /// <summary>
        ///     Wyszukuje dostępnych sąsiadów wybranego pola
        /// </summary>
        /// <param name="fields">Tablica pól z zadeklarowną dostępnością</param>
        /// <param name="x">Pozycja X</param>
        /// <param name="y">Pozycja Y</param>
        /// <param name="minAvailabilityLevel">Minimalny poziom dostępności</param>
        /// <returns>Lista dostępnych sąsiadów</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static List<Vector2Int> FindAvailableNeighbors(IField[][] fields, int x, int y, int minAvailabilityLevel)
        {
            var neightbors = new List<Vector2Int>(2);

            for (var direction = 0; direction < 6; direction++)
            {
                var neighbor = IndexOfNeighbor(fields[x][y].Position, (Direction)direction);

                if (HasValidIndex(neighbor, fields) && fields[neighbor.x][neighbor.y].AvailabilityLevel >= minAvailabilityLevel)
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
        private static bool HasValidIndex(Vector2Int index, IField[][] fields)
        {
            return index.x >= 0
                && index.y >= 0
                && index.x < fields.Length
                && index.y < fields[0].Length;
        }

        #endregion
    }
}
