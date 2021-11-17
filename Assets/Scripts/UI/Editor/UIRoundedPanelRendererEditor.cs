using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIRoundedPanelRenderer))]
public class UIRoundedPanelRendererEditor : Editor {
    public override void OnInspectorGUI() {
        serializedObject.Update();
        UIRoundedPanelRenderer uiRoundedPanelRenderer = (UIRoundedPanelRenderer)target;

        //SerializedProperty color_prop = serializedObject.FindProperty("color");
        SerializedProperty edgeThickness_prop = serializedObject.FindProperty("edgeThickness");
        SerializedProperty numCornerVerts_prop = serializedObject.FindProperty("numCornerVerts");

        SerializedProperty upperLeftCorner_prop = serializedObject.FindProperty("upperLeftCorner");
        SerializedProperty upperRightCorner_prop = serializedObject.FindProperty("upperRightCorner");
        SerializedProperty lowerLeftCorner_prop = serializedObject.FindProperty("lowerLeftCorner");
        SerializedProperty lowerRightCorner_prop = serializedObject.FindProperty("lowerRightCorner");

        //EditorGUILayout.PropertyField(color_prop);
        EditorGUILayout.ColorField("Color", uiRoundedPanelRenderer.color);
        EditorGUILayout.PropertyField(edgeThickness_prop);
        EditorGUILayout.PropertyField(numCornerVerts_prop, new GUIContent("No. Corner Vertices"));

        if (edgeThickness_prop.floatValue < 0) {
            edgeThickness_prop.floatValue = 0;
        }

        if (numCornerVerts_prop.intValue < 0) {
            numCornerVerts_prop.intValue = 0;
        }

        EditorGUILayout.Space(10);

        EditorGUILayout.PropertyField(upperLeftCorner_prop);
        EditorGUILayout.PropertyField(upperRightCorner_prop);
        EditorGUILayout.PropertyField(lowerLeftCorner_prop);
        EditorGUILayout.PropertyField(lowerRightCorner_prop);

        serializedObject.ApplyModifiedProperties();
    }
}