using System;
using System.Collections.Generic;

namespace Polymorph.DataTools {
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">T will be compared to T and float</typeparam>
    [System.Serializable]
    public class RangeArray<T> where T : IComparable {

        T[] arr;
        float range;
        int index;
        int _length;
        public int length {
            get { return _length; }
            private set { _length = value; }
        }
        public T this[int i] {
            get { return arr[index + i]; }
        }

        public RangeArray(ICollection<T> col, float range) {
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
            index = 0;
            length = 0;
        }

        void SetupIndicies(float start, float end) {
            index = Array.BinarySearch(arr, start);
            var endIndex = Array.BinarySearch(arr, end);
            if(index < 0) {
                index = ~index;
            }
            if(endIndex < 0) {
                endIndex = ~endIndex;
            } else {
                ++endIndex;
            }
            length = (endIndex - index);
        }

        public void SetPoint(float value) {
            SetupIndicies(value - range, value + range);
        }
    }
}
