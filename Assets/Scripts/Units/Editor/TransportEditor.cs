using UnityEditor;
using UnityEngine;

namespace Strategia.Editor {
    [CustomEditor(typeof(Transport))]
    public class TransportEditor : UnitEditor {

        public bool showArmies = false;
        SerializedProperty armies;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            SerializedObject so = new SerializedObject(target);
            Transport transport = (Transport)target;

            EditorGUILayout.Space(10);
            armies = so.FindProperty("armiesOnTransport");
            EditorGUILayout.PropertyField(armies, new GUIContent("Armies On Transport"));
            transport.isTransportFull = EditorGUILayout.Toggle("Is Transport Full", transport.isTransportFull);

            so.ApplyModifiedProperties();
        }
    }
}
