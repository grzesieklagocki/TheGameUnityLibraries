using Priority_Queue;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameBoard
{
    internal class Field : IEquatable<Field>
    {
        internal Vector2Int position;
        //internal List<Vector2Int> availableNeighbors;

        internal float g = 0;
        internal float h = 0;

        internal float F
        {
            get
            {
                return g + h;
            }
        }

        internal Field parent;

        internal Field(Vector2Int position)
        {
            this.position = position;
            //availableNeighbors = new List<Vector2Int>(2);
        }

        public bool Equals(Field other)
        {
            return position == other?.position;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Field);
        }

        public override int GetHashCode()
        {
            return (1000 * position.x) + position.y;
        }
    }
}
