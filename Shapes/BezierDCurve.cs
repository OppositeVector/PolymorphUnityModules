using System;
using System.Collections.Generic;
using Polymorph.Primitives;
using Polymorph;

namespace Polymorph.Shapes {

    public class BezierDCurve {

        public class InvalidIndex : Exception {
            public InvalidIndex(int i) : base() { }
            public static string FormMessege(int i) {
                return "There is no index " + i + " on a Bezier curve, only 0, 1, 2 and 3";
            }
        }

        public ControledPoint start;
        public ControledPoint end;

        public Vector3 startPoint { get { return start.v; } }
        public Vector3 endPoint { get { return end.v; } }
        public Vector3 startTangent { get { return start.sc; } }
        public Vector3 endTangent { get { return end.pc; } }

        double _length;
        public double length {
            get { return _length; }
            private set { _length = value; }
        }

        public BezierDCurve() { }
        public BezierDCurve(Vector3 start, Vector3 end, Vector3 control1 = default(Vector3), Vector3 control2 = default(Vector3)) {
            var d = (end - start) / 2;
            this.start = new ControledPoint(start, start - d, control1);
            this.end = new ControledPoint(end, control2, end + d);
            length = GetLength();
        }
        public BezierDCurve(ControledPoint start, ControledPoint end) {
            this.start = start;
            this.end = end;
            length = GetLength();
        }

        public Vector3 this[double d] {
            get {
                var t = GetT(d);
                return Bezier.GetPoint(start.v, end.v, start.sc, end.pc, t); }
        }

        /// <summary>
        /// I Values:
        ///     0: Start Point
        ///     1: Start Tangent
        ///     2: End Tangent
        ///     3: End Point
        /// </summary>
        public Vector3 this[int i] {
            get {
                switch(i) {
                    case 0: return start.v;
                    case 1: return start.sc;
                    case 2: return end.pc;
                    case 3: return end.v;
                    default: throw new InvalidIndex(i);
                }
            }
        }

        public Vector3 GetVelocity(double d) {
            var retVal = Bezier.GetFirstDerivative(start.v, end.v, start.sc, end.pc, GetT(d));   
            return retVal;
        }

        double GetLength() {
            return Bezier.GetLengthSimpsons(start.v, end.v, start.sc, end.pc, 0, 1);
        }

        double GetT(double d) {
            return Bezier.FindTValue(start.v, end.v, start.sc, end.pc, d, length);
        }
    }
}
