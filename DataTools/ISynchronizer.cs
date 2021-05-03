using System;

namespace Polymorph.DataTools
{
    public interface ISynchronizer<T> : IDisposable
    {
        SyncOp CurrentOp { get; }
        int CurrentIndex { get; }
        T CurrentItem { get; }
        bool MoveNext();
    }
}
