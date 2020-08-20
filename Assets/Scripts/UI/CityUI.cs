using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CityUI : MonoBehaviour {

    public GameObject panel;
    public UIInfo UIInfo;
    public ToggleGroup toggleGroup;
    public TMP_Text cityName;
    public TMP_Text turnsLeft;
    public float yOffset;

    public void Start() {
        panel.SetActive(false);
    }

    public void Update() {
        if (UIInfo.citySelected) {
            panel.SetActive(true);
            transform.position = new Vector3(UIInfo.cityWorldPos.x, yOffset, UIInfo.cityWorldPos.z);
            cityName.text = UIInfo.cityName;
            turnsLeft.text = "Days left : " + UIInfo.turnsLeft;
            switch (toggleGroup.GetFirstActiveToggle().name) {
                case "Army":
                    UIInfo.unitType = UnitType.Army;
                    break;
                case "Parachute":
                    UIInfo.unitType = UnitType.Parachute;
                    break;
                case "Fighter":
                    UIInfo.unitType = UnitType.Fighter;
                    break;
                case "Bomber":
                    UIInfo.unitType = UnitType.Bomber;
                    break;
                case "Transport":
                    UIInfo.unitType = UnitType.Transport;
                    break;
                case "Destroyer":
                    UIInfo.unitType = UnitType.Destroyer;
                    break;
                case "Submarine":
                    UIInfo.unitType = UnitType.Submarine;
                    break;
                case "Carrier":
                    UIInfo.unitType = UnitType.Carrier;
                    break;
                case "Battleship":
                    UIInfo.unitType = UnitType.Battleship;
                    break;
                default:
                    Debug.Log(toggleGroup.GetFirstActiveToggle().name);
                    break;
            }
        } else {
            panel.SetActive(false);
        }
    }
}