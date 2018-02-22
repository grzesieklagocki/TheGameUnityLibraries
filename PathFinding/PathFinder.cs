using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Priority_Queue;
using UnityEngine;
using static HexGameBoard.HexHelper;

namespace HexGameBoard
{
    public class PathFinder<TField>
    {
        private Field[][] fields;
        private Vector2Int size;


        #region Contructors

        public PathFinder(TField[,] gameBoard, Func<TField, Vector2Int> positionPredicate)
        {
            Initialize(gameBoard, positionPredicate);
        }

        #endregion


        /// <summary>
        ///     Blokuje/odblokowuje wybrane pole i usuwa/przywraca powiązania z sąsiadami
        /// </summary>
        /// <param name="field">Pozycja pola do zablokowania/odblokowania</param>
        /// <param name="block">true - blokowanie, false - odblokowywanie</param>
        public void BlockField(Vector2Int field, bool block = true)
        {
            var neighbors = fields[field.x][field.y].availableNeighbors;

            fields[field.x][field.y] = block ? null : new Field(field);

            foreach (var neighbor in neighbors)
                fields[neighbor.x][neighbor.y].availableNeighbors = FindAvailableNeighbors(neighbor.x, neighbor.y);
        }

        /// <summary>
        ///     Wyszukuje najkrótszą ścieżkę między dwoma polami
        /// </summary>
        /// <param name="start">Pozycja początkowa</param>
        /// <param name="destination">Pozycja końcowa</param>
        /// <returns>Kolejka pól z najkrótszą ścieżką</returns>
        public Queue<Vector2Int> Find(Vector2Int start, Vector2Int destination)
        {
            var path = new Queue<Vector2Int>();

            return path;
        }

        #region Initialization

        public void Initialize(TField[,] gameBoard, Func<TField, Vector2Int> positionPredicate)
        {
            size = new Vector2Int(gameBoard.GetLength(0), gameBoard.GetLength(1));

            InitializeArray();
            FillArrayWithFields(gameBoard, positionPredicate);
            FillFieldsWithNeighbors();

        }

        private void InitializeArray()
        {
            fields = new Field[size.x][];

            for (int x = 0; x < size.x; x++)
                fields[x] = new Field[size.y];
        }

        private void FillArrayWithFields(TField[,] gameBoard, Func<TField, Vector2Int> positionPredicate)
        {
            for (int y = 0; y < size.y; y++)
                for (int x = 0; x < size.x; x++)
                    fields[x][y] = new Field(positionPredicate.Invoke(gameBoard[x, y]));
        }

        private void FillFieldsWithNeighbors()
        {
            for (int y = 0; y < size.y; y++)
                for (int x = 0; x < size.x; x++)
                    fields[x][y].availableNeighbors = FindAvailableNeighbors(x, y);
        }

        private List<Vector2Int> FindAvailableNeighbors(int x, int y)
        {
            var neightbors = new List<Vector2Int>();

            for (Direction direction = 0; direction <= Direction.upperLeft; direction++)
            {
                var neighbor = IndexOfNeighbor(fields[x][y].position, direction);

                if (HasValidIndex(neighbor) && FieldAt(neighbor) != null)
                    neightbors.Add(neighbor);
            }

            return neightbors;
        }

        #endregion

        #region Has Valid Index

        private bool HasValidIndex(Vector2Int index)
        {
            return index.x >= 0
                && index.y >= 0
                && index.x < size.x
                && index.y < size.y;
        }
        #endregion

        #region Field At

        private Field FieldAt(Vector2Int index)
        {
            return fields[index.x][index.y];
        }

        #endregion
    }
}
