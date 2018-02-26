using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace HexGameBoard
{
    /// <summary>
    ///     Zawiera funkcje pomocnicze dla planszy w układzie hexagonalnym
    /// </summary>
    public abstract partial class HexHelper
    {
        /// <summary>
        ///     Możliwe położenia sąsiada względem pola
        /// </summary>
        public enum Direction
        {
            /// <summary>Powyżej</summary>
            up,
            /// <summary>W prawym górnym rogu</summary>
            upperRight,
            /// <summary>W prawym dolnym rogu</summary>
            lowerRight,
            /// <summary>Poniżej</summary>
            bottom,
            /// <summary>W lewym dolnym rogu</summary>
            lowerLeft,
            /// <summary>W lewym górnym rogu</summary>
            upperLeft
        }

        /// <summary>
        ///     Offsety do obliczania indeksu pola sąsiada
        /// </summary>
        //private static readonly Vector2Int[,] offsets;
        protected static readonly int[][][] offsets = new int[][][]
            {
                new int[][]
                {
                    new int[] {  0, -1 },
                    new int[] {  1, -1 },
                    new int[] {  1,  0 },
                    new int[] {  0,  1 },
                    new int[] { -1,  0 },
                    new int[] { -1, -1 }
                },
                new int[][]
                {
                    new int[] {  0, -1 },
                    new int[] {  1,  0 },
                    new int[] {  1,  1 },
                    new int[] {  0,  1 },
                    new int[] { -1,  1 },
                    new int[] { -1,  0 },
                }
            };



        /// <summary>
        ///     Podaje koordynaty wybranego, sąsiedniego hexa
        /// </summary>
        /// <param name="position">Pozycja bazowego hexa</param>
        /// <param name="direction">Kierunek sąsiedniego hexa względem bazowego</param>
        /// <returns>Pozycja sąsiedniego hexa</returns>
        public static Vector2Int IndexOfNeighbor(Vector2Int position, Direction direction)
        {
            return IndexOfNeighbor(position.x, position.y, direction);
        }

        /// <summary>
        ///     Podaje koordynaty wybranego, sąsiedniego hexa
        /// </summary>
        /// <param name="parentX">Pozycja X bazowego hexa</param>
        /// <param name="parentY">Pozycja Y bazowego hexa</param>
        /// <param name="direction">Kierunek sąsiedniego hexa względem bazowego</param>
        /// <returns></returns>
        public static Vector2Int IndexOfNeighbor(int parentX, int parentY, Direction direction)
        {
            var indexX = Math.Abs(parentX & 1);

            return new Vector2Int(parentX + offsets[indexX][(int)direction][0], parentY + offsets[indexX][(int)direction][1]);
        }

        /// <summary>
        ///     Zwraca listę wszystkich hexów będacych w odległości nie większej niż zadany promień
        /// </summary>
        /// <param name="centerPositionX">Pozycja X bazowego hexa</param>
        /// <param name="centerPositionY">Pozycja Y bazowego hexa</param>
        /// <param name="outerRadius">Promień zewnętrzny</param>
        /// <param name="innerRadius">Promień wewnętrzny</param>
        /// <returns>Lista hexów w zasięgu</returns>
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
        /// <returns>Lista hexów w zasięgu</returns>
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


/// <summary>
///     Zwraca listę wszystkich hexów będacych na określonej warstwie. Funkcja pomonicza dla FieldsInRange().
/// </summary>
/// <param name="field">Pole leżace w lewym dolnym rogu okręgu</param>
/// <param name="layer">Numer warstwy</param>
/// <returns>Lista pól</returns>
[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    }
}
