using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class City : MonoBehaviour {

    // Unit order Tank, Parachute, Fighter, Bomber, Transport, Destroyer, Submarine, Carrier, Battleship
    private bool _isOwned = false;
    public bool isOwned { 
        get { 
            return _isOwned; 
        } 
        private set { 
            _isOwned = value; 
            UpdateCanvas();
        } 
    }

    private bool _showCityNameUI = true;
    public bool showCityNameUI {
        get {
            return _showCityNameUI;
        }
        set {
            _showCityNameUI = value;
            UpdateCanvas();
        }
    }

    private string _cityName = "";
    public string cityName {
        get {
            return _cityName;
        }
        set {
            _cityName = value;
            UpdateCanvas();
        }
    }

    public Player player;
    public UnitInfo unitInfo;
    public UnitType unitType;
    public Vector2Int pos;
    public List<Unit> unitsInCity;
    public Canvas canvas;
    public TMP_Text cityNameText;
    public Button button;

    public Action fastProdDelegate;

    public int turnsLeft;
    public int currentIndex;
    private Color defaultColor;
    private List<UnitData> unitData = new List<UnitData>();
    private bool fastProd;

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

    public void Start() {
        GameManager.Instance.fastProdDelegate += FastProd;
    }

    public void OnDestroy() {
        GameManager.Instance.fastProdDelegate -= FastProd;
    }

    public void UpdateCanvas() {
        player.cityDataChangedDelegate?.Invoke();

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

        if (fastProd) {
            turnsLeft = 1;
        } else {
            turnsLeft = unitData[currentIndex].turnsToCreate;
        }
    }
    
    public void FastProd(bool value) {
        fastProd = value;

        if (value) {
            turnsLeft = 1;
        } else {
            turnsLeft = unitData[currentIndex].turnsToCreate;
        }

        fastProdDelegate?.Invoke();
    }

    public void StartGame(Player player) {
        GetOwned(player);
        cityName = player.country.capitalCity;
        CreateUnit();
    }

    public void GetOwned(Player player) {
        if (isOwned) {
            if (this.player != null) {
                if (this.player == player) { return; }
                this.player.playerCities.Remove(this);
            }
        }
        this.transform.GetChild(0).GetComponent<MeshRenderer>().materials[0].color = player.playerColor;
        this.player = player;
        this.player.playerCities.Add(this);
        isOwned = true;
        GameManager.Instance.newDayDelegate += TakeTurn;
        turnsLeft = unitData[currentIndex].turnsToCreate;
        
        if (cityName == "") 
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
                UIData.SetCity(this);
            }
        }
    }
}

