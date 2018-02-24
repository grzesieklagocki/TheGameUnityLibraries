using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexGameBoard
{
    public class PriorityQueue<T> : Priority_Queue.SimplePriorityQueue<T>
    {
        public bool FastContains(T item)
        {
            return this.Any(i => i.Equals(item));
        }
    }
}
