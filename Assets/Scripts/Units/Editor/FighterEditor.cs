using UnityEditor;

namespace Strategia.Editor {
    [CustomEditor(typeof(Fighter))]
    public class FighterEditor : UnitEditor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            SerializedObject so = new SerializedObject(target);
            Fighter fighter = (Fighter)target;

            so.ApplyModifiedProperties();
        }
    }
}
