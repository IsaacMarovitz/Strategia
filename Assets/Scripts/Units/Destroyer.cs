using UnityEngine;

public class Destroyer : Unit {
    public override void Start() {
        base.Start();
        unitType = UnitType.Destroyer;
        // Set damage percentages in order of Army, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, and Battleship
        damagePercentages = new float[9] { 0.3f, 0f, 0.25f, 0f, 1f, 0.34f, 1f, 0.34f, 0.1f };
    }
}