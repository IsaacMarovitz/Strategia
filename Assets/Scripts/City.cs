using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour {

    public bool isOwned;
    public UIInfo UIInfo;
    public UnitType unitType;
    public GameObject[] unitPrefabs = new GameObject[9];
    public Vector2Int pos;
    public Strategia.Grid gridScript;
    public string cityName = "London";

    //private bool selected = false;
    public int turnsLeft;
    private int currentIndex;
    private readonly int[] unitTTCs = { 4, 8, 8, 12, 2, 6, 6, 10, 12 };
    private bool oldIsOwned;

    public void ShowNearbyTiles() {
        List<Tile> nearbyTiles = GridUtilities.RadialSearch(gridScript.grid, pos, 5);
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

    public void UpdateUnitType(UnitType _unitType) {
        unitType = _unitType;
        currentIndex = (int)unitType;
        turnsLeft = unitTTCs[currentIndex];
    }

    public Unit StartGame() {
        isOwned = true;
        return CreateUnit();
    }

    public void TakeTurn() {
        if (isOwned) {
            turnsLeft--;
            if (turnsLeft <= 0) {
                //CreateUnit();
                turnsLeft = unitTTCs[currentIndex];
            }
        }
    }

    public Unit CreateUnit() {
        GameObject instantiatedUnit = GameObject.Instantiate(unitPrefabs[currentIndex], new Vector3(pos.x * gridScript.tileWidth, 0.75f, pos.y * gridScript.tileHeight), Quaternion.identity);
        instantiatedUnit.GetComponent<Unit>().pos = pos;
        return instantiatedUnit.GetComponent<Unit>();
    }

    public void Selected() {
        //selected = true;
        UIInfo.city = this;
    }

    public void Deselected() {
        //selected = false;
    }
}

public enum UnitType { Army, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, Battleship };