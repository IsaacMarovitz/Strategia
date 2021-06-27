using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class City : MonoBehaviour {

    // Unit order Tank, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, Battleship
    public bool isOwned = false;
    public bool showCityNameUI = true;
    public Player player;
    public UnitInfo unitInfo;
    public UnitType unitType;
    public Vector2Int pos;
    public string cityName = "London";
    public List<Unit> unitsInCity;
    public Canvas canvas;
    public TMP_Text cityNameText;
    public Button button;

    public int turnsLeft;
    public int currentIndex;
    private Color defaultColor;
    private List<UnitData> unitData = new List<UnitData>();

    public void Awake() {
        canvas = GetComponentInChildren<Canvas>();
        cityNameText = GetComponentInChildren<TMP_Text>();
        button = GetComponentInChildren<Button>();
        button.onClick.AddListener(SelectCity);
        canvas.worldCamera = Camera.main;
        canvas.enabled = false;
        defaultColor = this.transform.GetChild(0).GetComponent<MeshRenderer>().materials[0].color;
        unitData = unitInfo.allUnits;
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
        turnsLeft = unitData[currentIndex].turnsToCreate;
    }

    public void StartGame(Player player) {
        GetOwned(player);
        cityName = player.country.capitalCity;
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
        turnsLeft = unitData[currentIndex].turnsToCreate;
        cityName = player.AssignName();
    }

    public void TakeTurn() {
        if (isOwned) {
            turnsLeft--;
            if (turnsLeft <= 0) {
                CreateUnit();
                turnsLeft = unitData[currentIndex].turnsToCreate;
            }
        }
    }

    public void CreateUnit() {
        GameObject instantiatedUnit = GameObject.Instantiate(unitData[currentIndex].prefab,  GridUtilities.TileToWorldPos(pos, 0.75f), Quaternion.identity);
        instantiatedUnit.transform.parent = player.gameObject.transform;
        Unit newUnit = instantiatedUnit.GetComponent<Unit>();
        newUnit.pos = pos;
        newUnit.oldCity = this;
        newUnit.isInCity = true;
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

    public void SelectCity() {
        if (player != null) {
            if (player == GameManager.Instance.GetCurrentPlayer()) {
                UIData.Instance.currentCity = this;
            }
        }
    }
}

