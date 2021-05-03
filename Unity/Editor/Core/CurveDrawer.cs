using UnityEditor;
using UnityEngine;

namespace Polymorph.Unity.Core.Editor {

    [CustomPropertyDrawer(typeof(CurveLibrary.Curve))]
    class CurveDrawer : PropertyDrawer {
       
        float lineHeight = 16;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            Rect rect = position;
            rect.height = lineHeight;
            rect.width *= 0.5f;
            rect.y = position.y + (position.height / 2) - (lineHeight / 2);
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("name"), GUIContent.none);
            rect.height = position.height;
            rect.y = position.y;
            rect.x += rect.width;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("curve"), GUIContent.none);
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return 50;
        }
    }
}
