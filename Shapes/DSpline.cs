using System.Collections.Generic;
using Polymorph.Primitives;

namespace Polymorph.Shapes {

    public class DSpline {

        List<Vector3> points;
        BezierDCurve[] curves;
        SortedList<double, BezierDCurve> crossRef;
        double mult;
        public int count { get { return curves.Length; } }
        public BezierDCurve this[int i] { get { return curves[i]; } }
        double _length;
        public double length {
            get { return _length; }
            private set { _length = value; }
        }

        public System.Action onSplineRecalculated;

        public DSpline(Vector3[] points, double mult) {
            curves = new BezierDCurve[points.Length - 1];
            for(int i = 0; i < curves.Length; ++i) {
                curves[i] = new BezierDCurve(points[i], points[i + 1]);
            }
            this.mult = mult;
        }

        void RecalculateBeziers() {
            crossRef = new SortedList<double, BezierDCurve>();
            if(points.Count < 2) {
                curves = new BezierDCurve[0];
                return;
            } else if(points.Count == 2) {
                curves = new BezierDCurve[1];
                curves[0] = new BezierDCurve(points[0], points[1]);
                crossRef.Add(1, curves[0]);
                return;
            }
            curves = new BezierDCurve[points.Count - 1];
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

                curves[i] = new BezierDCurve(s, e, c1, c2);

                s = e;
                c1 = s + delta;
                length += curves[i].length;
            }
            double current = 0;
            for(int i = 0; i < curves.Length; ++i) {
                var curve = curves[i];
                crossRef.Add(current, curve);
                current += curve.length;
            }
            if(onSplineRecalculated != null) {
                onSplineRecalculated();
            }
        }

        void FindCurve(double d, out BezierDCurve curve, out double start, out double end) {
            int i = 1;
            start = crossRef.Keys[crossRef.Keys.Count - 1];
            end = length;
            curve = crossRef[start];
            for(; i < crossRef.Keys.Count; ++i) {
                if(crossRef.Keys[i] >= d) {
                    start = crossRef.Keys[i - 1];
                    end = crossRef.Keys[i];
                    curve = crossRef[crossRef.Keys[i - 1]];
                    break;
                }
            }
        }

        public Vector3 GetPoint(double d) {
            d = PMath.Clamp(d, 0, length);
            double start;
            double end;
            BezierDCurve curve;
            FindCurve(d, out curve, out start, out end);
            d = PMath.Rebase(start, end, d, 0, curve.length);
            return curve[d];
        }

        public Vector3 GetVelocity(double d) {
            d = PMath.Clamp(d, 0, length);
            double start;
            double end;
            BezierDCurve curve;
            FindCurve(d, out curve, out start, out end);
            d = PMath.Rebase(start, end, d, 0, curve.length);
            return curve.GetVelocity(d);
        }

        public void Reverse() {
            points.Reverse();
        }
    }
}
