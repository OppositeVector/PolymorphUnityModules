using System;
using System.Collections.Generic;
using UnityEngine;

namespace Polymorph.Unity.MVVM {

    [ExecuteInEditMode]
    public class ComplexDataContext : DataContext {

        [System.Serializable]
        public class SecondaryPath {
            public Path path;
            public View[] views;
        }

        [Space]
        [SerializeField]
        SecondaryPath[] secondaryPaths;

        protected override void Awake() {
            // base.Awake();
            if(secondaryPaths != null) {
                if(Application.isPlaying) {
                    for(int i = 0; i < secondaryPaths.Length; ++i) {
                        var secondaryObj = secondaryPaths[i];
                        var absolutePath = secondaryObj.path.GetAbsolutePath(transform);
                        if(!string.IsNullOrEmpty(absolutePath)) {
                            var actions = ExtractActions(secondaryObj.views);
                            ViewModelRegistry.Subscribe(absolutePath, actions);
                        }
                    }
                }
            }
        }

        protected override void Update() {
            base.Update();
            if(secondaryPaths != null) {
                for(int i = 0; i < secondaryPaths.Length; ++i) {
                    if(secondaryPaths[i].path.updateRequired) {
                        secondaryPaths[i].path.UpdateAbsolutePath(transform);
                    }
                }
            }
        }

        protected override void OnDestroy() {
            // base.OnDestroy();
            if(secondaryPaths != null) {
                if(Application.isPlaying) {
                    for(int i = 0; i < secondaryPaths.Length; ++i) {
                        var secondaryObj = secondaryPaths[i];
                        var absolutePath = secondaryObj.path.GetAbsolutePath(transform);
                        if(!string.IsNullOrEmpty(absolutePath)) {
                            var actions = ExtractActions(secondaryObj.views);
                            ViewModelRegistry.Unsubscribe(absolutePath, actions);
                        }
                    }
                }
            }
        }

        protected override void OnTransformParentChanged() {
            // base.OnTransformParentChanged();
            if(mainPath.updateRequired) {
                mainPath.UpdateAbsolutePath(transform);
            }
            if(secondaryPaths != null) {
                string prevPath;
                string newPath;
                for(int i = 0; i < secondaryPaths.Length; ++i) {
                    var secondaryObj = secondaryPaths[i];
                    if(secondaryObj.path.UpdateAbsolutePath(transform, out prevPath, out newPath)) {
                        if(Application.isPlaying) {
                            var actions = ExtractActions(secondaryObj.views);
                            if(!string.IsNullOrEmpty(prevPath)) {
                                ViewModelRegistry.Unsubscribe(prevPath, actions);
                            }
                            if(!string.IsNullOrEmpty(newPath)) {
                                ViewModelRegistry.Subscribe(newPath, actions);
                            }
                        }
                    }
                }
            }
        }
    }
}
