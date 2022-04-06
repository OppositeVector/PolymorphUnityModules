using System;
using System.Threading;

namespace Polymorph.Unity.Threading {

    [System.Serializable]
    internal class ThreadRunner : IThread {

        static int total = 0;

        private int index = ++total;

        private Action _threadedUpdate;
        private AutoResetEvent _updateSignal;
        private Thread _thread; // Should i keep thread reference?
        public bool Killed { get; private set; } = false;

        public Exception Exception;

        public ThreadRunner(Action update) {
            _threadedUpdate = update;
            _updateSignal = new AutoResetEvent(true);
            _thread = new Thread(ThreadLoop);
            _thread.Start();
        }

        public bool Update() {
            if(Killed) {
                return true;
            }
            _updateSignal.Set();
            return false;
        }

        public void Kill() {
            Killed = true;
        }

        private void ThreadLoop() {
            _updateSignal.WaitOne();
            while(!Killed) {
                lock(this) {
                    try {
                        _threadedUpdate();
                    } catch (Exception e) {
                        Exception = e;
                    }
                }
                _updateSignal.WaitOne();
            }
        }

        public override string ToString() {
            return "ThreadRunner " + index;
        }
    }
}
