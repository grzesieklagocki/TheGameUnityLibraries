using Priority_Queue;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameBoard
{
    /// <summary>
    /// Pole (węzeł) ścieżki dla algorytmu A*.
    /// Współpracuje z FastPriorityQueue<T>:
    /// https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp
    /// </summary>
    internal class Node : FastPriorityQueueNode
    {
        /// <summary>
        /// Poprzednie pole aktualnej ścieżki
        /// </summary>
        internal Node parent;

        /// <summary>
        /// Index pola (pozycja w tablicy)
        /// </summary>
        internal Vector2Int position;

        /// <summary>
        /// Lista indeksów pól na które można przejść
        /// </summary>
        internal List<Vector2Int> neighbors;

        /// <summary>
        /// Określa, czy pole jest na zamkniętej liście algorytmu A*
        /// </summary>
        internal bool isInClosedSet = false;

        /// <summary>
        /// Określa, czy pole jest na otwartej liście algorytmu A*
        /// </summary>
        internal bool isInOpenSet = false;

        /// <summary>
        /// Odległość od pola startowego
        /// </summary>
        internal float g = 0;

        /// <summary>
        /// Szacowana odległość do celu
        /// </summary>
        internal float h = 0;

        /// <summary>
        /// Szacowana długość ścieżki
        /// </summary>
        internal float F
        {
            get
            {
                return g + h;
            }
        }


        /// <summary>
        /// Inicjalizuje nową instację pola
        /// </summary>
        /// <param name="position">Index pola (pozycja w tablicy)</param>
        internal Node(Vector2Int position)
        {
            this.position = position;
        }
    }
}
