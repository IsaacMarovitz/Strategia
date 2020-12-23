using UnityEngine;

public class Submarine : Unit {
    public override void Start() {
        unitType = UnitType.Submarine;
        // Set damage percentages in order of Army, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, and Battleship
        damagePercentages = new float[9] { 0f, 0f, 0f, 0f, 1f, 0f, 0.34f, 0.8f, 0.8f };
    }
}