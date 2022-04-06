using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Text;
#endif

namespace Polymorph.Unity.Threading {
    internal static class ThreadHandler {
#if UNITY_EDITOR
        public class ThreadDebugger : MonoBehaviour {
            [CustomEditor(typeof(ThreadDebugger))]
            public class ThreadDebuggerDrawer : Editor {
                public override void OnInspectorGUI() {
                    var builder = new StringBuilder();
                    for(int i = 0; i < _updateExclusiveThreads.Count; ++i) {
                        builder.AppendLine(_updateExclusiveThreads[i].ToString());
                    }
                    EditorGUILayout.TextArea(builder.ToString());
                }
            }
        }
#endif

        private static List<ThreadRunner> _updateExclusiveThreads = new List<ThreadRunner>();
        public static void AddUpdateExclusiveThread(ThreadRunner thread) {
            _updateExclusiveThreads.Add(thread);
        }
        public static IEnumerator<ThreadRunner> EnumerateUpdateExclusiveThreads() {
            return _updateExclusiveThreads.GetEnumerator();
        }
        public static void RemoveUpdateExclusiveThreads(IEnumerable<ThreadRunner> threads) {
            foreach(var thread in threads) {
                _updateExclusiveThreads.Remove(thread);
            }
        }
    }
}
