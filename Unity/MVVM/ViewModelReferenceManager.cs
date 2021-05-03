using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Polymorph.Unity.MVVM {

    /// <summary>
    /// Base class for creating static View Model Registry references that can be used in code
    /// </summary>
    public class ViewModelReferenceManager {

        /// <summary>
        /// Generic reference
        /// </summary>
        /// <typeparam name="T">View Model Type to be used inside the reference</typeparam>
        public class Reference<T> where T : ViewModel {

            /// <summary>
            /// Path in the View Model Registry
            /// </summary>
            public string path { get; private set; }

            /// <summary>
            /// Contructor
            /// </summary>
            /// <param name="path">Path in the View Model Registry</param>
            public Reference(string path) {
                this.path = path;
            }
            /// <summary>
            /// Set the View Model in this reference's path
            /// </summary>
            /// <param name="viewModel"></param>
            public void Set(T viewModel) {
                ViewModelRegistry.DeclareProvider(path, viewModel);
            }
            /// <summary>
            /// Get the View Model in this reference's path
            /// </summary>
            /// <returns></returns>
            public T Get() {
                return ViewModelRegistry.GetProvider(path) as T;
            }
            public void Clear(ViewModel vm = null) {
                ViewModelRegistry.ClearProvider(path, vm);
            }
            public void Subscribe(Action<object> subscriber) {
                ViewModelRegistry.Subscribe(path, subscriber);
            }
            public void Subscribe(ICollection<Action<object>> subscribers) {
                ViewModelRegistry.Subscribe(path, subscribers);
            }
            public void Unsubscribe(Action<object> subscriber) {
                ViewModelRegistry.Unsubscribe(path, subscriber);
            }
            public void Unsubscribe(ICollection<Action<object>> subscribers) {
                ViewModelRegistry.Unsubscribe(path, subscribers);
            }
        }
    }
}
