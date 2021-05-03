using UnityEditor;
using UnityEngine;

namespace Polymorph.Unity.MVVM.Editor {

    //[CustomEditor(typeof(DataContext))]
    //public class DataContextEditor : UnityEditor.Editor {

    //    DataContext _tar;
    //    DataContext tar {
    //        get {
    //            if(_tar != target) {
    //                _tar = target as DataContext;
    //            }
    //            return _tar;
    //        }
    //    }

    //    public override void OnInspectorGUI() {

    //        serializedObject.Update();

    //        EditorGUI.BeginDisabledGroup(true);
    //        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
    //        EditorGUILayout.TextField("Full Path", tar.GetAbsolutePath());
    //        EditorGUI.EndDisabledGroup();

    //        EditorGUI.BeginChangeCheck();
    //        EditorGUILayout.PropertyField(serializedObject.FindProperty("path"));
    //        var absolute = serializedObject.FindProperty("absolute");
    //        var absVal = absolute.boolValue;
    //        EditorGUILayout.BeginHorizontal();
    //        EditorGUI.BeginDisabledGroup(!absVal);
    //        if(GUILayout.Button(new GUIContent("Relative", "a relative DataContext will look for a parent DataContext and parent's path as the begining of his own path (parent.path + . + path)"))) {
    //            absolute.boolValue = false;
    //        }
    //        EditorGUI.EndDisabledGroup();
    //        EditorGUI.BeginDisabledGroup(absVal);
    //        if(GUILayout.Button(new GUIContent("Absolute", "an absolute DataContext will use his path as is"))) {
    //            absolute.boolValue = true;
    //        }
    //        EditorGUI.EndDisabledGroup();
    //        EditorGUILayout.EndHorizontal();

    //        serializedObject.ApplyModifiedProperties();

    //        if(EditorGUI.EndChangeCheck()) {
    //            tar.AbsolutePathChanged();
    //        }
    //    }
    //}
}
