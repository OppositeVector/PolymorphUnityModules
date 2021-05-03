using System;
using System.Collections;
using UnityEngine;
using Polymorph.Unity.Core;

namespace Polymorph.Unity.MVVM {

    /// <summary>
    /// Interface for views that do not inherit from View
    /// </summary>
    public interface IView {
        void ViewModelChanged(object m);
    }

    public class View : AdvancedBehaviour, IView {

        protected object viewModel;

        public virtual void ViewModelChanged(object m) {
            viewModel = m;
        }

        protected void RunOnFirstFrame(Action action) {
            StartCoroutine(WaitFor(0, action));
        }

        IEnumerator WaitFor(float time, Action action) {
            while(time > 0) {
                yield return null;
                time -= Time.deltaTime;
            }
            action();
        }
    }

    public class View<T> : View where T : ViewModel  {
        new protected T viewModel => base.viewModel as T; 
    }
}
