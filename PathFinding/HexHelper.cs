using System;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameBoard
{
    public static class HexHelper
    {
        public enum Direction
        {
            up,
            upperRight,
            lowerRight,
            bottom,
            lowerLeft,
            upperLeft
        }

        private static Vector2Int[,] translations;


        /// <summary>
        ///     Podaje koordynaty wybranego, sąsiedniego hexa
        /// </summary>
        /// <param name="position">Pozycja bazowego hexa</param>
        /// <param name="direction">Kierunek sąsiedniego hexa względem bazowego</param>
        /// <returns>Pozycja sąsiedniego hexa</returns>
        public static Vector2Int IndexOfNeighbor(Vector2Int position, Direction direction)
        {
            return position + translations[Mathf.Abs(position.x % 2), (int)direction];
        }


        /// <summary>
        ///     Zwraca listę wszystkich hexów będacych w odległości nie większej niż zadany promień
        /// </summary>
        /// <param name="centerPosition">Pozycja bazowego hexa</param>
        /// <param name="outerRadius">Promień zewnętrzny</param>
        /// <param name="innerRadius">Promień wewnętrzny</param>
        /// <returns></returns>
        public static List<Vector2Int> FieldsInRange(int centerPositionX, int centerPositionY, int outerRadius, int innerRadius = 0)
        {
            return FieldsInRange(new Vector2Int(centerPositionX, centerPositionY), outerRadius, innerRadius);
        }

        /// <summary>
        ///     Zwraca listę wszystkich hexów będacych w odległości nie większej niż zadany promień
        /// </summary>
        /// <param name="centerPosition">Pozycja bazowego hexa</param>
        /// <param name="outerRadius">Promień zewnętrzny</param>
        /// <param name="innerRadius">Promień wewnętrzny</param>
        /// <returns></returns>
        public static List<Vector2Int> FieldsInRange(Vector2Int centerPosition, int outerRadius, int innerRadius = 0)
        {
            if (outerRadius < 0 || innerRadius < 0)
                throw new ArgumentException("Radius < 0");

            if (outerRadius < innerRadius)
                throw new ArgumentException("Inner Radius > Outer Radius");

            //if (centerPosition == null)
            //    throw new ArgumentNullException($"Argument {nameof(centerPosition)} is null");


            var field = centerPosition;
            var fields = new List<Vector2Int>();

            if (innerRadius == 0)
                fields.Add(centerPosition);                        
            
            for (int layer = 1; layer <= outerRadius; layer++)
            {
                field = IndexOfNeighbor(field, Direction.lowerLeft);

                if (layer >= innerRadius)
                    fields.AddRange(GetHexRing(field, layer));
            }

            return fields;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field">Pole leżace w lewym dolnym rogu okręgu</param>
        /// <param name="layer">Numer warstwy</param>
        /// <returns></returns>
        private static List<Vector2Int> GetHexRing(Vector2Int field, int layer)
        {
            var fields = new List<Vector2Int>();

            for (var direction = 0; direction < 6; direction++)
            {
                for (int i = 0; i < layer; i++)
                {
                    field = IndexOfNeighbor(field, (Direction)direction);

                    if (field.x >= 0 && field.y >= 0)
                        fields.Add(field);
                }
            }

            return fields;
        }


        static HexHelper()
        {
            translations = new Vector2Int[,]
            {
                {
                    new Vector2Int( 0, -1),
                    new Vector2Int( 1, -1),
                    new Vector2Int( 1,  0),
                    new Vector2Int( 0,  1),
                    new Vector2Int(-1,  0),
                    new Vector2Int(-1, -1)
                },
                {
                    new Vector2Int( 0, -1),
                    new Vector2Int( 1,  0),
                    new Vector2Int( 1,  1),
                    new Vector2Int( 0,  1),
                    new Vector2Int(-1,  1),
                    new Vector2Int(-1,  0)
                }
            };
        }
    }
}
