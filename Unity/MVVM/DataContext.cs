using UnityEngine;
using System;
using System.Collections.Generic;

namespace Polymorph.Unity.MVVM {

    /// <summary>
    /// <para>Declares view located on the same GameObject to the ViewModelRegistry so that when a ViewModel is
    /// available or has changed, the messeges will be recieved by the View</para>
    /// <para>Absolute - the DataContext will not consider the parents of the GameObject when determining the decleration
    /// path of the views, whatever is written in the path field will be used as the decleration path</para>
    /// <para>Non Absolute / Relative - the DataContext will look in the parent GameObjects to find another DataContext,
    /// if a parent DataContext is found, then the path will be (parent.path + "." + path)</para>
    /// </summary>
    [AddComponentMenu("Polymorph/MVVM/Data Context")]
    [ExecuteInEditMode]
    public class DataContext : MonoBehaviour, ISerializationCallbackReceiver {

        [Serializable]
        public class Path {

            [SerializeField]
            string path;
            [SerializeField]
            bool absolute = true;
            bool pathAquired = false;
            public string absolutePath;

            public bool updateRequired = false;
            [SerializeField]
            bool paramsSet = false;

            public void SetParams(string path, bool absolute) {
                if(!paramsSet) {
                    paramsSet = true;
                    this.path = path;
                    this.absolute = absolute;
                }
            }

            public bool UpdateAbsolutePath(Transform transform) {
                string p;
                string n;
                return UpdateAbsolutePath(transform, out p, out n);
            }

            public bool UpdateAbsolutePath(Transform transform, out string prevPath, out string newPath) {

                paramsSet = true;
                prevPath = absolutePath;
                pathAquired = false;
                newPath = GetAbsolutePath(transform);
                updateRequired = false;

                if(absolutePath != prevPath) {

                    for(int i = 0; i < transform.childCount; ++i) {
                        var child = transform.GetChild(i);
                        var childContexts = child.gameObject.GetComponentsInChildren<DataContext>(true);
                        foreach(var context in childContexts) {
                            context.OnTransformParentChanged();
                        }
                    }
                    return true;
                }
                return false;
            }

            public string GetAbsolutePath(Transform transform) {

                if(!pathAquired) {
                    if(absolute) {
                        absolutePath = path;
                    } else {
                        var current = transform;
                        while(true) {
                            current = current.parent;
                            if(current == null) {
                                absolutePath = path;
                                break;
                            } else {
                                var context = current.gameObject.GetComponent<DataContext>();
                                if(context != null) {
                                    var contextPath = context.GetAbsolutePath();
                                    var imEmpty = string.IsNullOrEmpty(path);
                                    var contextEmpty = string.IsNullOrEmpty(contextPath);
                                    if(!imEmpty && !contextEmpty) {
                                        absolutePath = contextPath + "." + path;
                                    } else if(!imEmpty) {
                                        absolutePath = path;
                                    } else if(!contextEmpty) {
                                        absolutePath = contextPath;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    pathAquired = true;
                }
                return absolutePath;
            }
        }

        public static string GetPath(Transform transform) {
            if(transform == null) { return ""; }
            var dataContext = transform.gameObject.GetComponent<DataContext>();
            if(dataContext == null) {
                return GetPath(transform.parent);
            } else {
                return dataContext.GetAbsolutePath();
            }
        }

#pragma warning disable 0649
        [SerializeField]
        [HideInInspector]
        string path;
        [SerializeField]
        [HideInInspector]
        bool absolute = true;
        Action<object>[] viewActions;
#pragma warning restore 0649

        // bool pathAquired = false;
        /// <summary>
        /// Calculated path of this context, including parent's path if the context is set to non-absolute.
        /// </summary>
        // public string absolutePath { get; private set; }

        [SerializeField]
        protected Path mainPath;

        /// <summary>
        /// Standard MonoBehaviour.Awake
        /// </summary>
        protected virtual void Awake() {
            if(Application.isPlaying) {
                var absolutePath = mainPath.GetAbsolutePath(transform);
                // GetAbsolutePath();
                if(!string.IsNullOrEmpty(absolutePath)) {
                    viewActions = ExtractActions(GetComponents<IView>());
                    ViewModelRegistry.Subscribe(absolutePath, viewActions);
                }
            }
        }

        protected virtual void Update() {
            if(mainPath != null) {
                if(mainPath.updateRequired) {
                    mainPath.UpdateAbsolutePath(transform);
                }
            }
        }

        /// <summary>
        /// Standard MonoBehaviour.OnDestroy
        /// </summary>
        protected virtual void OnDestroy() {
            if(Application.isPlaying) {
                var absolutePath = mainPath.GetAbsolutePath(transform);
                if(!string.IsNullOrEmpty(absolutePath) && (viewActions != null)) {
                    ViewModelRegistry.Unsubscribe(absolutePath, viewActions);
                }
            }
        }

        /// <summary>
        /// Standard MonoBehaviour.OnTransformParentChanged
        /// </summary>
        protected virtual void OnTransformParentChanged() {
            string prevPath;
            string newPath;
            if(mainPath.UpdateAbsolutePath(transform, out prevPath, out newPath)) {
                if(Application.isPlaying && (viewActions != null)) {
                    if(!string.IsNullOrEmpty(prevPath)) {
                        ViewModelRegistry.Unsubscribe(prevPath, viewActions);
                    }
                    if(!string.IsNullOrEmpty(newPath)) {
                        ViewModelRegistry.Subscribe(newPath, viewActions);
                    }
                }
            }
        }

        public string GetAbsolutePath() {
            return mainPath.GetAbsolutePath(transform);
        }

        protected Action<object>[] ExtractActions(IView[] views) {
            var retVal = new Action<object>[views.Length];
            for(int i = 0; i < views.Length; ++i) {
                retVal[i] = views[i].ViewModelChanged;
            }
            return retVal;
        }

        public void OnAfterDeserialize() {
            mainPath.SetParams(path, absolute);
        }

        public void OnBeforeSerialize() { }
    }
}
