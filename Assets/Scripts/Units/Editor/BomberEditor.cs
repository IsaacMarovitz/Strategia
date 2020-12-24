using UnityEditor;
using UnityEngine;

namespace Strategia.Editor {
    [CustomEditor(typeof(Bomber))]
    public class BomberEditor : UnitEditor {

        public bool showFuel = false;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            SerializedObject so = new SerializedObject(target);
            Bomber bomber = (Bomber)target;

            EditorGUILayout.Space(10);
            showFuel = EditorGUILayout.BeginFoldoutHeaderGroup(showFuel, "Fuel");
            if (showFuel) {
                bomber.fuel = EditorGUILayout.IntSlider("Fuel", bomber.fuel, 0, bomber.maxFuel);
                bomber.fuelPerMove = EditorGUILayout.IntSlider("Fuel Per Move", bomber.fuelPerMove, 0, bomber.maxFuel);
                bomber.maxFuel = EditorGUILayout.IntField("Max Fuel", bomber.maxFuel);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space(10);
            if (GUILayout.Button("Detonate") && EditorApplication.isPlaying) {
                bomber.Detonate();
            }

            so.ApplyModifiedProperties();
        }
    }
}
