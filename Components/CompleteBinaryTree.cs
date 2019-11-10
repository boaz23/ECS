using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    class CompleteBinaryTree<T> : IEnumerable<T>
    {
        public const int ROOT_INDEX = 0;

        public CompleteBinaryTree(int height)
        {
            Height = height;
            Items = new T[(int)Math.Pow(2, height + 1) - 1];
        }

        public T this[int index]
        {
            get
            {
                return Items[index];
            }
            set
            {
                Items[index] = value;
            }
        }

        public int Height { get; }
        public T[] Items { get; }
        public int ItemsCount => Items.Length;

        public T Root => Items[ROOT_INDEX];

        public T Parent(int index)
        {
            return Items[ParentIndex(index)];
        }
        public int ParentIndex(int index)
        {
            return (index / 2) - (1 - (index % 2));
        }

        public T LeftChild(int index)
        {
            return Items[LeftChildIndex(index)];
        }
        public int LeftChildIndex(int index)
        {
            return (2 * index) + 1;
        }

        public T RightChild(int index)
        {
            return Items[RightChildIndex(index)];
        }
        public int RightChildIndex(int index)
        {
            return (2 * index) + 2;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IList<T>)Items).GetEnumerator();
        }

        public Range GetDepthIndexRange(int depth)
        {
            int count = (int)Math.Pow(2, depth);
            int start = count - 1;
            int end = start + count;
            return new Range(start, end, count);
        }
        public IEnumerable<ItemIndexPair> GetDepthEnumerator(int depth)
        {
            Range range = GetDepthIndexRange(depth);
            for (int i = range.Start; i < range.End; i++)
            {
                yield return new ItemIndexPair(i, Items[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal class ItemIndexPair
        {
            public ItemIndexPair(int index, T item)
            {
                Index = index;
                Item = item;
            }

            public int Index { get; }
            public T Item { get; }
        }
    }
}
