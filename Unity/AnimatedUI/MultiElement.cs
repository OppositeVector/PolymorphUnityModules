using System;
using UnityEngine;

namespace Polymorph.Unity.AnimatedUI {

    /// <summary>
    /// <para>A single element that calls <see cref="AnimatedUIBehaviour.In()"/> and <see cref="AnimatedUIBehaviour.Out()"/></para>
    /// <para>on all the elements defined in its chain</para>
    /// </summary>
    [AddComponentMenu("Polymorph/Animated UI/Multi Element")]
    public class MultiElement : AnimatedUIBehaviour {

        /// <summary>
        /// The chain to call when and In or Out is requested
        /// </summary>
        public AnimatedUIBehaviour[] chain;

        /// <summary>
        /// <see cref="AnimatedUIBehaviour.In(float, UnityEngine.AnimationCurve, Action)"/>
        /// The callback is raised after all elements have finished
        /// </summary>
        public override void In(float time, UnityEngine.AnimationCurve curve, Action callback = null) {
            base.In(time, curve, callback);
            if(chain == null) return;
            Action onFinish = null;
            if(callback != null) {
                onFinish = CreateReturnBlock(chain.Length, callback);
            }
            foreach(var ele in chain) {
                ele.In(onFinish);
            }
        }


        /// <summary>
        /// <see cref="AnimatedUIBehaviour.Out(float, UnityEngine.AnimationCurve, Action)"/>
        /// The callback is raised after all elements have finished
        /// </summary>
        public override void Out(float time, UnityEngine.AnimationCurve curve, Action callback = null) {
            base.Out(time, curve, callback);
            if(chain == null) return;
            Action onFinish = null;
            if(callback != null) {
                onFinish = CreateReturnBlock(chain.Length, callback);
            }
            foreach(var ele in chain) {
                ele.Out(onFinish);
            }
        }

        Action CreateReturnBlock(int count, Action callback) {
            int returnedCount = 0;
            return delegate {
                ++returnedCount;
                if(returnedCount == count) {
                    if(callback != null) {
                        callback();
                    }
                }
            };
        }
    }
}
