using UnityEditor;

namespace Strategia.Editor {
    [CustomEditor(typeof(Tank))]
    public class TankEditor : UnitEditor {

        public bool showReducedMovement = false;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            serializedObject.Update();
            Tank tank = (Tank)target;

            SerializedProperty reducedMoveDistance_prop = serializedObject.FindProperty("reducedMoveDistance");
            SerializedProperty isMoveDistanceReduced_prop = serializedObject.FindProperty("isMoveDistanceReduced");
            SerializedProperty isOnTransport_prop = serializedObject.FindProperty("isOnTransport");

            EditorGUILayout.Space(10);
            showReducedMovement = EditorGUILayout.BeginFoldoutHeaderGroup(showReducedMovement, "Reduced Movement");
            if (showReducedMovement) {
                reducedMoveDistance_prop.intValue = EditorGUILayout.IntSlider("Reduced Move Distance", reducedMoveDistance_prop.intValue, 0, tank.maxMoves);
                EditorGUILayout.PropertyField(isMoveDistanceReduced_prop);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.PropertyField(isOnTransport_prop);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
