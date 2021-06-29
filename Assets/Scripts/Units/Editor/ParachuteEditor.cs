using UnityEditor;
using UnityEngine;

namespace Strategia.Editor {
    [CustomEditor(typeof(Parachute))]
    public class ParachuteEditor : UnitEditor {

        public bool showFuel = false;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            serializedObject.Update();
            Parachute parachute = (Parachute)target;

            SerializedProperty unitPrefab_prop = serializedObject.FindProperty("unitPrefab");

            /*EditorGUILayout.Space(10);
            showFuel = EditorGUILayout.BeginFoldoutHeaderGroup(showFuel, "Fuel");
            if (showFuel) {
                parachute.fuel = EditorGUILayout.IntSlider("Fuel", parachute.fuel, 0, parachute.maxFuel);
                parachute.fuelPerMove = EditorGUILayout.IntSlider("Fuel Per Move", parachute.fuel, 0, parachute.maxFuel);
                EditorGUILayout.IntField(parachute.maxFuel);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();*/

            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(unitPrefab_prop);
            if (GUILayout.Button("Deploy Tank") && EditorApplication.isPlaying) {
                parachute.DeployArmy();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
