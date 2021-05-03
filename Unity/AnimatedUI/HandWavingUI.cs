using System.Collections;
using UnityEngine;
using Polymorph.Unity.Core;

namespace Polymorph.Unity.AnimatedUI {

    /// <summary>
    /// An animation that rotates the object back and forth, ending in the same spot
    /// </summary>
    [AddComponentMenu("Polymorph/Animated UI/Hand Waving UI")]
    public class HandWavingUI : AnimatedUIBehaviour {

        /// <summary>
        /// The degrees of the deflection from its static point during the animation
        /// </summary>
        public float deflection = 40;

        /// <summary>
        /// <see cref="AnimatedUIBehaviour.In(float, AnimationCurve, System.Action)"/>
        /// </summary>
        public override void In(float time, AnimationCurve curve, System.Action callback = null) {
            base.In(time, curve, callback);
            StartCoroutine(DefletionCo(time, curve)).Then(callback);
        }

        /// <summary>
        /// <see cref="AnimatedUIBehaviour.Out(float, AnimationCurve, System.Action)"/>
        /// </summary>
        public override void Out(float time, AnimationCurve curve, System.Action callback = null) {
            base.Out(time, curve, callback);
            StartCoroutine(DefletionCo(time, curve)).Then(callback);
        }

        IEnumerator DefletionCo(float time, AnimationCurve curve) {

            yield return new AquireDriveThroughSemaphore();

            yield return null;
            yield return null;

            float currentTime = 0;
            var startRotation = transform.localRotation;

            while(currentTime <= time) {

                transform.localRotation = startRotation * Quaternion.Euler(0, 0, deflection * GetCurveValue(currentTime, time, curve));
                currentTime += Time.deltaTime;
                yield return null;
            }

            transform.localRotation = startRotation;

        }

        /// <summary>
        /// Unity Reset entry point
        /// </summary>
        public override void Reset() {
            SetDefaultCurve(BuiltIn.Sinusoidal);
        }

    }
}
