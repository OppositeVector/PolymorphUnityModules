using System;
using System.Collections.Generic;
using System.Text;

namespace Polymorph.DataTools.Functions {

    public class Function : BaseFunction {

        delegate IList<double> GetKeysDelegate();

        SortedList<double, BaseFunction> sections;

        IList<double> cachedKeys;
        GetKeysDelegate getKeys;

        public Function(KeyValuePair<double, BaseFunction>[] sections) {
            Init();
            for(int i = 0; i < sections.Length; ++i) {
                this.sections.Add(sections[i].Key, sections[i].Value);
            }
        }

        public Function() {
            Init();
        }

        void Init() {
            sections = new SortedList<double, BaseFunction>();
            sections.Add(double.MinValue, new ConstantFunction(0));
            getKeys = GetCacheAndGetKeys;
        }

        IList<double> GetKeys() {
            return cachedKeys;
        }

        IList<double> GetCacheAndGetKeys() {
            cachedKeys = sections.Keys;
            getKeys = GetKeys;
            return cachedKeys;
        }

        public void Add(double from, BaseFunction function) {
            sections.Add(from, function);
            getKeys = GetCacheAndGetKeys;
        }

        public override double Eval(double input) {
            var index = BinarySearch(input);
            if(index < 0) {
                index = (~index - 1);
            }
            var f = sections[getKeys()[index]];
            var retVal = f.Eval(input);
            // Console.WriteLine("f(" + input + ") = " + retVal + ", I: " + index + ", F:" + f);
            return retVal;
        }

        Int32 BinarySearch(double x) {

            var keys = getKeys();

            Int32 lower = 0;
            Int32 upper = keys.Count - 1;

            while(lower <= upper) {
                Int32 middle = lower + (upper - lower) / 2;
                Int32 comparisonResult = x.CompareTo(keys[middle]);
                if(comparisonResult == 0) {
                    return middle;
                } else if(comparisonResult < 0) {
                    upper = middle - 1;
                } else {
                    lower = middle + 1;
                }
            }

            return ~lower;
        }

        public override string ToString() {
            var builder = new StringBuilder();
            builder.Append("f(x) => \t");
            var keys = getKeys();
            var first = true;
            for(int i = 1; i < keys.Count; ++i) {
                if(first) {
                    first = false;
                } else {
                    builder.Append("\n\t\t");
                }
                builder.Append("x > ");
                builder.Append(keys[i]);
                builder.Append(": ");
                builder.Append(sections[keys[i]].ToString());
            }
            return builder.ToString();
        }
    }
}
