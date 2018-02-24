using System;
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
    public class PathFinder
    {
        private bool[][] fields;
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
        //public static Stack<Vector2Int> Find(IField[][] fields, Vector2Int start, Vector2Int destination, int minAvailabilityLevel, Action<Vector2Int> action = null)
        //{
        //    var openSet = new SimplePriorityQueue<Field>();
        //    var closedSet = new List<Field>();

        //    var startField = new Field(start);
        //    //startField.h = Heuristics(start, destination);

        //    openSet.Enqueue(startField, 0);

        //    while (openSet.Count > 0)
        //    {
        //        var actualField = openSet.Dequeue();

        //        //System.Diagnostics.Debug.WriteLine($"\nWybrano {actualField.position}, g={actualField.g}, h={actualField.h}, F={actualField.F}, parent={actualField.parent?.position}\n");

        //        // znaleziono ścieżkę
        //        if (actualField.position == destination)
        //            return CombinePath(actualField, startField);

        //        closedSet.Add(actualField);

        //        foreach (var neighborPosition in FindAvailableNeighbors(fields, actualField.position.x, actualField.position.y, minAvailabilityLevel))
        //        {
        //            var neighbor = new Field(neighborPosition);

        //            if (closedSet.Contains(neighbor))
        //            {
        //                //System.Diagnostics.Debug.WriteLine($"Odrzucam {neighbor.position}, g={neighbor.g}, h={neighbor.h}, F={neighbor.F}, parent={neighbor.parent?.position}");
        //                continue;
        //            }

        //            if (!openSet.Contains(neighbor))
        //            {
        //                neighbor.parent = actualField;
        //                neighbor.g = actualField.g + 1;
        //                neighbor.h = Heuristics(neighbor.position, destination);

        //                openSet.Enqueue(neighbor, neighbor.F);

        //                //System.Diagnostics.Debug.WriteLine($"Rozpatrzony {neighbor.position}, g={neighbor.g}, h={neighbor.h}, F={neighbor.F}, parent={neighbor.parent?.position}");
        //            }
        //            else
        //            {
        //                var neighborInQueue = openSet.First(f => f.position == neighborPosition); // raczej powolne
        //                var estimatedG = actualField.g + 1;

        //                if (estimatedG < neighborInQueue.g)
        //                {
        //                    neighborInQueue.parent = actualField;
        //                    neighborInQueue.g = estimatedG;

        //                    openSet.UpdatePriority(neighborInQueue, neighborInQueue.F);

        //                    //System.Diagnostics.Debug.WriteLine($"Rozpatrzony (updated) {neighborInQueue.position}, g={neighborInQueue.g}, h={neighborInQueue.h}, F={neighborInQueue.F}, parent={neighborInQueue.parent?.position}");
        //                }
        //            }

        //            action?.Invoke(neighborPosition);

        //        }
        //    }

        //    return new Stack<Vector2Int>();
        //}

        public static Stack<Vector2Int> Find(IField[][] fields, Vector2Int start, Vector2Int destination, int minAvailabilityLevel, Action<Vector2Int> action = null)
        {
            var tempFields = new Field[fields.Length][];

            for (int x = 0; x < fields.Length; x++)
                tempFields[x] = new Field[fields[0].Length];

            var startField = new Field(start);
            var openSet = new FastPriorityQueue<Field>(fields.Length * fields[0].Length); // sprawdzić
            var closedSet = new List<Field>();

            AddToOpenSet(openSet, tempFields, startField, 0);

            while (openSet.Count > 0)
            {
                var actualField = openSet.Dequeue();                

                // znaleziono ścieżkę
                if (actualField.position == destination)
                    return CombinePath(actualField, startField);

                closedSet.Add(actualField);

                var neighbors = GetNeighbors(fields, tempFields, actualField.position, minAvailabilityLevel);

                foreach (var neighborPosition in neighbors)
                {
                    if (IsOnClosedSet(tempFields, neighborPosition))                      
                        continue;

                    if (IsOnOpenSet(tempFields, neighborPosition))
                    {
                        var neighbor = new Field(neighborPosition)
                        {
                            parent = actualField,
                            g = actualField.g + 1,
                        };

                        neighbor.h = Heuristics(neighbor.position, destination);

                        AddToOpenSet(openSet, tempFields, neighbor, neighbor.F);                      
                    }
                    else
                    {
                        var neighbor = tempFields[neighborPosition.x][neighborPosition.y];
                        var estimatedG = actualField.g + 1;

                        if (estimatedG < neighbor.g)
                        {
                            neighbor.parent = actualField;
                            neighbor.g = estimatedG;

                            openSet.UpdatePriority(neighbor, neighbor.F);                            
                        }
                    }

                    action?.Invoke(neighborPosition);

                }
            }

            return new Stack<Vector2Int>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsOnOpenSet(Field[][] fields, Vector2Int field)
        {
            return (fields[field.x][field.y] == null || !fields[field.x][field.y].onOpenSet);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsOnClosedSet(Field[][] fields, Vector2Int field)
        {
            return fields[field.x][field.y]?.onClosedSet ?? false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static List<Vector2Int> GetNeighbors(IField[][] fields, Field[][] tempFields, Vector2Int field, int minAvailabilityLevel)
        {
            return tempFields[field.x][field.y]?.neighbors
                    ?? (tempFields[field.x][field.y].neighbors = FindAvailableNeighbors(fields, field.x, field.y, minAvailabilityLevel));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddToOpenSet(FastPriorityQueue<Field> queue, Field[][] fields, Field field, float priority)
        {
            field.onOpenSet = true;
            queue.Enqueue(field, priority);
            fields[field.position.x][field.position.y] = field;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Heuristics(Vector2Int a, Vector2Int b)
        {
            //return Vector2Int.Distance(a, b);
            //return Mathf.Abs(a.x - a.y)

            var distance = GetDistance(a, b);
            return distance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Stack<Vector2Int> CombinePath(Field destination, Field start)
        {
            var path = new Stack<Vector2Int>(1);

            //path.Push(destination.position);

            //var field = destination;

            //while ((field = field.parent) != null)
            //    path.Push(field.position);

            for (var field = destination; field != null; field = field.parent)
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
