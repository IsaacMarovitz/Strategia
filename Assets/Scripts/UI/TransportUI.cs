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
        if (unit.unitType == UnitType.Transport) {
            Transport transport = (Transport)unit;
            if (GridUtilities.DiagonalCheck(unit.currentTile.pos).Any(tile => tile.tileType != TileType.Sea) && !unit.currentTile.isCityTile && transport.tanksOnTransport.Count > 0) {
                panel.SetActive(true);
                transform.position = GridUtilities.TileToWorldPos(unit.pos, yOffset);
                UpdateUnitButtons();
            } else {
                panel.SetActive(false);
            }
        } else {
            panel.SetActive(false);
        }
    }

    void UpdateUnitButtons() {
        for (int i = horizontalLayoutGroup.transform.childCount - 1; i >= 0; i--) {
            GameObject.Destroy(horizontalLayoutGroup.transform.GetChild(i).gameObject);
        }
        foreach (var unit in ((Transport)UIData.currentUnit).tanksOnTransport) {
            GameObject newButton = GameObject.Instantiate(unitButtonPrefab, Vector3.zero, Quaternion.identity);
            newButton.transform.SetParent(horizontalLayoutGroup.transform, false);
            UnitButtonUI unitButton = newButton.GetComponent<UnitButtonUI>();
            unitButton.unit = unit;
        }
    }
}
