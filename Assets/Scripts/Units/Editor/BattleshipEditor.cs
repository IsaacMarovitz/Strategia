using UnityEditor;

namespace Strategia.Editor {
    [CustomEditor(typeof(Battleship))]
    public class BattleshipEditor : UnitEditor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            serializedObject.Update();
            Battleship battleship = (Battleship)target;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
