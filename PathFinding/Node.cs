using Priority_Queue;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameBoard
{
    /// <summary>
    ///     Pole (węzeł) ścieżki dla algorytmu A*.
    /// Współpracuje z FastPriorityQueue:
    /// https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp
    /// </summary>
    internal class Node : FastPriorityQueueNode
    {
        internal enum States
        {
            unexamined,
            onOpenList,
            onClosedList
        }
        /// <summary>
        ///     Poprzednie pole aktualnej ścieżki
        /// </summary>
        internal Node parent;

        /// <summary>
        ///     Index pola (pozycja w tablicy)
        /// </summary>
        internal Vector2Int position;

        /// <summary>
        ///     Lista indeksów pól na które można przejść
        /// </summary>
        internal Node[] neighbors;

        internal States state = 0;

        /// <summary>
        ///     Odległość od pola startowego
        /// </summary>
        internal float g = 0;

        /// <summary>
        ///     Szacowana odległość do celu
        /// </summary>
        internal float h = 0;

        /// <summary>
        ///     Szacowana długość ścieżki
        /// </summary>
        internal float F
        {
            get
            {
                return g + h;
            }
        }


        /// <summary>
        ///     Inicjalizuje nową instację pola
        /// </summary>
        /// <param name="position">Index pola (pozycja w tablicy)</param>
        internal Node(Vector2Int position)
        {
            this.position = position;
        }
    }
}
