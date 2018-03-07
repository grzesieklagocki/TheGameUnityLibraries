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
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static List<Vector2Int> DrawLine(Vector2Int a, Vector2Int b)
        {
            var line = new List<Vector2Int>(2) { a };

            Vector3 epsilon = new Vector3(1e-6f, 1e-6f, -2e-6f);
            Vector3 aCube = AxialToCubeCoordinates(a) + epsilon;
            Vector3 bCube = AxialToCubeCoordinates(b) + epsilon;

            int distance = GetDistance(a, b);
            float multiplier = 1.0f / distance;

            for (int i = 1; i < distance - 1; i++)
                line.Add(CubeToAxialCoordinates(Round(Vector3.Lerp(aCube, bCube,  i * multiplier))));

            line.Add(b);

            return line;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="boardTemplate"></param>
        /// <returns></returns>
        public static bool CanDrawLine(Vector2Int a, Vector2Int b, bool[][] boardTemplate)
        {
            Vector3 epsilon = new Vector3(1e-6f, 1e-6f, -2e-6f);
            Vector3 aCube = AxialToCubeCoordinates(a) + epsilon;
            Vector3 bCube = AxialToCubeCoordinates(b) + epsilon;

            int distance = GetDistance(a, b);
            float multiplier = 1.0f / distance;

            for (int i = 0; i < distance; i++)
            {
                Vector2Int field = CubeToAxialCoordinates(Round(Vector3.Lerp(aCube, bCube, i * multiplier)));

                if (boardTemplate[field.x][field.y] == false)
                    return false;
            }

            return true;
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector3Int Round(Vector3 vector)
        {
            var roundedVector = Vector3Int.RoundToInt(vector);
            var differences = new Vector3()
            {
                x = Math.Abs(vector.x - roundedVector.x),
                y = Math.Abs(vector.y - roundedVector.y),
                z = Math.Abs(vector.z - roundedVector.z)
            };

            if (differences.x > differences.y && differences.x > differences.z)
                roundedVector.x = -(roundedVector.y + roundedVector.z);
            else if (differences.y > differences.z)
                roundedVector.y = -(roundedVector.x + roundedVector.z);
            else
                roundedVector.z = -(roundedVector.x + roundedVector.y);

            return roundedVector;
        }
    }
}
