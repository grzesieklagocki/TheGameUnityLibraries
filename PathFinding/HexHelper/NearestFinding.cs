using HexGameBoard.Nodes;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace HexGameBoard
{
    /// <summary>
    ///     Zawiera funkcje pomocnicze dla planszy w układzie hexagonalnym
    /// </summary>
    public abstract partial class HexHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="boardTemplate"></param>
        /// <param name="position"></param>
        /// <param name="maxDistance"></param>
        /// <returns></returns>
        public List<Vector2Int> FindNearests(bool[][] boardTemplate, Vector2Int position, int maxDistance)
        {
            return FindNearestsWhere(boardTemplate, position, maxDistance, v => true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Vector2Int> FindNearestsWhere(bool[][] boardTemplate, Vector2Int field, int maxDistance, Func<Vector2Int, bool> predicate)
        {
            var minIndexes = Vector2Int.zero;
            var maxIndexes = new Vector2Int(boardTemplate.Length - 1, boardTemplate[0].Length);
            DijkstraNode[][] nodes = InitializeDijkstraNodes(boardTemplate);
            var openSet = new FastPriorityQueue<DijkstraNode>((maxIndexes.x - minIndexes.x) * (maxIndexes.y - minIndexes.y));
            var fields = new List<Vector2Int>();


            return fields;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static DijkstraNode[][] InitializeDijkstraNodes(bool[][] fields)
        {
            int sizeX = fields.Length;
            int sizeY = fields[0].Length;
            var nodes = new DijkstraNode[sizeX][];

            for (int x = 0; x < sizeX; x++)
                nodes[x] = new DijkstraNode[sizeY];

            return nodes;
        }
    }
}
