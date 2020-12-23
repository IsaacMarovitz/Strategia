using UnityEngine;

public class Battleship : Unit {
    public override void Start() {
        unitType = UnitType.Battleship;
        // Set damage percentages in order of Army, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, and Battleship
        damagePercentages = new float[9] { 1f, 0f, 0.5f, 0f, 1f, 1f, 0.2f, 0.8f, 0.25f };
    }
}