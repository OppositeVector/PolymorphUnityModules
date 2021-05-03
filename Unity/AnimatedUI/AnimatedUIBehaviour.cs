using System;
using UnityEngine;
using Polymorph.Unity.Core;

namespace Polymorph.Unity.AnimatedUI {

    /// <summary>
    /// Base class for all animated ui components
    /// </summary>
    [AddComponentMenu("")]
    public class AnimatedUIBehaviour : AdvancedBehaviour {

        //        /// <summary>
        //        /// <para>Used internaly to create the static standard curves,</para>
        //        /// <para>used by Polymorph.Unity.AnimatedUI.Editor.AnimatedUIBehaviourDrawer</para>
        //        /// </summary>
        //        /// <param name="type">Which curve to generate</param>
        //        /// <returns>AnimationCurve representation of the curve requested</returns>
        //        public static AnimationCurve GetStandardCurve(BuiltIn type) {
        //            AnimationCurve retVal = new AnimationCurve();
        //            float points = 0;
        //            switch(type) {
        //                case BuiltIn.Linear:
        //                    retVal.AddKey(new Keyframe(0f, 0f, 0f, 1f));
        //                    retVal.AddKey(new Keyframe(1f, 1f, 1f, 0f));
        //                    break;
        //                case BuiltIn.Lerp:
        //                    points = 15;
        //                    retVal.AddKey(new Keyframe(0f, 0f, 0f, points * 0.6f));
        //                    for(int i = 1; i < points; ++i) {
        //                        var timeStamp = i / points;
        //                        timeStamp *= timeStamp;
        //                        var b = 1 - timeStamp;
        //                        var acos = Mathf.Acos(b);
        //                        var s = Mathf.Sin(acos);
        //                        var m = Mathf.Sin((Mathf.PI / 2) - acos) / s;
        //                        retVal.AddKey(new Keyframe(timeStamp, s, m, m));
        //                    }
        //                    retVal.AddKey(new Keyframe(1f, 1f, 0f, 0f));
        //                    break;
        //                case BuiltIn.Overshoot:
        //                    retVal.AddKey(new Keyframe(0f, 0f, 0f, 2f));
        //                    retVal.AddKey(new Keyframe(0.5f, 1.2f, 3f, -1f));
        //                    retVal.AddKey(new Keyframe(1f, 1f, 0f, 0f));
        //                    break;
        //                case BuiltIn.Undershoot:
        //                    retVal.AddKey(new Keyframe(0f, 0f, 0f, 0.5f));
        //                    retVal.AddKey(new Keyframe(0.6f, 0.8f, 0f, 1.5f));
        //                    retVal.AddKey(new Keyframe(1f, 1f, 0f, 0f));
        //                    break;

        //                case BuiltIn.Sinusoidal:
        //                    points = 4;
        //                    for(int i = 0; i < points + 1; ++i) {
        //                        var timeStamp = i / points;
        //                        var b = timeStamp * 2 * Mathf.PI;
        //                        var s = Mathf.Sin(b);
        //                        var m = Mathf.Cos(b) * Mathf.PI * 2;
        //                        retVal.AddKey(new Keyframe(timeStamp, s, m, m));
        //                    }
        //                    break;
        //                case BuiltIn.Jagged:
        //                    retVal.AddKey(new Keyframe(0f, 0f, 0f, 4f));
        //                    retVal.AddKey(new Keyframe(0.25f, 1f, 4f, -4f));
        //                    retVal.AddKey(new Keyframe(0.75f, -1f, -4f, 4f));
        //                    retVal.AddKey(new Keyframe(1, 0, 4, 0));
        //                    break;
        //            }
        //            return retVal;
        //        }

        //        /// <summary>
        //        /// The Curve that will be used if no curve was supplied during the animation call
        //        /// </summary>
        //        [HideInInspector]
        //        [SerializeField]
        //        protected AnimationCurve defaultCurve;
        //        [HideInInspector]
        //        [SerializeField]
        //#pragma warning disable 0414
        //        CurveType curveType = CurveType.BuiltIn;
        //#pragma warning restore 0414
        //        [HideInInspector]
        //        [SerializeField]
        //        BuiltIn standardCurve = BuiltIn.Linear;

