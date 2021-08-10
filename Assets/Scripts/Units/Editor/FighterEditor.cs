using UnityEditor;

namespace Strategia.Editor {
    [CustomEditor(typeof(Fighter))]
    public class FighterEditor : UnitEditor {

        public bool showFuel = false;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            serializedObject.Update();
            Fighter fighter = (Fighter)target;

            SerializedProperty fuel_prop = serializedObject.FindProperty("_fuel");
            SerializedProperty maxFuel_prop = serializedObject.FindProperty("_maxFuel");
            SerializedProperty fuelPerMove_prop = serializedObject.FindProperty("_fuelPerMove");

            SerializedProperty carrier_prop = serializedObject.FindProperty("carrier");

            EditorGUILayout.Space(10);
            showFuel = EditorGUILayout.BeginFoldoutHeaderGroup(showFuel, "Fuel");
            if (showFuel) {
                fuel_prop.intValue = EditorGUILayout.IntSlider("Fuel", fuel_prop.intValue, 0, maxFuel_prop.intValue);
                fuelPerMove_prop.intValue = EditorGUILayout.IntSlider("Fuel Per Move", fuelPerMove_prop.intValue, 0, maxFuel_prop.intValue);
                EditorGUILayout.PropertyField(maxFuel_prop);
                if (maxFuel_prop.intValue < 0) {
                    maxFuel_prop.intValue = 0;
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(carrier_prop);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
