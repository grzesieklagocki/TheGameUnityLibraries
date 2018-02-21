using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexGameBoard.PriorityQueue
{
    public class PriorityQueue<TKey, TValue> : SortedList<TKey, TValue>
    {
        #region Constructors

        public PriorityQueue() : base()
        {

        }

        public PriorityQueue(int capacity) : base(capacity)
        {

        }

        public PriorityQueue(IComparer<TKey> comparer) : base(comparer)
        {

        }

        public PriorityQueue(Dictionary<TKey, TValue> dictionary) : base(dictionary)
        {

        }

        public PriorityQueue(int capacity, IComparer<TKey> comparer) : base(capacity, comparer)
        {

        }

        public PriorityQueue(Dictionary<TKey, TValue> dictionary, IComparer<TKey> comparer) : base(dictionary, comparer)
        {

        }

        #endregion

        public TValue Dequeue()
        {
            var min = this.ElementAt(0);

            RemoveAt(0);

            return min.Value;
        }

        public TValue Peek()
        {
            return this.ElementAt(0).Value;
        }
    }
}
