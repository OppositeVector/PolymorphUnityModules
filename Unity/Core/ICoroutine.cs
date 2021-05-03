namespace Polymorph.Unity.Core {

    public delegate void InjectedAction(ICoroutine self);

    public interface ICoroutine {

        bool complete { get; }
        ICoroutine Then(System.Action callback);
        ICoroutine Then(InjectedAction callback);
        ICoroutine OnFinished(System.Action callback);
        ICoroutine OnFinished(InjectedAction callback);
        ICoroutine OnKilled(System.Action callback);
        ICoroutine OnKilled(InjectedAction callback);
    }
}
