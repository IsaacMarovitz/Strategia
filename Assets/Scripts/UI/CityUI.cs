using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CityUI : MonoBehaviour {

    public GameObject panel;
    public TMP_Text cityName;
    public TMP_Text turnsLeft;
    public float yOffset;
    public HorizontalLayoutGroup horizontalLayoutGroup;
    public GameObject unitButtonPrefab;

    private City oldCity;
    private bool hasUpdated = false;

    public void Start() {
        panel.SetActive(false);
    }

    public void Update() {
        if (UIData.Instance.currentCity != null) {
            panel.SetActive(true);
            transform.position = new Vector3(UIData.Instance.currentCity.transform.position.x, yOffset, UIData.Instance.currentCity.transform.position.z);
            cityName.text = UIData.Instance.currentCity.cityName;
            turnsLeft.text = "Days left : " + UIData.Instance.currentCity.turnsLeft;
            if (oldCity != UIData.Instance.currentCity) {
                hasUpdated = false;
                oldCity = UIData.Instance.currentCity;
            }
            if (!hasUpdated) {
                hasUpdated = true;
                UpdateUnitButtons();
            }
        } else {
            panel.SetActive(false);
            hasUpdated = false;
        }
    }

    public void ChangeUnitType(int unitType) {
        if (UIData.Instance.currentCity != null) {
            UIData.Instance.currentCity.UpdateUnitType((UnitType)unitType);
        }
    }

    public void UpdateUnitButtons() {
        for (int i = horizontalLayoutGroup.transform.childCount-1; i >= 0; i--) {
            GameObject.Destroy(horizontalLayoutGroup.transform.GetChild(i).gameObject);
        }
        foreach (var unit in UIData.Instance.currentCity.unitsInCity) {
            GameObject newButton = GameObject.Instantiate(unitButtonPrefab, Vector3.zero, Quaternion.identity);
            newButton.transform.SetParent(horizontalLayoutGroup.transform, false);
            UnitButtonUI unitButton = newButton.GetComponent<UnitButtonUI>();
            unitButton.unit = unit;
        }
    }
}