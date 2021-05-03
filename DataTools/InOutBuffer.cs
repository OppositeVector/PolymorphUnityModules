using System;
using System.Collections.Generic;

namespace Polymorph.DataTools {
    public class EnterExitBuffer<T> {
        public class Buffer : IEnumerable<T> {
            T[] arr;
            public int length { get; set; }
            public T this[int i] {
                get { return arr[i]; }
                internal set { arr[i] = value; }
            }
            internal void Clear() {
                length = 0;
            }
            internal void Add(T ele) {
                if(length < (arr.Length - 1)) {
                    arr[length] = ele;
                    ++length;
                }
            }
            internal Buffer(int size) {
                arr = new T[size];
            }

            public IEnumerator<T> GetEnumerator() {
                for(int i = 0; i < length; ++i) {
                    yield return arr[i];
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }
        }
        public Buffer enter;
        public Buffer exit;
        internal void Clear() {
            enter.Clear();
            exit.Clear();
        }
        internal EnterExitBuffer(int size) {
            enter = new Buffer(size);
            exit = new Buffer(size);
        }
    }
}
