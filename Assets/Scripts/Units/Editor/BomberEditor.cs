using UnityEditor;

namespace Strategia.Editor {
    [CustomEditor(typeof(Bomber))]
    public class BomberEditor : UnitEditor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            SerializedObject so = new SerializedObject(target);
            Bomber bomber = (Bomber)target;

            Others();
            so.ApplyModifiedProperties();
        }
    }
}
