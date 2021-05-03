namespace Polymorph.Unity.Core {

    /// <summary>
    /// Aquire a semaphore that kills this coroutine, if another one is running
    /// </summary>
    public class AquireFirewallSemaphore : BaseAquire {
        public AquireFirewallSemaphore() { }
        public AquireFirewallSemaphore(object h) : base(h) { }
        public override AquireState AttemptAquire() {
            if(sem.running || (sem.index < index)) {
                return AquireState.Kill;
            }
            return AquireState.Aquired;
        }
    }
}
