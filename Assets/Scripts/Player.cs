using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour {

    public List<City> playerCities;
    public List<Unit> playerUnits;
    public Camera mainCamera;
    public Tile[,] grid;
    public GameObject moveButtons;
    public UIInfo UIInfo;
    public GameObject startUnitPrefab;
    public bool turnStarted = false;
    public bool turnCompleted = false;
    [HideInInspector]
    public GameMode gameMode;

    private GameManager gameManager;
    private List<Unit> unitQueue;

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

    public void CheckFogOfWar() {
        foreach (var grid in gameManager.grid.grid) {
            if (grid.tileScript.visibility == Visibility.Visable) {
                grid.tileScript.ChangeVisibility(Visibility.Hidden);
            }
        }
        foreach (var unit in playerUnits) {
            List<Tile> nearbyTiles = GridUtilities.RadialSearch(gameManager.grid.grid, unit.pos, 5);
            foreach (var tile in nearbyTiles) {
                tile.tileScript.ChangeVisibility(Visibility.Visable);
            }
        }
    }

    public void StartTurn() {
        UIInfo.unit = unitQueue[0];
        unitQueue[0].StartTurn();
        turnStarted = true;
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

    public void Update() {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                if (!EventSystem.current.IsPointerOverGameObject()) {
                    if (hit.transform.tag == "Unit") {
                        Unit hitUnit = hit.transform.gameObject.GetComponent<Unit>();
                        if (playerUnits.Contains(hitUnit)) {
                            UIInfo.unit?.Deselected();
                            UIInfo.unit = hitUnit;
                            UIInfo.unit.Selected();
                        }
                    } else {
                        UIInfo.unit?.Deselected();
                        UIInfo.unit = null;
                    }
                    if (hit.transform.tag == "City") {
                        City hitCity = hit.transform.gameObject.GetComponent<City>();
                        if (playerCities.Contains(hitCity)) {
                            UIInfo.city?.Deselected();
                            UIInfo.city = hit.transform.gameObject.GetComponent<City>();
                            UIInfo.city.Selected();
                        }
                    } else {
                        UIInfo.city?.Deselected();
                        UIInfo.city = null;
                    }
                }
            }
        }
    }
}