using UnityEngine;
using UnityEditor;

namespace Polymorph.Unity.MVVM.Editor {

    [CustomPropertyDrawer(typeof(DataContext.Path))]
    public class DataContextPathDrawer : PropertyDrawer {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            Rect rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("absolutePath"), new GUIContent("Full Path"));
            rect.y += rect.height;
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("path"));
            rect.y += rect.height;
            var absolute = property.FindPropertyRelative("absolute");
            var absVal = absolute.boolValue;
            
            EditorGUI.BeginDisabledGroup(!absVal);
            rect.width = rect.width / 2;
            if(GUI.Button(rect, new GUIContent("Relative", "a relative DataContext will look for a parent DataContext and parent's path as the begining of his own path (parent.path + . + path)"))) {
                absolute.boolValue = false;
            }
            rect.x += rect.width;
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(absVal);
            if(GUI.Button(rect, new GUIContent("Absolute", "an absolute DataContext will use his path as is"))) {
                absolute.boolValue = true;
            }
            EditorGUI.EndDisabledGroup();
            if(EditorGUI.EndChangeCheck()) {
                property.FindPropertyRelative("updateRequired").boolValue = true;
            } 
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight * 3;
        }
    }
}
