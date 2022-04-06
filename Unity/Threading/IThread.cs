
namespace Polymorph.Unity.Threading {
    // Original name was ISyncedThread, but Unity would not compile the assembly for some reason with that name.
    public interface IThread {
        void Kill();
    }
}
