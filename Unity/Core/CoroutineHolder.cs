using UnityEngine;
using System;
using System.Threading;

namespace Polymorph.Unity.Core {

    /// <summary>
    /// All coroutines created by AdvancedBehavoiurs will run on a seperate object
    /// with this component attached to it
    /// </summary>
    [AddComponentMenu("")]
    public class CoroutineHolder : MonoBehaviour {

        static bool created = false;
        static CoroutineHolder _holder;
        public static CoroutineHolder holder {
            get {
                if(_holder == null) {
                    _holder = GameObject.FindObjectOfType<CoroutineHolder>();
                    if(!created && (_holder == null)) {
                        created = true;
                        var obj = new GameObject("Coroutine Holder");
                        _holder = obj.AddComponent<CoroutineHolder>();
                        DontDestroyOnLoad(obj);
                    }
                }
                return _holder;
            }
        }

        public static void ResetState() {
            if(_holder != null) {
                Destroy(_holder.gameObject);
                created = false;
            }
        }

        Action onNextFrame;
        object key = new object();

        void Update() {
            if(Monitor.TryEnter(key, 1)) {
                if(onNextFrame != null) {
                    try {
                        onNextFrame();
                    } catch(Exception e) {
                        Debug.LogException(e);
                    }
                    onNextFrame = null;
                }
                Monitor.Exit(key);
            }
        }

        public void RunOnNextFrame(Action action) {
            lock(key) {
                onNextFrame += action;
            }
        }
    }
}
