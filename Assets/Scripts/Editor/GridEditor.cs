using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Strategia.TileGrid))]
public class TileGridEditor : Editor {

    public Vector2Int pos;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        Strategia.TileGrid grid = (Strategia.TileGrid)target;

        if (GUILayout.Button("Create Grid")) {
            grid.CreateGrid();
        }
        if (GUILayout.Button("Delete Grid")) {
            grid.DeleteGrid();
        }

        EditorGUILayout.Space(10);
        pos = EditorGUILayout.Vector2IntField("Tile to Find", pos);
        if (GUILayout.Button("Find Tile")) {
            grid.FindTile(pos);
        }

    }
}
