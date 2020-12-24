using UnityEditor;

namespace Strategia.Editor {
    [CustomEditor(typeof(Destroyer))]
    public class DestroyerEditor : UnitEditor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            SerializedObject so = new SerializedObject(target);
            Destroyer destroyer = (Destroyer)target;

            so.ApplyModifiedProperties();
        }
    }
}
