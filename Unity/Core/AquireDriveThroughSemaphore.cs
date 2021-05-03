namespace Polymorph.Unity.Core {

    /// <summary>
    /// Aquire a semaphore that kills the currently running coroutine, and let this coroutine through
    /// </summary>
    public class AquireDriveThroughSemaphore : BaseAquire {
        public AquireDriveThroughSemaphore() { }
        public AquireDriveThroughSemaphore(object h) : base(h) { }
        public override AquireState AttemptAquire() {
            if(sem.index > index) {
                return AquireState.Kill;
            }
            if(sem.running) {
                sem.kill = true;
                return AquireState.Waiting;
            }
            return AquireState.Aquired;
        }
    }
}
