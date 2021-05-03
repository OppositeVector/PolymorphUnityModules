using Polymorph.DataTools;
using UnityEngine;

namespace Polymorph.Unity
{
    public class SynchedPool<T, U> : Pool<T> where T : Object {

        private ISynchronizer<U> _synchronizer;

        public System.Action<T, U, int> OnNewElement;
        public System.Action<T, U, int> OnElementRemoved;
        public System.Action OnClear;

        public void SetModel(ISynchronizer<U> synchronizer) {
            _synchronizer = synchronizer;
        }
        public void Update() {
            if(_synchronizer != null) {
                T ele;
                while(_synchronizer.MoveNext())
                {
                    switch(_synchronizer.CurrentOp)
                    {
                        case SyncOp.Add:
                            ele = Generate();
                            OnNewElement?.Invoke(ele, _synchronizer.CurrentItem,  Length - 1);
                            break;
                        case SyncOp.Remove:
                            ele = this[_synchronizer.CurrentIndex];
                            Degenerate(this[_synchronizer.CurrentIndex]);
                            OnElementRemoved?.Invoke(ele, _synchronizer.CurrentItem, _synchronizer.CurrentIndex);
                            break;
                        case SyncOp.Insert:
                            ele = Generate(_synchronizer.CurrentIndex);
                            OnNewElement?.Invoke(ele, _synchronizer.CurrentItem, _synchronizer.CurrentIndex);
                            break;
                        case SyncOp.Exchange:
                            OnElementRemoved?.Invoke(this[_synchronizer.CurrentIndex], _synchronizer.CurrentItem, _synchronizer.CurrentIndex);
                            OnNewElement?.Invoke(this[_synchronizer.CurrentIndex], _synchronizer.CurrentItem, _synchronizer.CurrentIndex);
                            break;
                        case SyncOp.Clear:
                            ClearActives();
                            OnClear?.Invoke();
                            break;
                    }
                }
            }
        }
    }
}
