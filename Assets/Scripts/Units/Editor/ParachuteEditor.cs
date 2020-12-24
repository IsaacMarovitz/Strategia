using UnityEditor;
using UnityEngine;

namespace Strategia.Editor {
    [CustomEditor(typeof(Parachute))]
    public class ParachuteEditor : UnitEditor {

        public bool showFuel = false;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            SerializedObject so = new SerializedObject(target);
            Parachute parachute = (Parachute)target;

            EditorGUILayout.Space(10);
            showFuel = EditorGUILayout.BeginFoldoutHeaderGroup(showFuel, "Fuel");
            if (showFuel) {
                parachute.fuel = EditorGUILayout.IntSlider("Fuel", parachute.fuel, 0, parachute.maxFuel);
                parachute.fuelPerMove = EditorGUILayout.IntSlider("Fuel Per Move", parachute.fuelPerMove, 0, parachute.maxFuel);
                parachute.maxFuel = EditorGUILayout.IntField("Max Fuel", parachute.maxFuel);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space(10);
            if (GUILayout.Button("Deploy Army") && EditorApplication.isPlaying) {
                parachute.DeployArmy();
            }

            so.ApplyModifiedProperties();
        }
    }
}
