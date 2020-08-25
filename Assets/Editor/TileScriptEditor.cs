using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(TileScript)), CanEditMultipleObjects]
public class TileScriptEdior : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        TileScript[] tileScripts = Array.ConvertAll(targets, item => (TileScript) item);

        if (GUILayout.Button("Update Tile")) {
            foreach (var tileScript in tileScripts) {
               tileScript.UpdateTile(); 
            }
        }
    }
}