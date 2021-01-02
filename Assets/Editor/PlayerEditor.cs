using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor {

    Vector2Int unitPos = Vector2Int.zero;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        Player player = (Player)target;

        if (GUILayout.Button("Reveal All Tiles")) {
            player.RevealAllTiles();
        }
        unitPos = EditorGUILayout.Vector2IntField("Unit Pos", unitPos);
        if (GUILayout.Button("Spawn Army")) {
            player.SpawnArmy(unitPos);
        }
    }
}