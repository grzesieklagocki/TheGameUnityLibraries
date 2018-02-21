using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace HexGameBoard
{
    public class PriorityQueue<T> where T : IComparable<T>
    {
        public int Count { get { return list.Count; } }
        public int Capacity { get { return list.Capacity; } }

        private List<T> list;


        #region Constructors

        /// <summary>
        ///     Domyślny konstruktor
        /// </summary>
        public PriorityQueue() : this(4)
        {

        }

        /// <summary>
        ///     Konstruktor z inicjalizacją pojemności
        /// </summary>
        /// <param name="capacity">Pojemność początkowa kolejki</param>
        public PriorityQueue(int capacity)
        {
            list = new List<T>(capacity);
        }

        /// <summary>
        ///     Konstruktor z inicjalizacją elementów
        /// </summary>
        /// <param name="collection">Elementy, które zostaną dodane do kolejki</param>
        public PriorityQueue(IEnumerable<T> collection) : this(collection.Count())
        {
            if (collection == null)
                throw new ArgumentNullException();

            foreach (var item in collection)
                Insert(item);
        }

        #endregion


        /// <summary>
        ///     Dodaje element do kolejki priorytetowej
        /// </summary>
        /// <param name="item">Element do dodania</param>
        public void Enqueue(T item)
        {
            Insert(item);
        }

        /// <summary>
        ///     Wyjmuje element z początku kolejki priorytetowej
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            if (Count == 0)
                throw new Exception("Collection is empty");

            T item = list[0];
            list.RemoveAt(0);

            return item;
        }

        /// <summary>
        ///     Zwraca element z początku kolejki priorytetowej bez usuwania go
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            return list[0];
        }

        public bool Contains(T item)
        {
            return list.Contains(item);
        }

              

        private void Swap(int x, int y)
        {
            var temp = list[x];
            list[x] = list[y];
            list[y] = temp;
        }

        private void Insert(T newItem)
        {
            //int i, j;

            //i = Count;
            //j = (i - 1) / 2;

            //while (i > 0 && list[j].CompareTo(newItem) < 0)
            //{
            //    list[i] = list[j];
            //    i = j;
            //    j = (i - 1) / 2;
            //}

            //list[i] = newItem;

            //for (int i = 0; i > 0 && ; )
            //{
            //    if (n > data[Index / 2])
            //        Swap(Index, Index / 2);
            //    else break;
            //    Index = Index / 2;
            //}
        }

        private void Heapify(int index)
        {
            var leftChild = GetLeftChildIndex(index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetLeftChildIndex(int parentIndex)
        {
            return parentIndex * 2 + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetRightChildIndex(int parentIndex)
        {
            return parentIndex * 2 + 2;
        }
    }
}