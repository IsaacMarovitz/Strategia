using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

namespace Strategia.Editor {
    [CustomEditor(typeof(Unit), true)]
    public class UnitEditor : UnityEditor.Editor {

        public Unit unit;

        public override void OnInspectorGUI() {

            SerializedObject so = new SerializedObject(target);
            unit = (Unit)target;

            unit.turnStage = (TurnStage)EditorGUILayout.EnumPopup("Turn Stage", unit.turnStage);

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Movement", EditorStyles.boldLabel);
            unit.moves = EditorGUILayout.IntSlider("Moves Left", unit.moves, 0, unit.maxMoves);
            unit.maxMoves = EditorGUILayout.IntField("Move Distance", unit.maxMoves);

            if (unit.maxMoves < 0) {
                unit.maxMoves = 0;
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Health", EditorStyles.boldLabel);
            unit.health = EditorGUILayout.IntSlider("Health", unit.health, 0, unit.maxHealth);
            unit.maxHealth = EditorGUILayout.IntField("Max Health", unit.maxHealth);
            if (unit.maxHealth < 0) {
                unit.maxHealth = 0;
            }

            if (unit.gridScript != null) {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Position", EditorStyles.boldLabel);
                unit.pos.x = EditorGUILayout.IntSlider("X Position", unit.pos.x, 1, unit.gridScript.width - 2);
                unit.pos.y = EditorGUILayout.IntSlider("Y Position", unit.pos.y, 1, unit.gridScript.height - 2);
            }
            if (unit.pos.x < 0) {
                unit.pos.x = 0;
            }
            if (unit.pos.y < 0) {
                unit.pos.y = 0;
            }

            so.ApplyModifiedProperties();
        }

        public void Others() {
            EditorGUILayout.Space(10);
            unit.gridScript = (Strategia.TileGrid)EditorGUILayout.ObjectField("Grid", unit.gridScript, typeof(Strategia.TileGrid), true);
            unit.sleepEffect = (VisualEffect)EditorGUILayout.ObjectField("Sleep Effect", unit.sleepEffect, typeof(VisualEffect), true);
            unit.mainMesh = (GameObject)EditorGUILayout.ObjectField("Main Mesh", unit.mainMesh, typeof(GameObject), true);
        }
    }
}

