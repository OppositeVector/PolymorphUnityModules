using System;
using System.Collections.Generic;
using System.Text;

namespace Polymorph.DataTools.Functions {

    public class PolynomialFunction : BaseFunction {

        Dictionary<int, double> coefficients;

        public PolynomialFunction() {
            coefficients = new Dictionary<int, double>();
            coefficients.Add(0, 0);
        }

        public PolynomialFunction(ICollection<KeyValuePair<int, double>> coefficients) {
            this.coefficients = new Dictionary<int, double>();
            foreach(var pair in coefficients) {
                this.coefficients.Add(pair.Key, pair.Value);
            }
        }

        public PolynomialFunction(PolynomialFunction other) {
            coefficients = new Dictionary<int, double>();
            foreach(var key in other.coefficients.Keys) {
                coefficients.Add(key, other.coefficients[key]);
            }
        }

        public void SetCoefficient(int exponent, double coefficient) {
            if(coefficients.ContainsKey(exponent)) {
                coefficients[exponent] = coefficient;
            } else {
                coefficients.Add(exponent, coefficient);
            }
        }

        public PolynomialFunction SetC(int e, double c) {
            SetCoefficient(e, c);
            return this;
        }

        public override double Eval(double input) {
            var retVal = coefficients[0];
            foreach(var key in coefficients.Keys) {
                retVal += Math.Pow(input, key) * coefficients[key];
            }
            return retVal;
        }

        public override string ToString() {
            var builder = new StringBuilder();
            SortedList<int, double> sortedKeys = new SortedList<int, double>();
            foreach(var key in coefficients.Keys) {
                sortedKeys.Add(key, coefficients[key]);
            }
            builder.Append("f(x) = ");
            var first = true;
            for(int i = sortedKeys.Count - 1; i > -1; --i) {
                var key = sortedKeys.Keys[i];
                var value = coefficients[key];
                if(value == 0) { continue; }
                if(first) {
                    first = false;
                } else {
                    builder.Append(" + ");
                }

                if(key == 1) {
                    if(value != 1) {
                        builder.Append(value);
                    }
                    builder.Append("x");
                } else if(key == 0) {
                    builder.Append(value);
                } else {
                    if(value != 1) {
                        builder.Append(value);
                    }
                    builder.Append("x^");
                    builder.Append(key);
                }
            }
            return builder.ToString();
        }
    }
}
