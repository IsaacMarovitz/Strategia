using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(City))]
public class CityEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        City city = (City)target;
        if (GUILayout.Button("Show Nearby Tiles")) {
            city.ShowNearbyTiles();
        }
    }
}