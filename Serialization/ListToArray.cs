using System;
using System.Collections.Generic;
using System.Collections;

namespace Polymorph.Serialization {

    internal class ListToArray {

        public class Element {
            public object obj;
            public long[] indicies;
        }

        IList list;
        int rank;
        public long[] lengths;
        public List<Element> elements;

        public ListToArray(IList l, int r) {
            list = l;
            rank = r;
            lengths = new long[rank];
            elements = new List<Element>();
            Traverse(list, new long[rank]);
        }

        void Traverse(IList l, long[] indicies, int depth = 0) {
            if(depth >= rank) { return; }
            if(l == null) { return; }
            if(l.Count > lengths[depth]) {
                lengths[depth] = l.Count;
            }
            if(depth < (rank - 1)) {
                for(int i = 0; i < l.Count; ++i) {
                    indicies[depth] = i;
                    if(l[i] is IList) {
                        Traverse(l[i] as IList, indicies, depth + 1);
                    }
                }
            } else {
                for(int i = 0; i < l.Count; ++i) {
                    indicies[depth] = i;
                    var ele = new Element();
                    ele.obj = l[i];
                    ele.indicies = (long[]) indicies.Clone();
                    elements.Add(ele);
                }
            }
        }
    }
}
