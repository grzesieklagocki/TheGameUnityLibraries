using Priority_Queue;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameBoard.Nodes
{
    /// <summary>
    ///     Węzeł ścieżki dla algorytmu A*.
    ///     Współpracuje z FastPriorityQueue:
    ///     https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp
    /// </summary>
    internal class Node : FastPriorityQueueNode
    {
        /// <summary>
        ///     Stany, w których może znajdować się węzeł w trakcie rozpatrywania przez algorytm A*
        /// </summary>
        internal enum States
        {
            /// <summary>Niesprawdzony</summary>
            Unexamined,
            /// <summary>Na liście otwartej</summary>
            InOpenSet,
            /// <summary>Na liście zamkniętej</summary>
            InClosedSet
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
        internal IEnumerable<Vector2Int> neighbors2;
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
        ///     Inicjalizuje nową instację węzła
        /// </summary>
        /// <param name="position">Index pola (pozycja w tablicy)</param>
        internal Node(Vector2Int position)
        {
            this.position = position;
        }

        /// <summary>
        ///     Inicjalizuje nową instację węzła
        /// </summary>
        /// <param name="x">Index X pola (pozycja w tablicy)</param>
        /// <param name="y">Index Y pola (pozycja w tablicy)</param>
        internal Node(int x, int y) : 
            this(new Vector2Int(x, y))
        {

        }
    }
}
