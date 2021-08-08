using UnityEngine;
using UnityEngine.UI;

public class UnitButtonUI : MonoBehaviour {

    public Button button;
    public Image image;
    public Unit unit;

    public void Start() {
        button.onClick.AddListener(OnClick);
    }

    public void OnClick() {
        UIData.SetCity(null);
        UIData.SetUnit(unit);
    }
}