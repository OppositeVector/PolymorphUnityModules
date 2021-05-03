using UnityEngine.SceneManagement;
using UnityEngine;

namespace Polymorph.Unity {

    [System.Serializable]
    public class SceneReference {
        #region Operators
        public static bool operator ==(Scene scene, SceneReference selector) {
            return scene.path == selector.sceneName;
        }
        public static bool operator ==(SceneReference selector, Scene scene) {
            return scene.path == selector.sceneName;
        }
        public static bool operator !=(Scene scene, SceneReference selector) {
            return scene.path != selector.sceneName;
        }
        public static bool operator !=(SceneReference selector, Scene scene) {
            return scene.path != selector.sceneName;
        }
        #endregion

        System.Action callback;
        public string sceneName;
        public void GoHere() {
            SceneManager.LoadSceneAsync(sceneName);
        }
        public void GoHere(System.Action callback) {
            this.callback = callback;
            SceneManager.sceneLoaded += SceneLoaded;
            SceneManager.LoadSceneAsync(sceneName);
        }
        void SceneLoaded(Scene scene, LoadSceneMode mode) {
            if(callback != null) {
                var r = callback;
                callback = null;
                r();
            }
            SceneManager.sceneLoaded -= SceneLoaded;
        }
        public override string ToString() {
            return sceneName;
        }
        public override int GetHashCode() {
            return sceneName.GetHashCode();
        }
        public override bool Equals(object obj) {
            var other = obj as SceneReference;
            if(other != null) {
                return other.GetHashCode() == GetHashCode();
            } else {
                return false;
            }
        }
    }
}
