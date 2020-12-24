using UnityEditor;

namespace Strategia.Editor {
    [CustomEditor(typeof(Transport))]
    public class TransportEditor : UnitEditor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            SerializedObject so = new SerializedObject(target);
            Transport transport = (Transport)target;

            so.ApplyModifiedProperties();
        }
    }
}
