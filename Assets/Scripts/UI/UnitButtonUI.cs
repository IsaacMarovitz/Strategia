using UnityEngine;
using UnityEngine.UI;

public class UnitButtonUI : MonoBehaviour {

    public Button button;
    public Unit unit;

    public void Start() {
        button.onClick.AddListener(OnClick);
    }

    public void OnClick() {
        UIData.Instance.currentCity = null;
        UIData.Instance.currentUnit = unit;
    }
}