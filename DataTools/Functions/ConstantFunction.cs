using System;
using System.Collections.Generic;
using System.Text;

namespace Polymorph.DataTools.Functions {

    public class ConstantFunction : BaseFunction {
        public static implicit operator double(ConstantFunction c) {
            return c.value;
        }
        double value;
        public ConstantFunction(double value) {
            this.value = value;
        }
        public override double Eval(double input) {
            return value;
        }
        public override string ToString() {
            var builder = new StringBuilder();
            builder.Append("f(x) = ");
            builder.Append(value);
            return builder.ToString();
        }
    }
}
