using UnityEditor;

namespace Strategia.Editor {
    [CustomEditor(typeof(Transport))]
    public class TransportEditor : UnitEditor {

        SerializedProperty tanks;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            serializedObject.Update();
            Transport transport = (Transport)target;

            EditorGUILayout.Space(10);
            tanks = serializedObject.FindProperty("tanksOnTransport");
            EditorGUILayout.PropertyField(tanks);
            transport.isTransportFull = EditorGUILayout.Toggle("Is Transport Full", transport.isTransportFull);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
