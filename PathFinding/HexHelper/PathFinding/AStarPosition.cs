using Priority_Queue;
using UnityEngine;

namespace HexGameBoard.PathFinding
{
    internal class AStarPosition : FastPriorityQueueNode
    {
        public Vector2Int position;

        public AStarPosition(Vector2Int position)
        {
            this.position = position;
        }
    }
}
