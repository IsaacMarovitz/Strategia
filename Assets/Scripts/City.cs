using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour {

    public bool isOwned;
    public Player player;
    public UIInfo UIInfo;
    public UnitType unitType;
    public GameObject[] unitPrefabs = new GameObject[9];
    public Vector2Int pos;
    public GameManager gameManager;
    public string cityName = "London";

    //private bool selected = false;
    public int turnsLeft;
    private int currentIndex;
    private readonly int[] unitTTCs = { 4, 8, 8, 12, 2, 6, 6, 10, 12 };
    private bool oldIsOwned;

    public void ShowNearbyTiles() {
        List<Tile> nearbyTiles = GridUtilities.RadialSearch(gameManager.grid.grid, pos, 5);
        foreach (var tile in nearbyTiles) {
            tile.tileScript.ChangeVisibility(Visibility.Visable);
            tile.tileScript.isOwnedByCity = true;
        }
    }

    public void Update() {
        if (isOwned && oldIsOwned != isOwned) {
            oldIsOwned = isOwned;
            ShowNearbyTiles();
        }
    }

    public void UpdateUnitType(UnitType unitType) {
        this.unitType = unitType;
        currentIndex = (int)unitType;
        turnsLeft = unitTTCs[currentIndex];
    }

    public void StartGame(Player player) {
        this.player = player;
        isOwned = true;
        CreateUnit();
        gameManager.newDayDelegate += TakeTurn;
    }

    public void TakeTurn() {
        if (isOwned) {
            turnsLeft--;
            if (turnsLeft <= 0) {
                CreateUnit();
                turnsLeft = unitTTCs[currentIndex];
            }
        }
    }

    public void CreateUnit() {
        GameObject instantiatedUnit = GameObject.Instantiate(unitPrefabs[currentIndex], new Vector3(pos.x * gameManager.grid.tileWidth, 0.75f, pos.y * gameManager.grid.tileHeight), Quaternion.identity);
        Unit newUnit = instantiatedUnit.GetComponent<Unit>();
        newUnit.pos = pos;
        newUnit.gridScript = gameManager.grid;
        player.playerUnits.Add(newUnit);
    }

    public void Selected() {
        //selected = true;
        UIInfo.city = this;
    }

    public void Deselected() {
        //selected = false;
    }
}

public enum UnitType { Army, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, Battleship }