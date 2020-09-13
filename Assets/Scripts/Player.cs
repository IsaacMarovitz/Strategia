using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    public List<City> playerCities;
    public List<Unit> playerUnits;
    public Tile[,] grid;
    public UIInfo UIInfo;
    public GameObject startUnitPrefab;
    public bool turnStarted = false;
    public bool turnCompleted = false;
    [HideInInspector]
    public GameMode gameMode;

    private GameManager gameManager;
    private List<Unit> unitQueue;
    public Texture2D fogOfWarTexture;
    public float[,] fogOfWarMatrix;

    public void Start() {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void UpdateFogOfWar(Unit unit) {
        foreach (var tile in unit.oldTiles) {
            fogOfWarMatrix[tile.index.x, tile.index.y] = 0.5f;
        }
        unit.oldTiles = GridUtilities.RadialSearch(gameManager.grid.grid, unit.pos, 5);
        foreach (var tile in unit.oldTiles) {
            fogOfWarMatrix[tile.index.x, tile.index.y] = 1f;
        }
        foreach (var city in playerCities) {
            List<Tile> returnTiles = GridUtilities.RadialSearch(gameManager.grid.grid, city.pos, 5);
            foreach (var tile in returnTiles) {
                fogOfWarMatrix[tile.index.x, tile.index.y] = 1.0f;
            }
        }
        GenerateTexture();
    }

    public void GenerateTexture() {
        for (int x = 0; x < gameManager.grid.width; x++) {
            for (int y = 0; y < gameManager.grid.height; y++) {
                if (fogOfWarMatrix[x, y] == 1) {
                    fogOfWarTexture.SetPixel(x, y, new Color(0, 0, 0, 0));
                } else if (fogOfWarMatrix[x, y] == 0) {
                    fogOfWarTexture.SetPixel(x, y, Color.black);
                } else if (fogOfWarMatrix[x, y] == 0.5f) {
                    fogOfWarTexture.SetPixel(x, y, new Color(1, 1, 1, 0.5f));
                } else {
                    fogOfWarTexture.SetPixel(x, y, Color.red);
                }
            }
        }
        fogOfWarTexture.Apply();
        if (turnStarted && !turnCompleted) {
            gameManager.fogOfWarTexture.material.mainTexture = fogOfWarTexture;
            gameManager.UpdateFogOfWarObjects(fogOfWarMatrix);
        }
    }

    public void NewDay(GameManager _gameManager) {
        gameManager = _gameManager;

        foreach (var unit in playerUnits) {
            unit.NewDay(this);
        }

        unitQueue?.Clear();
        unitQueue = new List<Unit>(playerUnits);
        turnStarted = false;
        turnCompleted = false;
    }

    public void InitaliseStartCity() {
        playerCities[0].StartGame(this);
    }

    public void StartTurn() {
        UIInfo.player = this;
        UIInfo.unit = unitQueue[0];
        unitQueue[0].StartTurn();
        turnStarted = true;
        if (fogOfWarMatrix == null) {
            fogOfWarMatrix = new float[gameManager.grid.width, gameManager.grid.height];
            fogOfWarTexture = new Texture2D(gameManager.grid.width, gameManager.grid.height);
            fogOfWarTexture.filterMode = FilterMode.Point;
            for (int x = 0; x < gameManager.grid.width; x++) {
                for (int y = 0; y < gameManager.grid.height; y++) {
                    fogOfWarMatrix[x, y] = 0;
                }
            }
        }
        UpdateFogOfWar(playerUnits[0]);
    }

    public void NextUnit(Unit unit, bool movingLater) {
        if (movingLater) {
            unitQueue.Remove(unit);
            unitQueue.Add(unit);
        } else {
            unitQueue.Remove(unit);
        }
        if (unitQueue.Count > 0) {
            UIInfo.unit?.Deselected();
            UIInfo.unit = unitQueue[0];
            unitQueue[0].StartTurn();
        } else {
            TurnComplete();
        }
    }

    public void TurnComplete() {
        turnCompleted = true;
        gameManager.NextPlayer();
    }
}