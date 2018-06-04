using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(Palette.PaletteItem))]
public class PaletteEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var rect = position;
        rect.width /= 5;
        EditorGUIUtility.labelWidth = 32;
        EditorGUI.PropertyField(rect, property.FindPropertyRelative("albedo"), new GUIContent("A", "Albedo"));
        rect.x += rect.width;
        EditorGUI.PropertyField(rect, property.FindPropertyRelative("smoothness"), new GUIContent("S", "Smoothness"));
        rect.x += rect.width;
        EditorGUI.PropertyField(rect, property.FindPropertyRelative("metallic"), new GUIContent("M", "Metallic"));
        rect.x += rect.width;
        EditorGUI.PropertyField(rect, property.FindPropertyRelative("emission"), new GUIContent("E", "Emission"));
        rect.x += rect.width;
        EditorGUI.PropertyField(rect, property.FindPropertyRelative("noiseScale"), new GUIContent("N", "Noise Scale"));
        property.serializedObject.ApplyModifiedProperties();
    }
}
