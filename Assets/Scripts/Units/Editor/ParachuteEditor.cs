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

            SerializedProperty fuel_prop = serializedObject.FindProperty("fuel");
            SerializedProperty fuelPerMove_prop = serializedObject.FindProperty("fuelPerMove");
            SerializedProperty maxFuel_prop = serializedObject.FindProperty("maxFuel");
            SerializedProperty unitPrefab_prop = serializedObject.FindProperty("unitPrefab");

            EditorGUILayout.Space(10);
            showFuel = EditorGUILayout.BeginFoldoutHeaderGroup(showFuel, "Fuel");
            if (showFuel) {
                fuel_prop.intValue = EditorGUILayout.IntSlider("Fuel", fuel_prop.intValue, 0, maxFuel_prop.intValue);
                fuelPerMove_prop.intValue = EditorGUILayout.IntSlider("Fuel Per Move", fuelPerMove_prop.intValue, 0, maxFuel_prop.intValue);
                EditorGUILayout.PropertyField(maxFuel_prop);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(unitPrefab_prop);
            if (GUILayout.Button("Deploy Tank") && EditorApplication.isPlaying) {
                parachute.DeployArmy();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
