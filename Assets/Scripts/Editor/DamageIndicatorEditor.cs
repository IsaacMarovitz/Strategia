using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DamageIndicator))]
public class DamageIndicatorEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        DamageIndicator damageIndicator = (DamageIndicator)target;

        if (GUILayout.Button("Test Damage")) {
            damageIndicator.IndicateDamage(999);
        }
    }
}