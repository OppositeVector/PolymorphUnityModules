using System;
using System.Collections;
using System.Collections.Generic;

namespace Polymorph.DataTools
{
    public class SyncList<T> : IList<T>
    {
        class Synchronizer : IDisposable
        {
            protected class Operation
            {
                public SyncOp Op;
                public int Index;
                public object Item;
            }

            Queue<Operation> _ops = new Queue<Operation>();
            protected Operation CurrentOperation;
            SyncList<T> _parent;

            public Synchronizer(SyncList<T> parent)
            {
                _parent = parent;
                for(int i = 0; i < _parent.list.Count; ++i)
                    _ops.Enqueue(new Operation() { Op = SyncOp.Add, Index = 0, Item = _parent.list[i] });
            }

            public void AddOp(SyncOp op, int index = -1, T item = default(T)) => _ops.Enqueue(new Operation() { Op = op, Index = index, Item = item });

            public void Dispose() => _parent.RemoveSynchronizer(this);

            public bool MoveNext()
            {
                if(_ops.Count > 0)
                {
                    CurrentOperation = _ops.Dequeue();
                    return true;
                } else
                {
                    CurrentOperation = null;
                    return false;
                }
            }
        }

        class Synchronizer<U> : Synchronizer, ISynchronizer<U>
        {
            public Synchronizer(SyncList<T> parent) : base(parent) { }

            public SyncOp CurrentOp => CurrentOperation.Op;
            public U CurrentItem => (U) CurrentOperation.Item;
            public int CurrentIndex => CurrentOperation.Index;
        }

        List<T> list = new List<T>();
        List<Synchronizer> syncs = new List<Synchronizer>();

        public T this[int index] {
            get => list[index];
            set
            {
                list[index] = value;
                for(int i = 0; i < syncs.Count; ++i)
                    syncs[i].AddOp(SyncOp.Exchange, index, value);
            }
        }

        public int Count => list.Count;

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            list.Add(item);
            for(int i = 0; i < syncs.Count; ++i)
                syncs[i].AddOp(SyncOp.Add, -1, item);
        }

        public void Clear()
        {
            list.Clear();
            for(int i = 0; i < syncs.Count; ++i)
                syncs[i].AddOp(SyncOp.Clear);
        }

        public bool Contains(T item) => list.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

        public int IndexOf(T item) => list.IndexOf(item);

        public void Insert(int index, T item)
        {
            list.Insert(index, item);
            for(int i = 0; i < syncs.Count; ++i)
                syncs[i].AddOp(SyncOp.Insert, index, item);
        }

        public bool Remove(T item)
        {
            var index = IndexOf(item);
            if(index > -1)
            {
                RemoveAt(index);
                return true;
            } else
            {
                return false;
            }
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
            for(int i = 0; i < syncs.Count; ++i)
                syncs[i].AddOp(SyncOp.Remove, index);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public ISynchronizer<T> GetSynchronizer()
        {
            var retVal = new Synchronizer<T>(this);
            syncs.Add(retVal);
            return retVal;
        }

        public ISynchronizer<U> GetSynchronizer<U>()
        {
            var retVal = new Synchronizer<U>(this);
            syncs.Add(retVal);
            return retVal;
        }

        void RemoveSynchronizer(Synchronizer s) => syncs.Remove(s);
    }
}
