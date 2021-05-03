using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Polymorph.Unity.Core;

namespace Polymorph.Unity.AnimatedUI.Editor {

    [CustomPropertyDrawer(typeof(PolyCurve))]
    public class PolyCurveDrawer : PropertyDrawer {

        protected static AnimationCurve GetStandardCurve(SerializedProperty prop) {
            return PolyCurve.GetStandardCurve((BuiltIn) prop.enumValueIndex);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label);
            rect.y += rect.height;
            if(property.isExpanded) {
                EditorGUI.BeginChangeCheck();
                rect.x += EditorGUIUtility.singleLineHeight;
                rect.width -= rect.x;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("time"));
                rect.y += rect.height;
                var curveTypeProp = property.FindPropertyRelative("curveType");
                EditorGUI.PropertyField(rect, curveTypeProp);
                rect.y += rect.height;
                // EditorGUILayout.PropertyField(curveTypeProp);
                var refreshCurve = false;
                if(EditorGUI.EndChangeCheck()) {
                    // serializedObject.ApplyModifiedProperties();
                    refreshCurve = true;
                }
                var curveType = (CurveType) curveTypeProp.enumValueIndex;
                var curveEditingDisabled = true;
                switch(curveType) {
                    case CurveType.BuiltIn:
                        var sCurveProp = property.FindPropertyRelative("standardCurve");
                        EditorGUI.BeginChangeCheck();
                        EditorGUI.PropertyField(rect, sCurveProp);
                        rect.y += rect.height;
                        if(EditorGUI.EndChangeCheck() || refreshCurve) {
                            // serializedObject.ApplyModifiedProperties();
                            property.FindPropertyRelative("curve").animationCurveValue = GetStandardCurve(sCurveProp);
                        }
                        break;
                    case CurveType.Curve:
                        curveEditingDisabled = false;
                        break;
                    case CurveType.CurveLibrary:
                        EditorGUI.BeginChangeCheck();
                        var library = property.FindPropertyRelative("library");
                        library.objectReferenceValue = EditorGUI.ObjectField(rect, "library", library.objectReferenceValue, typeof(CurveLibrary), false);
                        rect.y += rect.height;
                        if(EditorGUI.EndChangeCheck()) {
                            property.FindPropertyRelative("curveIndex").intValue = 0;
                            refreshCurve = true;
                        }
                        library = property.FindPropertyRelative("library");
                        if(library.objectReferenceValue != null) {
                            var index = property.FindPropertyRelative("curveIndex");
                            var curveLib = (CurveLibrary) library.objectReferenceValue;
                            EditorGUI.BeginChangeCheck();
                            index.intValue = EditorGUI.Popup(rect, index.intValue, curveLib.GetNames());
                            rect.y += rect.height;
                            if(EditorGUI.EndChangeCheck() || refreshCurve) {
                                // property.ApplyModifiedProperties();
                                index = property.FindPropertyRelative("curveIndex");
                                property.FindPropertyRelative("curve").animationCurveValue = curveLib[index.intValue];
                            }
                        }

                        break;
                }
                EditorGUI.BeginDisabledGroup(curveEditingDisabled);
                rect.height = EditorGUIUtility.singleLineHeight * 4;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("curve"));
                rect.y += rect.height;
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.EndDisabledGroup();
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            float retVal = EditorGUIUtility.singleLineHeight;
            if(property.isExpanded) {
                var curveTypeProp = property.FindPropertyRelative("curveType");
                var curveType = (CurveType) curveTypeProp.enumValueIndex;
                retVal += EditorGUIUtility.singleLineHeight * 2;
                switch(curveType) {
                    case CurveType.BuiltIn:
                        retVal += EditorGUIUtility.singleLineHeight;
                        break;
                    case CurveType.CurveLibrary:
                        retVal += EditorGUIUtility.singleLineHeight;
                        var library = property.FindPropertyRelative("library");
                        if(library.objectReferenceValue != null) {
                            retVal += EditorGUIUtility.singleLineHeight;
                        }
                        break;
                }
                retVal += EditorGUIUtility.singleLineHeight * 4;
            }
            return retVal;
        }
    }
}
