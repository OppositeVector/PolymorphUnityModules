using UnityEngine;
using System.Collections.Generic;

namespace Polymorph.Unity.Threading {
    internal class ThreadUpdateEndSentinel : MonoBehaviour {
        private void Awake() {
            DontDestroyOnLoad(gameObject);
        }

        private void Update() {
            var threadsToRemove = new List<ThreadRunner>();
            var threadEnumerator = ThreadHandler.EnumerateUpdateExclusiveThreads();
            while(threadEnumerator.MoveNext()) {
                var thread = threadEnumerator.Current;
                if(thread.Update()) {
                    threadsToRemove.Add(thread);
                }
            }
            ThreadHandler.RemoveUpdateExclusiveThreads(threadsToRemove);
        }
    }
}
