using UnityEngine;
using UnityEngine.UI;

public class CityUIToggle : MonoBehaviour {

    public UnitType unitType;

    Toggle toggle;
    CityUI cityUI;

    public void Start() {
        toggle = GetComponent<Toggle>();
        cityUI = GameObject.FindObjectOfType<CityUI>();
        toggle.onValueChanged.AddListener((value) => cityUI.ChangeUnitType(value, unitType));
        cityUI.updateToggleDelegate += UpdateToggle;
    }

    public void UpdateToggle(UnitType cityUnitType, bool isCostalCity) {
        if (!isCostalCity) {
            toggle.interactable = !GameManager.Instance.GetUnitFromType(unitType).blockedTileTypes.Contains(TileType.City);
        } else {
            toggle.interactable = true;
        }

        if (unitType == cityUnitType) {
            toggle.SetIsOnWithoutNotify(true);
        } else {
            toggle.SetIsOnWithoutNotify(false);
        }
    }
} 