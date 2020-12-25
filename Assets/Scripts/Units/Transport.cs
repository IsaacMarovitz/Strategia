using UnityEngine;
using System.Collections.Generic;

// 6 armies per transport
public class Transport : Unit {

    public List<Army> armiesOnTransport;
    public bool isTransportFull;

    public override void Start() {
        base.Start();
        unitType = UnitType.Transport;
        // Set damage percentages in order of Army, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, and Battleship
        damagePercentages = new float[9] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
    }

    public override void Update() {
        base.Update();
        if (armiesOnTransport.Count >= 6) {
            isTransportFull = true;
        } else {
            isTransportFull = false;
        }
    }

    public override void CheckDirs() {
        base.CheckDirs();

        Tile[] tiles = GridUtilities.DiagonalCheck(gridScript.grid, gridScript.width, gridScript.height, pos);
        for (int i = 0; i < tiles.Length; i++) {
            if (tiles[i] == null) {
                moveDirs[i] = TileMoveStatus.Blocked;
            } else if (tiles[i].tileType != TileType.Sea) {
                moveDirs[i] = TileMoveStatus.Blocked;
            } else {
                moveDirs[i] = TileMoveStatus.Move;
            }
        }

        if (turnStage == TurnStage.Started) {
            for (int i = 0; i < tiles.Length; i++) {
                if (tiles[i].unitOnTile != null) {
                    moveDirs[i] = TileMoveStatus.Blocked;
                } else {
                    if (tiles[i].tileType == TileType.CostalCity) {
                        City city = tiles[i].gameObject.GetComponent<City>();
                        if (!player.playerCities.Contains(city)) {
                            moveDirs[i] = TileMoveStatus.Blocked;
                        }
                    }
                }
            }
        }
    }

    public override void Move(int dir) {
        base.Move(dir);

        foreach (var army in armiesOnTransport) {
            army.pos = pos;
        }
    }
}