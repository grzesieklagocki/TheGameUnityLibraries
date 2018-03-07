using System;
using UnityEngine;

namespace HexGameBoard
{
    /// <summary>
    ///     Zawiera funkcje pomocnicze dla planszy w układzie hexagonalnym
    /// </summary>
    public abstract partial class HexHelper
    {
        /// <summary>
        ///     Zwraca indeks pola, do którego należy podany punkt
        /// </summary>
        /// <param name="point"></param>
        /// <param name="hexSize"></param>
        /// <returns></returns>
        public static Vector2Int PixelToHexIndex(Vector2 point, float hexSize = 1.0f)
        {
            return PixelToHexIndex(point.x, point.y, hexSize);
        }

        /// <summary>
        ///     Zwraca indeks pola, do którego należy podany punkt
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="hexSize"></param>
        /// <returns></returns>
        public static Vector2Int PixelToHexIndex(float x, float y, float hexSize = 1.0f)
        {
            float q = (x * 2.0f / 3.0f) / hexSize;
            float r = (-x / 3.0f + (float)Math.Sqrt(3) / 3.0f * y) / hexSize;

            return CubeToAxialCoordinates(Round(new Vector3(q, -(q + r), r)));
        }
    }
}
