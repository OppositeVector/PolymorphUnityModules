using System.Collections;
using UnityEngine;
using System;
using Polymorph.Unity.Core;

namespace Polymorph.Unity.AnimatedUI {

    /// <summary>
    /// An animation that pops in and out of the single point
    /// </summary>
    [AddComponentMenu("Polymorph/Animated UI/Scaling UI")]
    public class ScalingUI : AnimatedUIBehaviour {

        /// <summary>
        /// <see cref="AnimatedUIBehaviour.In(float, AnimationCurve, Action)"/>
        /// </summary>
        public override void In(float time, AnimationCurve curve, Action callback = null) {
            base.In(time, curve, callback);
            StartCoroutine(ChangeScale(time, 1, curve)).Then(callback);
        }

        /// <summary>
        /// <see cref="AnimatedUIBehaviour.Out(float, AnimationCurve, Action)"/>
        /// </summary>
        public override void Out(float time, AnimationCurve curve, Action callback = null) {
            base.Out(time, curve, callback);
            StartCoroutine(ChangeScale(time, 0, curve)).Then(callback);
        }

        /// <summary>
        /// Change scale to (1, 1, 1)
        /// </summary>
        /// <param name="time">The time that the animation should take</param>
        /// <param name="callback">Callback for when the animation finishes</param>
        public void Appear(float time, Action callback = null) {
            StartCoroutine(ChangeScale(time, 1, inCurve.curve)).Then(callback);
        }

        /// <summary>
        /// Change scale to (0, 0, 0)
        /// </summary>
        /// <param name="time">The time that the animation should take</param>
        /// <param name="callback">Callback for when the animation finishes</param>
        public void Disappear(float time, Action callback = null) {
            if(singleCurve) {
                StartCoroutine(ChangeScale(time, 0, inCurve.curve)).Then(callback);
            } else {
                StartCoroutine(ChangeScale(time, 0, outCurve.curve)).Then(callback);
            }
        }

        IEnumerator ChangeScale(float time, float scale, AnimationCurve curve) {

            yield return new AquireDriveThroughSemaphore();
            yield return null;
            yield return null;

            var startScale = transform.localScale.x;
            var startTime = time;

            while(time > 0) {
                float newScale = Mathf.LerpUnclamped(startScale, scale, GetCurveValue(startTime - time, startTime, curve));
                transform.localScale = new Vector3(newScale, newScale, newScale);
                time -= Time.deltaTime;
                yield return null;
            }

            transform.localScale = new Vector3(scale, scale, scale);
        }

        /// <summary>
        /// Unity Reset entry point
        /// </summary>
        public override void Reset() {
            SetDefaultCurve(BuiltIn.Lerp);
        }
    }
}
