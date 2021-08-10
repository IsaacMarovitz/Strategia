using UnityEngine;
using UnityEditor;

namespace Strategia.Editor {
    [CustomEditor(typeof(Carrier))]
    public class CarrierEditor : UnitEditor {

        SerializedProperty fighters;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            serializedObject.Update();
            Carrier carrier = (Carrier)target;

            SerializedProperty fighters = serializedObject.FindProperty("_unitsOnTransport");

            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(fighters, new GUIContent("Fighters On Carrier"));
            EditorGUILayout.Toggle("Is Carrier Full", carrier.isTransportFull);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
