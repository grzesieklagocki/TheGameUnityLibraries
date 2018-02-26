using Priority_Queue;
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
        /// <summary>
        ///     Stany, w których może znajdować się węzeł w trakcie rozpatrywania przez algorytm A*
        /// </summary>
        internal enum States
        {
            /// <summary>niesprawdzony</summary>
            unexamined,
            /// <summary>na liście otwarter</summary>
            inOpenSet,
            /// <summary>na liście zamkniętej</summary>
            inClosedSet
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

        /// <summary>
        ///     Aktualny stan, w którym jest węzeł w trakcie rozpatrywania przez algorytm A*
        /// </summary>
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
