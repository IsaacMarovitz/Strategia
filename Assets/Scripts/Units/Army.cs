using UnityEngine;

public class Army : Unit {

    public bool isMoveDistanceReduced;
    public bool isOnTransport = false;
    public int reducedMoveDistance;

    public override void Start() {
        base.Start();
        unitType = UnitType.Army;
        // Set damage percentages in order of Army, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, and Battleship
        damagePercentages = new float[9] { 0.34f, 0f, 0.25f, 0f, 0.2f, 0.1f, 0.3f, 0.1f, 0f };
    }

    public override void Update() {
        base.Update();
        if (isOnTransport) {
            isInCity = false;
            mainMesh.SetActive(false);
        }
    }

    public override void NewDay(Player _player) {
        base.NewDay(_player);

        if (isMoveDistanceReduced) {
            moves -= reducedMoveDistance;
        }
    }

    public override void CheckDirs() {
        base.CheckDirs();

        Tile[] tiles = GridUtilities.DiagonalCheck(pos);
        for (int i = 0; i < tiles.Length; i++) {
            if (tiles[i] == null) {
                moveDirs[i] = TileMoveStatus.Blocked;
            } else if (tiles[i].tileType == TileType.Trees || tiles[i].tileType == TileType.Mountains || tiles[i].tileType == TileType.Sea) {
                moveDirs[i] = TileMoveStatus.Blocked;
            } else {
                moveDirs[i] = TileMoveStatus.Move;
            }
        }

        if (turnStage == TurnStage.Started) {
            for (int i = 0; i < tiles.Length; i++) {
                if (tiles[i].unitOnTile != null) {
                    if (tiles[i].tileType == TileType.City || tiles[i].tileType == TileType.CostalCity) {
                        City city = tiles[i].gameObject.GetComponent<City>();
                        if (!player.playerCities.Contains(city)) {
                            if (!isOnTransport) {
                                moveDirs[i] = TileMoveStatus.Attack;
                            }
                        }
                    } else {
                        if (player.playerUnits.Contains(tiles[i].unitOnTile)) {
                            if (tiles[i].unitOnTile.GetType() == typeof(Transport)) {
                                if (!tiles[i].unitOnTile.GetComponent<Transport>().isTransportFull) {
                                    moveDirs[i] = TileMoveStatus.Transport;
                                } else {
                                    moveDirs[i] = TileMoveStatus.Blocked;
                                }
                            } else {
                                moveDirs[i] = TileMoveStatus.Blocked;
                            }
                        } else {
                            if (!isOnTransport) {
                                moveDirs[i] = TileMoveStatus.Attack;
                            } else {
                                moveDirs[i] = TileMoveStatus.Blocked;
                            }
                        }
                    }
                } 
            }
        }
    }

    public override void Move(int dir) {
        moves--;
        int[] rotationOffset = new int[8] { -45, 0, 45, -90, 90, 225, 180, 135 };
        Vector2Int offset = Vector2Int.zero;
        if (dir == 1 || dir == 4 || dir == 6) {
            offset.x--;
        } else if (dir == 3 || dir == 5 || dir == 8) {
            offset.x++;
        }
        if (dir <= 3) {
            offset.y++;
        } else if (dir >= 6) {
            offset.y--;
        }
        if (moveDirs[dir - 1] == TileMoveStatus.Move) {
            if (isOnTransport) {
                isOnTransport = false;
                mainMesh.SetActive(true);
                ((Transport)GameManager.Instance.grid.grid[pos.x, pos.y].unitOnTile).armiesOnTransport.Remove(this);
            } else {
                GameManager.Instance.grid.grid[pos.x, pos.y].unitOnTile = null;
            }
            pos += offset;
            GameManager.Instance.grid.grid[pos.x, pos.y].unitOnTile = this;
            this.transform.eulerAngles = new Vector3(0, rotationOffset[dir - 1], 0);
        } else if (moveDirs[dir - 1] == TileMoveStatus.Attack) {
            Attack(pos + offset);
        } else if (moveDirs[dir - 1] == TileMoveStatus.Transport) {
            pos += offset;
            isOnTransport = true;
            ((Transport)GameManager.Instance.grid.grid[pos.x, pos.y].unitOnTile).armiesOnTransport.Add(this);
        }

        if (oldCity != null) {
            oldCity.RemoveUnit(this);
            oldCity = null;
            isInCity = false;
            mainMesh.SetActive(true);
        }

        if (GameManager.Instance.grid.grid[pos.x, pos.y].tileType == TileType.City || GameManager.Instance.grid.grid[pos.x, pos.y].tileType == TileType.CostalCity) {
            City city = GameManager.Instance.grid.grid[pos.x, pos.y].gameObject.GetComponent<City>();
            city.GetOwned(player);
            city.AddUnit(this);
            oldCity = city;
            isInCity = true;
            mainMesh.SetActive(false);
        } else {
            isInCity = false;
            mainMesh.SetActive(true);
        }

        if (moves <= 0) {
            turnStage = TurnStage.Complete;
            EndTurn();
        }
        player.UpdateFogOfWar();

        if (GameManager.Instance.grid.grid[pos.x, pos.y].tileType == TileType.Swamp) {
            isMoveDistanceReduced = true;
            moves -= reducedMoveDistance;
        } else {
            isMoveDistanceReduced = false;
        }
    }
}