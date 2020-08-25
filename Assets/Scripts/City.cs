using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour {

    public IsOwned isOwned;
    public UIInfo UIInfo;
    public UnitType unitType;
    public GameObject[] unitPrefabs = new GameObject[9];
    public Vector2Int pos;
    public Strategia.Grid gridScript;   
    public string cityName = "London";

    private bool selected = false;
    private int turnsLeft;
    private int currentIndex;
    private readonly int[] unitTTCs = { 4, 8, 8, 12, 2, 6, 6, 10, 12 };

    public void Start() {
        ShowNearbyTiles();
    }

    public void ShowNearbyTiles() {
        List<Tile> nearbyTiles = gridScript.RadialSearch(pos, 5);
        foreach (var tile in nearbyTiles) {
            tile.tileScript.ChangeVisibility(Visibility.Visable);
        }
    }

    public void Update() {
        if (selected) {
            if (UIInfo.unitType != unitType) {
                unitType = UIInfo.unitType;
                currentIndex = (int)unitType;
                turnsLeft = unitTTCs[currentIndex];
            }
            UIInfo.turnsLeft = turnsLeft;
        }
    }

    public void TakeTurn() {
        if ((int)isOwned != 0) {
            turnsLeft--;
            if (turnsLeft <= 0) {
                //CreateUnit();
                turnsLeft = unitTTCs[currentIndex];
            }
        }
    }

    public void CreateUnit() {
        GameObject.Instantiate(unitPrefabs[currentIndex], new Vector3(pos.x * gridScript.tileWidth, 0, pos.y * gridScript.tileHeight), Quaternion.identity);
    }

    public void Selected() {
        selected = true;
        UIInfo.unitType = unitType;
        UIInfo.cityName = cityName;
    }

    public void Deselected() {
        selected = false;
    }
}

public enum IsOwned { No, Player1 };
public enum UnitType { Army, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, Battleship };