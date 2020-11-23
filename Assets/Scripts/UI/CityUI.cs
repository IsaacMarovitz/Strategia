using UnityEngine;
using TMPro;

public class CityUI : MonoBehaviour {

    public GameObject panel;
    public TMP_Text cityName;
    public TMP_Text turnsLeft;
    public float yOffset;

    public void Start() {
        panel.SetActive(false);
    }

    public void Update() {
        if (UIData.Instance.currentCity != null) {
            panel.SetActive(true);
            transform.position = new Vector3(UIData.Instance.currentCity.transform.position.x, yOffset, UIData.Instance.currentCity.transform.position.z);
            cityName.text = UIData.Instance.currentCity.cityName;
            turnsLeft.text = "Days left : " + UIData.Instance.currentCity.turnsLeft;
        } else {
            panel.SetActive(false);
        }
    }

    public void ChangeUnitType(int unitType) {
        if (UIData.Instance.currentCity != null) {
            UIData.Instance.currentCity.UpdateUnitType((UnitType)unitType);
        }
    }
}