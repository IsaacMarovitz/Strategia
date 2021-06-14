using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    public List<City> playerCities;
    public List<Unit> playerUnits;
    public Tile[,] grid;
    public bool turnStarted = false;
    public bool turnCompleted = false;
    [HideInInspector]
    public GameMode gameMode;
    public Color playerColor;
    public bool hasDied = false;
    public UnitInfo unitInfo;
    public CameraController cameraController;

    private List<Unit> unitQueue;
    private bool revealAllTiles = false;
    public Texture2D minimapTexture;
    public Texture2D fogOfWarTexture;
    public float[,] fogOfWarMatrix;

    public void UpdateFogOfWar() {
        if (fogOfWarMatrix == null) {
            fogOfWarMatrix = new float[GameManager.Instance.grid.width, GameManager.Instance.grid.height];
            fogOfWarTexture = new Texture2D(GameManager.Instance.grid.width, GameManager.Instance.grid.height);
            fogOfWarTexture.filterMode = FilterMode.Point;
            minimapTexture = new Texture2D(GameManager.Instance.grid.width, GameManager.Instance.grid.height);
            minimapTexture.filterMode = FilterMode.Point;
            for (int x = 0; x < GameManager.Instance.grid.width; x++) {
                for (int y = 0; y < GameManager.Instance.grid.height; y++) {
                    fogOfWarMatrix[x, y] = 0;
                }
            }
        }
        if (!revealAllTiles) {
            for (int x = 0; x < GameManager.Instance.grid.width; x++) {
                for (int y = 0; y < GameManager.Instance.grid.height; y++) {
                    if (fogOfWarMatrix[x, y] == 1f) {
                        fogOfWarMatrix[x, y] = 0.5f;
                    }
                }
            }
            foreach (var unit in playerUnits) {
                List<Tile> revealedTiles = GridUtilities.RadialSearch(unit.pos, 5);
                foreach (var tile in revealedTiles) {
                    fogOfWarMatrix[tile.pos.x, tile.pos.y] = 1f;
                }
            }
            foreach (var city in playerCities) {
                List<Tile> revealedTiles = GridUtilities.RadialSearch(city.pos, 5);
                foreach (var tile in revealedTiles) {
                    fogOfWarMatrix[tile.pos.x, tile.pos.y] = 1f;
                }
            }
        } else {
            for (int x = 0; x < GameManager.Instance.grid.width; x++) {
                for (int y = 0; y < GameManager.Instance.grid.height; y++) {
                    fogOfWarMatrix[x, y] = 1f;
                }
            }
        }

        GenerateTexture();
    }

    public void GenerateTexture() {
        for (int x = 0; x < GameManager.Instance.grid.width; x++) {
            for (int y = 0; y < GameManager.Instance.grid.height; y++) {
                if (fogOfWarMatrix[x, y] == 1) {
                    fogOfWarTexture.SetPixel(x, y, new Color(1, 1, 1, 0));
                    if (GameManager.Instance.grid.grid[x, y].cityOfInfluence.player != null) {
                        minimapTexture.SetPixel(x, y, GameManager.Instance.grid.grid[x, y].cityOfInfluence.player.playerColor);
                    } else {
                        minimapTexture.SetPixel(x, y, Color.grey);
                    }
                } else if (fogOfWarMatrix[x, y] == 0) {
                    fogOfWarTexture.SetPixel(x, y, Color.black);
                    minimapTexture.SetPixel(x, y, Color.black);
                } else if (fogOfWarMatrix[x, y] == 0.5f) {
                    fogOfWarTexture.SetPixel(x, y, new Color(1, 1, 1, 0.5f));
                    if (GameManager.Instance.grid.grid[x, y].cityOfInfluence.player != null) {
                        minimapTexture.SetPixel(x, y, GameManager.Instance.grid.grid[x, y].cityOfInfluence.player.playerColor);
                    } else {
                        minimapTexture.SetPixel(x, y, Color.grey);
                    }
                } else {
                    fogOfWarTexture.SetPixel(x, y, Color.red);
                }
            }
        }
        fogOfWarTexture.Apply();
        minimapTexture.Apply();
        if (turnStarted && !turnCompleted) {
            GameManager.Instance.fogOfWarTexture.material.mainTexture = fogOfWarTexture;
            GameManager.Instance.UpdateFogOfWarObjects(fogOfWarMatrix);
        }
    }

    public void NewDay() {
        //Debug.Log($"<b>{this.gameObject.name}:</b> Received New Day");
        foreach (var unit in playerUnits) {
            unit.NewDay(this);
        }

        unitQueue?.Clear();
        unitQueue = new List<Unit>(playerUnits);
        turnStarted = false;
        turnCompleted = false;
    }

    public void InitaliseStartCity(City city) {
        city.StartGame(this);
    }

    public void AddUnit(Unit unit) {
        unit.gameObject.name = "Unit " + (playerUnits.Count + 1) + ", " + this.gameObject.name;
        unit.SetColor(playerColor);
        playerUnits.Add(unit);
    }

    public void StartTurn() {
        Debug.Log($"<b>{this.gameObject.name}:</b> Turn is starting");
        turnStarted = true;
        if (playerUnits != null) {
            UpdateFogOfWar();
        }
        if (unitQueue.Count > 0) {
            UIData.Instance.currentUnit = unitQueue[0];
            cameraController.Focus(GridUtilities.TileToWorldPos(unitQueue[0].pos), false);
            // Prevents Camera Controller from sometimes defocusing unit because of Next Player UI Button press
            cameraController.didClickUI = true;
            unitQueue[0].StartTurn();
        } else {
            TurnComplete();
        }
    }

    public void NextUnit(Unit unit, bool movingLater) {
        if (movingLater) {
            unitQueue.Remove(unit);
            unitQueue.Add(unit);
        } else {
            unitQueue.Remove(unit);
        }
        if (unitQueue.Count > 0) {
            Debug.Log($"<b>{this.gameObject.name}:</b> Starting next unit turn");
            UIData.Instance.currentUnit = unitQueue[0];
            unitQueue[0].StartTurn();
        } else {
            // TurnComplete();
            EndTurnButton();
        }
    }

    public void EndTurnButton() {
        turnCompleted = true;
        //UIData.Instance.currentUnit = null;
        //UIData.Instance.currentCity = null;
    }

    public void TurnComplete() {
        turnCompleted = true;
        UIData.Instance.currentUnit = null;
        UIData.Instance.currentCity = null;
        Debug.Log($"<b>{this.gameObject.name}:</b> Turn is ending");
        foreach (var unit in playerUnits) {
            unit.MoveAlongSetPath();
        }
        GameManager.Instance.NextPlayer();
    }

    public Unit GetCurrentUnit() {
        if (unitQueue.Count > 0) {
            return unitQueue[0];
        }
        return null;
    }

    public bool HasDied() {
        if (playerUnits.Count <= 0 && playerCities.Count <= 0) {
            Debug.Log($"<b>GameManager:</b> Player {GameManager.Instance.currentPlayerIndex} has died!");
            hasDied = true;
            return true;
        }
        return false;
    }

    public void AddToUnitQueue(Unit unit) {
        unitQueue.Add(unit);
    }

