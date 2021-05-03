using System;
using System.Collections.Generic;
using System.Text;

namespace Polymorph.DataTools {

    public class SectionedRange {

        List<Span> spans;
        public Span this[int i] { get { return spans[i]; } }
        public int length { get { return spans.Count; } }

        public SectionedRange(object defaultMeta) {
            spans = new List<Span>();
            spans.Add(new Span(float.MinValue, float.MaxValue, defaultMeta));
        }

        Int32 BinarySearchIndexOf(List<Span> list, double value) {

            Int32 lower = 0;
            Int32 upper = list.Count - 1;

            while(lower <= upper) {
                Int32 middle = lower + (upper - lower) / 2;
                Int32 comparisonResult = value.CompareTo(list[middle].end);
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

        void GetIndecies(double start, double end, out int startIndex, out int endIndex) {

            startIndex = BinarySearchIndexOf(spans, start);
            endIndex = BinarySearchIndexOf(spans, end);

            if(startIndex < 0) {
                startIndex = ~startIndex;
            }
            if(endIndex < 0) {
                endIndex = ~endIndex;
            }
        }

        public void AddSpan(double start, double end, object meta) {

            int startIndex;
            int endIndex;
            GetIndecies(start, end, out startIndex, out endIndex);

            if(startIndex == endIndex) {

                var startSpan = spans[startIndex];
                if(!startSpan.meta.Equals(meta)) {
                    var tmpEnd = startSpan.end;
                    spans[startIndex].end = start;
                    spans.Insert(startIndex + 1, new Span(start, end, meta));
                    spans.Insert(startIndex + 2, new Span(end, tmpEnd, startSpan.meta));
                }
            } else {

                while((endIndex - startIndex) > 1) {
                    spans.RemoveAt(endIndex - 1);
                    --endIndex;
                }

                bool insertNew = false;
                var startSpan = spans[startIndex];
                if(startSpan.meta.Equals(meta)) {
                    startSpan.end = end;
                } else {
                    startSpan.end = start;
                    insertNew = true;
                }

                var endSpan = spans[endIndex];
                if(insertNew) {
                    if(endSpan.meta.Equals(meta)) {
                        endSpan.start = start;
                        insertNew = false;
                    } else {
                        endSpan.start = end;
                    }
                } else {
                    if(startSpan.meta.Equals(endSpan.meta)) {
                        startSpan.end = endSpan.end;
                        spans.RemoveAt(endIndex);
                    } else {
                        endSpan.start = end;
                    }
                }
                if(insertNew) {
                    spans.Insert(endIndex, new Span(start, end, meta));
                }
            }
        }

        public List<Span> GetInRange(double start, double end) {

            int startIndex;
            int endIndex;
            GetIndecies(start, end, out startIndex, out endIndex);

            List<Span> retVal = new List<Span>();
            for(int i = startIndex; i < (endIndex + 1); ++i) {
                var span = spans[i];
                retVal.Add(new Span(spans[i], start, end));
            }

            return retVal;
        }

        public List<Span> GetInRange(double start, double end, object meta) {

            int startIndex;
            int endIndex;
            GetIndecies(start, end, out startIndex, out endIndex);

            List<Span> retVal = new List<Span>();
            for(int i = startIndex; i < (endIndex + 1); ++i) {
                var span = spans[i];
                if(span.meta.Equals(meta)) {
                    retVal.Add(new Span(spans[i], start, end));
                }
            }

            return retVal;
        }

        public List<TypedSpan<T>> GetInRange<T>(double start, double end) {

            int startIndex;
            int endIndex;
            GetIndecies(start, end, out startIndex, out endIndex);

            List<TypedSpan<T>> retVal = new List<TypedSpan<T>>();
            for(int i = startIndex; i < (endIndex + 1); ++i) {
                var span = spans[i];
                if(span.meta is T) {
                    retVal.Add(new TypedSpan<T>(spans[i], start, end));
                }
            }

            return retVal;
        }

        public override string ToString() {
            var builder = new StringBuilder();
            builder.Append("Range:\n");
            for(int i = 0; i < spans.Count; ++i) {
                builder.Append("\t");
                builder.Append(i);
                builder.Append(": ");
                builder.Append(spans[i]);
                builder.Append("\n");
            }
            return builder.ToString();
        }
    }
}
