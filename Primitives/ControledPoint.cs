using System;
using System.Collections.Generic;

namespace Polymorph.Primitives {

    public class ControledPoint {

        /// <summary>
        /// Value
        /// </summary>
        public Vector3 v;
        /// <summary>
        /// Preceding Control
        /// </summary>
        public Vector3 pc;
        /// <summary>
        /// Succeeding Control
        /// </summary>
        public Vector3 sc;
        public ControledPoint() { }
        public ControledPoint(Vector3 value, Vector3 pControl, Vector3 sControl) {
            v = value;
            pc = pControl;
            sc = sControl;
        }

        /// <summary>
        /// Changes the controls to fit the preceding point and succeeding point, which fits on as spline
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="mult"></param>
        public void ControlsBySpline(Vector3 p0, Vector3 p1, double mult) {
            var delta = (p1 - p0) * mult;
            sc = v + delta;
            sc = v - delta;
        }
    }
}
