#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class AudioCustom {
	public AudioClip clip;
	[Range(0, 1)]public float volume;
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(AudioCustom))]
public class AudioCustomDrawer : PropertyDrawer {
	public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(pos, label, property);
		pos = EditorGUI.PrefixLabel(pos, GUIUtility.GetControlID(FocusType.Passive), label);
		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		EditorGUI.PropertyField(new Rect(pos.x   , pos.y, 30,           pos.height), property.FindPropertyRelative("volume"), GUIContent.none);
		EditorGUI.PropertyField(new Rect(pos.x+30, pos.y, pos.width-30, pos.height), property.FindPropertyRelative("clip"), GUIContent.none);
		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}
}
#endif