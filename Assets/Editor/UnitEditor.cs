using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Unit))]
public class UnitEditor : Editor {

    bool showGridData = false;

    public override void OnInspectorGUI() {
        Unit unit = (Unit)target;

        unit.gridScript = (Strategia.Grid)EditorGUILayout.ObjectField("Grid", unit.gridScript, typeof(Strategia.Grid), true);
        if (unit.gridScript != null) {
            unit.grid = unit.gridScript.grid;
        }
        if (unit.grid != null) {
            showGridData = EditorGUILayout.Foldout(showGridData, "Grid Data");
            if (showGridData) {
                foreach (var tile in unit.grid) {
                    tile.tileType = (TileType)EditorGUILayout.EnumPopup("Tile Type", tile.tileType);
                }
            }
        }
        unit.moveType = (UnitMoveType)EditorGUILayout.EnumPopup("Move Type", unit.moveType);
        unit.moveDistance = EditorGUILayout.IntField("Move Distance", unit.moveDistance);
        if (unit.moveDistance < 0) {
            unit.moveDistance = 0;
        }
        unit.pos = EditorGUILayout.Vector2IntField("Position", unit.pos);
        if (unit.pos.x < 0) {
            unit.pos.x = 0;
        }
        if (unit.pos.y < 0) {
            unit.pos.y = 0;
        }
        unit.maxHealth = EditorGUILayout.IntField("Max Health", unit.maxHealth);
        if (unit.maxHealth < 0) {
            unit.maxHealth = 0;
        }
        unit.health = EditorGUILayout.IntSlider("Health", unit.health, 0, unit.maxHealth);
        unit.hasFuel = EditorGUILayout.Toggle("Has Fuel", unit.hasFuel);
        if (unit.hasFuel) {
            unit.maxFuel = EditorGUILayout.IntField("Max Fuel", unit.maxFuel);
            if (unit.maxFuel < 0) {
                unit.maxFuel = 0;
            }
            unit.fuel = EditorGUILayout.IntSlider("Fuel", unit.fuel, 0, unit.maxFuel);
        }
    }
}
