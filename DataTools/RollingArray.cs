using System;
using System.Collections.Generic;

namespace Polymorph.DataTools {
    /// <summary>
    /// A rolling array must always be "rolling" forward, meaning from one call to the other
    /// of Advance, the value has to grow, or the internal structure of the array will go haywire
    /// </summary>
    /// <typeparam name="T">T will be compared to T and float</typeparam>
    public class RollingArray<T> : IEnumerable<T> where T : IComparable {

        T[] arr;
        float range;
        int currentStart;
        int currentEnd;

        EnterExitBuffer<T> buffer;

        public T this[int i] {
            get { return arr[i]; }
        }
        public int length {
            get { return arr.Length; }
        }

        public RollingArray(ICollection<T> col, float range, int bufferSize) {
            if(col.Count == 0) throw new ArgumentException("Wrapped collection cannot be empty", "col");
            if(range == 0) throw new ArgumentException("Range cannot be 0", "range");
            this.range = range;
            arr = new T[col.Count];
            int i = 0;
            foreach(var val in col) {
                arr[i] = val;
                ++i;
            }
            Array.Sort(arr);
            currentStart = 0;
            currentEnd = 0;
            buffer = new EnterExitBuffer<T>(bufferSize);
        }

        public EnterExitBuffer<T> Advance(float point) {
            var start = point - range;
            var end = point + range;
            buffer.Clear();
            while((currentStart < arr.Length) && (arr[currentStart].CompareTo(start) < 0)) {
                buffer.exit.Add(arr[currentStart]);
                ++currentStart;
            }
            while((currentEnd < arr.Length) && (arr[currentEnd].CompareTo(end) < 0)) {
                buffer.enter.Add(arr[currentEnd]);
                ++currentEnd;
            }
            return buffer;
        }

        public void Reset() {
            currentStart = 0;
            currentEnd = 0;
        }

        public IEnumerator<T> GetEnumerator() {
            for(int i = 0; i < arr.Length; ++i) {
                yield return arr[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
