using System;
using System.Collections.Generic;
using System.Text;

namespace Polymorph {

    public static class PCol {

        public delegate bool CheckObjectDelegate<T,U>(T left, U right);

        public static List<T> Project<T, U>(ICollection<T> from, ICollection<U> to) {
            List<T> retVal = new List<T>();
            foreach(var target in to) {
                foreach(var source in from) {
                    if(target.Equals(source)) {
                        retVal.Add(source);
                        break;
                    }
                }
            }
            return retVal;
        }

        public static List<T> Project<T, U>(ICollection<T> from, ICollection<U> to, CheckObjectDelegate<T,U> check) {
            List<T> retVal = new List<T>();
            foreach(var target in to) {
                foreach(var source in from) {
                    if(check(source, target)) {
                        retVal.Add(source);
                        break;
                    }
                }
            }
            return retVal;
        }

        public static bool InRange(Array arr, int index) {
            return (index > -1) && (index < arr.Length);
        }
    }
}
