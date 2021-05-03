using System.Collections;
using System.Collections.Generic;

namespace Polymorph.Threading
{
    public class SequentialFrontQueue<T> : DoubleBuffer<Queue<T>>
    {
        public delegate void DequeueDelegate(T item);

        public void Enqueue(T item)
        {
            lock(this)
            {
                var rear = GetRear();
                rear.Enqueue(item);
            }
        }

        public void DequeueAll(DequeueDelegate action)
        {
            lock(this)
                Flip();
            var front = GetFront();
            while(front.Count > 0)
                action(front.Dequeue());
        }

        public void Clear()
        {
            lock(this)
            {
                GetFront().Clear();
                GetRear().Clear();
            }
        }
    }
    public class SequentialRearQueue<T> : DoubleBuffer<Queue<T>>
    {
        public T Dequeue()
        {
            var front = GetFront();
            if(front.Count == 0)
            {
                lock(this)
                    Flip();
                front = GetFront();
            }
            return front.Dequeue();
        }

        public void EnqueueAll(IEnumerable<T> items)
        {
            lock(this)
            {
                var rear = GetRear();
                foreach(var item in items)
                    rear.Enqueue(item);
            }
        }

        public void Clear()
        {
            lock(this)
            {
                GetFront().Clear();
                GetRear().Clear();
            }
        }
    }
}