using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TransportUI : TurnBehaviour {

    public GameObject panel;
    public HorizontalLayoutGroup horizontalLayoutGroup;
    public GameObject unitButtonPrefab;
    public float yOffset;

    void Start() {
        panel.SetActive(false);
    }

    public override void OnUnitSelected(Unit unit) {
        UpdateUI(unit);
    }

    public override void OnUnitDeselected() {
        panel.SetActive(false);
    }

    public override void OnFogOfWarUpdate(Player player) {
        Unit currentUnit = UIData.currentUnit;
        if (currentUnit != null) {
            UpdateUI(currentUnit);
        } else {
            panel.SetActive(false);
        }
    }

    public void UpdateUI(Unit unit) {
        ITransport transportInterface = unit as ITransport;

        if (transportInterface != null) {
            if (transportInterface.transportUIVisible) {
                panel.SetActive(true);
                transform.position = GridUtilities.TileToWorldPos(unit.pos, yOffset);
                UpdateUnitButtons(transportInterface);
                return;
            }
        }

        panel.SetActive(false);
    }

    void UpdateUnitButtons(ITransport transportInterface) {
        for (int i = horizontalLayoutGroup.transform.childCount - 1; i >= 0; i--) {
            GameObject.Destroy(horizontalLayoutGroup.transform.GetChild(i).gameObject);
        }
        foreach (var unit in transportInterface.unitsOnTransport) {
            GameObject newButton = GameObject.Instantiate(unitButtonPrefab, Vector3.zero, Quaternion.identity);
            newButton.transform.SetParent(horizontalLayoutGroup.transform, false);
            UnitButtonUI unitButton = newButton.GetComponent<UnitButtonUI>();
            unitButton.unit = unit;
            unitButton.image.sprite = unit.unitIcon;
        }
    }
}
