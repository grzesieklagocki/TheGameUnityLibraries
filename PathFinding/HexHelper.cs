using System.Collections.Generic;
using UnityEngine;

namespace HexGameBoard
{
    public static class HexHelper
    {
        public enum Direction
        {
            up,
            upperRight,
            lowerRight,
            bottom,
            lowerLeft,
            upperLeft
        }

        private static Vector2Int[,] translations;

        public static Vector2Int IndexOfNeighbor(Vector2Int position, Direction direction)
        {
            return position + translations[Mathf.Abs(position.x % 2), (int)direction];
        }

        public static List<Vector2Int> FieldsInRange(Vector2Int centerPosition, int radius)
        {
            if (radius < 0)
                throw new System.Exception("radius < 0");

            var fields = new List<Vector2Int> { centerPosition };

            if (radius == 0)
                return fields;

            var startPosition = centerPosition;

            for (int layer = 0; layer < radius; layer++)
            {
                var field = startPosition = IndexOfNeighbor(startPosition, Direction.lowerLeft);

                for (int direction = 0; direction < 6; direction++)
                {
                    for (int i = 0; i <= layer; i++)
                    {
                        field = IndexOfNeighbor(field, (Direction)direction);

                        if (field.x >= 0 && field.y >= 0)
                            fields.Add(field);
                    }
                }

                startPosition = field;
            }

            return fields;
        }


        static HexHelper()
        {
            translations = new Vector2Int[,]
            {
            {
                new Vector2Int(0, -1),
                new Vector2Int(1,-1),
                new Vector2Int(1,0),
                new Vector2Int(0,1),
                new Vector2Int(-1,0),
                new Vector2Int(-1,-1)
            },
            {
                new Vector2Int(0, -1),
                new Vector2Int(1, 0),
                new Vector2Int(1, 1),
                new Vector2Int(0, 1),
                new Vector2Int(-1, 1),
                new Vector2Int(-1, 0)
            }
            };
        }
    }
}
