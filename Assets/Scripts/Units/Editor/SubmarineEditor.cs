using UnityEditor;

namespace Strategia.Editor {
    [CustomEditor(typeof(Submarine))]
    public class SubmarineEditor : UnitEditor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            serializedObject.Update();
            Submarine submarine = (Submarine)target;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
