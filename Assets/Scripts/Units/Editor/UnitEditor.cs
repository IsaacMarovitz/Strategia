using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

namespace Strategia.Editor {
    [CustomEditor(typeof(Unit), true)]
    public class UnitEditor : UnityEditor.Editor {

        public Unit unit;
        public bool showMovement = false;
        public bool showHealth = false;
        public bool showPosition = false;

        public override void OnInspectorGUI() {

            SerializedObject so = new SerializedObject(target);
            unit = (Unit)target;

            unit.turnStage = (TurnStage)EditorGUILayout.EnumPopup("Turn Stage", unit.turnStage);
            unit.gridScript = (Strategia.TileGrid)EditorGUILayout.ObjectField("Grid", unit.gridScript, typeof(Strategia.TileGrid), true);
            unit.sleepEffect = (VisualEffect)EditorGUILayout.ObjectField("Sleep Effect", unit.sleepEffect, typeof(VisualEffect), true);
            unit.mainMesh = (GameObject)EditorGUILayout.ObjectField("Main Mesh", unit.mainMesh, typeof(GameObject), true);

            EditorGUILayout.Space(10);
            showMovement = EditorGUILayout.BeginFoldoutHeaderGroup(showMovement, "Movement");
            if (showMovement) {
                unit.moves = EditorGUILayout.IntSlider("Moves Left", unit.moves, 0, unit.maxMoves);
                unit.maxMoves = EditorGUILayout.IntField("Move Distance", unit.maxMoves);
                unit.yOffset = EditorGUILayout.FloatField("Y Offset", unit.yOffset);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (unit.maxMoves < 0) {
                unit.maxMoves = 0;
            }

            EditorGUILayout.Space(10);
            showHealth = EditorGUILayout.BeginFoldoutHeaderGroup(showHealth, "Health");
            if (showHealth) {
                unit.health = EditorGUILayout.IntSlider("Health", unit.health, 0, unit.maxHealth);
                unit.maxHealth = EditorGUILayout.IntField("Max Health", unit.maxHealth);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (unit.maxHealth < 0) {
                unit.maxHealth = 0;
            }

            if (unit.gridScript != null) {
                EditorGUILayout.Space(10);
                showPosition = EditorGUILayout.BeginFoldoutHeaderGroup(showPosition, "Position");
                if (showPosition) {
                    unit.pos.x = EditorGUILayout.IntSlider("X Position", unit.pos.x, 1, unit.gridScript.width - 2);
                    unit.pos.y = EditorGUILayout.IntSlider("Y Position", unit.pos.y, 1, unit.gridScript.height - 2);
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
            if (unit.pos.x < 0) {
                unit.pos.x = 0;
            }
            if (unit.pos.y < 0) {
                unit.pos.y = 0;
            }

            so.ApplyModifiedProperties();
        }
    }
}

