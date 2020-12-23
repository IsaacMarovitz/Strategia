using UnityEditor;

namespace Strategia.Editor {
    [CustomEditor(typeof(Carrier))]
    public class CarrierEditor : UnitEditor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            SerializedObject so = new SerializedObject(target);
            Carrier carrier = (Carrier)target;

            Others();
            so.ApplyModifiedProperties();
        }
    }
}
