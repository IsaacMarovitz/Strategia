using UnityEditor;

namespace Strategia.Editor {
    [CustomEditor(typeof(Army))]
    public class ArmyEditor : UnitEditor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            SerializedObject so = new SerializedObject(target);
            Army army = (Army)target;

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Reduced Movement", EditorStyles.boldLabel);
            army.reducedMoveDistance = EditorGUILayout.IntSlider("Reduced Move Distance", army.reducedMoveDistance, 0, army.maxMoves);
            army.isMoveDistanceReduced = EditorGUILayout.Toggle("Is Move Distance Reduced", army.isMoveDistanceReduced);

            Others();

            so.ApplyModifiedProperties();
        }
    }
}
