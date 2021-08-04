using UnityEngine;
using UnityEngine.UI;

public class TransportUI : MonoBehaviour {

    public GameObject panel;
    public HorizontalLayoutGroup horizontalLayoutGroup;
    public GameObject unitButtonPrefab;
    public float yOffset;

    private Transport oldTransport;
    private bool hasUpdated = false;

    void Start() {
        panel.SetActive(false);
    }

    void Update() {
        if (UIData.Instance.currentUnit != null) {
            if (UIData.Instance.currentUnit.GetType() == typeof(Transport)) {
                panel.SetActive(true);
                transform.position = new Vector3(UIData.Instance.currentUnit.transform.position.x, yOffset, UIData.Instance.currentUnit.transform.position.z);
                if (oldTransport != UIData.Instance.currentUnit) {
                    hasUpdated = false;
                    oldTransport = (Transport)UIData.Instance.currentUnit;
                }
                if (!hasUpdated) {
                    hasUpdated = true;
                    UpdatedUnitButtons();
                }
            } else {
                panel.SetActive(false);
                hasUpdated = false;
            }
        } else {
            panel.SetActive(false);
            hasUpdated = false;
        }
    }

    void UpdatedUnitButtons() {
        for (int i = horizontalLayoutGroup.transform.childCount - 1; i >= 0; i--) {
            GameObject.Destroy(horizontalLayoutGroup.transform.GetChild(i).gameObject);
        }
        foreach (var unit in ((Transport)UIData.Instance.currentUnit).tanksOnTransport) {
            GameObject newButton = GameObject.Instantiate(unitButtonPrefab, Vector3.zero, Quaternion.identity);
            newButton.transform.SetParent(horizontalLayoutGroup.transform, false);
            UnitButtonUI unitButton = newButton.GetComponent<UnitButtonUI>();
            unitButton.unit = unit;
        }
    }
}
