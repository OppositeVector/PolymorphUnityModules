using System;
using UnityEngine;

namespace Polymorph.Unity.AnimatedUI {

    internal class FinishState : StateMachineBehaviour {
        public Action onEnter;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            if(onEnter != null) {
                onEnter();
            }
        }
    }
}
