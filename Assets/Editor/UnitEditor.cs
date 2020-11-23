using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Unit))]
public class UnitEditor : Editor {

    /*bool showGridData = false;*/

    public override void OnInspectorGUI() {

        SerializedObject so = new SerializedObject(target);
        Unit unit = (Unit)target;

        unit.gridScript = (Strategia.TileGrid)EditorGUILayout.ObjectField("Grid", unit.gridScript, typeof(Strategia.TileGrid), true);

        /*if (unit.grid != null) {
            showGridData = EditorGUILayout.Foldout(showGridData, "Grid Data");
            if (showGridData) {
                foreach (var tile in unit.grid) {
                    string label = tile.index.x + ", " + tile.index.y;
                    tile.tileType = (TileType)EditorGUILayout.EnumPopup(label, tile.tileType);
                }
            }
        }*/
        unit.movesLeft = EditorGUILayout.IntSlider("Moves Left", unit.movesLeft, 0, unit.moveDistance);
        unit.moveType = (UnitMoveType)EditorGUILayout.EnumPopup("Move Type", unit.moveType);
        unit.moveDistance = EditorGUILayout.IntField("Move Distance", unit.moveDistance);
        unit.moveDistanceReductionFactor = EditorGUILayout.IntSlider("Move Distance Reduction Factor", unit.moveDistanceReductionFactor, 0, unit.moveDistance);
        unit.moveDistanceReduced = EditorGUILayout.Toggle("Move Distance Reduced", unit.moveDistanceReduced);
        if (unit.moveDistance < 0) {
            unit.moveDistance = 0;
        }
        if (unit.gridScript != null) {
            unit.pos.x = EditorGUILayout.IntSlider("X Position", unit.pos.x, 1, unit.gridScript.width - 2);
            unit.pos.y = EditorGUILayout.IntSlider("Y Position", unit.pos.y, 1, unit.gridScript.height - 2);
        }
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
        SerializedProperty meshArray = so.FindProperty("meshes");
        EditorGUILayout.PropertyField(meshArray, true);
        so.ApplyModifiedProperties();
        if (GUILayout.Button("Check Dirs")) {
            unit.CheckDirs();
        }
    }
}
