using System.Collections;
using UnityEngine;
using System;
using Polymorph.Unity.Core;

namespace Polymorph.Unity.AnimatedUI {

    /// <summary>
    /// Animation for UI that uses a <see cref="UnityEngine.CanvasGroup"/> to animate a fade in and fade out
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    [AddComponentMenu("Polymorph/Animated UI/Fading UI")]
    public class FadingUI : AnimatedUIBehaviour {

        CanvasGroup _group;
        /// <summary>
        /// The <see cref="UnityEngine.CanvasGroup"/> used by this animation behaviour
        /// </summary>
        public CanvasGroup group {
            get {
                if(_group == null) {
                    _group = GetComponent<CanvasGroup>();
                }
                return _group;
            }
        }

        /// <summary>
        /// <see cref="AnimatedUIBehaviour.In(float, AnimationCurve, Action)"/>
        /// </summary>
        public override void In(float time, AnimationCurve curve, Action callback = null) {
            base.In(time, curve, callback);
            // gameObject.SetActive(true);
            FadeToAlpha(1, time, curve, callback);
        }

        /// <summary>
        /// <see cref="AnimatedUIBehaviour.Out(float, AnimationCurve, Action)"/>
        /// </summary>
        public override void Out(float time, AnimationCurve curve, Action callback = null) {
            base.Out(time, curve, callback);
            FadeToAlpha(0, time, curve, callback);
        }

        /// <summary>
        /// Fades the element to a specific alpha state
        /// </summary>
        /// <param name="alpha">The destination</param>
        /// <param name="time">The time it should take</param>
        /// <param name="curve">The curve to use for extra customization</param>
        /// <param name="callback">A callback for when the animation finishes</param>
        public void FadeToAlpha(float alpha, float time, AnimationCurve curve, Action callback = null) {
            StartCoroutine(FadeCo(alpha, time, curve)).Then(callback);
        }

        ///// <summary>
        ///// Shorthand for <see cref="FadeToAlpha(float, float, AnimationCurve, Action)"/> whiout the curve
        ///// </summary>
        //public void FadeToAlpha(float alpha, float time, Action callback = null) {
        //    StartCoroutine(FadeCo(alpha, time, inCurve.curve)).Then(callback);
        //}

        ///// <summary>
        ///// Shorthand for <see cref="FadeToAlpha(float, float, AnimationCurve, Action)"/> whiout the curve and the time
        ///// </summary>
        //public void FadeToAlpha(float alpha, Action callback = null) {
        //    StartCoroutine(FadeCo(alpha, defaultTime, defaultCurve)).Then(callback);
        //}

        IEnumerator FadeCo(float alpha, float time, AnimationCurve curve) {

            yield return new AquireDriveThroughSemaphore();

            if(!gameObject.activeSelf && (alpha > 0)) {
                gameObject.SetActive(true);
            }

            alpha = Mathf.Clamp01(alpha);
            var startAlpha = group.alpha;
            var startTime = Mathf.Abs(alpha - startAlpha) * time;
            time = startTime;
            while(time > 0) {
                group.alpha = Mathf.Lerp(startAlpha, alpha, GetCurveValue(startTime - time, startTime, curve) /*curve.Evaluate(1 - (time / startTime))*/);
                time -= Time.deltaTime;
                yield return null;
            }

            group.alpha = alpha;
            if(alpha == 0) {
                gameObject.SetActive(false);
            }
        }
    }
}
