using UnityEditor;

namespace Strategia.Editor {
    [CustomEditor(typeof(Destroyer))]
    public class DestroyerEditor : UnitEditor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            serializedObject.Update();
            Destroyer destroyer = (Destroyer)target;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