        [HideInInspector]
        [SerializeField]
        protected PolyCurve inCurve = new PolyCurve();
        [HideInInspector]
        [SerializeField]
        protected bool singleCurve = true;
        [HideInInspector]
        [SerializeField]
        protected PolyCurve outCurve = new PolyCurve();
        /// <summary>
        /// The time that will be used if no time was supplied during the animation call
        /// </summary>
        //[HideInInspector]
        //[SerializeField]
        //protected float defaultTime = 0.7f;
        //[HideInInspector]
        //[SerializeField]
        //bool repeat = false;
        //[HideInInspector]
        //[SerializeField]
        //int repeatCount = 1;

        public AnimatedUIBehaviour() {
            SetDefaultCurve(BuiltIn.Linear);
        }

        /// <summary>
        /// Call the animtion to bring the object in to view
        /// </summary>
        /// <param name="time">The amount of time the animation should take</param>
        /// <param name="curve">The curve used to customize the animation</param>
        /// <param name="callback">Callback to when the animation has finished (Not always at the end of the time)</param>
        public virtual void In(float time, AnimationCurve curve, Action callback = null) { }

        /// <summary>
        /// Shorthand for <see cref="In(float, AnimationCurve, Action)"/> without the curve
        /// </summary>
        public void In(float time, Action callback = null) {
            In(time, inCurve.curve, callback);
        }

        /// <summary>
        /// Shorthand for <see cref="In(float, AnimationCurve, Action)"/> without the curve and time
        /// </summary>
        public void In(Action callback) {
            In(inCurve.time, inCurve.curve, callback);
        }

        /// <summary>
        /// Action like shorthand for <see cref="In(float, AnimationCurve, Action)"/>
        /// </summary>
        [ContextMenu("In")]
        public void In() {
            In(inCurve.time, inCurve.curve, null);
        }

        /// <summary>
        /// Call the animtion to bring the object out of view
        /// </summary>
        /// <param name="time">The amount of time the animation should take</param>
        /// <param name="curve">The curve used to customize the animation</param>
        /// <param name="callback">Callback to when the animation has finished (Not always at the end of the time)</param>
        public virtual void Out(float time, AnimationCurve curve, Action callback = null) { }

        /// <summary>
        /// Shorthand for <see cref="Out(float, AnimationCurve, Action)"/> without the curve
        /// </summary>
        public void Out(float time, Action callback = null) {
            if(singleCurve) {
                Out(time, inCurve.curve, callback);
            } else {
                Out(time, outCurve.curve, callback);
            }
        }

        /// <summary>
        /// Shorthand for <see cref="Out(float, AnimationCurve, Action)"/> without the curve and time
        /// </summary>
        public void Out(Action callback) {
            if(singleCurve) {
                Out(inCurve.time, inCurve.curve, callback);
            } else {
                Out(outCurve.time, outCurve.curve, callback);
            }
        }

        /// <summary>
        /// Action like shorthand for <see cref="Out(float, AnimationCurve, Action)"/>
        /// </summary>
        [ContextMenu("Out")]
        public void Out() {
            if(singleCurve) {
                Out(inCurve.time, inCurve.curve, null);
            } else {
                Out(outCurve.time, outCurve.curve, null);
            }
        }

        /// <summary>
        /// Get the value on the curve, taking in to account repeat
        /// </summary>
        /// <param name="time">The current time to evaluate</param>
        /// <param name="totalTime">The total time of the animation</param>
        /// <param name="curve">The curve which should be evaluated</param>
        /// <returns></returns>
        protected float GetCurveValue(float time, float totalTime, AnimationCurve curve) {
            float normalizedTime = totalTime;
            return curve.Evaluate((time % normalizedTime) / normalizedTime);
        }

        /// <summary>
        /// Used internally to generate the defalut AnimationCurve based on <see cref="GetStandardCurve(BuiltIn)"/>
        /// </summary>
        /// <param name="curve">The curve type to generate</param>
        protected void SetDefaultCurve(BuiltIn curve) {
            inCurve.SetBuiltIn(curve);
            outCurve.SetBuiltIn(curve);
        }

        /// <summary>
        /// Used internally by unity when the object is created in the editor
        /// </summary>
        public virtual void Reset() {
            SetDefaultCurve(BuiltIn.Linear);
        }
    }
}