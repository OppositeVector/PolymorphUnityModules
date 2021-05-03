using System;
using System.Collections;
using UnityEngine;

namespace Polymorph.Unity.Core {

    internal class CoroutineWrapper : ICoroutine, IDisposable {

        public Coroutine unityObject;
        public IEnumerator routine;
        public Semaphore semaphore;
        public object owner;
        public ulong semIndex;

        bool _complete;
        public bool complete {
            get { return _complete; }
            private set { _complete = value; }
        }

        int hash;
        Action doAfter;
        InjectedAction doAfterInjected;
        Action whenFinished;
        InjectedAction whenFinishedInjected;
        Action whenKilled;
        InjectedAction whenKilledInjected;
        InjectedAction afterComplete;

        public CoroutineWrapper(IEnumerator r, object o) {
            routine = r;
            owner = o;
            semaphore = null;
            var type = routine.GetType();
            string fullName = type.FullName;
            hash = fullName.GetHashCode();
            complete = false;
        }

        public void Dispose() {
            unityObject = null;
            routine = null;
            semaphore = null;
            owner = null;
            doAfter = null;
            doAfterInjected = null;
            whenFinished = null;
            whenFinishedInjected = null;
            whenKilled = null;
            whenKilledInjected = null;
            afterComplete = null;
        }

        public override int GetHashCode() {
            return hash;
        }

        public ICoroutine Then(System.Action callback) {
            doAfter += callback;
            return this;
        }

        public ICoroutine Then(InjectedAction callback) {
            doAfterInjected += callback;
            return this;
        }

        public ICoroutine OnFinished(Action callback) {
            whenFinished += callback;
            return this;
        }

        public ICoroutine OnFinished(InjectedAction callback) {
            whenFinishedInjected += callback;
            return this;
        }

        public ICoroutine OnKilled(Action callback) {
            whenKilled += callback;
            return this;
        }

        public ICoroutine OnKilled(InjectedAction callback) {
            whenKilledInjected += callback;
            return this;
        }

        public ICoroutine OnComplete(InjectedAction callback) {
            afterComplete += callback;
            return this;
        }

        void RunSafe(Action action) {
            if(action != null) {
                try {
                    action();
                } catch(Exception e) {
                    Debug.LogException(e);
                }
            }
        }

        void RunSafe(InjectedAction action) {
            if(action != null) {
                try {
                    action(this);
                } catch(Exception e) {
                    Debug.LogException(e);
                }
            }
        }

        internal void Killed() {
            RunSafe(whenKilled);
            RunSafe(whenKilledInjected);
            Complete();
        }

        internal void Finished() {
            RunSafe(whenFinished);
            RunSafe(whenFinishedInjected);
            Complete();
        }

        internal void Complete() {
            RunSafe(doAfter);
            RunSafe(doAfterInjected);
            if(semaphore != null) {
                semaphore.running = false;
            }
            complete = true;
            RunSafe(afterComplete);
        }

        internal bool ShouldStop() {
            if(semaphore != null) {
                if(semaphore.kill || (semIndex < semaphore.index)) { // Check if kill command was send from another coroutine instance
                    semaphore.running = false; // Infrom semaphpre we are ceasing operation
                    Killed();
                    return true;// Kill our selves
                }
                // ++semaphore.frame; // Inform the semaphore that we are still alive
            }
            return false;
        }
    }
}
