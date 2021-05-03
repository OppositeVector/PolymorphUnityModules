using System;
using System.Collections;
using System.Text;
using UnityEngine;

namespace Polymorph.Unity {

    public static class UnityTools {

        public static string GetPath(Transform transform) {
            var builder = new StringBuilder();
            GetPathInternal(transform, builder);
            return builder.ToString();
        }

        static void GetPathInternal(Transform current, StringBuilder builder, bool top = true) {
            if(current == null) {
                return;
            } else {
                GetPathInternal(current.parent, builder, false);
                if(top) {
                    builder.Append(current.name);
                } else {
                    builder.Append(current.name);
                    builder.Append("/");
                }
            }
        }

        static IEnumerator WaitFor(float time, Action action) {
            while(time > 0) {
                yield return null;
                time -= Time.deltaTime;
            }
            action();
        }
    }
}
