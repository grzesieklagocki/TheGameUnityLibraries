using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static HexGameBoard.HexHelper;

namespace HexGameBoard
{
    public class PathFinder<T>
    {
        private Field[][] fields;
        private Vector2Int size;


        #region Contructors

        public PathFinder() { }

        public PathFinder(T[,] gameBoard, Func<T, Vector2Int> positionPredicate)
        {
            Initialize(gameBoard, positionPredicate);
        }

        #endregion


        #region Initialization

        public void Initialize(T[,] gameBoard, Func<T, Vector2Int> positionPredicate)
        {
            size = new Vector2Int(gameBoard.GetLength(0), gameBoard.GetLength(1));

            InitializeArray();
            FillArrayWithFields(gameBoard, positionPredicate);
            FindAllNeighbors();

        }

        private void InitializeArray()
        {
            fields = new Field[size.x][];

            for (int x = 0; x < size.x; x++)
                fields[x] = new Field[size.y];
        }

        private void FillArrayWithFields(T[,] gameBoard, Func<T, Vector2Int> positionPredicate)
        {
            for (int y = 0; y < size.y; y++)
                for (int x = 0; x < size.x; x++)
                    fields[x][y] = new Field(positionPredicate.Invoke(gameBoard[x, y]));
        }

        private void FindAllNeighbors()
        {
            for (int y = 0; y < size.y; y++)
                for (int x = 0; x < size.x; x++)
                {
                    var field = fields[x][y];

                    for (Direction direction = 0; direction <= Direction.upperLeft; direction++)
                    {
                        var neighbor = IndexOfNeighbor(field.position, direction);

                        if (HasValidIndex(neighbor) && FieldAt(neighbor) != null)
                            field.availableNeighbors.Add(neighbor);
                    }
                }

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
