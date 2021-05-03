using UnityEditor;

namespace Polymorph.Unity.AnimatedUI.Editor {

    [CustomEditor(typeof(MultiElement))]
    public class MultiElementDrawer : UnityEditor.Editor {

        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("chain"), true);
            if(EditorGUI.EndChangeCheck()) {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
