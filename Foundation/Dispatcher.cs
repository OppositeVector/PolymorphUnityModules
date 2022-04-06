using System.Collections.Generic;
using System;

namespace Polymorph.Foundation {
    public static class Dispatcher {

        static Dictionary<string, List<Action<object>>> _accepters;

        static Dispatcher() {
            _accepters = new Dictionary<string, List<Action<object>>>();
        }

        public static void Dispatch(string name, object data) {
            if(_accepters.ContainsKey(name)) {
                var l = _accepters[name];
                for(int i = 0; i < l.Count; ++i) {
                    l[i](data);
                }
            }
        }

        public static void Dispatch(string name) {
            Dispatch(name, null);
        }

        public static void Accept(string name, Action<object> accepter) {
            if(_accepters.ContainsKey(name)) {
                _accepters[name].Add(accepter);
            } else {
                var l = new List<Action<object>>();
                l.Add(accepter);
                _accepters.Add(name, l);
            }
        }
    }
}