using UnityEditor;

namespace Strategia.Editor {
    [CustomEditor(typeof(Fighter))]
    public class FighterEditor : UnitEditor {

        public bool showFuel = false;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            serializedObject.Update();
            Fighter fighter = (Fighter)target;

            SerializedProperty isOnCarrier_prop = serializedObject.FindProperty("isOnCarrier");

            /*EditorGUILayout.Space(10);
            showFuel = EditorGUILayout.BeginFoldoutHeaderGroup(showFuel, "Fuel");
            if (showFuel) {
                fighter.fuel = EditorGUILayout.IntSlider("Fuel", fighter.fuel, 0, fighter.maxFuel);
                fighter.fuelPerMove = EditorGUILayout.IntSlider("Fuel Per Move", fighter.fuel, 0, fighter.maxFuel);
                EditorGUILayout.IntField("Max Fuel", fighter.maxFuel);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();*/

            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(isOnCarrier_prop);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
