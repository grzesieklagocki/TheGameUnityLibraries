using System.Collections.Generic;
using UnityEngine;

namespace HexGameBoard.PathFinding
{
    internal struct AStarNode
    {
        public enum States
        {
            /// <summary>Niesprawdzony</summary>
            Unexamined,
            /// <summary>Na liście otwartej</summary>
            InOpenSet,
            /// <summary>Na liście zamkniętej</summary>
            InClosedSet
        }

        public float h;
        public float g;
        public float F { get { return g + h; } }
        public States state;
        public bool neighborFinded;
        public IEnumerable<Vector2Int> neighbors;
    }
}
