using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polymorph.Unity.Core {

    [AddComponentMenu("")]
    public class AdvancedBehaviour : MonoBehaviour {

        CoroutineRunner _runner;
        CoroutineRunner runner {
            get {
                if(_runner == null) {
                    _runner = new CoroutineRunner(this);
                }
                return _runner;
            }
        }

        new public ICoroutine StartCoroutine(IEnumerator coroutine) {
            return runner.Start(coroutine);
        }

        public void StopCoroutine(ICoroutine co) {
            runner.Stop(co);
        }

        public void RunOnNextFrame(Action action) {
            runner.RunOnNextFrame(action);
        }
    }
}