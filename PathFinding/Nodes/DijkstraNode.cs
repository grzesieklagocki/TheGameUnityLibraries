using Priority_Queue;

namespace HexGameBoard.Nodes
{
    internal class DijkstraNode : FastPriorityQueueNode
    {
        /// <summary>
        ///     Poprzednie pole aktualnej ścieżki
        /// </summary>
        internal DijkstraNode parent;

        /// <summary>
        ///     Lista indeksów pól na które można przejść
        /// </summary>
        internal DijkstraNode[] neighbors;

        /// <summary>
        ///     Odległość od pola startowego
        /// </summary>
        internal float g = 0;
    }
}
