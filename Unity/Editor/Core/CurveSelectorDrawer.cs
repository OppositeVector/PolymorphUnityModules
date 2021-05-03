using UnityEditor;
using UnityEngine;

namespace Polymorph.Unity.Core.Editor {

    [CustomPropertyDrawer(typeof(CurveSelector), true)]
    public class CurveSelectorDrawer : PropertyDrawer {

        string[] emptyNames = new string[0];
        AnimationCurve emptyCurve = new AnimationCurve();

        public override void OnGUI(UnityEngine.Rect position, SerializedProperty property, UnityEngine.GUIContent label) {
            // base.OnGUI(position, property, label);

            label = EditorGUI.BeginProperty(position, label, property);
            Rect rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;
            // rect.width = EditorGUIUtility.labelWidth;
            EditorGUI.LabelField(rect, label);
            rect.x += EditorGUIUtility.labelWidth;
            rect.width = position.width - EditorGUIUtility.labelWidth;

            var libraryProp = property.FindPropertyRelative("library");
            var indexProp = property.FindPropertyRelative("index");
            AnimationCurve curve;

            if(libraryProp.objectReferenceValue == null) {
                indexProp.intValue = 0;
                curve = emptyCurve;
            }

            EditorGUI.BeginChangeCheck();
            libraryProp.objectReferenceValue = EditorGUI.ObjectField(rect, new GUIContent(), libraryProp.objectReferenceValue, typeof(CurveLibrary), false);
            rect.y += rect.height;
            if(EditorGUI.EndChangeCheck()) {
                indexProp.intValue = 0;
            }

            if(libraryProp.objectReferenceValue != null) {

                var curveLib = (CurveLibrary) libraryProp.objectReferenceValue;
                var names = curveLib.GetNames();
                if(names.Length == 0) {
                    indexProp.intValue = 0;
                    EditorGUI.Popup(rect, 0, emptyNames);
                    curve = emptyCurve;
                } else {
                    indexProp.intValue = EditorGUI.Popup(rect, indexProp.intValue, names);
                    curve = curveLib[indexProp.intValue];
                }
            } else {
                EditorGUI.Popup(rect, 0, emptyNames);
                indexProp.intValue = 0;
                curve = emptyCurve;
            }

            rect.y += rect.height;

            var previewCurve = property.FindPropertyRelative("curvePreview");
            if(previewCurve.animationCurveValue != curve) {
                previewCurve.animationCurveValue = curve;
            }
            rect.height *= 2;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(rect, previewCurve, new GUIContent());
            EditorGUI.EndDisabledGroup();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight * 4;
        }
    }
}
