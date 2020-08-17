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

    private Unit currentUnit;

    public void Start() {
        UIInfo.unitSelected = false;
        UIInfo.worldPos = Vector3.zero;
        UIInfo.pos = Vector2Int.zero;
        UIInfo.movesLeft = 0;
        UIInfo.moveDirs = new bool[8];
        UIInfo.day = 1;
        UIInfo.dir = 1;
        UIInfo.newMove = false;
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
                }
            }
        }
    }
}