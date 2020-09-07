using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Strategia.Grid))]
public class GridEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        Strategia.Grid grid = (Strategia.Grid)target;
        if (GUILayout.Button("Create Grid")) {
            grid.CreateGrid();
        }
        if (GUILayout.Button("Delete Grid")) {
            grid.DeleteGrid();
        }
    }
}
