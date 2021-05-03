using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Polymorph.Unity.Editor
{
    [CustomPropertyDrawer(typeof(Generator), true)]
    public class GeneratorEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, property.displayName);
            rect.y += rect.height;
            if(property.isExpanded) {
                rect.x += EditorGUIUtility.singleLineHeight;
                rect.width -= rect.x;
                var proto = property.FindPropertyRelative("Proto");
                if(proto != null) {
                    EditorGUI.PropertyField(rect, proto);
                    rect.y += rect.height;
                }
                var pool = property.FindPropertyRelative("PoolParent");
                if(pool != null) {
                    EditorGUI.PropertyField(rect, pool);
                    rect.y += rect.height;
                }
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("ContentParent"));
                rect.y += rect.height;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("WorldPositionStays"));
            }
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var retVal = EditorGUIUtility.singleLineHeight;
            if(property.isExpanded) {
                if(property.FindPropertyRelative("Proto") != null)
                    retVal += EditorGUIUtility.singleLineHeight;
                if(property.FindPropertyRelative("PoolParent") != null)
                    retVal += EditorGUIUtility.singleLineHeight;
                retVal += EditorGUIUtility.singleLineHeight * 2;
            }
            return retVal;
        }
    }
}
