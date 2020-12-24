using UnityEditor;

namespace Strategia.Editor {
    [CustomEditor(typeof(Submarine))]
    public class SubmarineEditor : UnitEditor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            SerializedObject so = new SerializedObject(target);
            Submarine submarine = (Submarine)target;

            so.ApplyModifiedProperties();
        }
    }
}
