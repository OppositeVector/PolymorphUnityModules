using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Polymorph.Unity.Core;

namespace Polymorph.Unity.AnimatedUI {

    /// <summary>
    /// An animation the chagnes the size based on dynamic UnityEngine.UI properties
    /// </summary>
    [AddComponentMenu("Polymorph/Animated UI/Shrinking UI")]
    public class ShrinkingUI : AnimatedUIBehaviour {

        /// <summary>
        /// Based on what should the element determin the size to take
        /// </summary>
        public enum Constraint { 
            /// <summary>
            /// Use origSize on this element
            /// </summary>
            Static,
            /// <summary>
            /// <para>Use <see cref="UnityEngine.UI.LayoutUtility"/> to determin the min/preffered/flexible</para>
            /// <para>based LayoutProperties unity provides</para>
            /// </summary>
            Dynamic,
            /// <summary>
            /// Use another RectTransform to determin the size
            /// </summary>
            Sourced 
        }

        /// <summary>
        /// <para>The UnityEngine.Layout constraint to be used when guaging the size that the element should take.</para>
        /// <para>Used exclusivle with <see cref="Constraint.Dynamic"/></para>
        /// </summary>
        public enum DynamicConstraint {
            /// <summary>
            /// Do not change
            /// </summary>
            None, 
            /// <summary>
            /// LayoutElement.Min
            /// </summary>
            Min,
            /// <summary>
            /// LayoutElement.Preffered
            /// </summary>
            Preffered,
            /// <summary>
            /// LayoutElement.Flexible
            /// </summary>
            Flexible 
        }

        RectTransform _rTrans;
        RectTransform rTrans {
            get {
                if(_rTrans == null) {
                    _rTrans = transform as RectTransform;
                }
                return _rTrans;
            }
        }

        [HideInInspector]
        [SerializeField]
        Constraint constraint = Constraint.Dynamic;

        [HideInInspector]
        [SerializeField]
        DynamicConstraint verticalCon = DynamicConstraint.None;
        [HideInInspector]
        [SerializeField]
        DynamicConstraint horizontalCon = DynamicConstraint.None;

        [HideInInspector]
        [SerializeField]
        Vector2 origSize;
        [HideInInspector]
        [SerializeField]
        bool vertical = false;
        [HideInInspector]
        [SerializeField]
        bool horizontal = false;
        [HideInInspector]
        [SerializeField]
#pragma warning disable 0649
        RectTransform source;
#pragma warning restore 0649

        /// <summary>
        /// Set the current sizeDelta as the original size
        /// </summary>
        public void SetSize() {
            origSize = rTrans.sizeDelta;
        }

        /// <summary>
        /// <see cref="AnimatedUIBehaviour.In(float, AnimationCurve, Action)"/>
        /// </summary>
        public override void In(float time, AnimationCurve curve, Action callback = null) {
            base.In(time, curve, callback);
            Vector2 size = rTrans.rect.size;
            switch(constraint) {
                case Constraint.Static:
                    if(horizontal) { size.x = origSize.x; }
                    if(vertical) { size.y = origSize.y; }
                    break;
                case Constraint.Dynamic:
                    switch(horizontalCon) {
                        case DynamicConstraint.Min: size.x = LayoutUtility.GetMinWidth(rTrans); break;
                        case DynamicConstraint.Preffered: size.x = LayoutUtility.GetPreferredWidth(rTrans); break;
                        case DynamicConstraint.Flexible: size.x = LayoutUtility.GetFlexibleWidth(rTrans); break;
                    }
                    switch(verticalCon) {
                        case DynamicConstraint.Min: size.y = LayoutUtility.GetMinHeight(rTrans); break;
                        case DynamicConstraint.Preffered: size.y = LayoutUtility.GetPreferredHeight(rTrans); break;
                        case DynamicConstraint.Flexible: size.y = LayoutUtility.GetFlexibleHeight(rTrans); break;
                    }
                    break;
                case Constraint.Sourced:
                    if(horizontal) { size.x = source.rect.width; }
                    if(vertical) { size.y = source.rect.height; }
                    break;
            }
            StartCoroutine(ChangeSize(time, rTrans.GetSizeDelta(size), curve)).Then(callback);
        }

        /// <summary>
        /// <see cref="AnimatedUIBehaviour.Out(float, AnimationCurve, Action)"/>
        /// </summary>
        public override void Out(float time, AnimationCurve curve, Action callback = null) {
            base.Out(time, curve, callback);
            Vector2 size = rTrans.rect.size;
            switch(constraint) {
                case Constraint.Static:
                case Constraint.Sourced:
                    if(horizontal) { size.x = 0; }
                    if(vertical) { size.y = 0; }
                    break;
                case Constraint.Dynamic:
                    if(horizontalCon != DynamicConstraint.None) { size.x = 0; }
                    if(verticalCon != DynamicConstraint.None) { size.y = 0; }
                    break;
            }
            StartCoroutine(ChangeSize(time, rTrans.GetSizeDelta(size), curve)).Then(callback);
        }

        IEnumerator ChangeSize(float time, Vector2 size, AnimationCurve curve) {

            yield return new AquireDriveThroughSemaphore();
            yield return null;
            yield return null;

            var startSize = rTrans.sizeDelta;
            var startTime = time;

            while(time > 0) {
                rTrans.sizeDelta = Vector2.LerpUnclamped(startSize, size, GetCurveValue(startTime - time, startTime, curve));
                time -= Time.deltaTime;
                yield return null;
            }

            rTrans.sizeDelta = size;
        }

        [ContextMenu("Check Size")]
        void InternalSetSize() {
            Vector2 size = rTrans.rect.size;
            switch(constraint) {
                case Constraint.Static:
                    if(horizontal) { size.x = origSize.x; }
                    if(vertical) { size.y = origSize.y; }
                    break;
                case Constraint.Dynamic:
                    switch(horizontalCon) {
                        case DynamicConstraint.Min: size.x = LayoutUtility.GetMinWidth(rTrans); break;
                        case DynamicConstraint.Preffered: size.x = LayoutUtility.GetPreferredWidth(rTrans); break;
                        case DynamicConstraint.Flexible: size.x = LayoutUtility.GetFlexibleWidth(rTrans); break;
                    }
                    switch(verticalCon) {
                        case DynamicConstraint.Min: size.y = LayoutUtility.GetMinHeight(rTrans); break;
                        case DynamicConstraint.Preffered: size.y = LayoutUtility.GetPreferredHeight(rTrans); break;
                        case DynamicConstraint.Flexible: size.y = LayoutUtility.GetFlexibleHeight(rTrans); break;
                    }
                    break;
                case Constraint.Sourced:
                    if(horizontal) { size.x = source.rect.width; }
                    if(vertical) { size.y = source.rect.height; }
                    break;
            }
            Debug.Log(size);
            Debug.Log(rTrans.GetSizeDelta(size));
            rTrans.sizeDelta = rTrans.GetSizeDelta(size);
        }

        /// <summary>
        /// Used by the constructor to setup the curve when this element is being created initially
        /// </summary>
        public override void Reset() {
            SetDefaultCurve(BuiltIn.Lerp);
        }
    }
}
