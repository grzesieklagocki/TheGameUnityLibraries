using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HexGameBoard
{
    public class PriorityQueue<T> where T : IComparable<T>
    {
        public int Count { get { return count; } }
        public int Capacity { get { return capacity; } }

        private T[] queue;
        private int firstItemIndex;
        private int count;
        private int capacity;


        #region Constructors

        /// <summary>
        ///     Domyślny konstruktor
        /// </summary>
        public PriorityQueue() : this(1)
        {

        }

        /// <summary>
        ///     Konstruktor z inicjacją pojemności
        /// </summary>
        /// <param name="capacity"></param>
        public PriorityQueue(int capacity)
        {
            queue = new T[this.capacity = capacity];
        }

        #endregion


        /// <summary>
        ///     Dodaje element do kolejki prioritetowej
        /// </summary>
        public void Enqueue(T item)
        {
            if (count == capacity)
                Array.Resize(ref queue, capacity *= 2);

            Insert(item);

            count++;
        }

        /// <summary>
        ///     Wyjmuje element z początku kolejki priorytetowej
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            if (count == 0)
                throw new Exception("Empty");

            T item = queue[firstItemIndex];
            count--;

            Swap(firstItemIndex, GetIndex(count));

            firstItemIndex = GetIndex(1);

            return item;
        }

        /// <summary>
        ///     Zwraca element z początku kolejki priorytetowej bez usuwania go
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            return queue[firstItemIndex];
        }

        public T this[int index]
        {
            get
            {
                if (index >= count)
                    throw new Exception();
                return queue[GetIndex(index)];
            }

            set
            {
                queue[GetIndex(index)] = value;
            }
        }


        private int GetIndex(int itemIndex)
        {
            return ((firstItemIndex + itemIndex) % capacity);
        }

        private void Swap(int x, int y)
        {
            var temp = queue[x];
            queue[x] = queue[y];
            queue[y] = temp;
        }

        private void Insert(T newItem)
        {
            int i, j;

            i = count;
            j = (i - 1) / 2;

            while (i > 0 && queue[GetIndex(j)].CompareTo(newItem) < 0)
            {
                queue[GetIndex(i)] = queue[GetIndex(j)];
                i = j;
                j = (i - 1) / 2;
            }

            queue[GetIndex(i)] = newItem;
        }
    }
}