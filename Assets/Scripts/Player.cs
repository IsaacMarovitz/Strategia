using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour {

    public List<CityTileData> playerCities;
    public List<Unit> playerUnits;
    public Camera mainCamera;
    public Tile[,] grid;
    public GameObject moveButtons;
    public UIInfo UIInfo;
    public GameObject startUnitPrefab;

    private Unit currentUnit;
    private City currentCity;

    public void Start() {
        UIInfo.unitSelected = false;
        UIInfo.unitWorldPos = Vector3.zero;
        UIInfo.pos = Vector2Int.zero;
        UIInfo.movesLeft = 0;
        UIInfo.moveDirs = new bool[8];
        UIInfo.day = 1;
        UIInfo.dir = 1;
        UIInfo.newMove = false;

        UIInfo.citySelected = false;
    }

    public void StartGame(CityTileData startingCity) {
        playerCities.Add(startingCity);
        GameObject instantiatedUnit = GameObject.Instantiate(startUnitPrefab, new Vector3(0, 0.5f, 0), Quaternion.identity);
        Unit newUnit = instantiatedUnit.GetComponent<Unit>();
        newUnit.pos = startingCity.index;
        playerUnits.Add(newUnit);
    }

    public void Update() {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                if (!EventSystem.current.IsPointerOverGameObject()) {
                    if (hit.transform.tag == "Player1Unit") {
                        currentUnit?.Deselected();
                        currentUnit = hit.transform.gameObject.GetComponent<Unit>();
                        currentUnit.Selected();
                        UIInfo.unitSelected = true;
                    } else {
                        currentUnit?.Deselected();
                        currentUnit = null;
                        UIInfo.unitSelected = false;
                    }
                    if (hit.transform.tag == "City") {
                        currentCity?.Deselected();
                        currentCity = hit.transform.gameObject.GetComponent<City>();
                        currentCity.Selected();
                        UIInfo.citySelected = true;
                        UIInfo.cityWorldPos = currentCity.transform.position;
                    } else {
                        UIInfo.citySelected = false;
                        currentCity?.Deselected();
                    }
                }
            }
        }
    }
}