using System.Threading;
using System.Collections;

namespace Polymorph.Threading
{
    public class DoubleBuffer<T> where T : ICollection, new()
    {
        T _buffer1 = new T();
        T _buffer2 = new T();
        bool _forward = true;

        public int Count { get => GetFront().Count + GetRear().Count; }

        public void Flip() => _forward = !_forward;

        public void LockFront()
        {
            if(_forward)
                Monitor.Enter(_buffer1);
            else
                Monitor.Enter(_buffer2);
        }

        public T GetFront()
        {
            if(_forward)
                return _buffer1;
            else
                return _buffer2;
        }

        public void UnlockFront()
        {
            if(_forward)
                Monitor.Exit(_buffer1);
            else
                Monitor.Exit(_buffer2);
        }

        public void LockRear()
        {
            if(_forward)
                Monitor.Enter(_buffer2);
            else
                Monitor.Enter(_buffer1);
        }

        public T GetRear()
        {
            if(_forward)
                return _buffer2;
            else
                return _buffer1;
        }

        public void UnlockRear()
        {
            if(_forward)
                Monitor.Exit(_buffer2);
            else
                Monitor.Exit(_buffer1);
        }
    }
}