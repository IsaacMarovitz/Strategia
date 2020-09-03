using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CityUI : MonoBehaviour {

    public GameObject panel;
    public UIInfo UIInfo;
    public TMP_Text cityName;
    public TMP_Text turnsLeft;
    public float yOffset;

    public void Start() {
        panel.SetActive(false);
    }

    public void Update() {
        if (UIInfo.city != null) {
            panel.SetActive(true);
            transform.position = new Vector3(UIInfo.city.transform.position.x, yOffset, UIInfo.city.transform.position.z);
            cityName.text = UIInfo.city.cityName;
            turnsLeft.text = "Days left : " + UIInfo.city.turnsLeft;
        } else {
            panel.SetActive(false);
        }
    }

    public void ChangeUnitType(int unitType) {
        UIInfo.city.UpdateUnitType((UnitType)unitType);
    }
}