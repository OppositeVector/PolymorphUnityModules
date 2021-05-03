using System;
using System.Collections.Generic;
using System.Text;

namespace Polymorph.DataTools {

    public class Span {

        public double start;
        public double end;
        public double length;
        public object meta;

        public Span(double s, double e, object m) {
            start = s;
            end = e;
            length = end - start;
            meta = m;
        }

        public Span(Span other, double inRangeStart, double inRangeEnd) {

            if(inRangeStart > other.start) {
                start = inRangeStart;
            } else {
                start = other.start;
            }

            if(inRangeEnd < other.end) {
                end = inRangeEnd;
            } else {
                end = other.end;
            }

            length = end - start;
            meta = other.meta;
        }

        public override string ToString() {
            return start.ToString("0.00") + " - " + end.ToString("0.00") + ": " + meta;
        }
    }

    public class TypedSpan<T> : Span {

        new public T meta { get { return (T)base.meta; } }
        public TypedSpan(double s, double e, T m) :base(s, e, m) { }
        public TypedSpan(Span other, double inRangeStart, double inRangeEnd) : base(other, inRangeStart, inRangeEnd) { }
    }
}
