using System;
using UnityEngine;

namespace Polymorph.Unity.Threading {

    public static class ThreadSyncronizer {

        public static IThread UpdateExlusiveThread(Action update) {
            var retVal = new ThreadRunner(update);
            ThreadHandler.AddUpdateExclusiveThread(retVal);
            return retVal;
        }

        [RuntimeInitializeOnLoadMethod()]
        private static void CreateThreadSentinels() {

            var obj = new GameObject("Thread Sentinels");
#if UNITY_EDITOR
            obj.AddComponent<ThreadHandler.ThreadDebugger>();
#endif
            obj.AddComponent<ThreadUpdateStartSentinel>();
            obj.AddComponent<ThreadUpdateEndSentinel>();
        }
    }
}
