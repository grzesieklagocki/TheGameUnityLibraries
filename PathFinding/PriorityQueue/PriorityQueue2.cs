using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexGameBoard.PriorityQueue
{
    public class PriorityQueue2<TKey, TValue>
    {
        /// <summary>
        /// 
        /// </summary>
        public int Count { get { return list.Count; } }

        /// <summary>
        /// 
        /// </summary>
        public bool IsEmpty { get { return list.Count > 0; } }


        private IComparer<TKey> comparer;

        private List<KeyValuePair<TKey, TValue>> list;


        #region Contructors

        public PriorityQueue2(IComparer<TKey> comparer, ICollection<KeyValuePair<TKey, TValue>> collection) : this(comparer, collection.Count)
        {
            foreach(var item in collection)
            {
                Enqueue(item);
            }
        }

        public PriorityQueue2(IComparer<TKey> comparer, int capacity = 0) : this(capacity)
        {
            this.comparer = comparer;
        }

        private PriorityQueue2(int capacity)
        {
            list = new List<KeyValuePair<TKey, TValue>>(capacity);
        }

        #endregion


        public void Enqueue(KeyValuePair<TKey, TValue> item)
        {
            list.Add(item);
        }

        public void Enqueue(TKey key, TValue value)
        {
            Enqueue(new KeyValuePair<TKey, TValue>(key, value));
        }

        public TValue Dequeue()
        {
            return list[0].Value;
        }
    }
}
