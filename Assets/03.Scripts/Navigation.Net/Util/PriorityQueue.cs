using System;
using System.Collections.Generic;

namespace Navigation.Net.Math
{
    public class PriorityQueue<TValue, TKey> where TKey : IComparable<TKey>
    {
        private readonly List<(TValue value, TKey key)> _heap = new();

        public int Count => _heap.Count;
        
        public bool Enqueue(TValue value, TKey key)
        {
            _heap.Add((value, key));
        
            var index = _heap.Count - 1;

            while (index > 0)
            {
                var parent = (index - 1) / 2;
            
                if (_heap[index].key.CompareTo(_heap[parent].key) >= 0)
                    return true;

                (_heap[index], _heap[parent]) = (_heap[parent], _heap[index]);
                index = parent;
            }

            return false;
        }

        public bool Dequeue()
        {
            if (_heap.Count <= 0) return false;
        
            var lastElement = _heap[^1];
            _heap[0] = lastElement;
            _heap.RemoveAt(_heap.Count - 1);

            var index = 0;
            var count = _heap.Count;

            while (true)
            {
                var left = 2 * index + 1;
                var right = 2 * index + 2;
                var current = index;
            
                if (left < count && _heap[left].key.CompareTo(_heap[current].key) < 0)
                {
                    current = left;
                }
                if (right < count && _heap[right].key.CompareTo(_heap[current].key) < 0)
                {
                    current = right;
                }
                if (current == index)
                {
                    return true;
                }

                (_heap[index], _heap[current]) = (_heap[current], _heap[index]);
                index = current;
            }
            return true;
        }

        public TValue Peek()
        {
            return _heap[0].value;
        }

        public bool TryPeek(out TValue value, out TKey key)
        {
            if (_heap.Count <= 0)
            {
                value = default(TValue);
                key =  default(TKey);
                return false;
            }
            value = _heap[0].value;
            key = _heap[0].key;
            return true;
        }
    }
}