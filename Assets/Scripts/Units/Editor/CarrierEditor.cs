using UnityEditor;

namespace Strategia.Editor {
    [CustomEditor(typeof(Carrier))]
    public class CarrierEditor : UnitEditor {

        SerializedProperty fighters;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            serializedObject.Update();
            Carrier carrier = (Carrier)target;

            EditorGUILayout.Space(10);
            fighters = serializedObject.FindProperty("fightersOnCarrier");
            EditorGUILayout.PropertyField(fighters);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
