using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UnitMoveUI))]
public class UnitMoveUIEditor : Editor {

    public bool showBooleans = false;
    public bool showTileSelectorMats = false;
    public bool showLineRendererMats = false;

    public override void OnInspectorGUI() {
        serializedObject.Update();
        UnitMoveUI unitMoveUI = (UnitMoveUI)target;

        SerializedProperty unit_prop = serializedObject.FindProperty("unit");
        SerializedProperty lineRenderer_prop = serializedObject.FindProperty("lineRenderer");
        SerializedProperty tileSelector_prop = serializedObject.FindProperty("tileSelector");
        SerializedProperty tileSelectorMeshRenderer_prop = serializedObject.FindProperty("tileSelectorMeshRenderer");
        SerializedProperty numberOfTurns_prop = serializedObject.FindProperty("numberOfTurns");
        SerializedProperty canvas_prop = serializedObject.FindProperty("canvas");
        SerializedProperty path_prop = serializedObject.FindProperty("path");

        SerializedProperty isSelected_prop = serializedObject.FindProperty("isSelected");
        SerializedProperty isMoving_prop = serializedObject.FindProperty("isMoving");

        SerializedProperty hiddenTRMaterial_prop = serializedObject.FindProperty("hiddenTRMaterial");
        SerializedProperty moveTRMaterial_prop = serializedObject.FindProperty("moveTRMaterial");
        SerializedProperty blockedTRMaterial_prop = serializedObject.FindProperty("blockedTRMaterial");
        SerializedProperty setPathTRMaterial_prop = serializedObject.FindProperty("setPathTRMaterial");

        SerializedProperty hiddenLRMaterial_prop = serializedObject.FindProperty("hiddenLRMaterial");
        SerializedProperty moveLRMaterial_prop = serializedObject.FindProperty("moveLRMaterial");
        SerializedProperty setPathLRMaterial_prop = serializedObject.FindProperty("setPathLRMaterial");

        EditorGUILayout.PropertyField(unit_prop);
        EditorGUILayout.PropertyField(lineRenderer_prop);
        EditorGUILayout.PropertyField(tileSelector_prop);
        EditorGUILayout.PropertyField(tileSelectorMeshRenderer_prop);
        EditorGUILayout.PropertyField(numberOfTurns_prop);
        EditorGUILayout.PropertyField(canvas_prop);
        EditorGUILayout.PropertyField(path_prop);

        EditorGUILayout.Space(10);
        showBooleans = EditorGUILayout.BeginFoldoutHeaderGroup(showBooleans, "Booleans");
        if (showBooleans) {
            EditorGUILayout.PropertyField(isSelected_prop);
            EditorGUILayout.PropertyField(isMoving_prop);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(10);
        showTileSelectorMats = EditorGUILayout.BeginFoldoutHeaderGroup(showTileSelectorMats, "Tile Selector Materials");
        if (showTileSelectorMats) {
            EditorGUILayout.PropertyField(hiddenTRMaterial_prop, new GUIContent("Hidden Material"));
            EditorGUILayout.PropertyField(moveTRMaterial_prop, new GUIContent("Move Material"));
            EditorGUILayout.PropertyField(blockedTRMaterial_prop, new GUIContent("Blocked Material"));
            EditorGUILayout.PropertyField(setPathTRMaterial_prop, new GUIContent("Set Path Material"));
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(10);
        showLineRendererMats = EditorGUILayout.BeginFoldoutHeaderGroup(showLineRendererMats, "Line Renderer Materials");
        if (showLineRendererMats) {
            EditorGUILayout.PropertyField(hiddenLRMaterial_prop, new GUIContent("Hidden Material"));
            EditorGUILayout.PropertyField(moveLRMaterial_prop, new GUIContent("Move Material"));
            EditorGUILayout.PropertyField(setPathLRMaterial_prop, new GUIContent("Set Path Material"));
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        serializedObject.ApplyModifiedProperties();
    }
}