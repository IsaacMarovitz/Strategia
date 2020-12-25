using UnityEditor;

namespace Strategia.Editor {
    [CustomEditor(typeof(Transport))]
    public class TransportEditor : UnitEditor {

        SerializedProperty armies;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            serializedObject.Update();
            Transport transport = (Transport)target;

            EditorGUILayout.Space(10);
            armies = serializedObject.FindProperty("armiesOnTransport");
            EditorGUILayout.PropertyField(armies);
            transport.isTransportFull = EditorGUILayout.Toggle("Is Transport Full", transport.isTransportFull);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
