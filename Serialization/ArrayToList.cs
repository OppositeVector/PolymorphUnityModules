using System;
using System.Collections.Generic;

namespace Polymorph.Serialization {

    internal class ArrayToList {

        public List<object> elements;
        Array array;

        public ArrayToList(Array arr) {
            elements = new List<object>();
            array = arr;
            Traverse(elements, new long[array.Rank]);
        }

        void Traverse(List<object> list, long[] indicies, int depth = 0) {

            if(depth < (array.Rank - 1)) {
                for(int i = 0; i < array.GetLength(depth); ++i) {
                    var newList = new List<object>();
                    indicies[depth] = i;
                    Traverse(newList, indicies, depth + 1);
                    list.Add(newList);
                }
            } else {
                for(int i = 0; i < array.GetLength(depth); ++i) {
                    indicies[depth] = i;
                    list.Add(array.GetValue(indicies));
                }
            }
        }
    }
}
