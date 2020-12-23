using UnityEditor;

namespace Strategia.Editor {
    [CustomEditor(typeof(Parachute))]
    public class ParachuteEditor : UnitEditor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            SerializedObject so = new SerializedObject(target);
            Parachute parachute = (Parachute)target;

            Others();
            so.ApplyModifiedProperties();
        }
    }
}
