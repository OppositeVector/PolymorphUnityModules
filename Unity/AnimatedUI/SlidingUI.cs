using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Polymorph.Unity.Core;

namespace Polymorph.Unity.AnimatedUI {

    /// <summary>
    /// A dynamic animation component that moves this element in and out of its parent
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("Polymorph/Animated UI/Sliding UI")]
    public class SlidingUI : AnimatedUIBehaviour {

        RectTransform _rTrans;
        RectTransform rTrans {
            get {
                if(_rTrans == null) {
                    _rTrans = transform as RectTransform;
                }
                return _rTrans;
            }
        }
        /// <summary>
        /// Direction where this element will move to when its hidden
        /// </summary>
        public Direction hideDirection = Direction.Bottom;
        [HideInInspector]
        [SerializeField]
        Vector2 origPos;

        ICoroutine currentlyRunning = null;

        /// <summary>
        /// Get or set the position based on the origin point
        /// </summary>
        public Vector2 distanceFromOrigin {
            get { return origPos - rTrans.anchoredPosition; }
            set { rTrans.anchoredPosition = origPos - value; }
        }

        /// <summary>
        /// Used by AnimatedUIBehaviour to apply the "Get in" Animation
        /// </summary>
        /// <param name="time">Time the animation should take</param>
        /// <param name="curve">Curve to be used when applying the animation</param>
        /// <param name="callback">Callback when the animation finishes</param>
        public override void In(float time, AnimationCurve curve, Action callback = null) {

            base.In(time, curve, callback);

            var hidePos = rTrans.CalculateOffparentPoint(origPos, hideDirection);
            var naturalDistance = (hidePos - origPos).magnitude;
            var destination = origPos;
            var absoluteTime = time * ((destination - rTrans.anchoredPosition).magnitude / naturalDistance);

            RunCoroutine(rTrans.anchoredPosition, destination, absoluteTime, curve, callback);
        }

        /// <summary>
        /// Used by AnimatedUIBehaviour to apply the "Get out" Animation
        /// </summary>
        /// <param name="time">Time the animation should take</param>
        /// <param name="curve">Curve to be used when applying the animation</param>
        /// <param name="callback">Callback when the animation finishes</param>
        public override void Out(float time, AnimationCurve curve, Action callback = null) {

            base.Out(time, curve, callback);

            var destination = rTrans.CalculateOffparentPoint(origPos, hideDirection);
            var naturalDistance = (destination - origPos).magnitude;
            var absoluteTime = time * ((destination - rTrans.anchoredPosition).magnitude / naturalDistance);

            RunCoroutine(rTrans.anchoredPosition, destination, absoluteTime, curve, callback);
        }

        /// <summary>
        /// <para>Move element to the original position defined in the element, time will be calculated</para>
        /// <para>using the default time set on the element</para>
        /// </summary>
        /// <param name="callback">A callback to when the move has finished</param>
        public void MoveToOrigin(Action callback = null) {
            MoveTo(origPos, callback);
        }

        /// <summary>
        /// <para>Move element to the original position defined in the element</para>
        /// </summary>
        /// <param name="time">The time the transition should take</param>
        /// <param name="callback">A callback to when the move has finished</param>
        public void MoveToOrigin(float time, Action callback = null) {
            MoveTo(origPos, time, callback);
        }

        /// <summary>
        /// <para>Move the element to an arbitrary off parent direction, the time will be calculated</para>
        /// <para>using the default time set on the element</para>
        /// </summary>
        /// <param name="direction">The off parent direction to move to</param>
        /// <param name="callback">A callback to when the move has finished</param>
        public void MoveTo(Direction direction, Action callback = null) {

            var hidePos = rTrans.CalculateOffparentPoint(origPos, hideDirection);
            var naturalDistance = (hidePos - origPos).magnitude;
            var destination = rTrans.CalculateOffparentPoint(origPos, direction);
            var absoluteTime = inCurve.time * ((destination - rTrans.anchoredPosition).magnitude / naturalDistance);

            MoveTo(destination, absoluteTime, callback);
        }

        /// <summary>
        /// Move the element to an arbitrary off parent direction
        /// </summary>
        /// <param name="direction">The off parent direction to move to</param>
        /// <param name="time">The time the transition should take</param>
        /// <param name="callback">A callback to when the move has finished</param>
        public void MoveTo(Direction direction, float time, Action callback = null) {
            var destination = rTrans.CalculateOffparentPoint(origPos, direction);
            MoveTo(destination, time, callback);
        }

        /// <summary>
        /// <para>Move the element to an arbitrary anchored position, the time will be calculated</para>
        /// <para>using the default time set on the element</para>
        /// </summary>
        /// <param name="destination">The anchored position to move to</param>
        /// <param name="callback">A callback to when the transition has finished</param>
        public void MoveTo(Vector2 destination, Action callback = null) {

            var hidePos = rTrans.CalculateOffparentPoint(origPos, hideDirection);
            var naturalDistance = (hidePos - origPos).magnitude;
            var absoluteTime = inCurve.time * ((destination - rTrans.anchoredPosition).magnitude / naturalDistance);

            MoveTo(destination, absoluteTime, callback);
        }

        /// <summary>
        /// Move the element to an arbitrary anchored position
        /// </summary>
        /// <param name="destination">The anchored position to move to</param>
        /// <param name="time">The time the transition should take</param>
        /// <param name="callback">A callback to when the transition has finished</param>
        public void MoveTo(Vector2 destination, float time, Action callback = null) {
            RunCoroutine(rTrans.anchoredPosition, destination, time, inCurve.curve, callback);
        }

        /// <summary>
        /// Move without animation to origin point
        /// </summary>
        public void MoveToImmediate() {
            MoveToImmediate(origPos);
        }

        /// <summary>
        /// Move without animation to off parent direction
        /// </summary>
        /// <param name="direction">Off parent direction</param>
        public void MoveToImmediate(Direction direction) {
            MoveToImmediate(rTrans.CalculateOffparentPoint(origPos, direction));
        }

        /// <summary>
        /// Move without animation to an arbitrary anchored position
        /// </summary>
        /// <param name="destination"></param>
        public void MoveToImmediate(Vector2 destination) {
            rTrans.anchoredPosition = destination;
        }

        /// <summary>
        /// Stops the currently running coroutine, if there is any
        /// </summary>
        public void StopMovement() {
            if(currentlyRunning != null) {
                StopCoroutine(currentlyRunning);
            }
        }

        /// <summary>
        /// Set the reference position where this element is supposed to be usually
        /// </summary>
        public void SetPosition() {
            origPos = rTrans.anchoredPosition;
        }

        void RunCoroutine(Vector2 startPos, Vector2 endPos, float time, AnimationCurve curve, Action callback) {
            currentlyRunning = StartCoroutine(SlideCo(startPos, endPos, time, curve));
            currentlyRunning.Then(callback).Then(CorountineEnd);
        }

        void CorountineEnd(ICoroutine coroutine) {
            if(currentlyRunning == coroutine) {
                currentlyRunning = null;
            }
        }

        IEnumerator SlideCo(Vector2 startPos, Vector2 endPos, float time, AnimationCurve curve) {

            yield return new AquireDriveThroughSemaphore();

            yield return null;
            yield return null;

            var startTime = time;

            while(time > 0) {
                rTrans.anchoredPosition = Vector3.LerpUnclamped(startPos, endPos, GetCurveValue(startTime - time, startTime, curve) /*curve.Evaluate(1 - (time / startTime))*/);
                time -= Time.deltaTime;
                yield return null;
            }

            rTrans.anchoredPosition = endPos;

        }

        /// <summary>
        /// Unity Reset entry point
        /// </summary>
        public override void Reset() {
            SetDefaultCurve(BuiltIn.Lerp);
        }
    }
}
