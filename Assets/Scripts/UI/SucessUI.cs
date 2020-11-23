using UnityEngine;
using UnityEngine.UI;

public class SucessUI : MonoBehaviour {
    public Button okButton;
    public GameObject sucessPanel;

    void Start() => okButton.onClick.AddListener(Ok);

    void Ok() => sucessPanel.SetActive(false);
}
