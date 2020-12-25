using UnityEditor;

namespace Strategia.Editor {
    [CustomEditor(typeof(Fighter))]
    public class FighterEditor : UnitEditor {

        public bool showFuel = false;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            serializedObject.Update();
            Fighter fighter = (Fighter)target;

            SerializedProperty fuel_prop = serializedObject.FindProperty("fuel");
            SerializedProperty fuelPerMove_prop = serializedObject.FindProperty("fuelPerMove");
            SerializedProperty maxFuel_prop = serializedObject.FindProperty("maxFuel");
            SerializedProperty isOnCarrier_prop = serializedObject.FindProperty("isOnCarrier");

            EditorGUILayout.Space(10);
            showFuel = EditorGUILayout.BeginFoldoutHeaderGroup(showFuel, "Fuel");
            if (showFuel) {
                fuel_prop.intValue = EditorGUILayout.IntSlider("Fuel", fuel_prop.intValue, 0, maxFuel_prop.intValue);
                fuelPerMove_prop.intValue = EditorGUILayout.IntSlider("Fuel Per Move", fuelPerMove_prop.intValue, 0, maxFuel_prop.intValue);
                EditorGUILayout.PropertyField(maxFuel_prop);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(isOnCarrier_prop);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
