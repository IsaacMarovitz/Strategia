using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Strategia.TileGrid))]
public class TileGridEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        Strategia.TileGrid grid = (Strategia.TileGrid)target;
        if (GUILayout.Button("Create Grid")) {
            grid.CreateGrid();
        }
        if (GUILayout.Button("Delete Grid")) {
            grid.DeleteGrid();
        }
    }
}
