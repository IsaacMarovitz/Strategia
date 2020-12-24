using UnityEngine;
using System.Collections.Generic;

public class City : MonoBehaviour {

    public bool isOwned = false;
    public Player player;
    public UnitType unitType;
    public GameObject[] unitPrefabs = new GameObject[9];
    public Vector2Int pos;
    public string cityName = "London";
    public List<Unit> unitsInCity;

    public int turnsLeft;
    public int currentIndex;
    private readonly int[] unitTTCs = { 4, 8, 8, 12, 2, 6, 6, 10, 12 };

    public void UpdateUnitType(UnitType unitType) {
        this.unitType = unitType;
        currentIndex = (int)unitType;
        turnsLeft = unitTTCs[currentIndex];
    }

    public void StartGame(Player player) {
        GetOwned(player);
        CreateUnit();
        turnsLeft = unitTTCs[currentIndex];
    }

    public void GetOwned(Player player) {
        if (isOwned) {
            if (this.player != null) {
                this.player.playerCities.Remove(this);
            }
        }
        this.player = player;
        this.player.playerCities.Add(this);
        isOwned = true;
        GameManager.Instance.newDayDelegate += TakeTurn;
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
        GameObject instantiatedUnit = GameObject.Instantiate(unitPrefabs[currentIndex], new Vector3(pos.x * GameManager.Instance.grid.tileWidth, 0.75f, pos.y * GameManager.Instance.grid.tileHeight), Quaternion.identity);
        instantiatedUnit.transform.parent = player.gameObject.transform;
        Unit newUnit = instantiatedUnit.GetComponent<Unit>();
        newUnit.pos = pos;
        newUnit.gridScript = GameManager.Instance.grid;
        newUnit.oldCity = this;
        player.AddUnit(newUnit);
        AddUnit(newUnit);
    }

    public void AddUnit(Unit unit) {
        unitsInCity.Add(unit);
    }

    public void RemoveUnit(Unit unit) {
        unitsInCity.Remove(unit);
    }

    public void Nuke() {
        foreach (var unit in unitsInCity) {
            unit.Die();
            GameObject.Destroy(unit.gameObject);
        }
        unitsInCity.Clear();
        isOwned = false;
        player.playerCities.Remove(this);
        player = null;
        GameManager.Instance.newDayDelegate -= TakeTurn;
    }
}

