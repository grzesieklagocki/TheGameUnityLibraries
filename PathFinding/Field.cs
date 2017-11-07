using System.Collections.Generic;
using UnityEngine;

namespace HexGameBoard
{
    internal class Field
    {
        internal Vector2Int position;
        internal List<Vector2Int> availableNeighbors;

        internal float g = 0;
        internal float h = 0;

        internal float F { get { return g + h; } }


        internal Field(Vector2Int position)
        {
            this.position = position;
            availableNeighbors = new List<Vector2Int>(2);
        }
    }
}
