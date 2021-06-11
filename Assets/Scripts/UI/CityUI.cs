using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CityUI : MonoBehaviour {

    public GameObject panel;
    public TMP_Text cityName;
    public TMP_Text turnsLeft;
    public TMP_InputField inputField;
    public float yOffset;
    public HorizontalLayoutGroup horizontalLayoutGroup;
    public GameObject unitButtonPrefab;
    public List<Toggle> costalCityToggles;
    public Toggle[] toggles;

    private City oldCity;
    private bool hasUpdated = false;

    public void Start() {
        panel.SetActive(false);
        for (int i = 0; i < toggles.Length; i++) {
            Toggle toggle = toggles[i];
            int tempInt = i;
            toggles[i].onValueChanged.AddListener( delegate {  ChangeUnitType(toggle, tempInt); } );
        }
    }

    public void Update() {
        if (UIData.Instance.currentCity != null) {
            UIData.Instance.currentCity.showCityNameUI = false;
            panel.SetActive(true);
            transform.position = new Vector3(UIData.Instance.currentCity.transform.position.x, yOffset, UIData.Instance.currentCity.transform.position.z);
            cityName.text = UIData.Instance.currentCity.cityName;
            turnsLeft.text = "Days Left: " + UIData.Instance.currentCity.turnsLeft;
            if (oldCity != UIData.Instance.currentCity) {
                hasUpdated = false;
                if (oldCity != null) {
                    oldCity.showCityNameUI = true;
                }
                oldCity = UIData.Instance.currentCity;
            }
            if (!hasUpdated) {
                hasUpdated = true;
                UpdateUnitButtons();
                for (int i = 0; i < toggles.Length; i++) {
                    if (i == (int)UIData.Instance.currentCity.unitType) {
                        toggles[i].SetIsOnWithoutNotify(true);
                    } else {
                        toggles[i].SetIsOnWithoutNotify(false);
                    }
                }
                if (GameManager.Instance.grid.grid[UIData.Instance.currentCity.pos.x, UIData.Instance.currentCity.pos.y].tileType == TileType.CostalCity) {
                    foreach (var toggle in costalCityToggles) {
                        toggle.interactable = true;
                    }
                } else {
                    foreach (var toggle in costalCityToggles) {
                        toggle.interactable = false;
                    }
                }
            }
        } else {
            panel.SetActive(false);
            hasUpdated = false;
            if (oldCity != null) {
                oldCity.showCityNameUI = true;
            }
        }
    }

    public void ChangeUnitType(Toggle toggle, int tempInt) {
        if (!toggle.isOn) { return; }
        if (UIData.Instance.currentCity != null) {
            UIData.Instance.currentCity.UpdateUnitType((UnitType)tempInt);
        }
    }

    public void UpdateUnitButtons() {
        for (int i = horizontalLayoutGroup.transform.childCount - 1; i >= 0; i--) {
            GameObject.Destroy(horizontalLayoutGroup.transform.GetChild(i).gameObject);
        }
        foreach (var unit in UIData.Instance.currentCity.unitsInCity) {
            GameObject newButton = GameObject.Instantiate(unitButtonPrefab, Vector3.zero, Quaternion.identity);
            newButton.transform.SetParent(horizontalLayoutGroup.transform, false);
            UnitButtonUI unitButton = newButton.GetComponent<UnitButtonUI>();
            unitButton.unit = unit;
        }
    }

    public void ShowInputField() {
        inputField.gameObject.SetActive(true);
        inputField.Select();
        inputField.ActivateInputField();
        inputField.text = UIData.Instance.currentCity.cityName;
        GameManager.Instance.Pause();
    }

    public void FinishChangingName() {
        UIData.Instance.currentCity.cityName = inputField.text.Trim();
        inputField.gameObject.SetActive(false);
        GameManager.Instance.Resume();
    }
}