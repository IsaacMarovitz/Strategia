using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class City : MonoBehaviour {

    // Unit order Army, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, Battleship
    public bool isOwned = false;
    public bool showCityNameUI = true;
    public Player player;
    public UnitType unitType;
    public GameObject[] unitPrefabs = new GameObject[9];
    public Vector2Int pos;
    public string cityName = "London";
    public List<Unit> unitsInCity;
    public Canvas canvas;
    public TMP_Text cityNameText;

    public int turnsLeft;
    public int currentIndex;
    private readonly int[] unitTTCs = { 4, 8, 8, 12, 2, 6, 6, 10, 12 };
    private Color defaultColor;

    public void Awake() {
        canvas = GetComponentInChildren<Canvas>();
        cityNameText = GetComponentInChildren<TMP_Text>();
        canvas.worldCamera = Camera.main;
        canvas.enabled = false;
        defaultColor = this.transform.GetChild(0).GetComponent<MeshRenderer>().materials[0].color;
    }

    public void Update() {
        if (isOwned && showCityNameUI) {
            canvas.enabled = true;
            cityNameText.text = cityName;
        } else {
            canvas.enabled = false;
        }
    }

    public void UpdateUnitType(UnitType unitType) {
        this.unitType = unitType;
        currentIndex = (int)unitType;
        turnsLeft = unitTTCs[currentIndex];
    }

    public void StartGame(Player player) {
        GetOwned(player);
        CreateUnit();
    }

    public void GetOwned(Player player) {
        this.transform.GetChild(0).GetComponent<MeshRenderer>().materials[0].color = player.playerColor;
        if (isOwned) {
            if (this.player != null) {
                this.player.playerCities.Remove(this);
            }
        }
        this.player = player;
        this.player.playerCities.Add(this);
        isOwned = true;
        GameManager.Instance.newDayDelegate += TakeTurn;
        turnsLeft = unitTTCs[currentIndex];
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
        GameObject instantiatedUnit = GameObject.Instantiate(unitPrefabs[currentIndex],  GridUtilities.TileToWorldPos(pos, 0.75f), Quaternion.identity);
        instantiatedUnit.transform.parent = player.gameObject.transform;
        Unit newUnit = instantiatedUnit.GetComponent<Unit>();
        newUnit.pos = pos;
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
        if (player != null) {
            player.playerCities.Remove(this);
            player = null;
        }
        this.transform.GetChild(0).GetComponent<MeshRenderer>().materials[0].color = defaultColor;
        GameManager.Instance.newDayDelegate -= TakeTurn;
    }
}

