using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public List<CityTileData> playerCities;
    public List<Unit> playerUnits;
    public Camera mainCamera;
    public Tile[,] grid;
    public GameObject moveButtons;
    public Button UL;
    public Button U;
    public Button UR;
    public Button L;
    public Button R;
    public Button DL;
    public Button D;
    public Button DR;

    private Unit currentUnit;

    public void Update() {
        if (Input.GetMouseButtonDown(0) && (currentUnit == null)) {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                if (hit.transform.gameObject.TryGetComponent<Unit>(out currentUnit)) {
                    moveButtons.SetActive(true);
                    Vector3 buttonPos = new Vector3(hit.transform.position.x, 5f, hit.transform.position.z);
                    moveButtons.transform.position = buttonPos;
                    CheckUnitDirs();
                } else {
                    currentUnit = null;
                }
            }
        }
        if (currentUnit == null) {
            moveButtons.SetActive(false);
        } else {
            moveButtons.SetActive(true);
            Vector3 buttonPos = new Vector3(currentUnit.transform.position.x, 5f, currentUnit.transform.position.z);
            moveButtons.transform.position = buttonPos;
        }
    }

    public void CheckUnitDirs() {
        bool[] moveDirs = currentUnit.CheckDirs();
        UL.interactable = true;
        U.interactable = true;
        UR.interactable = true;
        L.interactable = true;
        R.interactable = true;
        DL.interactable = true;
        D.interactable = true;
        DR.interactable = true;
        if (!moveDirs[0]) {
            UL.interactable = false;
        }
        if (!moveDirs[1]) {
            U.interactable = false;
        }
        if (!moveDirs[2]) {
            UR.interactable = false;
        }
        if (!moveDirs[3]) {
            L.interactable = false;
        }
        if (!moveDirs[4]) {
            R.interactable = false;
        }
        if (!moveDirs[5]) {
            DL.interactable = false;
        }
        if (!moveDirs[6]) {
            D.interactable = false;
        }
        if (!moveDirs[7]) {
            DR.interactable = false;
        }
    }

    public void Move(int dir) {
        if (currentUnit != null) {
            currentUnit.Move(dir);
            CheckUnitDirs();
        }
    }
}