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
            UnitInfo unitInfo = unit.unitInfo;

            SerializedProperty unitInfo_prop = serializedObject.FindProperty("unitInfo");
            SerializedProperty turnStage_prop = serializedObject.FindProperty("unitTurnStage");
            SerializedProperty sleepEffectPrefab_prop = serializedObject.FindProperty("sleepEffectPrefab");
            SerializedProperty damageIndicatorPrefab_prop = serializedObject.FindProperty("damageIndicatorPrefab");

            SerializedProperty moves_prop = serializedObject.FindProperty("moves");
            SerializedProperty health_prop = serializedObject.FindProperty("health");
            SerializedProperty pos_prop = serializedObject.FindProperty("pos");

            SerializedProperty unitMoveUI_prop = serializedObject.FindProperty("unitMoveUI");
            SerializedProperty unitAppearanceManager_prop = serializedObject.FindProperty("unitAppearanceManager");

            EditorGUILayout.PropertyField(unitInfo_prop);
            EditorGUILayout.PropertyField(turnStage_prop);
            EditorGUILayout.PropertyField(sleepEffectPrefab_prop);
            EditorGUILayout.PropertyField(damageIndicatorPrefab_prop);

            EditorGUILayout.PropertyField(unitAppearanceManager_prop);
            EditorGUILayout.PropertyField(unitMoveUI_prop);

            EditorGUILayout.Space(10);
            showMovement = EditorGUILayout.BeginFoldoutHeaderGroup(showMovement, "Movement");
            if (showMovement && unitInfo != null) {
                moves_prop.intValue = EditorGUILayout.IntSlider("Moves Left", moves_prop.intValue, 0, unitInfo.maxMoves);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space(10);
            showHealth = EditorGUILayout.BeginFoldoutHeaderGroup(showHealth, "Health");
            if (showHealth && unitInfo != null) {
                health_prop.intValue = EditorGUILayout.IntSlider("Health", health_prop.intValue, 0, unitInfo.maxHealth);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (GameManager.Instance != null) {
                EditorGUILayout.Space(10);
                showPosition = EditorGUILayout.BeginFoldoutHeaderGroup(showPosition, "Position");
                if (showPosition) {
                    pos_prop.vector2IntValue = new Vector2Int(EditorGUILayout.IntSlider("X Position", pos_prop.vector2IntValue.x, 1, GameManager.Instance.tileGrid.width - 2), pos_prop.vector2IntValue.y);
                    pos_prop.vector2IntValue = new Vector2Int(pos_prop.vector2IntValue.x, EditorGUILayout.IntSlider("Y Position", pos_prop.vector2IntValue.y, 1, GameManager.Instance.tileGrid.height - 2));
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

