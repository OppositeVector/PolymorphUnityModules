using UnityEditor;
using UnityEngine;

namespace Polymorph.Unity.AnimatedUI.Editor {

    [CustomEditor(typeof(ShrinkingUI))]
    class ShrinkingUIDrawer : AnimatedUIBehaviourDrawer {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            EditorGUI.BeginChangeCheck();
            var constraintProp = serializedObject.FindProperty("constraint");
            EditorGUILayout.PropertyField(constraintProp);
            var constraint = (ShrinkingUI.Constraint) constraintProp.enumValueIndex;
            switch(constraint) {
                case ShrinkingUI.Constraint.Dynamic:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("verticalCon"), new GUIContent("Vertical"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("horizontalCon"), new GUIContent("Horizontal"));
                    break;
                case ShrinkingUI.Constraint.Static:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("vertical"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("horizontal"));
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("origSize"));
                    EditorGUI.EndDisabledGroup();
                    if(GUILayout.Button("Set Size")) {
                        ((ShrinkingUI) tar).SetSize();
                    }
                    break;
                case ShrinkingUI.Constraint.Sourced:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("vertical"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("horizontal"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("source"));
                    break;
            }
            if(EditorGUI.EndChangeCheck()) {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
