using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace Polymorph.DataTools {
    [System.Serializable]
    public class CircularBuffer<T> : IEnumerable<T> {
        T[] arr;
        int offset;
        public int length {
            get { return arr.Length; }
        }
        public CircularBuffer(int size) {
            arr = new T[size];
            offset = 0;
        }
        public CircularBuffer(ICollection<T> col) {
            arr = new T[col.Count];
            offset = 0;
            var i = 0;
            foreach(var item in col) {
                arr[i] = item;
                ++i;
            }
        }
        public CircularBuffer() { }
        public T this[int i] {
            get { return arr[MobiusIndex(i + offset, arr.Length)]; }
            set { arr[MobiusIndex(i + offset, arr.Length)] = value; }
        }
        public void Shift(int by) {
            offset = MobiusIndex(offset - by, arr.Length);
        }
        static int MobiusIndex(int index, int count) {
            var flooredDivide = (int) System.Math.Floor((double) index / count);
            var closestRoundDivision = flooredDivide * count;
            return index - closestRoundDivision;
        }
        public void CopyTo(ref T[] buffer, int startIndex) {
            for(int i = 0; i < buffer.Length; ++i) {
                buffer[i] = this[startIndex + i];
            }
        }
        public void CopyInReverse(ref T[] buffer, int startIndex) {
            startIndex += buffer.Length - 1;
            for(int i = 0; i < buffer.Length; ++i) {
                buffer[i] = this[startIndex - i];
            }
        }
        public void Swap(T[] buffer, int startIndex) {
            for(int i = 0; i < buffer.Length; ++i) {
                this[startIndex + i] = buffer[i];
            }
        }
        public IEnumerator<T> GetEnumerator() {
            for(int i = 0; i < length; ++i) {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
