using UnityEditor;
using UnityEngine;
using Polymorph.Unity.Core;

namespace Polymorph.Unity.AnimatedUI.Editor {

    [CustomEditor(typeof(AnimatedUIBehaviour), true)]
    public class AnimatedUIBehaviourDrawer : UnityEditor.Editor {

        AnimatedUIBehaviour _tar;
        protected AnimatedUIBehaviour tar {
            get {
                if(_tar != target) {
                    _tar = target as AnimatedUIBehaviour;
                }
                return _tar;
            }
        }

        //protected static AnimationCurve GetStandardCurve(SerializedProperty prop) {
        //    return AnimatedUIBehaviour.GetStandardCurve((BuiltIn) prop.enumValueIndex);
        //}

        public override void OnInspectorGUI() {

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            EditorGUI.EndDisabledGroup();

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            //EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultTime"));
            //var repeat = serializedObject.FindProperty("repeat");
            //if(repeat.boolValue) {
            //    var repeatCount = serializedObject.FindProperty("repeatCount");
            //    var repeatCountValue = repeatCount.intValue;
            //    EditorGUILayout.BeginHorizontal();
            //    EditorGUILayout.PropertyField(repeat);
            //    var newValue = EditorGUILayout.IntField(GUIContent.none, repeatCountValue);
            //    if(newValue != repeatCountValue) {
            //        if(newValue >= 1) {
            //            repeatCount.intValue = newValue;
            //        }
            //    }
            //    EditorGUILayout.EndHorizontal();
            //} else {
            //    EditorGUILayout.PropertyField(repeat);
            //}

            EditorGUILayout.PropertyField(serializedObject.FindProperty("inCurve"));
            var rect = EditorGUILayout.GetControlRect(false, 0);
            rect.x = rect.width - EditorGUIUtility.singleLineHeight;
            rect.width -= rect.x;
            rect.height = EditorGUIUtility.singleLineHeight;
            var singleCurveProp = serializedObject.FindProperty("singleCurve");
            var singleCurve = singleCurveProp.boolValue;
            singleCurve = !EditorGUI.Toggle(rect, !singleCurve);
            singleCurveProp.boolValue = singleCurve;
            EditorGUI.BeginDisabledGroup(singleCurve);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("outCurve"));
            EditorGUI.EndDisabledGroup();


            //if(singleCurve) {

            //    EditorGUILayout.PropertyField()
            //} else {
            //    EditorGUILayout.PropertyField(serializedObject.FindProperty("inCurve"), new GUIContent("In Curve"));
            //    var rect = EditorGUILayout.GetControlRect();
            //}

            //EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultCurve"));
            //var singleCurveProp = serializedObject.FindProperty("singleCurve");
            //EditorGUILayout.PropertyField(singleCurveProp);
            //var singleCurve = singleCurveProp.boolValue;
            //if(!singleCurve) {
            //    EditorGUILayout.PropertyField(serializedObject.FindProperty("secondCurve"));
            //}
            if(EditorGUI.EndChangeCheck()) {
                serializedObject.ApplyModifiedProperties();
            }
            //var curveTypeProp = serializedObject.FindProperty("curveType");
            //curveTypeProp.isExpanded = EditorGUILayout.Foldout(curveTypeProp.isExpanded, "Curve");
            //if(curveTypeProp.isExpanded) {
            //    EditorGUILayout.PropertyField(curveTypeProp);
            //    var refreshCurve = false;
            //    if(EditorGUI.EndChangeCheck()) {
            //        serializedObject.ApplyModifiedProperties();
            //        refreshCurve = true;
            //    }
            //    var curveType = (CurveType) curveTypeProp.enumValueIndex;
            //    var curveEditingDisabled = true; ;
            //    switch(curveType) {
            //        case CurveType.BuiltIn:
            //            var sCurveProp = serializedObject.FindProperty("standardCurve");
            //            EditorGUI.BeginChangeCheck();
            //            EditorGUILayout.PropertyField(sCurveProp);
            //            if(EditorGUI.EndChangeCheck() || refreshCurve) {
            //                serializedObject.ApplyModifiedProperties();
            //                serializedObject.FindProperty("defaultCurve").animationCurveValue = GetStandardCurve(serializedObject.FindProperty("standardCurve"));
            //            }
            //            break;
            //        case CurveType.Curve:
            //            curveEditingDisabled = false;
            //            break;
            //        case CurveType.CurveLibrary:
            //            EditorGUI.BeginChangeCheck();
            //            var library = serializedObject.FindProperty("library");
            //            library.objectReferenceValue = EditorGUILayout.ObjectField("library", library.objectReferenceValue, typeof(CurveLibrary), false);
            //            if(EditorGUI.EndChangeCheck()) {
            //                serializedObject.FindProperty("curveIndex").intValue = 0;
            //                refreshCurve = true;
            //            }
            //            library = serializedObject.FindProperty("library");
            //            if(library.objectReferenceValue != null) {
            //                var index = serializedObject.FindProperty("curveIndex");
            //                var curveLib = (CurveLibrary) library.objectReferenceValue;
            //                EditorGUI.BeginChangeCheck();
            //                EditorGUILayout.BeginHorizontal();
            //                EditorGUILayout.PrefixLabel("  ");
            //                index.intValue = EditorGUILayout.Popup(index.intValue, curveLib.GetNames());
            //                if(EditorGUI.EndChangeCheck() || refreshCurve) {
            //                    serializedObject.ApplyModifiedProperties();
            //                    index = serializedObject.FindProperty("curveIndex");
            //                    serializedObject.FindProperty("defaultCurve").animationCurveValue = curveLib[index.intValue];
            //                }
            //                EditorGUILayout.EndHorizontal();
            //            }

            //            break;
            //    }
            //    EditorGUI.BeginDisabledGroup(curveEditingDisabled);
            //    EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultCurve"), GUILayout.Height(50));
            //    EditorGUI.EndDisabledGroup();
            //}
            // EditorGUI.EndDisabledGroup();
            #region Unity Default Inspector
            EditorGUI.BeginChangeCheck();
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while(iterator.NextVisible(enterChildren)) {
                if(iterator.propertyPath != "m_Script") {
                    EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
                }
                enterChildren = false;
            }
            if(EditorGUI.EndChangeCheck()) {
                serializedObject.ApplyModifiedProperties();
            }
            #endregion
            // serializedObject.ApplyModifiedProperties();
        }
    }
}
