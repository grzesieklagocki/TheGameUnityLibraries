﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Priority_Queue;
using UnityEngine;
using static HexGameBoard.HexHelper;

namespace HexGameBoard
{
    public static class PathFinder
    {
        //private Field[][] fields;
        //private Vector2Int size;


        #region Contructors

        //public PathFinder(TField[,] gameBoard, Func<TField, Vector2Int> positionPredicate)
        //{
        //    Initialize(gameBoard, positionPredicate);
        //}

        #endregion


        /// <summary>
        ///     Blokuje/odblokowuje wybrane pole i usuwa/przywraca powiązania z sąsiadami
        /// </summary>
        /// <param name="field">Pozycja pola do zablokowania/odblokowania</param>
        /// <param name="block">true - blokowanie, false - odblokowywanie</param>
        //public void BlockField(Vector2Int field, bool block = true)
        //{
        //    var neighbors = fields[field.x][field.y].availableNeighbors;

        //    fields[field.x][field.y] = block ? null : new Field(field);

        //    foreach (var neighbor in neighbors)
        //        fields[neighbor.x][neighbor.y].availableNeighbors = FindAvailableNeighbors(neighbor.x, neighbor.y);
        //}


        /// <summary>
        ///     Wyszukuje najkrótszą ścieżkę między dwoma polami
        /// </summary>
        /// <param name="start">Pozycja początkowa</param>
        /// <param name="destination">Pozycja końcowa</param>
        /// <returns>Kolejka pól z najkrótszą ścieżką</returns>
        public static Stack<Vector2Int> Find(IField[][] fields, Vector2Int start, Vector2Int destination, int minAvailabilityLevel)
        {
            var openSet = new SimplePriorityQueue<Field>();
            var closedSet = new List<Field>();

            var startField = new Field(start);

            openSet.Enqueue(startField, 0);

            while (openSet.Count > 0)
            {
                var actualField = openSet.Dequeue();

                // znaleziono ścieżkę
                if (actualField.position == destination)
                    return CombinePath(actualField, startField);

                closedSet.Add(actualField);

                foreach (var neighborPosition in FindAvailableNeighbors(fields, actualField.position.x, actualField.position.y, minAvailabilityLevel))
                {
                    var neighbor = new Field(neighborPosition);

                    if (closedSet.Contains(neighbor))
                        break;

                    if (!openSet.Contains(neighbor))
                    {
                        neighbor.parent = actualField;
                        neighbor.g = actualField.g + 1;
                        neighbor.h = Heuristics(neighbor.position, actualField.position);

                        openSet.Enqueue(neighbor, neighbor.F);
                    }
                    else
                    {                        
                        var neighborInQueue = openSet.First(f => f.position == neighborPosition); // raczej powolne
                        var estimatedG = neighbor.g + 1;

                        if (estimatedG < neighborInQueue.g)
                        {
                            neighborInQueue.parent = actualField;
                            neighborInQueue.g = estimatedG;

                            openSet.UpdatePriority(neighborInQueue, neighborInQueue.F);
                        }
                    }
                }
            }

            return new Stack<Vector2Int>();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Heuristics(Vector2Int a, Vector2Int b)
        {
            return Vector2Int.Distance(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Stack<Vector2Int> CombinePath(Field destination, Field start)
        {
            var path = new Stack<Vector2Int>(1);

            path.Push(destination.position);

            var field = destination;

            while ((field = field.parent) != null)
                path.Push(field.position);

            return path;
        }
        #region Initialization

        //public void Initialize(TField[,] gameBoard, Func<TField, Vector2Int> positionPredicate)
        //{
        //    size = new Vector2Int(gameBoard.GetLength(0), gameBoard.GetLength(1));

        //    InitializeArray();
        //    FillArrayWithFields(gameBoard, positionPredicate);
        //    FillFieldsWithNeighbors();

        //}

        //private void InitializeArray()
        //{
        //    fields = new Field[size.x][];

        //    for (int x = 0; x < size.x; x++)
        //        fields[x] = new Field[size.y];
        //}

        //private void FillArrayWithFields(TField[,] gameBoard, Func<TField, Vector2Int> positionPredicate)
        //{
        //    for (int y = 0; y < size.y; y++)
        //        for (int x = 0; x < size.x; x++)
        //            fields[x][y] = new Field(positionPredicate.Invoke(gameBoard[x, y]));
        //}

        //private void FillFieldsWithNeighbors()
        //{
        //    for (int y = 0; y < size.y; y++)
        //        for (int x = 0; x < size.x; x++)
        //            fields[x][y].availableNeighbors = FindAvailableNeighbors(x, y);
        //}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static List<Vector2Int> FindAvailableNeighbors(IField[][] fields, int x, int y, int minAvailabilityLevel)
        {
            var neightbors = new List<Vector2Int>();

            for (var direction = 0; direction < 6; direction++)
            {
                var neighbor = IndexOfNeighbor(fields[x][y].Position, (Direction)direction);

                if (HasValidIndex(neighbor, fields) && fields[neighbor.x][neighbor.y].AvailabilityLevel >= minAvailabilityLevel)
                    neightbors.Add(neighbor);
            }

            return neightbors;
        }

        #endregion

        #region Has Valid Index

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HasValidIndex(Vector2Int index, IField[][] fields)
        {
            return index.x >= 0
                && index.y >= 0
                && index.x < fields.Length
                && index.y < fields[0].Length;
        }
        #endregion

        #region Field At

        //private IField FieldAt(IField[][] fields, Vector2Int index)
        //{
        //    return fields[index.x][index.y];
        //}

        #endregion
    }
}
