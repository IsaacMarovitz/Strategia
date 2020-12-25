using UnityEditor;
using UnityEngine;

namespace Strategia.Editor {
    [CustomEditor(typeof(Unit), true), CanEditMultipleObjects]
    public class UnitEditor : UnityEditor.Editor {

        public Unit unit;
        public bool showMovement = false;
        public bool showHealth = false;
        public bool showPosition = false;

        public override void OnInspectorGUI() {
            serializedObject.Update();
            Unit unit = (Unit)target;

            SerializedProperty turnStage_prop = serializedObject.FindProperty("turnStage");
            SerializedProperty gridScript_prop = serializedObject.FindProperty("gridScript");
            SerializedProperty sleepEffect_prop = serializedObject.FindProperty("sleepEffect");
            SerializedProperty mainMesh_prop = serializedObject.FindProperty("mainMesh");

            SerializedProperty moves_prop = serializedObject.FindProperty("moves");
            SerializedProperty maxMoves_prop = serializedObject.FindProperty("maxMoves");
            SerializedProperty yOffset_prop = serializedObject.FindProperty("yOffset");

            SerializedProperty health_prop = serializedObject.FindProperty("health");
            SerializedProperty maxHealth_prop = serializedObject.FindProperty("maxHealth");

            SerializedProperty pos_prop = serializedObject.FindProperty("pos");

            EditorGUILayout.PropertyField(turnStage_prop);
            EditorGUILayout.PropertyField(gridScript_prop);
            EditorGUILayout.PropertyField(sleepEffect_prop);
            EditorGUILayout.PropertyField(mainMesh_prop);

            EditorGUILayout.Space(10);
            showMovement = EditorGUILayout.BeginFoldoutHeaderGroup(showMovement, "Movement");
            if (showMovement) {
                moves_prop.intValue = EditorGUILayout.IntSlider("Moves Left", moves_prop.intValue, 0, maxMoves_prop.intValue);
                EditorGUILayout.PropertyField(maxMoves_prop);
                EditorGUILayout.PropertyField(yOffset_prop);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (maxMoves_prop.intValue < 0) {
                maxMoves_prop.intValue = 0;
            }

            EditorGUILayout.Space(10);
            showHealth = EditorGUILayout.BeginFoldoutHeaderGroup(showHealth, "Health");
            if (showHealth) {
                health_prop.intValue = EditorGUILayout.IntSlider("Health", health_prop.intValue, 0, maxHealth_prop.intValue);
                EditorGUILayout.PropertyField(maxHealth_prop);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (maxHealth_prop.intValue < 0) {
                maxHealth_prop.intValue = 0;
            }

            if (unit.gridScript != null) {
                EditorGUILayout.Space(10);
                showPosition = EditorGUILayout.BeginFoldoutHeaderGroup(showPosition, "Position");
                if (showPosition) {
                    pos_prop.vector2IntValue = new Vector2Int(EditorGUILayout.IntSlider("X Position", pos_prop.vector2IntValue.x, 1, unit.gridScript.width - 2), pos_prop.vector2IntValue.y);
                    pos_prop.vector2IntValue = new Vector2Int(pos_prop.vector2IntValue.x, EditorGUILayout.IntSlider("Y Position", pos_prop.vector2IntValue.y, 1, unit.gridScript.height - 2));
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
            if (pos_prop.vector2IntValue.x < 0) {
                pos_prop.vector2IntValue = new Vector2Int(0, pos_prop.vector2IntValue.y);
            }
            if (pos_prop.vector2IntValue.y < 0) {
                pos_prop.vector2IntValue = new Vector2Int(pos_prop.vector2IntValue.x, 0);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

