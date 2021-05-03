namespace Polymorph.Unity.Core {

    public abstract class BaseAquire {

        internal object hash = null;
        protected Semaphore sem;
        protected ulong index;
        public BaseAquire() { }
        public BaseAquire(object h) {
            hash = h;
        }
        internal void SetSemaphore(Semaphore s) {
            sem = s;
            index = sem.index;
        }
        public abstract AquireState AttemptAquire();
    }
}
