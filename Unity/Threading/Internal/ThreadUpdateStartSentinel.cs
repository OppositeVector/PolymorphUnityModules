using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Polymorph.Unity.Threading {
    internal class ThreadUpdateStartSentinel : MonoBehaviour {
        private void Awake() {
            DontDestroyOnLoad(gameObject);
        }

        private void Update() {
            var threadsToRemove = new List<ThreadRunner>();
            var threadEnumerator = ThreadHandler.EnumerateUpdateExclusiveThreads();
            while(threadEnumerator.MoveNext()) {
                var thread = threadEnumerator.Current;
                Monitor.Enter(thread);
                if(thread.Exception != null) {
                    Debug.LogException(thread.Exception);
                    thread.Exception = null;
                }
                if(thread.Killed) {
                    threadsToRemove.Add(thread);
                }
                Monitor.Exit(thread);
            }
            ThreadHandler.RemoveUpdateExclusiveThreads(threadsToRemove);
        }
    }
}