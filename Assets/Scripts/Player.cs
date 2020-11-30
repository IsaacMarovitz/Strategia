using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    public List<City> playerCities;
    public List<Unit> playerUnits;
    public Tile[,] grid;
    public GameObject startUnitPrefab;
    public bool turnStarted = false;
    public bool turnCompleted = false;
    [HideInInspector]
    public GameMode gameMode;
    public Color playerColor;
    public bool hasDied = false;

    private List<Unit> unitQueue;
    public Texture2D fogOfWarTexture;
    public float[,] fogOfWarMatrix;

    public void UpdateFogOfWar() {
        for (int x = 0; x < GameManager.Instance.grid.width; x++) {
            for (int y = 0; y < GameManager.Instance.grid.height; y++) {
                if (fogOfWarMatrix[x, y] == 1f) {
                    fogOfWarMatrix[x, y] = 0.5f;
                }
            }
        }
        foreach (var unit in playerUnits) {
            List<Tile> revealedTiles = GridUtilities.RadialSearch(GameManager.Instance.grid.grid, unit.pos, 5);
            foreach (var tile in revealedTiles) {
                fogOfWarMatrix[tile.index.x, tile.index.y] = 1f;
            }
        }
        foreach (var city in playerCities) {
            List<Tile> revealedTiles = GridUtilities.RadialSearch(GameManager.Instance.grid.grid, city.pos, 5);
            foreach (var tile in revealedTiles) {
                fogOfWarMatrix[tile.index.x, tile.index.y] = 1f;
            }
        }

        GenerateTexture();
    }

    public void GenerateTexture() {
        for (int x = 0; x < GameManager.Instance.grid.width; x++) {
            for (int y = 0; y < GameManager.Instance.grid.height; y++) {
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
        unit.gameObject.name = "Unit " + (playerUnits.Count+1) + ", " + this.gameObject.name;
        unit.SetColor(playerColor);
        playerUnits.Add(unit);
    }

    public void StartTurn() {
        Debug.Log($"<b>{this.gameObject.name}:</b> Turn is starting");
        turnStarted = true;
        if (unitQueue.Count > 0) {
            UIData.Instance.currentUnit = unitQueue[0];
            unitQueue[0].StartTurn();
        } else {
            TurnComplete();
        }
        if (fogOfWarMatrix == null) {
            fogOfWarMatrix = new float[GameManager.Instance.grid.width, GameManager.Instance.grid.height];
            fogOfWarTexture = new Texture2D(GameManager.Instance.grid.width, GameManager.Instance.grid.height);
            fogOfWarTexture.filterMode = FilterMode.Point;
            for (int x = 0; x < GameManager.Instance.grid.width; x++) {
                for (int y = 0; y < GameManager.Instance.grid.height; y++) {
                    fogOfWarMatrix[x, y] = 0;
                }
            }
        }
        if (playerUnits != null) {
            UpdateFogOfWar();
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
            TurnComplete();
        }
    }

    public void TurnComplete() {
        turnCompleted = true;
        UIData.Instance.currentUnit = null;
        UIData.Instance.currentCity = null;
        Debug.Log($"<b>{this.gameObject.name}:</b> Turn is ending");
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
            hasDied = true;
            return true;
        } 
        return false;
    }
}