#if UNITY_EDITOR
    public void RevealAllTiles() {
        revealAllTiles = true;
        UpdateFogOfWar();
    }

    public void SpawnArmy(Vector2Int pos) {
        if (GameManager.Instance.grid.grid[pos.x, pos.y].tileType == TileType.City || GameManager.Instance.grid.grid[pos.x, pos.y].tileType == TileType.CostalCity) {
            Unit unit = GameObject.Instantiate(unitInfo.allUnits[0].prefab, Vector3.zero, Quaternion.identity).GetComponent<Unit>();
            unit.gameObject.transform.position = GridUtilities.TileToWorldPos(pos, unit.yOffset);
            unit.pos = pos;
            unit.gameObject.transform.parent = this.gameObject.transform;
            unit.player = this;
            unit.mainMesh.SetActive(false);
            unit.isInCity = true;
            unit.oldCity = GameManager.Instance.grid.grid[pos.x, pos.y].gameObject.GetComponent<City>();
            AddUnit(unit);
            GameManager.Instance.grid.grid[pos.x, pos.y].unitOnTile = unit;
            GameManager.Instance.grid.grid[pos.x, pos.y].gameObject.GetComponent<City>().AddUnit(unit);
        } else {
            if (GameManager.Instance.grid.grid[pos.x, pos.y].tileType == TileType.Plains || GameManager.Instance.grid.grid[pos.x, pos.y].tileType == TileType.Swamp) {
                if (GameManager.Instance.grid.grid[pos.x, pos.y].unitOnTile == null) {
                    Unit unit = GameObject.Instantiate(unitInfo.allUnits[0].prefab, Vector3.zero, Quaternion.identity).GetComponent<Unit>();
                    unit.gameObject.transform.position = GridUtilities.TileToWorldPos(pos, unit.yOffset);
                    unit.pos = pos;
                    unit.gameObject.transform.parent = this.gameObject.transform;
                    unit.player = this;
                    AddUnit(unit);
                    GameManager.Instance.grid.grid[pos.x, pos.y].unitOnTile = unit;
                }
            }
        }
    }
#endif
}