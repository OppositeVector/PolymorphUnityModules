using UnityEditor;
using UnityEngine;

namespace Polymorph.Unity.AnimatedUI.Editor {

    [CustomEditor(typeof(SlidingUI))]
    class SlidingUIDrawer : AnimatedUIBehaviourDrawer {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("origPos"));
            EditorGUI.EndDisabledGroup();
            if(GUILayout.Button("Set Position")) {
                ((SlidingUI) tar).SetPosition();
            }
        }
    }
}
