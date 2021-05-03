using UnityEngine;
using System.Collections;
using System;
using Polymorph.Unity.Core;

namespace Polymorph.Unity.AnimatedUI {

    /// <summary>
    /// Operates as an interface between AnimatedUIBehaviour and a Unity Animator Controller 
    /// </summary>
    [AddComponentMenu("Polymorph/Animated UI/Animator UI")]
    public class AnimatorUI : AnimatedUIBehaviour {

        /// <summary>
        /// The Animator that will receive the events
        /// </summary>
        public Animator subject;
        FinishState[] states;
        bool finished = false;

        /// <summary>
        /// Unity Awake entry point
        /// </summary>
        protected virtual void Awake() {
            states = subject.GetBehaviours<FinishState>();
            foreach(var state in states) {
                state.onEnter += FinishReached;
            }
        }

        /// <summary>
        /// Unity OnDestroy entry point
        /// </summary>
        protected virtual void OnDestroy() {
            foreach(var state in states) {
                state.onEnter -= FinishReached;
            }
        }

        /// <summary>
        /// <see cref="AnimatedUIBehaviour.In(float, AnimationCurve, Action)"/>
        /// </summary>
        public override void In(float time, AnimationCurve curve, System.Action callback = null) {
            base.In(time, curve, callback);
            StartCoroutine(SetAndWait("In", callback));
        }

        /// <summary>
        /// <see cref="AnimatedUIBehaviour.Out(float, AnimationCurve, Action)"/>
        /// </summary>
        public override void Out(float time, AnimationCurve curve, System.Action callback = null) {
            base.Out(time, curve, callback);
            StartCoroutine(SetAndWait("Out", callback));
        }

        IEnumerator SetAndWait(string triggerName, Action callback) {
            yield return new AquireDriveThroughSemaphore();

            finished = false;
            subject.SetTrigger(triggerName);
            while(!finished) {
                yield return null;
            }
            callback();
            finished = false;
        }

        void FinishReached() {
            finished = true;
        }
    }
}
