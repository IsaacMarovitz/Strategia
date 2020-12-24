using UnityEditor;

namespace Strategia.Editor {
    [CustomEditor(typeof(Army))]
    public class ArmyEditor : UnitEditor {

        public bool showReducedMovement = false;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            SerializedObject so = new SerializedObject(target);
            Army army = (Army)target;

            EditorGUILayout.Space(10);
            showReducedMovement = EditorGUILayout.BeginFoldoutHeaderGroup(showReducedMovement, "Reduced Movement");
            if (showReducedMovement) {
                army.reducedMoveDistance = EditorGUILayout.IntSlider("Reduced Move Distance", army.reducedMoveDistance, 0, army.maxMoves);
                army.isMoveDistanceReduced = EditorGUILayout.Toggle("Is Move Distance Reduced", army.isMoveDistanceReduced);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            so.ApplyModifiedProperties();
        }
    }
}
