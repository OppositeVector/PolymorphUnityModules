using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Polymorph.Unity.Core {

    [System.Serializable]
    public class CurveSelector {

        public static implicit operator AnimationCurve(CurveSelector selector) {
            return selector.library[selector.index];
        }

        [SerializeField]
        CurveLibrary library;
        [SerializeField]
        int index;
        [SerializeField]
        AnimationCurve curvePreview;

        public void SetGraphOnLibrary(AnimationCurve curve) {
            library[index] = curve;
        }

        public float Evaluate(float time) {
            return library[index].Evaluate(time);
        }
    }
}
