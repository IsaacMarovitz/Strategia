using UnityEditor;
using UnityEngine;

namespace Strategia.Editor {
    [CustomEditor(typeof(Bomber))]
    public class BomberEditor : UnitEditor {

        public bool showFuel = false;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            serializedObject.Update();
            Bomber bomber = (Bomber)target;

            /*EditorGUILayout.Space(10);
            showFuel = EditorGUILayout.BeginFoldoutHeaderGroup(showFuel, "Fuel");
            if (showFuel) {
                bomber.fuel = EditorGUILayout.IntSlider("Fuel", bomber.fuel, 0, bomber.maxFuel);
                bomber.fuelPerMove = EditorGUILayout.IntSlider("Fuel Per Move", bomber.fuel, 0, bomber.maxFuel);
                EditorGUILayout.IntField(bomber.maxFuel);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();*/

            EditorGUILayout.Space(10);
            if (GUILayout.Button("Detonate") && EditorApplication.isPlaying) {
                bomber.Detonate();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
