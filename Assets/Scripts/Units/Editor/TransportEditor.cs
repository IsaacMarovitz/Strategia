using UnityEngine;
using UnityEditor;

namespace Strategia.Editor {
    [CustomEditor(typeof(Transport))]
    public class TransportEditor : UnitEditor {

        SerializedProperty tanks;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            serializedObject.Update();
            Transport transport = (Transport)target;

            SerializedProperty tanks = serializedObject.FindProperty("_unitsOnTransport");

            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(tanks, new GUIContent("Tanks On Transport"));
            EditorGUILayout.Toggle("Is Transport Full", transport.isTransportFull);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
