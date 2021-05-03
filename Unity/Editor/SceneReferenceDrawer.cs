using Polymorph.Unity.Core;
using UnityEditor;
using UnityEngine;

namespace Polymorph.Unity.Core.Editor {
    [CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferenceDrawer : PropertyDrawer {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

            var sceneName = property.FindPropertyRelative("sceneName");
            var oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneName.stringValue);

            EditorGUI.BeginChangeCheck();
            var newScene = EditorGUI.ObjectField(position, label, oldScene, typeof(SceneAsset), false);

            if(EditorGUI.EndChangeCheck()) {
                var newPath = AssetDatabase.GetAssetPath(newScene);
                sceneName.stringValue = newPath;
            }
        }
    }
}
