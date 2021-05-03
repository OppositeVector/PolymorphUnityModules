using System.Collections.Generic;
using Polymorph.Primitives;

namespace Polymorph.Shapes {

    public class TSpline {
        
        List<Vector3> points;
        BezierTCurve[] curves;
        SortedList<double, BezierTCurve> crossRef;
        double mult;
        public int count { get { return curves.Length; } }
        public BezierTCurve this[int i] { get { return curves[i]; } }
        double _length;
        public double length {
            get { return _length; }
            private set { _length = value; }
        }

        public System.Action onSplineRecalculated;

        public TSpline(Vector3[] points, double mult) {
            curves = new BezierTCurve[points.Length - 1];
            for(int i = 0; i < curves.Length; ++i) {
                curves[i] = new BezierTCurve(points[i], points[i + 1]);
            }
            this.mult = mult;
        }

        void RecalculateBeziers() {
            crossRef = new SortedList<double, BezierTCurve>();
            if(points.Count < 2) {
                curves = new BezierTCurve[0];
                return;
            } else if(points.Count == 2) {
                curves = new BezierTCurve[1];
                curves[0] = new BezierTCurve(points[0], points[1]);
                crossRef.Add(1, curves[0]);
                return;
            }
            curves = new BezierTCurve[points.Count - 1];
            Vector3 s = points[0];
            Vector3 e = points[1];
            Vector3 c1 = s + ((e - s) * mult);
            Vector3 delta;
            Vector3 c2;
            length = 0;
            for(int i = 0; i < curves.Length; ++i) {
                e = points[i + 1];
                if((i + 2) < points.Count) {
                    delta = (points[i + 2] - s);
                } else {
                    delta = (e - s);
                }
                delta *= mult;
                c2 = e - delta;

                curves[i] = new BezierTCurve(s, e, c1, c2);

                s = e;
                c1 = s + delta;
                length += curves[i].length;
            }
            double current = 0;
            for(int i = 0; i < curves.Length; ++i) {
                var curve = curves[i];
                crossRef.Add(current, curve);
                current += curve.length / length;
            }
            if(onSplineRecalculated != null) {
                onSplineRecalculated();
            }
        }

        void FindCurve(double t, out BezierTCurve curve, out double start, out double end) {
            int i = 1;
            start = crossRef.Keys[crossRef.Keys.Count - 1];
            end = length;
            curve = crossRef[start];
            for(; i < crossRef.Keys.Count; ++i) {
                if(crossRef.Keys[i] >= t) {
                    start = crossRef.Keys[i - 1];
                    end = crossRef.Keys[i];
                    curve = crossRef[crossRef.Keys[i - 1]];
                    break;
                }
            }
        }

        public Vector3 GetPoint(double t) {
            t = PMath.Clamp01(t);
            double start;
            double end;
            BezierTCurve curve;
            FindCurve(t, out curve, out start, out end);
            t = PMath.Rebase(start, end, t, 0, 1);
            return curve[t];
        }

        public Vector3 GetVelocity(double t) {
            t = PMath.Clamp01(t);
            double start;
            double end;
            BezierTCurve curve;
            FindCurve(t, out curve, out start, out end);
            t = PMath.Rebase(start, end, t, 0, 1);
            return curve.GetVelocity(t);
        }

        public void Reverse() {
            points.Reverse();
        }
    }
}
