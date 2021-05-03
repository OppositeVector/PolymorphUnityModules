using System;
using Polymorph.Primitives;

namespace Polymorph.Shapes {

    public static class Bezier {

        public static double GetPoint(double p1, double p2, double c, double t) {
            t = PMath.Clamp01(t);
            var t2 = t * t;
            var rt = 1 - t;
            var rt2 = rt * rt;
            return (rt2 * p1) + (2 * rt * t * c) + (t2 * p2);
        }

        public static double GetPoint(double p1, double p2, double c1, double c2, double t) {
            t = PMath.Clamp01(t);
            var t2 = t * t;
            var t3 = t2 * t;
            var rt = 1 - t;
            var rt2 = rt * rt;
            var rt3 = rt2 * rt;
            return (rt3 * p1) + (3 * rt2 * t * c1) + (3 * rt * t2 * c2) + (t3 * p2);
        }

        public static double GetFirstDerivative(double p1, double p2, double c, double t) {
            var rt = 1 - t;
            return (2 * rt * (c - p1)) + (2 * t * (p2 - c));
        }

        public static double GetFirstDerivative(double p1, double p2, double c1, double c2, double t) {
            t = PMath.Clamp01(t);
            var t2 = t * t;
            var rt = 1f - t;
            var rt2 = rt * rt;
            return (3f * rt2 * (c1 - p1)) + (6f * rt * t * (c2 - c1)) + (3f * t2 * (p2 - c2));
        }

        public static double DeCasteljausDerivative(double p1, double p2, double c1, double c2, double t) {
            var dU = t * t * (-3f * (p1 - 3f * (c1 - c2) - p2));
            dU += t * (6f * (p1 - 2f * c1 + c2));
            dU += -3f * (p1 - c1);
            return dU;
        }

        public static Vector3 GetPoint(Vector3 p1, Vector3 p2, Vector3 c, double t) {
            var x = GetPoint(p1.x, p2.x, c.x, t);
            var y = GetPoint(p1.y, p2.y, c.y, t);
            var z = GetPoint(p1.z, p2.z, c.z, t);
            return new Vector3(x, y, z);
        }
        public static Vector3 GetPoint(Vector3 p1, Vector3 p2, Vector3 c1, Vector3 c2, double t) {
            var x = GetPoint(p1.x, p2.x, c1.x, c2.x, t);
            var y = GetPoint(p1.y, p2.y, c1.y, c2.y, t);
            var z = GetPoint(p1.z, p2.z, c1.z, c2.z, t);
            return new Vector3(x, y, z);
        }

        public static Vector3 GetFirstDerivative(Vector3 p1, Vector3 p2, Vector3 c, double t) {
            var x = GetFirstDerivative(p1.x, p2.x, c.x, t);
            var y = GetFirstDerivative(p1.y, p2.y, c.y, t);
            var z = GetFirstDerivative(p1.z, p2.z, c.z, t);
            return new Vector3(x, y, z);
        }
        public static Vector3 GetFirstDerivative(Vector3 p1, Vector3 p2, Vector3 c1, Vector3 c2, double t) {
            var x = GetFirstDerivative(p1.x, p2.x, c1.x, c2.x, t);
            var y = GetFirstDerivative(p1.y, p2.y, c1.y, c2.y, t);
            var z = GetFirstDerivative(p1.z, p2.z, c1.z, c2.z, t);
            return new Vector3(x, y, z);
        }
        public static double GetArcLength(Vector3 p1, Vector3 p2, Vector3 c, double t) {
            var d = GetFirstDerivative(p1, p2, c, t);
            return d.magnitude;
        }
        public static double GetArcLength(Vector3 p1, Vector3 p2, Vector3 c1, Vector3 c2, double t) {
            var d = GetFirstDerivative(p1, p2, c1, c2, t);
            return d.magnitude;
        }

        public static double GetLengthSimpsons(Vector3 p1, Vector3 p2, Vector3 c, double tStart, double tEnd) {
            int n = 20;
            var delta = (tEnd - tStart) / n;
            var endPoints = GetArcLength(p1, p2, c, tStart) + GetArcLength(p1, p2, c, tEnd);
            var x4 = 0d;
            for(int i = 1; i < n; i += 2) {
                var t = tStart + delta * i;
                x4 += GetArcLength(p1, p2, c, t);
            }
            var x2 = 0d;
            for(int i = 2; i < n; i += 2) {
                var t = tStart + delta * i;
                x2 += GetArcLength(p1, p2, c, t);
            }
            var length = (delta / 3f) * (endPoints + 4f * x4 + 2f * x2);
            return length;
        }

        public static double GetLengthSimpsons(Vector3 p1, Vector3 p2, Vector3 c1, Vector3 c2, double tStart, double tEnd) {
            int n = 20;
            var delta = (tEnd - tStart) / n;
            var endPoints = GetArcLength(p1, p2, c1, c2, tStart) + GetArcLength(p1, p2, c1, c2, tEnd);
            var x4 = 0d;
            for(int i = 1; i < n; i += 2) {
                var t = tStart + delta * i;
                x4 += GetArcLength(p1, p2, c1, c2, t);
            }
            var x2 = 0d;
            for(int i = 2; i < n; i += 2) {
                var t = tStart + delta * i;
                x2 += GetArcLength(p1, p2, c1, c2, t);
            }
            var length = (delta / 3f) * (endPoints + 4f * x4 + 2f * x2);
            return length;
        }


        public static double FindTValue(Vector3 p1, Vector3 p2, Vector3 c, double d, double totalLength) {
            //Need a start value to make the method start
            //Should obviously be between 0 and 1
            //We can say that a good starting point is the percentage of distance traveled
            //If this start value is not working you can use the Bisection Method to find a start value
            //https://en.wikipedia.org/wiki/Bisection_method
            var t = d / totalLength;
            //Need an error so we know when to stop the iteration
            var error = 0.001d;
            //We also need to avoid infinite loops
            int iterations = 0;

            while(true) {
                //Newton's method
                var tNext = t - ((GetLengthSimpsons(p1, p2, c, 0f, t) - d) / GetArcLength(p1, p2, c, t));

                //Have we reached the desired accuracy?
                if(Math.Abs(tNext - t) < error) {
                    break;
                }

                t = tNext;
                iterations += 1;

                if(iterations > 1000) {
                    break;
                }
            }

            return t;
        }

        public static double FindTValue(Vector3 p1, Vector3 p2, Vector3 c1, Vector3 c2, double d, double totalLength) {
            //Need a start value to make the method start
            //Should obviously be between 0 and 1
            //We can say that a good starting point is the percentage of distance traveled
            //If this start value is not working you can use the Bisection Method to find a start value
            //https://en.wikipedia.org/wiki/Bisection_method
            if(d == 0) { return 0; }
            if(d == totalLength) { return 1; }
            var t = d / totalLength;
            //Need an error so we know when to stop the iteration
            var error = 0.001d;
            //We also need to avoid infinite loops
            int iterations = 0;

            while(true) {
                //Newton's method
                var tNext = t - ((GetLengthSimpsons(p1, p2, c1, c2, 0f, t) - d) / GetArcLength(p1, p2, c1, c2, t));
                //Have we reached the desired accuracy?
                if(Math.Abs(tNext - t) < error) {
                    break;
                }

                t = tNext;
                iterations += 1;

                if(iterations > 1000) {
                    break;
                }
            }

            return t;
        }
    }
}
