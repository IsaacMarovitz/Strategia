using UnityEditor;

namespace Strategia.Editor {
    [CustomEditor(typeof(Battleship))]
    public class BattleshipEditor : UnitEditor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            SerializedObject so = new SerializedObject(target);
            Battleship battleship = (Battleship)target;

            so.ApplyModifiedProperties();
        }
    }
}
