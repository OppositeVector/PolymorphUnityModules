using System;
using System.Collections.Generic;
using System.Text;

namespace Polymorph.DataTools.Functions {

    public class LinearFunction : BaseFunction {

        double slope, intercept;

        public LinearFunction(double slope, double intercept) {
            this.slope = slope;
            this.intercept = intercept;
        }

        public LinearFunction(double x1, double y1, double x2, double y2) {
            if(x1 == x2) {
                if(y1 < y2) {
                    slope = double.PositiveInfinity;
                    intercept = x1;
                } else if(y1 > y2) {
                    slope = double.NegativeInfinity;
                    intercept = x1;
                } else {
                    slope = 0;
                    intercept = y1;
                }
            } else {
                double xDiff = x2 - x1;
                double yDiff = y2 - y1;
                slope = yDiff / xDiff;
                intercept = y1 - (slope * x1);
            }
        }

        public LinearFunction(LinearFunction other) {
            slope = other.slope;
            intercept = other.intercept;
        }

        public override double Eval(double input) {
            return (input * slope) + intercept;
        }

        public override string ToString() {
            var builder = new StringBuilder();
            builder.Append("f(x) = ");
            if(double.IsInfinity(slope)) {
                builder.Append("∞; at x = ");
                builder.Append(intercept);
            } else {
                if(slope == 0) {
                    builder.Append(intercept);
                } else {
                    builder.Append(slope);
                    if(intercept == 0) {
                        builder.Append("x");
                    } else {
                        builder.Append("x + ");
                        builder.Append(intercept);
                    }
                }
            }
            return builder.ToString();
        }
    }
}
