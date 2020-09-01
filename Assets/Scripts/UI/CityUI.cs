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
        if (UIInfo.city != null) {
            panel.SetActive(true);
            transform.position = new Vector3(UIInfo.city.transform.position.x, yOffset, UIInfo.city.transform.position.z);
            cityName.text = UIInfo.city.cityName;
            turnsLeft.text = "Days left : " + UIInfo.city.turnsLeft;
            switch (toggleGroup.GetFirstActiveToggle().name) {
                case "Army":
                    UIInfo.city.UpdateUnitType(UnitType.Army);
                    break;
                case "Parachute":
                    UIInfo.city.UpdateUnitType(UnitType.Parachute);
                    break;
                case "Fighter":
                    UIInfo.city.UpdateUnitType(UnitType.Fighter);
                    break;
                case "Bomber":
                    UIInfo.city.UpdateUnitType(UnitType.Bomber);
                    break;
                case "Transport":
                    UIInfo.city.UpdateUnitType(UnitType.Transport);
                    break;
                case "Destroyer":
                    UIInfo.city.UpdateUnitType(UnitType.Destroyer);
                    break;
                case "Submarine":
                    UIInfo.city.UpdateUnitType(UnitType.Submarine);
                    break;
                case "Carrier":
                    UIInfo.city.UpdateUnitType(UnitType.Carrier);
                    break;
                case "Battleship":
                    UIInfo.city.UpdateUnitType(UnitType.Battleship);
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