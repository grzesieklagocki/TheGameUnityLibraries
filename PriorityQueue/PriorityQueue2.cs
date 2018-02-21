using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriorityQueue
{
    public class PriorityQueue2<TKey, TValue>
    {
        public int Count { get { return list.Count; } }
        public bool IsEmpty { get { return list.Count > 0; } }


        private List<KeyValuePair<TKey, TValue>> list;



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
