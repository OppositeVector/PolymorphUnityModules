using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Polymorph.Unity.Core;

namespace Polymorph.Unity.AnimatedUI {

    [System.Serializable]
    public class PolyCurve {
        
        public static implicit operator AnimationCurve(PolyCurve curve) {
            return curve.curve;
        } 

        /// <summary>
        /// <para>Used internaly to create the static standard curves,</para>
        /// <para>used by Polymorph.Unity.AnimatedUI.Editor.AnimatedUIBehaviourDrawer</para>
        /// </summary>
        /// <param name="type">Which curve to generate</param>
        /// <returns>AnimationCurve representation of the curve requested</returns>
        public static AnimationCurve GetStandardCurve(BuiltIn type) {
            var retVal = new AnimationCurve();
            float points = 0;
            switch(type) {
                case BuiltIn.Linear:
                    retVal.AddKey(new Keyframe(0f, 0f, 0f, 1f));
                    retVal.AddKey(new Keyframe(1f, 1f, 1f, 0f));
                    break;
                case BuiltIn.Lerp:
                    points = 15;
                    retVal.AddKey(new Keyframe(0f, 0f, 0f, points * 0.6f));
                    for(int i = 1; i < points; ++i) {
                        var timeStamp = i / points;
                        timeStamp *= timeStamp;
                        var b = 1 - timeStamp;
                        var acos = Mathf.Acos(b);
                        var s = Mathf.Sin(acos);
                        var m = Mathf.Sin((Mathf.PI / 2) - acos) / s;
                        retVal.AddKey(new Keyframe(timeStamp, s, m, m));
                    }
                    retVal.AddKey(new Keyframe(1f, 1f, 0f, 0f));
                    break;
                case BuiltIn.Overshoot:
                    retVal.AddKey(new Keyframe(0f, 0f, 0f, 2f));
                    retVal.AddKey(new Keyframe(0.5f, 1.2f, 3f, -1f));
                    retVal.AddKey(new Keyframe(1f, 1f, 0f, 0f));
                    break;
                case BuiltIn.Undershoot:
                    retVal.AddKey(new Keyframe(0f, 0f, 0f, 0.5f));
                    retVal.AddKey(new Keyframe(0.6f, 0.8f, 0f, 1.5f));
                    retVal.AddKey(new Keyframe(1f, 1f, 0f, 0f));
                    break;

                case BuiltIn.Sinusoidal:
                    points = 4;
                    for(int i = 0; i < points + 1; ++i) {
                        var timeStamp = i / points;
                        var b = timeStamp * 2 * Mathf.PI;
                        var s = Mathf.Sin(b);
                        var m = Mathf.Cos(b) * Mathf.PI * 2;
                        retVal.AddKey(new Keyframe(timeStamp, s, m, m));
                    }
                    break;
                case BuiltIn.Jagged:
                    retVal.AddKey(new Keyframe(0f, 0f, 0f, 4f));
                    retVal.AddKey(new Keyframe(0.25f, 1f, 4f, -4f));
                    retVal.AddKey(new Keyframe(0.75f, -1f, -4f, 4f));
                    retVal.AddKey(new Keyframe(1, 0, 4, 0));
                    break;
            }
            return retVal;
        }

        /// <summary>
        /// The Curve that will be used if no curve was supplied during the animation call
        /// </summary>
        [HideInInspector]
        public float time = 0.7f;
        [HideInInspector]
        public AnimationCurve curve;
#pragma warning disable 0414
        [HideInInspector]
        public CurveType curveType = CurveType.BuiltIn;
#pragma warning restore 0414
        [HideInInspector]
        public BuiltIn standardCurve = BuiltIn.Linear;
#pragma warning disable 0169
        [HideInInspector]
        public CurveLibrary library;
        [HideInInspector]
        public int curveIndex;
#pragma warning restore 0169

        public void SetBuiltIn(BuiltIn curve) {
            standardCurve = curve;
            this.curve = GetStandardCurve(curve);
        }
    }
}
